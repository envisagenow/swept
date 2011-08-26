using System;
using EnvDTE80;
using EnvDTE;
using System.Windows.Forms;

namespace swept.Addin
{
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
                _switchboard.Raise_SolutionClosed();
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
                // FIX:  Pull genuine content from the doc, or from fileSystem
                string content = "// foo \n // to do";

                string fileName = doc.FullName;
                _switchboard.Raise_FileSaved( fileName, content );
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
                // FIX:  Pull genuine content from the doc, or from fileSystem
                string content = "// foo \n // to do";

                string fileName = doc.FullName;
                _switchboard.Raise_FileOpened( fileName, content );
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

                // FIX:  Get the task window, flip through taskitems, remove those matching.
                //_studio.
            }
            catch (Exception e)
            {
                describeException( e );
            }
        }

        // TODO: move to UserGUIAdapter 
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
