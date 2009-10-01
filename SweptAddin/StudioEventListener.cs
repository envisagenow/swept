using System;
using EnvDTE80;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Windows.Forms;
//using System.Windows.Forms;

namespace swept.Addin
{
    public class StudioEventListener : IVsRunningDocTableEvents, IDisposable
    {
        private DTE2 _studio;
        private StudioAdapter _adapter;
        //private IVsRunningDocumentTable _runningDocs;
        private uint _runningDocsCookie;

        //  Class scoped to hold these references for the lifetime of the addin.
        private SolutionEvents _solutionEvents;
        private ProjectItemsEvents _solutionItemsEvents;
        private DocumentEvents _documentEvents;
        private CommandEvents _commandEvents;

        private TaskWindow_GUI _taskWindowForm;
        //  When in the middle of a save-as, this will hold the original name.
        string _saveAsOldName = string.Empty;

        private List<string> log = new List<string>();

        public void Disconnect()
        {
            _solutionEvents.Opened -= Hear_SolutionOpened;
            _solutionEvents.Renamed -= Hear_SolutionRenamed;

            _solutionItemsEvents.ItemRenamed -= Hear_ItemRenamed;

            _documentEvents.DocumentClosing -= Hear_DocumentClosing;
            _documentEvents.DocumentSaved -= Hear_DocumentSaved;

            _solutionEvents = null;
            _solutionItemsEvents = null;
            _documentEvents = null;

            _studio = null;
            _adapter = null;

            _commandEvents.BeforeExecute -= hear_all_CommandEvents_before;
            _commandEvents.AfterExecute -= hear_all_CommandEvents_after;

            _commandEvents = null;
        }

        public void Connect( DTE2 studio, Starter starter )
        {
            _studio = studio;
            _adapter = starter.Adapter;
            _solutionEvents = _studio.Events.SolutionEvents;
            _solutionItemsEvents = _studio.Events.SolutionItemsEvents;
            _documentEvents = _studio.Events.get_DocumentEvents( null );

            _solutionEvents.Opened += Hear_SolutionOpened;
            _solutionEvents.Renamed += Hear_SolutionRenamed;

            _solutionItemsEvents.ItemRenamed += Hear_ItemRenamed;

            _documentEvents.DocumentClosing += Hear_DocumentClosing;
            _documentEvents.DocumentSaved += Hear_DocumentSaved;

            #region Attach to RunningDocumentTable
            // TODO--0.3: Upgrade from this interface
            //_runningDocs = (IVsRunningDocumentTable)
            //    Package.GetGlobalService( typeof( SVsRunningDocumentTable ) );

            //int returnCode = _runningDocs.AdviseRunningDocTableEvents( this, out _runningDocsCookie );

            //if( returnCode != VSConstants.S_OK )
            //    throw new Exception( string.Format( "Got error [{0}] instead of S_OK.", returnCode ) );
            #endregion

            hook_all_CommandEvents();

            knit_TaskWindow( starter.TaskWindow );
        }

        #region CommandEvents
        private void hook_all_CommandEvents()
        {
            //  get and subscribe to all events prior to execution
            _commandEvents = _studio.Events.get_CommandEvents( "{00000000-0000-0000-0000-000000000000}", 0 );

            _commandEvents.BeforeExecute += hear_all_CommandEvents_before;
            _commandEvents.AfterExecute += hear_all_CommandEvents_after;
        }

        void hear_all_CommandEvents_before( string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault )
        {
            if( ID == 2200 || ID == 337 || ID == 1627 ) return;

            Command command = _studio.Commands.Item( Guid, ID );
            if( command == null )
            {
                log.Add( string.Format( "ID [{0}] with GUID [{1}] matches no command.", ID, Guid ) );
                return;
            }

            string commandName = command.Name;
            //if( commandName == "" || commandName == "Edit.GoToFindCombo" ) return;

            string message = string.Format( "Before [{0}] has guid [{1}] and id [{2}].", commandName, command.Guid, command.ID );

            switch( commandName )
            {
            case "File.SaveSelectedItemsAs":
                _saveAsOldName = _studio.ActiveDocument.FullName;
                break;

            default:
                log.Add( message );
                break;
            }
        }

        private void hear_all_CommandEvents_after( string Guid, int ID, object CustomIn, object CustomOut )
        {
            if( ID == 2200 || ID == 337 || ID == 1627 ) return;

            Command command = _studio.Commands.Item( Guid, ID );
            if( command == null ) return;

            string commandName = command.Name;
            //if( commandName == "" || commandName == "Edit.GoToFindCombo" ) return;

            string message = string.Format( "After [{0}] has guid [{1}] and id [{2}].", commandName, command.Guid, command.ID );

            switch( commandName )
            {
            case "File.SaveSelectedItemsAs":
                _saveAsOldName = string.Empty;
                break;

            case "Edit.LineDown":
                log.Add( message );
                break;

            default:
                log.Add( message );
                break;
            }
        }
        #endregion

        TaskWindow _taskWindow;
        private void knit_TaskWindow( TaskWindow taskWindow )
        {
            _taskWindow = taskWindow;
            _taskWindowForm = new TaskWindow_GUI();

            taskWindow.Event_TaskListReset += _taskWindowForm.Hear_TaskListReset;

            _taskWindowForm.tasks.ItemCheck += Hear_ItemCheck;// taskWindow.Hear_TaskCheck;

            _taskWindowForm.Show();
        }

        private void Hear_ItemCheck( object sender, ItemCheckEventArgs e )
        {
            TaskEventArgs args = new TaskEventArgs{ 
                Task = (Task)_taskWindowForm.tasks.Items[e.Index], 
                Checked = (e.NewValue == CheckState.Checked) };
            _taskWindow.Hear_TaskCheck( _taskWindowForm, args );
        }

        void IDisposable.Dispose()
        {
            if( _runningDocsCookie == 0 ) return;

            //_runningDocs.UnadviseRunningDocTableEvents( _runningDocsCookie );
            _runningDocsCookie = 0;

            // TODO--0.2: Finish shutdown: unsubscribe from events, dispose of windows, and ?
            _taskWindowForm.Dispose();
        }
        
        public void Hear_SolutionOpened()
        {
            _adapter.Raise_SolutionOpened( _studio.Solution.FileName );
        }

        public void Hear_SolutionRenamed( string oldName )
        {
            _adapter.Raise_SolutionRenamed( oldName, _studio.Solution.FileName );
        }


        int IVsRunningDocTableEvents.OnBeforeDocumentWindowShow( uint docCookie, int fFirstShow, IVsWindowFrame pFrame )
        {
            string fileName = string.Empty; // fileNameFromDocCookie( docCookie );            
            _adapter.Raise_FileGotFocus( fileName );
            return VSConstants.S_OK;
        }

        //private string fileNameFromDocCookie( uint docCookie )
        //{
        //    string fileName;
        //    uint rdtFlags;
        //    uint readLocks;
        //    uint editLocks;
        //    IVsHierarchy ppHier;
        //    uint pitemid;
        //    IntPtr ppunkDocData;
        //    _runningDocs.GetDocumentInfo( docCookie, out rdtFlags, out readLocks, out editLocks, out fileName,
        //        out ppHier, out pitemid, out ppunkDocData );
        //    return fileName;
        //}

        private void Hear_DocumentSaved( Document doc )
        {
            string fileName = doc.FullName;
            if( _saveAsOldName == string.Empty )
                _adapter.Raise_FileSaved( fileName );
            else
                _adapter.Raise_FileSavedAs( _saveAsOldName, fileName );
        }

        private void Hear_ItemRenamed( ProjectItem item, string oldName )
        {
            _adapter.Raise_FileRenamed( oldName, item.Name );
        }

        private void Hear_DocumentClosing( Document doc )
        {
            if( !doc.Saved )
                _adapter.Raise_FileChangesAbandoned( doc.Name );
        }

        //  above here, subscribed

        // TODO--0.2: Hear_FilePasted
        private void Hear_FilePasted( string fileName )
        {
            _adapter.Raise_FilePasted( fileName );
        }

        // TODO--0.2: determine if NonSourceGetsFocus is needed.
        private void Hear_NonSourceGetsFocus()
        {
            _adapter.Raise_NonSourceGetsFocus();
        }

        // TODO--0.2: Hear_FileDeleted
        private void Hear_FileDeleted( string fileName )
        {
            _adapter.Raise_FileDeleted( fileName );
        }

        //Will this be covered by individual file save events?  _adapter.RaiseSolutionSaved()

        #region IVsRunningDocTableEvents Members (empty boilerplate)

        public int OnAfterAttributeChange( uint docCookie, uint grfAttribs )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnAfterAttributeChange", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide( uint docCookie, IVsWindowFrame pFrame )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnAfterDocumentWindowHide", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock( uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnAfterFirstDocumentLock", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnAfterSave( uint docCookie )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnAfterSave", docCookie ) );
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock( uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining )
        {
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnBeforeLastDocumentUnlock", docCookie ) );
            return VSConstants.S_OK;
        }

        #endregion
    }
}
