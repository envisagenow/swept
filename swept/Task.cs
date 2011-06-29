//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;

namespace swept
{
    public class Task
    {
        public int LineNumber { get; private set; }
        private Change _Change;
        public IEnumerable<SeeAlso> SeeAlsos { get { return _Change.SeeAlsos; } }
        public string Description { get { return _Change.Description; } }

        public Task( Change change, int line )
        {
            _Change = change;
            LineNumber = line;
        }

        public static List<Task> FromChangesForFile( Change change, SourceFile sourceFile )
        {
            return FromMatch( change.GetMatches( sourceFile ), change );
        }

        public static List<Task> FromMatch( ClauseMatch match, Change change )
        {
            List<Task> tasks = new List<Task>();
            if (!match.DoesMatch) return tasks;

            // TODO: fix typesnort
            if (match is LineMatch)
            {
                LineMatch lineMatch = match as LineMatch;
                foreach (int line in lineMatch.Lines)
                {
                    tasks.Add( new Task( change, line ) );
                }
            }
            else
            {
                tasks.Add( new Task( change, 1 ) );
            }

            return tasks;
        }

        //public static List<Task> FromIssueSet( IssueSet set )
        //{
        //    List<Task> tasks = new List<Task>();
        //    if (!set.DoesMatch) return tasks;

        //    string description = string.Empty;
        //    if (set.Change != null && !string.IsNullOrEmpty( set.Change.Description ))
        //        description = set.Change.Description;

        //    // TODO: fix typesnort
        //    if (set.Match is LineMatch)
        //    {
        //        LineMatch lineMatch = set.Match as LineMatch;
        //        foreach (int line in lineMatch.Lines)
        //        {
        //            tasks.Add( new Task
        //            {
        //                Description = description,
        //                LineNumber = line,
        //            } );
        //        }
        //    }
        //    else if (set.DoesMatch)
        //    {
        //        tasks.Add( new Task
        //        {
        //            Description = description,
        //            LineNumber = 1,
        //        } );
        //    }

        //    return tasks;
        //}


        public override string ToString()
        {
            return Description;
        }
    }
}
