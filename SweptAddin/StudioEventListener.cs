using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using EnvDTE;

namespace swept.Addin
{
    public class StudioEventListener
    {
        private DTE2 _studio;
        private StudioAdapter _adapter;

        public void Subscribe( DTE2 studio, swept.StudioAdapter adapter )
        {
            _studio = studio;
            _adapter = adapter;

            _studio.Events.SolutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler( Hear_SolutionOpened );
            _studio.Events.SolutionEvents.Renamed += new _dispSolutionEvents_RenamedEventHandler( Hear_SolutionRenamed );


            // TODO:  Subscribe the adapter to the rest of the Visual Studio IDE events we care about
        }

        public void Hear_SolutionOpened()
        {
            _adapter.Raise_SolutionOpened( _studio.Solution.FileName );
        }

        public void Hear_SolutionRenamed( string oldName )
        {
            //_adapter.RaiseSolutionRenamed( oldName, _studio.Solution.FileName );
        }

        //public void RaiseFileGotFocus( string fileName )
        //public void RaiseNonSourceGetsFocus()
        //public void RaiseFileSaved( string fileName )
        //public void RaiseFilePasted( string fileName )
        //public void RaiseFileSavedAs( string oldName, string newName )
        //public void RaiseFileChangesAbandoned( string fileName )
        //public void RaiseFileDeleted( string fileName )
        //public void RaiseFileRenamed( string oldName, string newName )

        //Not implemented as such?  public void RaiseSolutionSaved()


    }
}
