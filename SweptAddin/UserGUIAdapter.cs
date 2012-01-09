//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Diagnostics;
using EnvDTE80;
using System.Collections.Generic;
using EnvDTE;
using swept.Addin;

namespace swept.Addin
{
    [CoverageExclude( "Wrapper around Windows GUI" )]
    public class UserGUIAdapter : IUserAdapter
    {
        private DTE2 _studio;
        public UserGUIAdapter( DTE2 studioApplication )
        {
            _studio = studioApplication;
        }

        public void ShowSeeAlso( SeeAlso seeAlso )
        {
            switch (seeAlso.TargetType)
            {
            case TargetType.URL:
                System.Diagnostics.Process.Start( seeAlso.Target );
                break;

            case TargetType.SVN:
                // TODO: Show SeeAlso SVN:  bring back two local files into temp directory, fire up svndiff...
                break;

            case TargetType.File:
                // TODO: Show SeeAlso File:  switch VS IDE to display chosen file at line number
                break;

            default:
                throw new Exception( string.Format( "Don't know how to show a SeeAlso Target of type [{0}].", seeAlso.TargetType ) );
            }
        }

        public void Hear_TasksChangedEvent( object sender, TasksEventArgs args )
        {
            AddNewSweptTasks( args.Tasks );
        }

        private void AddNewSweptTasks( List<Task> list )
        {
            TaskList studioTaskList = _studio.ToolWindows.TaskList;
            foreach (Task sweptTask in list)
            {
                studioTaskList.TaskItems.Add( 
                    "Swept", 
                    sweptTask.Change.ID, 
                    sweptTask.Description, 
                    vsTaskPriority.vsTaskPriorityMedium, 
                    vsTaskIcon.vsTaskIconNone, 
                    false, 
                    sweptTask.File.Name, 
                    sweptTask.LineNumber, 
                    false, 
                    true );
            }
        }

        public void Hear_TaskNavigated( TaskItem TaskItem, ref bool NavigateHandled )
        {
            if (TaskItem.Category == "Swept")
            {
                try
                {
                    _studio.ExecuteCommand(
                        "File.OpenFile",
                        string.Format( "\"{0}\"", TaskItem.FileName )
                    );

                    //  Trouble if I'm not on an editor document
                    var doc = _studio.ActiveDocument;
                    if (doc.Type != "Text")
                        throw new Exception( "Somehow I don't have a text editor window." );

                    var selection = doc.Selection as TextSelection;

                    // select content of the line.
                    selection.MoveTo( TaskItem.Line, 1, false );
                    selection.StartOfLine( vsStartOfLineOptions.vsStartOfLineOptionsFirstText, false );
                    selection.EndOfLine( true );
                }
                catch (Exception ex)
                {
                    StudioEventChannel.describeException( ex );
                }

                NavigateHandled = true;
            }
        }
    }
}
