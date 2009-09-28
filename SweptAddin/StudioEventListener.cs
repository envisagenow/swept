using System;
using EnvDTE80;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace swept.Addin
{
    public class StudioEventListener : IVsRunningDocTableEvents, IDisposable
    {
        private DTE2 _studio;
        private StudioAdapter _adapter;
        private IVsRunningDocumentTable _runningDocs;
        private uint _runningDocsCookie;

        //  Class scoped to hold these references for the lifetime of the plugin.
        private SolutionEvents _solutionEvents;
        private ProjectItemsEvents _solutionItemsEvents;
        private DocumentEvents _documentEvents;

        public void Subscribe( DTE2 studio, swept.StudioAdapter adapter )
        {
            _studio = studio;
            _adapter = adapter;
            _solutionEvents = _studio.Events.SolutionEvents;
            _solutionItemsEvents = _studio.Events.SolutionItemsEvents;
            _documentEvents = _studio.Events.get_DocumentEvents( null );

            _solutionEvents.Opened += Hear_SolutionOpened;
            _solutionEvents.Renamed += Hear_SolutionRenamed;

            _solutionItemsEvents.ItemRenamed += Hear_ItemRenamed;

            _documentEvents.DocumentSaved += Hear_DocumentSaved;

            // TODO--0.3: Upgrade from this interface
            _runningDocs = (IVsRunningDocumentTable)
                Package.GetGlobalService( typeof( SVsRunningDocumentTable ) );

            int returnCode = _runningDocs.AdviseRunningDocTableEvents( this, out _runningDocsCookie );

            if( returnCode != VSConstants.S_OK )
                throw new Exception( string.Format( "Got error [{0}] instead of S_OK.", returnCode ) );

        }

        void IDisposable.Dispose()
        {
            if( _runningDocsCookie == 0 ) return;

            _runningDocs.UnadviseRunningDocTableEvents( _runningDocsCookie );
            _runningDocsCookie = 0;

            // TODO--0.2: Finish shutdown: unsubscribe from events, dispose of windows, and ?
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
            string fileName = fileNameFromDocCookie( docCookie );            
            _adapter.Raise_FileGotFocus( fileName );
            return VSConstants.S_OK;
        }

        private string fileNameFromDocCookie( uint docCookie )
        {
            string fileName;
            uint rdtFlags;
            uint readLocks;
            uint editLocks;
            IVsHierarchy ppHier;
            uint pitemid;
            IntPtr ppunkDocData;
            _runningDocs.GetDocumentInfo( docCookie, out rdtFlags, out readLocks, out editLocks, out fileName,
                out ppHier, out pitemid, out ppunkDocData );
            return fileName;
        }

        private void Hear_DocumentSaved( Document doc )
        {
            string fileName = doc.FullName;
            _adapter.Raise_FileSaved( fileName );
        }

        private void Hear_ItemRenamed( ProjectItem item, string oldName )
        {
            _adapter.Raise_FileRenamed( oldName, item.Name );
        }

        //  above here, subscribed

        // TODO--0.1: Hear_FileSavedAs
        private void Hear_FileSavedAs( string oldName, string newName )
        {
            _adapter.Raise_FileSavedAs( oldName, newName );
        }

        // TODO--0.1: Hear_FilePasted
        private void Hear_FilePasted( string fileName )
        {
            _adapter.Raise_FilePasted( fileName );
        }

        // TODO--0.1: Hear_FileChangesAbandoned( string fileName )
        private void Hear_FileChangesAbandoned( string fileName )
        {
            _adapter.Raise_FileChangesAbandoned( fileName );
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
