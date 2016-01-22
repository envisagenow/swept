//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2016 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

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
                foreach (var myRule in myFile.Rules.Where(r => r.Is > 0))
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

        public string ForesightReport()
        {
            int totalRegressions = 0;
            StringBuilder regressionsReportBuilder = new StringBuilder();
            foreach (var file in Files.Where(f => f.Changed))
            {
                totalRegressions += ForesightFileRegression(file, regressionsReportBuilder);
            }


            int totalImprovements = 0;
            StringBuilder improvementsReportBuilder = new StringBuilder();
            foreach (var file in Files.Where(f => f.Changed))
            {
                var fileimprovements = 0;
                foreach (var rule in file.Rules)
                {
                    int improvementCount = rule.Was - rule.Is;
                    if (improvementCount < 1) continue;

                    fileimprovements += improvementCount;
                }

                if (fileimprovements > 0)
                {
                    improvementsReportBuilder.AppendLine(string.Format("\t{0}: {1} improvement{2}.", file.Name, fileimprovements, fileimprovements == 1 ? "" : "s"));
                }
                totalImprovements += fileimprovements;
            }


            var finalMessage = new StringBuilder();

            if (totalImprovements == 0 && totalRegressions == 0)
            {
                finalMessage.Append("No changes from baseline.  You're free to commit, boss!");
            }

            if (totalRegressions > 0)
            {
                finalMessage.AppendFormat("{0} regression{1}.\n{2}", totalRegressions, totalRegressions == 1 ? "" : "s", regressionsReportBuilder);
            }

            if (totalImprovements > 0 && totalRegressions > 0)
                finalMessage.AppendLine();

            if (totalImprovements > 0)
            {
                finalMessage.AppendFormat("{0} improvement{1}.\n{2}", totalImprovements, totalImprovements == 1 ? "" : "s", improvementsReportBuilder);
            }

            return finalMessage.ToString();
        }

        public int ForesightFileRegression(FileChange file, StringBuilder builder)
        {
            int deltaCount = 0;
            var deltaDetails = "";
            foreach (var rule in file.Rules)
            {
                int ruleDeltaCount = rule.Is - rule.Was;
                if (ruleDeltaCount < 1) continue;

                deltaCount += ruleDeltaCount;
                deltaDetails += ForesightRuleDelta(rule);
            }
            if (deltaCount == 0) return 0;

            string fileHeader = ForesightDeltaLine("\t" + file.Name, deltaCount);
            builder.Append(fileHeader);
            builder.Append(deltaDetails);

            return deltaCount;
        }

        public string ForesightRuleDelta(RuleChange rule)
        {
            string description = Rules.Single(r => r.ID == rule.ID).Description;
            return ForesightDeltaLine(rule.ID, rule.Is - rule.Was, description);
        }

        public string ForesightDeltaLine(string label, int delta, string description = "")
        {
            if (delta == 0) return string.Empty;

            bool isRegression = delta > 0;
            string changeDirection = isRegression ? "regression" : "improvement";
            delta = Math.Abs(delta);
            string plurality = delta == 1 ? "" : "s";
            
            var decoratedDescription = string.IsNullOrEmpty(description)
                ? ""
                : string.Format(" ({0})", description);

            return string.Format("{0}{1}:  {2} {3}{4}.{5}", label, decoratedDescription, delta, changeDirection, plurality, Environment.NewLine);
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
