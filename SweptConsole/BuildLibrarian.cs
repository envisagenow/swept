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

        public RunChanges ReadRunChanges()
        {
            return ReadRunChanges(_storage.LoadRunChanges(_args.ChangesFileName));
        }

        public RunChanges ReadRunChanges(XDocument doc)
        {
            var result = new RunChanges();

            if (doc.Root == null)
                return result;

            result.RunNumber = int.Parse(doc.Root.Attribute("RunNumber").Value);
            result.CurrentDateTime = DateTime.Parse(doc.Root.Attribute("CurrentDateTime").Value);
            result.PreviousDateTime = DateTime.Parse(doc.Root.Attribute("PreviousDateTime").Value);

            foreach (var fileXml in doc.Descendants("Files").Descendants("File"))
            {
                var file = ReadFileChange(fileXml);
                result.Files.Add(file);
            }

            foreach (var ruleXml in doc.Descendants("Rules").Descendants("Rule"))
            {
                var ruleDesc = ReadRuleDescription(ruleXml);
                result.Rules.Add(ruleDesc);
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

        public RuleDescription ReadRuleDescription(XElement ruleDescXml)
        {
            RuleDescription result = new RuleDescription();

            result.ID = ruleDescXml.Attribute("ID").Value;
            result.Description = ruleDescXml.Attribute("Description").Value;

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

        public void WriteRunChangesDoc(RunChanges runChanges, List<Rule> rules)
        {
            var runChangesDoc = BuildRunChangesDoc(runChanges, rules);

            _storage.SaveRunChanges(runChangesDoc, _args.ChangesFileName);
        }

        public XDocument BuildRunChangesDoc(RunChanges runChanges, List<Rule> rules)
        {
            return new XDocument(
                new XElement("RunChanges", 
                    new XAttribute("RunNumber", runChanges.RunNumber),
                    new XAttribute("CurrentDateTime", runChanges.CurrentDateTime.ToString()),
                    new XAttribute("PreviousDateTime", runChanges.PreviousDateTime.ToString()),
                    BuildRunChangeRules(rules),
                    BuildRunChangeFiles(runChanges.Files)
            ));
        }

        public static XElement BuildRunChangeRules(List<Rule> rules)
        {
            return new XElement("Rules",
                from rule in rules
                select new XElement("Rule",
                    new XAttribute("ID", rule.ID),
                    new XAttribute("Description", rule.Description)
            ));
        }

        public static XElement BuildRunChangeFiles(List<FileChange> fileChanges)
        {
            return new XElement("Files",
                from file in fileChanges
                select new XElement("File",
                    new XAttribute("Name", file.Name),
                    new XAttribute("Changed", file.Changed),
                    from rule in file.Rules.OrderBy(r => r.ID)
                    select new XElement("Rule",
                        new XAttribute("ID", rule.ID),
                        new XAttribute("Was", rule.Was),
                        new XAttribute("Is", rule.Is)
                    )
                )
            );
        }

    }
}
