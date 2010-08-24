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

        [Obsolete("Generate Tasks from an IssueSet instead.  Broken to ease refactoring.")]
        public static List<Task> FromChange( Change change )
        {
            List<Task> tasks = new List<Task>();
            //generate list of one task per spot
            foreach (int line in /*change._matches*/ new List<int>())
            {
                Task task = new Task
                {
                    ID = change.ID,
                    Description = change.Description,
                    LineNumber = line,
                };

                foreach (var seeAlso in change.SeeAlsos)
                {
                    task.SeeAlsos.Add( seeAlso.Clone() );
                }
                tasks.Add( task );
            }

            return tasks;
        }

        public static List<Task> FromIssueSet( IssueSet set )
        {
            List<Task> tasks = new List<Task>();
            if (!set.DoesMatch) return tasks;

            string description = string.Empty;
            if (set.Clause != null && !string.IsNullOrEmpty( set.Clause.Description ))
                description = set.Clause.Description;

            foreach (int line in set.Matches)
            {
                tasks.Add( new Task
                {
                    Description = description,
                    LineNumber = line,
                } );
            }

            return tasks;
        }


        public override string ToString()
        {
            return Description;
        }
    }
}
