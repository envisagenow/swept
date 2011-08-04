//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using EnvDTE80;
using EnvDTE;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace swept.Addin
{
    internal class StudioEventStarter : IDisposable
    {
        private DTE2 _studio;
        private AddIn _sweptAddin;
        private EventSwitchboard _switchboard;

        //  Class scoped to hold these references for the lifetime of the addin.
        private SolutionEvents _solutionEvents;
        private DocumentEvents _documentEvents;
        private StudioEventChannel _channel;

        public void Connect( DTE2 studio, AddIn sweptAddin, EventSwitchboard switchboard )
        {
            _studio = studio;
            _sweptAddin = sweptAddin;
            _switchboard = switchboard;

            _solutionEvents = _studio.Events.SolutionEvents;
            _documentEvents = _studio.Events.get_DocumentEvents( null );

            _channel = new StudioEventChannel( _switchboard, _studio );

            _solutionEvents.Opened += _channel.Hear_SolutionOpened;
            _solutionEvents.AfterClosing += _channel.Hear_SolutionClosed;

            _documentEvents.DocumentSaved += _channel.Hear_DocumentSaved;
            _documentEvents.DocumentOpened += _channel.Hear_DocumentOpened;
            _documentEvents.DocumentClosing += _channel.Hear_DocumentClosing;

            if (_studio.Solution != null)
            {
                _channel.Hear_SolutionOpened();
            }
        }

        public void Disconnect( Starter starter )
        {
            _documentEvents.DocumentSaved -= _channel.Hear_DocumentSaved;
            _documentEvents.DocumentOpened -= _channel.Hear_DocumentOpened;
            _documentEvents.DocumentClosing -= _channel.Hear_DocumentClosing;

            _solutionEvents.Opened -= _channel.Hear_SolutionOpened;
            _solutionEvents.AfterClosing -= _channel.Hear_SolutionClosed;

            _documentEvents = null;
            _solutionEvents = null;

            _switchboard = null;
            _studio = null;
            _channel = null;
        }

        void IDisposable.Dispose()
        {
            //_taskWindowControl.Dispose();
        }
    }

    public class StudioEventChannel
    {
        swept.EventSwitchboard _switchboard;
        private DTE2 _studio;

        public StudioEventChannel( EventSwitchboard switchboard, DTE2 studio )
        {
            _switchboard = switchboard;
            _studio = studio;
        }

        public void Hear_SolutionOpened()
        {
            try
            {
                _switchboard.Raise_SolutionOpened( _studio.Solution.FileName );
            }
            catch (Exception e)
            {
                describeException( e );

                // TODO: Work out if this was fatal, and should I unload the addin?
            }
        }

        public void Hear_SolutionClosed()
        {
            try
            {
                _switchboard.Raise_SolutionClosed( _studio.Solution.FileName );
            }
            catch (Exception e)
            {
                describeException( e );

                // TODO: Work out if this was fatal, and should I unload the addin?
            }
        }

        public void Hear_DocumentSaved( Document doc )
        {
            try
            {
                string fileName = doc.FullName;
                _switchboard.Raise_FileSaved( fileName );
            }
            catch (Exception e)
            {
                describeException( e );
            }
        }

        public void Hear_DocumentOpened( Document doc )
        {
            try
            {
                string fileName = doc.FullName;
                _switchboard.Raise_FileOpened( fileName );
            }
            catch (Exception e)
            {
                describeException( e );
            }
        }

        public void Hear_DocumentClosing( Document doc )
        {
            try
            {
                string fileName = doc.FullName;
                _switchboard.Raise_FileClosing( fileName );
            }
            catch (Exception e)
            {
                describeException( e );
            }
        }


        internal static void describeException( Exception e )
        {
            string exceptionText = string.Format(
                "{0}:\n{1}\n",
                e.Message, e.StackTrace );
            string message = string.Format( "{0}\n{1}\n{2}",
                "Swept caught exception:",
                exceptionText,
                "Shall I paste this exception text to your clipboard?"
            );

            var choice = MessageBox.Show( message, "Exception caught by Swept Addin", MessageBoxButtons.YesNo );

            if (choice == DialogResult.Yes)
                Clipboard.SetText( exceptionText );
        }
    }

}
