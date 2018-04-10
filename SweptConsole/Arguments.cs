//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2015 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace swept
{
    public enum PipeSource { None, SVN }
    public class Arguments
    {
        public bool BreakOnDeltaDrop { get; }
        public string ChangeSet { get; }
        public string ChangesFileName { get; }
        public bool Check { get; }
        public bool TeamCity { get; }
        public string DeltaFileName { get; }
        public string DetailsFileName { get; }
        public List<string> Exclude { get; private set; }
        public string Folder { get; }
        public string History { get; }
        public string Library { get; }
        public PipeSource PipeSource { get; }
        public bool ShowVersion { get; }
        public bool ShowUsage { get; }
        public List<string> SpecifiedRules { get; }
        public bool TrackHistory { get; }
        public bool Foresight { get; }
        public string AdHoc { get; }
        public string Show { get; }
        public int FileCountLimit { get; }
        public List<Pick> Picks { get; }

        public bool AreInvalid
        {
            get { return String.IsNullOrEmpty(Folder); }
        }

        public static string UsageMessage
        {
            get
            {
                return @"Swept usage:
> swept library:my_solution.swept.library details:logs\swept_report.xml
> swept folder:c:\code\project exclude:.svn,bin,build

=== Arguments ===
help:      Or 'h' or 'usage', gets this message.
version:   Prints a brief version and credits message, and terminates.

show:[pattern]  Show all rules ('show'), or a wildcarded subset ('show:ETC*').
rule:      Followed by an ID, runs that rule from the catalog.
tag:       Run rules with a tag, or don't run rules with tag:-that_tag
adhoc:     Followed by a literal rule, runs it in the current directory.

check:     Show a simple text list of failures instead of a detail xml.

folder:    The top folder Swept will sweep for rule violations.
    If no folder is specified, the current working directory is used.
library:   The Swept rules library file to check the files against.
    If no library is specified, Swept checks the top folder for files 
    matching '*.swept.library'.  If it finds exactly one, Swept uses it.
    Swept needs a rules library to run, unless an adhoc argument is given.
exclude:   A comma-separated list of folders Swept will not search
    within.  All folders below these are also excluded.
debug      Triggers a Debugger.Launch(), then continues as usual.

change:    The filename to get the change list since changes were
    last tracked.
details:   Save the detailed XML of the run with the following filename.
history:   The filename to read and update to maintain the delta.
    If no history file is specified, the library filename is used,
    with the '.library' suffix replaced with '.history'.
trackhistory:  Turns on tracking of result history.  Needed to generate
    the delta report.
delta:     The filename to get the delta of red-line rules.
    If no delta file is specified, a text delta report goes to the console.
breakondeltadrop:   If any rule has more violations than earlier by
    count of the delta report, Swept exits with return code 10.
";
            }
        }

        public static object VersionMessage
        {
            get
            {
                return @"Swept version 0.4, Swept core version 0.6.0
Copyright (c) 2009, 2016 Jason Cole and Envisage Technologies Corp.
This software is open source, MIT license.  See the file LICENSE for details.
";
            }
        }

        public Arguments(string[] args, IStorageAdapter storageAdapter)
        {
            BreakOnDeltaDrop = false;
            ChangeSet = "new_commits.xml";
            ChangesFileName = string.Empty;
            Check = false;
            DetailsFileName = string.Empty;
            DeltaFileName = string.Empty;
            Exclude = new List<string>();
            Folder = string.Empty;
            History = string.Empty;
            Library = string.Empty;
            AdHoc = string.Empty;
            ShowUsage = false;
            ShowVersion = false;
            SpecifiedRules = new List<string>();
            TrackHistory = false;
            Foresight = false;
            TeamCity = false;
            FileCountLimit = -1;
            Picks = new List<Pick>();

            List<string> exceptionMessages = new List<string>();

            foreach (string s in args)
            {
                if (!s.Contains(":"))
                {
                    switch (s.ToLower())
                    {
                    case "breakondeltadrop":
                        BreakOnDeltaDrop = true;
                        continue;

                    case "check":
                        Check = true;
                        continue;

                    case "debug":
                        Debugger.Launch();
                        continue;

                    case "version":
                        ShowVersion = true;
                        continue;

                    case "trackhistory":
                        TrackHistory = true;
                        continue;

                    case "foresight":
                        Foresight = true;
                        continue;

                    case "show":
                        Show = "*";
                        continue;

                    case "teamcity":
                        TeamCity = true;
                        continue;

                    case "usage":
                    case "help":
                    case "h":
                    case "/?":
                        ShowUsage = true;
                        return;

                    default:
                        exceptionMessages.Add(String.Format("Don't understand the input [{0}].  Try 'swept h' for help with arguments.", s));
                        continue;
                    }
                }

                // The 2 stops the split from fragmenting values containing colons (filepaths).
                string[] tokens = s.Split(new char[] { ':' }, 2);

                switch (tokens[0].ToLower())
                {
                case "changeset":
                    ChangeSet = tokens[1];
                    break;

                case "changes":
                    ChangesFileName = tokens[1];
                    break;

                case "delta":
                    DeltaFileName = tokens[1];
                    break;

                case "details":
                case "detail":
                    DetailsFileName = tokens[1];
                    break;

                case "exclude":
                    Exclude.AddRange( tokens[1].Split(',') );
                    break;

                case "filelimit":
                    FileCountLimit = int.Parse(tokens[1]);
                    break;

                case "folder":
                    Folder = tokens[1];
                    break;

                case "history":
                    History = tokens[1];
                    break;

                case "id":
                    Picks.Add(new Pick { Domain = PickDomain.ID, Value = tokens[1] });
                    break;

                case "library":
                    Library = tokens[1];
                    break;

                case "pipe":
                    // TODO: Friendly let-down if they have an unrecognized VCS pipe-source
                    PipeSource = (PipeSource)Enum.Parse(typeof(PipeSource), tokens[1], true);
                    break;

                case "rule":
                    if (tokens[1].IndexOf(",") == -1)
                        SpecifiedRules.Add(tokens[1]);
                    else
                        SpecifiedRules.AddRange(tokens[1].Split(','));
                    break;

                case "adhoc":
                    AdHoc = tokens[1].Replace("\"", "");
                    break;

                case "show":
                    Show = tokens[1];
                    break;

                case "tag":
                    Picks.Add(new Pick { Domain = PickDomain.Tag, Value = tokens[1] });
                    break;

                case null:
                case "":
                default:
                    exceptionMessages.Add(String.Format("Don't recognize the argument [{0}].", tokens[0]));
                    break;
                }
            }

            if (String.IsNullOrEmpty(Folder))
            {
                Folder = storageAdapter.GetCWD();
            }

            if (!Folder.Contains(storageAdapter.GetCWD()))
            {
                //  Path.Combine will ignore CWD if Folder is an absolute path
                Folder = Path.Combine(storageAdapter.GetCWD(), Folder);
            }


            if (!ShowVersion && !ShowUsage && exceptionMessages.Any())
                throw new Exception(string.Join("\n", exceptionMessages.ToArray()));


            if (string.IsNullOrEmpty( Library ))
            {
                var possibilities = storageAdapter.GetFilesInFolder( Folder, "*.swept.library" );
                int resultCount = possibilities.Count();

                if (resultCount == 0)
                    exceptionMessages.Add( String.Format( "A library is required for Swept to run.  No library found in folder [{0}].", Folder ) );
                else if (resultCount > 1)
                    exceptionMessages.Add( String.Format( "Too many libraries (*.swept.library) found in folder [{0}].", Folder ) );
                else
                    Library = possibilities.First();
            }

            if (!ShowVersion && !ShowUsage && exceptionMessages.Any())
                throw new Exception(string.Join("\n", exceptionMessages.ToArray()));


            if (string.IsNullOrEmpty(History))
            {
                var candidates = storageAdapter.GetFilesInFolder(Folder, "*.swept.history");
                int candidateCount = candidates.Count();

                if (candidateCount == 1)
                    History = candidates.First();
                else if (candidateCount == 0)
                {
                    History = Regex.Replace(Library, @"\.library", ".history");
                    if (History == Library)
                        History = Library + ".history";
                }
            }
            
            if (!string.IsNullOrEmpty(Folder) && Folder[1] == ':')
            {
                if (!string.IsNullOrEmpty(Library) && Library[1] != ':')
                    Library = Path.Combine(Folder, Library);
                if (!string.IsNullOrEmpty(History) && History[1] != ':')
                    History = Path.Combine(Folder, History);
            }
        }

        public void DisplayMessages(StringWriter writer)
        {
            if (ShowVersion)
                writer.Write(VersionMessage);

            if (ShowUsage)
                writer.Write(UsageMessage);
        }

        public void FillExclusions(List<string> args)
        {
            if (Exclude.Count() == 0)
            {
                Exclude = args;
            }
        }

    }
}
