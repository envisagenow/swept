//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using EnvDTE80;
using EnvDTE;
using System.Collections.Generic;
using System.Windows.Forms;

namespace swept.Addin
{
    internal class StudioEventListener : IDisposable
    {
        private DTE2 _studio;
        private AddIn _sweptAddin;
        private StudioAdapter _adapter;

        //  Class scoped to hold these references for the lifetime of the addin.
        private SolutionEvents _solutionEvents;
        private ProjectItemsEvents _solutionItemsEvents;
        private DocumentEvents _documentEvents;
        private CommandEvents _commandEvents;
        private WindowEvents _windowEvents;

        private Window _taskWindowWindow;
        private TaskWindow_GUI _taskWindowControl;
        //  When in the middle of a save-as, this will hold the original name.
        string _saveAsOldName = string.Empty;

        private List<string> log = new List<string>();

        public void Connect( DTE2 studio, Starter starter, AddIn sweptAddin )
        {
            _studio = studio;
            _sweptAddin = sweptAddin;
            _adapter = starter.Adapter;
            _solutionEvents = _studio.Events.SolutionEvents;
            _solutionItemsEvents = _studio.Events.SolutionItemsEvents;
            _documentEvents = _studio.Events.get_DocumentEvents( null );
            _windowEvents = _studio.Events.get_WindowEvents( null );

            _solutionEvents.Opened += Hear_SolutionOpened;
            _solutionEvents.Renamed += Hear_SolutionRenamed;

            _solutionItemsEvents.ItemRenamed += Hear_ItemRenamed;

            _documentEvents.DocumentClosing += Hear_DocumentClosing;
            _documentEvents.DocumentSaved += Hear_DocumentSaved;

            _windowEvents.WindowActivated += Hear_WindowActivated;

            hook_all_CommandEvents();
            hook_TaskWindow( starter.TaskWindow );

            if (_studio.Solution != null)
            {
                Hear_SolutionOpened();
                Hear_WindowActivated( _studio.ActiveWindow, null );
            }
        }

        public void Disconnect( Starter starter )
        {
            unhook_TaskWindow();
            unhook_all_CommandEvents();

            _windowEvents.WindowActivated -= Hear_WindowActivated;

            _documentEvents.DocumentSaved -= Hear_DocumentSaved;
            _documentEvents.DocumentClosing -= Hear_DocumentClosing;

            _solutionEvents.Renamed -= Hear_SolutionRenamed;
            _solutionEvents.Opened -= Hear_SolutionOpened;

            _solutionItemsEvents.ItemRenamed -= Hear_ItemRenamed;

            _windowEvents = null;
            _documentEvents = null;
            _solutionItemsEvents = null;
            _solutionEvents = null;

            _adapter = null;
            _studio = null;
        }

        #region CommandEvents
        private void hook_all_CommandEvents()
        {
            //  get and subscribe to all events prior to execution
            _commandEvents = _studio.Events.get_CommandEvents( "{00000000-0000-0000-0000-000000000000}", 0 );

            _commandEvents.BeforeExecute += hear_all_CommandEvents_before;
            _commandEvents.AfterExecute += hear_all_CommandEvents_after;
        }

        private void unhook_all_CommandEvents()
        {
            _commandEvents.BeforeExecute -= hear_all_CommandEvents_before;
            _commandEvents.AfterExecute -= hear_all_CommandEvents_after;
            _commandEvents = null;
        }

        private bool ignoreCommandEvent( int ID )
        {
            return (ID == 337 || ID == 1627 || ID == 1936 || ID == 2200);
        }

        void hear_all_CommandEvents_before( string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault )
        {
            if (ignoreCommandEvent( ID )) return;

            Command command = _studio.Commands.Item( Guid, ID );
            if (command == null)
            {
                log.Add( string.Format( "ID [{0}] with GUID [{1}] matches no command.", ID, Guid ) );
                return;
            }

            string commandName = command.Name;
            string message = string.Format( "Before [{0}] has guid [{1}] and id [{2}].", commandName, command.Guid, command.ID );

            switch (commandName)
            {
            case "File.SaveSelectedItemsAs":
                _saveAsOldName = _studio.ActiveDocument.FullName;
                break;

            default:
                //log.Add( message );
                break;
            }
        }

        private void hear_all_CommandEvents_after( string Guid, int ID, object CustomIn, object CustomOut )
        {
            if (ignoreCommandEvent( ID )) return;

            Command command = _studio.Commands.Item( Guid, ID );
            if (command == null) return;

            string commandName = command.Name;

            string message = string.Format( "After [{0}] has guid [{1}] and id [{2}].", commandName, command.Guid, command.ID );

            switch (commandName)
            {
            case "File.SaveSelectedItemsAs":
                _saveAsOldName = string.Empty;
                break;

            case "Edit.LineDown":
                //log.Add( message );
                //Clipboard.SetText( string.Join( "\n", log.ToArray() ) );
                break;

            default:
                //log.Add( message );
                break;
            }
        }
        #endregion

        TaskWindow _taskWindow;
        private void hook_TaskWindow( TaskWindow taskWindow )
        {
            _taskWindow = taskWindow;
            
            create_taskWindow();
            taskWindow.Event_TaskListReset += _taskWindowControl.Hear_TaskListReset;
            // TODO--0.3, Grid: get 'checkbox altered' events from the GridView, somewhat as below
            //_taskWindowForm._taskGridView.CheckBoxColumn.Event_ItemCheck += Hear_ItemCheck;

            _taskWindowControl.Event_SeeAlsoFollowed += taskWindow.Hear_SeeAlsoFollowed;

            _taskWindowWindow.Visible = true;
            _taskWindowControl.Show();
        }

        private void create_taskWindow()
        {
            if (_taskWindowControl == null)
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                Windows2 win2 = (Windows2)_studio.Windows;
                object taskWindowControl = null;

                _taskWindowWindow = win2.CreateToolWindow2(_sweptAddin, assembly.Location, "swept.Addin." + typeof(TaskWindow_GUI).Name, "Swept", "{9ED54F84-A89D-4fcd-A854-44251E925F09}", ref taskWindowControl);
                _taskWindowControl = (TaskWindow_GUI)taskWindowControl;
            }
        }

        private void unhook_TaskWindow()
        {
            _taskWindowWindow.Visible = false;
            
            _taskWindowControl.Event_SeeAlsoFollowed -= _taskWindow.Hear_SeeAlsoFollowed;

            // TODO--0.3, Grid: stop getting 'checkbox altered' events, somewhat as below
            //_taskWindowForm._taskGridView.CheckBoxColumn.Event_ItemCheck -= Hear_ItemCheck;
            _taskWindow.Event_TaskListReset -= _taskWindowControl.Hear_TaskListReset;
            _taskWindow = null;
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

        private void Hear_ItemCheck( object sender, ItemCheckEventArgs e )
        {
            TaskEventArgs args = new TaskEventArgs
            {
                // TODO--0.3, Grid:  deduce the task and checked state from the event
                //Task = (Task)_taskWindowForm.tasks.Items[e.Index],
                Checked = (e.NewValue == CheckState.Checked)
            };
            _taskWindow.Hear_TaskCheck( _taskWindowControl, args );
        }

        void IDisposable.Dispose()
        {
            _taskWindowControl.Dispose();
        }

        public void Hear_SolutionOpened()
        {
            try
            {
                _adapter.Raise_SolutionOpened( _studio.Solution.FileName );
            }
            catch (Exception e)
            {
                describeException( e );
                Disconnect( null );
                // TODO--0.3: convince the add-in manager that I'm outta here.
            }

        }

        public void Hear_SolutionRenamed( string oldName )
        {
            try
            {
                _adapter.Raise_SolutionRenamed( oldName, _studio.Solution.FileName );
            }
            catch (Exception e)
            {
                describeException( e );
            }
        }

        private void Hear_WindowActivated( Window GotFocus, Window LostFocus )
        {
            try
            {
                // TODO--0.N: If we later display tasks in the Studio Task List window, don't clear tasks on 
                //   don't hide our tasks when the Task List window gets the focus.
                if (GotFocus.Document == null)
                    _adapter.Raise_NonSourceGetsFocus();
                else
                {
                    var textDoc = GotFocus.Document.Object( "TextDocument" ) as TextDocument;
                    if (textDoc != null)
                    {
                        string content = textDoc.StartPoint.CreateEditPoint().GetText( textDoc.EndPoint );
                        _adapter.Raise_FileGotFocus( GotFocus.Document.FullName, content );
                    }
                }
            }
            catch (Exception e)
            {
                describeException( e );
            }
        }

        private void Hear_DocumentSaved( Document doc )
        {
            try
            {
                string fileName = doc.FullName;
                if (_saveAsOldName == string.Empty)
                    _adapter.Raise_FileSaved( fileName );
                else
                    _adapter.Raise_FileSavedAs( _saveAsOldName, fileName );
            }
            catch (Exception e)
            {
                describeException( e );
            }
        }

        private void Hear_ItemRenamed( ProjectItem item, string oldName )
        {
            try
            {
                _adapter.Raise_FileRenamed( oldName, item.Name );
            }
            catch (Exception e)
            {
                describeException( e );
            }
        }

        private void Hear_DocumentClosing( Document doc )
        {
            try
            {
                if (!doc.Saved)
                    _adapter.Raise_FileChangesAbandoned( doc.Name );
            }
            catch (Exception e)
            {
                describeException( e );
            }
        }

        //  above here, subscribed

        // TODO--0.3: Hear_FilePasted
        private void Hear_FilePasted( string fileName )
        {
            _adapter.Raise_FilePasted( fileName );
        }

        //Will this be covered by individual file save events?  _adapter.RaiseSolutionSaved()
    }
}
