using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace swept.Addin
{
    public class StudioEventListener : IVsRunningDocTableEvents
    {
        private DTE2 _studio;
        private StudioAdapter _adapter;
        private IVsRunningDocumentTable _runningDocs;

        public void Subscribe( DTE2 studio, swept.StudioAdapter adapter )
        {
            _studio = studio;
            _adapter = adapter;

            _studio.Events.SolutionEvents.Opened += Hear_SolutionOpened;
            _studio.Events.SolutionEvents.Renamed += Hear_SolutionRenamed;

            _studio.Events.SolutionItemsEvents.ItemRenamed += Hear_ItemRenamed;

            //DocTableListener listener = new DocTableListener();

            _runningDocs = (IVsRunningDocumentTable)
                Package.GetGlobalService( typeof( SVsRunningDocumentTable ) );

            uint listenerCookie;
            int returnCode = _runningDocs.AdviseRunningDocTableEvents( this, out listenerCookie );

            if( returnCode != VSConstants.S_OK )
                throw new Exception( string.Format( "Got error [{0}] instead of S_OK.", returnCode ) );

            //IVsMonitorSelection ?

        }

        public void Hear_SolutionOpened()
        {
            _adapter.Raise_SolutionOpened( _studio.Solution.FileName );
        }

        public void Hear_SolutionRenamed( string oldName )
        {
            _adapter.Raise_SolutionRenamed( oldName, _studio.Solution.FileName );
        }

        public int OnBeforeDocumentWindowShow( uint docCookie, int fFirstShow, IVsWindowFrame pFrame )
        {
            string fileName = fileNameFromDocCookie( docCookie );            
            _adapter.Raise_FileGotFocus( fileName );
            //MessageBox.Show( string.Format( "{0}( {1}, ... )", "OnBeforeDocumentWindowShow", docCookie ) );
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


        //  above here, subscribed

        private void Hear_ItemRenamed( ProjectItem item, string oldName )
        {
            _adapter.Raise_FileRenamed( oldName, item.Name );
        }

        private void Hear_FileGotFocus( string fileName )
        {
            _adapter.Raise_FileGotFocus( fileName );
        }

        private void Hear_NonSourceGetsFocus()
        {
            _adapter.Raise_NonSourceGetsFocus();
        }

        private void Hear_FileSaved( string fileName )
        {
            _adapter.Raise_FileSaved( fileName );
        }

        private void Hear_FilePasted( string fileName )
        {
            _adapter.Raise_FilePasted( fileName );
        }

        private void Hear_FileSavedAs( string oldName, string newName )
        {
            _adapter.Raise_FileSavedAs( oldName, newName );
        }

        private void Hear_FileChangesAbandoned( string fileName )
        {
            _adapter.Raise_FileChangesAbandoned( fileName );
        }

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
