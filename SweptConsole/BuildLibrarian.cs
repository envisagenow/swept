//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2015 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace swept
{
    public class BuildLibrarian
    {
        private readonly IStorageAdapter _storage;
        private readonly Arguments _args;

        public BuildLibrarian(Arguments args, IStorageAdapter storage)
        {
            _args = args;
            _storage = storage;
        }

        public RunChanges ReadRunChanges(XDocument doc)
        {
            var result = new RunChanges();

            if (doc.Root == null)
                return result;

            result.RunNumber = int.Parse(doc.Root.Attribute("RunNumber").Value);
            result.DateTime = DateTime.Parse(doc.Root.Attribute("DateTime").Value);

            foreach (var fileXml in doc.Descendants("Files").Descendants("File"))
            {
                var file = ReadFileChange(fileXml);
                result.Files.Add(file);

            }

            return result;
        }

        public FileChange ReadFileChange(XElement fileXml)
        {
            FileChange result = new FileChange();

            result.Name = fileXml.Attribute("Name").Value;
            result.Changed = bool.Parse(fileXml.Attribute("Changed").Value);

            foreach (var ruleXml in fileXml.Descendants("Rule"))
            {
                var rule = new RuleChange();
                rule.ID = ruleXml.Attribute("ID").Value;
                rule.Was = int.Parse(ruleXml.Attribute("Was").Value);
                rule.Is = int.Parse(ruleXml.Attribute("Is").Value);
                result.Rules.Add(rule);
            }

            return result;
        }


        public List<Commit> ReadChangeSet()
        {
            XDocument doc;
            try
            {
                doc = _storage.LoadChangeSet(_args.ChangeSet);
            }
            catch (FileNotFoundException)
            {
                doc = new XDocument(new XElement("new_commits"));
            }
            return ReadChangeSet(doc);
        }


        public List<Commit> ReadChangeSet(XDocument changeDoc)
        {
            var changes = new List<Commit>();

            foreach (var changeElem in changeDoc.Descendants("commit"))
            {
                var change = new Commit
                {
                    //<commit id='r46816' person='brian.eckelman' time='2013-08-07 11:24:34 -0400 (Wed, 07 Aug 2013)' />
                    ID = changeElem.Attribute("id").Value,
                    Person = changeElem.Attribute("person").Value,
                    Time = changeElem.Attribute("time").Value,
                };
                changes.Add(change);
            }

            return changes;
        }

        public RunHistory ReadRunHistory()
        {
            XDocument doc;
            try
            {
                doc = _storage.LoadRunHistory(_args.History);
            }
            catch (FileNotFoundException)
            {
                doc = new XDocument();
            }
            return ParseRunHistory(doc);
        }

        public RunHistory ParseRunHistory(XDocument historyXml)
        {
            RunHistory runHistory = new RunHistory();

            foreach (var runXml in historyXml.Descendants("Run"))
            {
                var run = ParseRunEntry(runXml);
                runHistory.AddEntry(run);
            }

            return runHistory;
        }

        public RunEntry ParseRunEntry(XElement runXml)
        {
            int number = int.Parse(runXml.Attribute("Number").Value);
            DateTime dateTime = DateTime.Parse(runXml.Attribute("DateTime").Value);
            bool passed = bool.Parse(runXml.Attribute("Passed").Value);

            RunEntry run = new RunEntry { Number = number, Date = dateTime, Passed = passed };

            foreach (var ruleXml in runXml.Descendants("Rule"))
            {
                var rule = ParseRuleResult(ruleXml);
                run.RuleResults[rule.ID] = rule;
            }

            foreach (var flagXml in runXml.Descendants("Flag"))
            {
                var flag = ParseFlag(flagXml);
                run.Flags.Add(flag);
            }

            return run;
        }

        public RuleResult ParseRuleResult(XElement ruleXml)
        {
            string id = ruleXml.Attribute("ID").Value;
            int taskCount = int.Parse(ruleXml.Attribute("TaskCount").Value);
            int threshold = int.Parse(ruleXml.Attribute("Threshold").Value);
            bool breaking = bool.Parse(ruleXml.Attribute("Breaking").Value);
            RuleFailOn ruleFailOn = (RuleFailOn)Enum.Parse(typeof(RuleFailOn), ruleXml.Attribute("FailOn").Value);
            string description = ruleXml.Attribute("Description").Value;

            return new RuleResult
            {
                ID = id,
                TaskCount = taskCount,
                Threshold = threshold,
                Breaking = breaking,
                FailOn = ruleFailOn,
                Description = description
            };
        }

        public Flag ParseFlag(XElement flagXml)
        {
            string id = flagXml.Attribute("RuleID").Value;
            int taskCount = int.Parse(flagXml.Attribute("TaskCount").Value);
            int threshold = int.Parse(flagXml.Attribute("Threshold").Value);

            Flag flag = new Flag { RuleID = id, TaskCount = taskCount, Threshold = threshold };

            foreach (var commitXml in flagXml.Descendants("Commit"))
            {
                var commit = ParseCommit(commitXml);
                flag.Commits.Add(commit);
            }

            return flag;
        }

        public Commit ParseCommit(XElement commitXml)
        {
            string ID = commitXml.Attribute("ID").Value;
            string person = commitXml.Attribute("Person").Value;
            string time = commitXml.Attribute("Time").Value;

            return new Commit { ID = ID, Person = person, Time = time };
        }


        public void WriteRunHistory(RunHistory runHistory)
        {
            if (!_args.TrackHistory)
                return;

            var report = new XDocument();

            XElement report_root = new XElement("RunHistory");
            report.Add(report_root);

            foreach (var run in runHistory.Runs)
            {
                var runElement = new XElement("Run",
                    new XAttribute("Number", run.Number),
                    new XAttribute("DateTime", run.Date.ToString()),
                    new XAttribute("Passed", run.Passed.ToString())
                );

                foreach (RuleResult result in run.RuleResults.Values.OrderBy(r => r.ID))
                {
                    var ruleElement = new XElement("Rule",
                        new XAttribute("ID", result.ID),
                        new XAttribute("TaskCount", result.TaskCount),
                        new XAttribute("Threshold", result.Threshold),
                        new XAttribute("FailOn", result.FailOn),
                        new XAttribute("Breaking", result.Breaking),
                        new XAttribute("Description", result.Description)
                    );

                    runElement.Add(ruleElement);
                    foreach (Flag flag in run.Flags.OrderBy(f => f.RuleID))
                    {
                        var flagElement = new XElement("Flag",
                            new XAttribute("RuleID", flag.RuleID),
                            new XAttribute("TaskCount", flag.TaskCount),
                            new XAttribute("Threshold", flag.Threshold));

                        foreach (Commit commit in flag.Commits)
                        {
                            var commitElment = new XElement("Commit",
                                new XAttribute("ID", commit.ID),
                                new XAttribute("Person", commit.Person),
                                new XAttribute("Time", commit.Time));

                            flagElement.Add(commitElment);
                        }
                        runElement.Add(flagElement);
                    }
                }

                report_root.Add(runElement);
            }

            _storage.SaveRunHistory(report, _args.History);
        }

        private string oo = @"<RunChanges RunNumber=""22"" DateTime=""4/4/2012 10:25:02 AM"">
    <Rules>
        <Rule ID=""INT-012"" Description=""This is strimly po tent."" />
    </Rules>
    <Files>
        <File Name=""foo.cs"">
            <Rule ID=""INT-012"" Was=""2"" Is=""5"" Breaking=""false"" />
        </File>
    </Files>
</RunChanges>
";
        public void WriteRunChanges(RunChanges runChanges, RuleCatalog ruleCatalog)
        {
            var ruleDescriptions = new Dictionary<string, string>();
            foreach (var rule in ruleCatalog.GetSortedRules())
                ruleDescriptions[rule.ID] = rule.Description;

            var report_root = new XElement("RunChanges");
            var rules = new XElement("Rules");
            var files = new XElement("Files");
            report_root.Add(rules, files);

            foreach (var fileChange in runChanges.Files)
            {
                var fileElement = new XElement("File",
                    new XAttribute("Name", fileChange.Name)
                );

                foreach (var ruleChange in fileChange.Rules.OrderBy(r => r.ID))
                {
                    var ruleElement = new XElement("Rule",
                        new XAttribute("ID", ruleChange.ID),
                        new XAttribute("Was", ruleChange.Was),
                        new XAttribute("Is", ruleChange.Is)
                    );

                    fileElement.Add(ruleElement);
                }

                files.Add(fileElement);
            }

            foreach (var rule in ruleCatalog.GetSortedRules())
            {
                rules.Add(new XElement("Rule",
                    new XAttribute("ID", rule.ID),
                    new XAttribute("Description", rule.Description)
                ));
            }

            var report = new XDocument(report_root);
            _storage.SaveRunChanges(report, _args.History);
        }
    }
}
