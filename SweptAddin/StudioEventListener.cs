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

            // TODO:  Subscribe the adapter to the rest of the Visual Studio IDE events we care about
        }

        public void Hear_SolutionOpened()
        {
            _adapter.RaiseSolutionOpened( _studio.Solution.FileName );
        }

    }
}
