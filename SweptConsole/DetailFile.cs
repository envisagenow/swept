using System;
using System.Linq;
using System.Collections.Generic;

namespace swept
{
    public class RunDetails
    {
        public int RunNumber = 0;
        public DateTime DateTime = DateTime.MinValue;
        public List<DetailFile> Files = new List<DetailFile>();


        public RunDetails InitializeNextRunDetails()
        {
            RunDetails nextDetails = new RunDetails { RunNumber = this.RunNumber + 1 };

            foreach (var myFile in Files)
            {
                DetailFile newDetailFile = new DetailFile();
                foreach (var myRule in myFile.Rules.Where( r => r.Is > 0 ))
                {
                    newDetailFile.Rules.Add(new DetailRule {
                        ID = myRule.ID,
                        Was = myRule.Is
                    });
                }

                if (newDetailFile.Rules.Count() > 0)
                {
                    newDetailFile.Name = myFile.Name;
                    nextDetails.Files.Add(newDetailFile);
                }
           }

            return nextDetails;
        }

        public void AddThisRun(RuleTasks ruleTasks, DateTime runTime)
        {
            foreach (Rule rule in ruleTasks.Keys)
            {
                FileTasks fileTask = ruleTasks[rule];

                foreach (SourceFile sourcefile in fileTask.Keys)
                {
                    ClauseMatch match = fileTask[sourcefile];

                    DetailFile newDetailFile = new DetailFile { Name = sourcefile.Name };
                    DetailRule newDetailRule = new DetailRule
                    { 
                        ID = rule.ID,
                        Is = match.Count,
                    };
                    newDetailFile.Rules.Add(newDetailRule);
                    this.Files.Add(newDetailFile);

                }

            }
            this.DateTime = runTime;

        }
    }

    public class DetailFile
    {
        public string Name = string.Empty;
        public bool Delta = false;
        public List<DetailRule> Rules = new List<DetailRule>();
    }

    public class DetailRule
    {
        public string ID = string.Empty;
        public int Was = 0;
        public int Is = 0;
        public bool Breaking = false;
    }

}
