//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;

namespace swept
{
    public class Task
    {
        public readonly SourceFile File;
        public readonly int LineNumber;
        public readonly Rule Rule;
        public IEnumerable<SeeAlso> SeeAlsos { get { return Rule.SeeAlsos; } }
        public string Description { get { return Rule.Description; } }

        public Task( Rule rule, SourceFile file, int line )
        {
            Rule = rule;
            File = file;
            LineNumber = line;
        }

        public static List<Task> FromRuleForFile( Rule rule, SourceFile sourceFile )
        {
            return FromMatch( rule.GetMatches( sourceFile ), rule, sourceFile );
        }

        public static List<Task> FromMatch( ClauseMatch match, Rule rule, SourceFile file )
        {
            List<Task> tasks = new List<Task>();
            if (!match.DoesMatch) return tasks;

            // TODO: fix typesnort
            if (match is LineMatch)
            {
                LineMatch lineMatch = match as LineMatch;
                foreach (int line in lineMatch.Lines)
                {
                    tasks.Add( new Task( rule, file, line ) );
                }
            }
            else
            {
                tasks.Add( new Task( rule, file, 1 ) );
            }

            return tasks;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
