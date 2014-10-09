//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
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
        public bool BreakOnDeltaDrop { get; private set; }
        public string ChangeSet { get; private set; }
        public bool Check { get; private set; }
        public string DeltaFileName { get; private set; }
        public string DetailsFileName { get; private set; }
        public IEnumerable<string> Exclude { get; private set; }
        public string Folder { get; private set; }
        public string History { get; private set; }
        public string Library { get; private set; }
        public PipeSource PipeSource { get; private set; }
        public bool ShowVersion { get; private set; }
        public bool ShowUsage { get; private set; }
        public List<string> SpecifiedRules { get; private set; }
        public bool TrackHistory { get; set; }
        public string AdHoc { get; set; }

        public bool AreInvalid
        {
            get { return String.IsNullOrEmpty( Folder ); }
        }

        public static string UsageMessage
        {
            get
            {
                return @"Swept usage:
> swept library:my_solution.swept.library details:logs\swept_report.xml
> swept folder:c:\code\project exclude:.svn,bin,build
  Arguments:
    help:      Or 'h' or 'usage', gets this message.
    version:   Prints a brief version and credits message, and terminates.
    debug:     Triggers a Debugger.Launch(), then continues as usual.
    rule:      Followed by an ID, runs that rule from the catalog.
    adhoc:     Followed by a literal rule, runs it in the current directory.
    folder:    The top folder Swept will sweep for rule violations.
      If no folder is specified, the current working directory is used.
    library:   The Swept rules library file to check against.
      If no library is specified, Swept checks the top folder for a file 
      named '*.swept.library'.  If it finds exactly one, Swept will use it.
      Swept needs a library to run.
    exclude:   A comma-separated list of folders Swept will not search
      within.  All folders below these are also excluded.
    pipe:svn:  Indicates that standard In will contain the output from an
      svn status command, and these files will be used as the file list
      to search for violations.
    details:   The filename to get the detailed XML of the run.
    history:   The filename to read and update to maintain the delta.
      If no history file is specified, the library filename is used,
      with the '.library' suffix replaced with '.history'.
    delta:     The filename to get the delta of red-line rules.
      If no delta file is specified, a text delta report goes to the console.
    trackhistory:  Turns on tracking of result history.
---
Features below are Not Yet Implemented:
*** files:  A comma-separated list of files to search for violations.  Not
      compatible with the 'folder' argument.
";
            }
        }

        public static object VersionMessage
        {
            get
            {
                return @"Swept version 0.4, Swept core version 0.6.0
Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
This software is open source, MIT license.  See the file LICENSE for details.
";
            }
        }

        public Arguments( string[] args, IStorageAdapter storageAdapter )
        {
            BreakOnDeltaDrop = false;
            ChangeSet = "new_commits.xml";
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

            List<string> exceptionMessages = new List<string>();

            foreach (string s in args)
            {
                if (!s.Contains( ":" ))
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

                    case "usage":
                    case "help":
                    case "h":
                    case "/?":
                        ShowUsage = true;
                        return;                        
                    case "trackhistory":
                        TrackHistory = true;
                        continue;

                    default:
                        exceptionMessages.Add( String.Format( "Don't understand the input [{0}].  Try 'swept h' for help with arguments.", s ) );
                        continue;
                    }
                }

                string[] tokens = s.Split( ':' );

                if (tokens.Length > 2)
                {
                    tokens[1] += ":" + tokens[2];
                }

                switch (tokens[0])
                {
                case "changeset":
                    ChangeSet = tokens[1];
                    break;

                case "delta":
                    DeltaFileName = tokens[1];
                    break;

                case "details":
                case "detail":
                    DetailsFileName = tokens[1];
                    break;

                case "exclude":
                    Exclude = tokens[1].Split( ',' );
                    break;

                case "folder":
                    Folder = tokens[1];
                    break;

                case "history":
                    History = tokens[1];
                    break;

                case "library":
                    Library = tokens[1];
                    break;

                case "pipe":
                    // TODO: Friendly let-down if they have an unrecognized VCS pipe-source
                    PipeSource = (PipeSource)Enum.Parse( typeof( PipeSource ), tokens[1], true );
                    break;

                case "rule":
                    if (tokens[1].IndexOf(",") == -1)
                        SpecifiedRules.Add( tokens[1] );
                    else
                        SpecifiedRules.AddRange( tokens[1].Split( ',' ) );
                    break;

                case "adhoc":
                    AdHoc = tokens[1].Replace("\"", "");
                    break;

                case null:
                case "":
                default:
                    exceptionMessages.Add( String.Format( "Don't recognize the argument [{0}].", tokens[0] ) );
                    break;
                }
            }

            if (String.IsNullOrEmpty( Folder ))
            {
                Folder = storageAdapter.GetCWD();
            }

            if (!Folder.Contains( storageAdapter.GetCWD() ))
            {
                Folder = Path.Combine( storageAdapter.GetCWD() , Folder);
            }

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

            if (string.IsNullOrEmpty( History ))
            {
                var candidates = storageAdapter.GetFilesInFolder( Folder, "*.swept.history" );
                int candidateCount = candidates.Count();

                if (candidateCount == 1)
                    History = candidates.First();
                else if (candidateCount == 0)
                {
                    History = Regex.Replace( Library, "library", "history" );
                    if (History == Library)
                        History = Library + ".history";
                }
            }

            if (!ShowVersion && !ShowUsage && exceptionMessages.Any())
                throw new Exception( string.Join( "\n", exceptionMessages.ToArray() ) );

            if (!string.IsNullOrEmpty( Folder ) && Folder[1] == ':')
            {
                if (!string.IsNullOrEmpty( Library ) && Library[1] != ':')
                    Library = Path.Combine( Folder, Library );
                if (!string.IsNullOrEmpty( History ) && History[1] != ':')
                    History = Path.Combine( Folder, History );
            }
        }

        public void DisplayMessages( StringWriter writer )
        {
            if (ShowVersion)
                writer.Write( VersionMessage );

            if (ShowUsage)
                writer.Write( UsageMessage );
        }

        public void FillExclusions( List<string> args )
        {
            if ( Exclude.Count() == 0 )
            {
                Exclude = args;
            }
        }


    }
}
