//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;

namespace swept
{
    public class Task : Change
    {
        public int LineNumber { get; private set; }
        public bool Completed;

        public static List<Task> FromChange( Change change )
        {
            List<Task> tasks = new List<Task>();
            //generate list of one task per spot
            foreach (int line in change._matchList)
            {
                Task task = new Task
                {
                    ID = change.ID,
                    Description = change.Description,
                    LineNumber = line,
                    Completed = false
                };

                foreach (var seeAlso in change.SeeAlsos)
                {
                    task.SeeAlsos.Add( seeAlso.Clone() );
                }
                tasks.Add( task );
            }

            return tasks;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
