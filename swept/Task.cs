//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept
{
    public class Task : Change
    {
        public bool Completed;

        public static Task FromChange( Change change )
        {
            Task task = new Task
            {
                ID = change.ID,
                Description = change.Description,
                Completed = false,
            };

            foreach (var seeAlso in change.SeeAlsos)
            {
                task.SeeAlsos.Add( seeAlso.Clone() );
            }

            return task;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
