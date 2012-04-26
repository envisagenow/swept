//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class RunHistory
    {
        public int NextRunNumber
        {
            get
            {
                if (Runs.Count == 0) return 1;
                else return Runs.Max( r => r.Number )  + 1;
            }
        }

        public List<RunHistoryEntry> Runs;

        public void AddRun( RunHistoryEntry run )
        {
            Runs.Add( run );
        }

        public RunHistory()
        {
            Runs = new List<RunHistoryEntry>();
        }

        public RunHistoryEntry GenerateEntry( Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> changeViolations, DateTime runDateTime )
        {
            var entry = new RunHistoryEntry();
            entry.Number = NextRunNumber;
            entry.Date = runDateTime;

            foreach( var keyChange in changeViolations.Keys )
            {
                entry.Violations[keyChange.ID] = countViolations( changeViolations[keyChange] );
            }

            return entry;
        }



        private int countViolations( Dictionary<SourceFile, ClauseMatch> problemsPerFile )
        {
            int count = 0;
            foreach (SourceFile source in problemsPerFile.Keys)
            {
                count += problemsPerFile[source].Count;
            }
            return count;
        }

    }

}
