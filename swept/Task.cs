//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;

namespace swept
{
    public class Task
    {
        public SourceFile File { get; private set; }
        public int LineNumber { get; private set; }
        public Change Change { get; private set; }
        public IEnumerable<SeeAlso> SeeAlsos { get { return Change.SeeAlsos; } }
        public string Description { get { return Change.Description; } }

        public Task( Change change, SourceFile file, int line )
        {
            Change = change;
            File = file;
            LineNumber = line;
        }

        public static List<Task> FromChangesForFile( Change change, SourceFile sourceFile )
        {
            return FromMatch( change.GetMatches( sourceFile ), change, sourceFile );
        }

        public static List<Task> FromMatch( ClauseMatch match, Change change, SourceFile file )
        {
            List<Task> tasks = new List<Task>();
            if (!match.DoesMatch) return tasks;

            // TODO: fix typesnort
            if (match is LineMatch)
            {
                LineMatch lineMatch = match as LineMatch;
                foreach (int line in lineMatch.Lines)
                {
                    tasks.Add( new Task( change, file, line ) );
                }
            }
            else
            {
                tasks.Add( new Task( change, file, 1 ) );
            }

            return tasks;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
