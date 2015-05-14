//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2015 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;

namespace swept
{
    public class RunChanges
    {
        public int RunNumber = 1;
        public DateTime CurrentDateTime = DateTime.MinValue;
        public DateTime PreviousDateTime = DateTime.MinValue;

        public List<RuleDescription> Rules = new List<RuleDescription>();
        public List<FileChange> Files = new List<FileChange>();


        public RunChanges InitializeNextRun()
        {
            RunChanges nextRun = new RunChanges { RunNumber = this.RunNumber + 1 };

            foreach (var myFile in Files)
            {
                FileChange nextFile = new FileChange();
                foreach (var myRule in myFile.Rules.Where( r => r.Is > 0 ))
                {
                    nextFile.Rules.Add(new RuleChange {
                        ID = myRule.ID,
                        Was = myRule.Is
                    });
                }

                if (nextFile.Rules.Count() > 0)
                {
                    nextFile.Name = myFile.Name;
                    nextRun.Files.Add(nextFile);
                }
           }

            return nextRun;
        }

        public void AddRuleTasks(RuleTasks ruleTasks, DateTime runTime)
        {
            CurrentDateTime = runTime;
            foreach (Rule rule in ruleTasks.Keys)
            {
                FileTasks fileTask = ruleTasks[rule];

                foreach (SourceFile sourcefile in fileTask.Keys)
                {
                    ClauseMatch match = fileTask[sourcefile];
                    if (!match.DoesMatch) continue;

                    var fileChange = GetFileChange(sourcefile);
                    var ruleChange = fileChange.GetRuleChange(rule.ID);
                    ruleChange.Is = match.Count;
                    fileChange.Changed = fileChange.Changed || (ruleChange.Is != ruleChange.Was);
                }
            }
        }
              

        private FileChange GetFileChange(SourceFile sourceFile)
        {
            var matchingFile = Files.SingleOrDefault(f => f.Name == sourceFile.Name);
            if (matchingFile == null)
            {
                matchingFile = new FileChange { Name = sourceFile.Name };
                Files.Add(matchingFile);
            }

            return matchingFile;
        }

    }


    public class FileChange
    {
        public string Name = string.Empty;
        public List<RuleChange> Rules = new List<RuleChange>();
        public bool Changed;

        public RuleChange GetRuleChange(string ruleID)
        {
            var matchingRule = Rules.SingleOrDefault(f => f.ID == ruleID);
            if (matchingRule == null)
            {
                matchingRule = new RuleChange { ID = ruleID };
                Rules.Add(matchingRule);
            }

            return matchingRule;
        }
    }

    public class RuleChange
    {
        public string ID = string.Empty;
        public int Was = 0;
        public int Is = 0;
    }

    public class RuleDescription
    {
        public string ID = string.Empty;
        public string Description = string.Empty;
    }
}
