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
    public interface IVsCodeWindowEvents
    {
    }
    public class StudioEventListener
    {
        private DTE2 _studio;
        private StudioAdapter _adapter;

        public void Subscribe( DTE2 studio, swept.StudioAdapter adapter )
        {
            _studio = studio;
            _adapter = adapter;

            _studio.Events.SolutionEvents.Opened += Hear_SolutionOpened;
            _studio.Events.SolutionEvents.Renamed += Hear_SolutionRenamed;

            _studio.Events.SolutionItemsEvents.ItemRenamed += Hear_ItemRenamed;

            DocTableListener listener = new DocTableListener();

            uint listenerCookie;

            IVsRunningDocumentTable rdt =
                (IVsRunningDocumentTable)Package.GetGlobalService( typeof( SVsRunningDocumentTable ) );

            int returnCode = rdt.AdviseRunningDocTableEvents( listener, out listenerCookie );

            if( returnCode != VSConstants.S_OK )
                throw new Exception( string.Format( "Got error [{0}] instead of S_OK.", returnCode ) );

            // TODO:  Subscribe the adapter to the rest of the Visual Studio IDE events we care about

/*
Dim monitorSelection As IVsMonitorSelection = CType(Me.GetService(GetType(SVsShellMonitorSelection)), IVsMonitorSelection)  
Dim cookie As UInteger  
Dim result As Integer = monitorSelection.AdviseSelectionEvents(SelectionEventsHandler.Instance, cookie) 
*/
            //IVsCodeWindowEvents codeEvents;
            //codeEvents
            //IVsMonitorSelection monitor;
        }

        public void Hear_SolutionOpened()
        {
            _adapter.Raise_SolutionOpened( _studio.Solution.FileName );
        }

        public void Hear_SolutionRenamed( string oldName )
        {
            _adapter.Raise_SolutionRenamed( oldName, _studio.Solution.FileName );
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
    }
}
