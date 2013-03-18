//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
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
        public string Library { get; private set; }
        public string History { get; private set; }
        public string Folder { get; private set; }
        public IEnumerable<string> Exclude { get; private set; }
        public bool Piping { get; private set; }
        public PipeSource PipeSource { get; private set; }
        public bool Check { get; private set; }
        public bool ShowVersion { get; private set; }
        public bool ShowUsage { get; private set; }

        public string DetailsFileName { get; private set; }
        public string SummaryFileName { get; private set; }

        public bool AreInvalid
        {
            get { return String.IsNullOrEmpty( Folder ); }
        }

        public static string UsageMessage
        {
            get
            {
                return @"SweptConsole usage:
> SweptConsole library:my_solution.swept.library output:logs\swept_report.xml
> SweptConsole folder:c:\code\project exclude:.svn,bin,build
  Arguments:
    help:  Or 'h' or 'usage', gets this message.
    version:  Prints a brief version and credits message, and terminates.
    debug:  Triggers a Debugger.Launch(), then continues as usual.
    folder:  The top folder SweptConsole will sweep for rule violations.
        If no folder is specified, the current working directory is used.
    library:  The Swept rules library file to check against.
        If no library is specified, Swept checks the top folder for a file 
        named '*.swept.library'.  If it finds exactly one, Swept will use it.
    exclude:  A comma-separated list of folders Swept will not search
        within.  All folders below these are also excluded.
    pipe:svn:  Indicates that standard In will contain the output from an
        svn status command, and these files will be used as the file list
        to search for violations.
---
Features below are Not Yet Implemented:
NYI output:  The file that will get the output of SweptConsole.  If none is
        specified, the report goes to standard Out.
NYI files:  A comma-separated list of files to search for violations.  Not
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
            Library = string.Empty;
            History = string.Empty;
            Folder = string.Empty;
            Exclude = new List<string>();
            DetailsFileName = string.Empty;
            SummaryFileName = string.Empty;
            Check = false;
            ShowUsage = false;
            ShowVersion = false;

            List<string> exceptionMessages = new List<string>();

            foreach (string s in args)
            {
                if (!s.Contains( ":" ))
                {
                    switch (s)
                    {
                    case "debug":
                        Debugger.Launch();
                        continue;

                    case "version":
                        ShowVersion = true;
                        return;

                    case "usage":
                    case "help":
                    case "h":
                    case "/?":
                        ShowUsage = true;
                        return;

                    case "check":
                        Check = true;
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
                case "library":
                    Library = tokens[1];
                    break;
                case "history":
                    History = tokens[1];
                    break;
                case "folder":
                    Folder = tokens[1];
                    break;
                case "exclude":
                    Exclude = tokens[1].Split( ',' );
                    break;
                case "details":
                case "detail":
                    DetailsFileName = tokens[1];
                    break;
                case "summary":
                    SummaryFileName = tokens[1];
                    break;
                case "pipe":
                    // TODO: Friendly let-down if they have an unrecognized VCS pipe-source
                    Piping = true;
                    PipeSource = (PipeSource)Enum.Parse( typeof( PipeSource ), tokens[1], true );
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

            if (string.IsNullOrEmpty( Library ))
            {
                var possibilities = storageAdapter.GetFilesInFolder( Folder, "*.swept.library" );
                int resultCount = possibilities.Count();

                if (resultCount == 0)
                    exceptionMessages.Add( String.Format( "No library found in folder [{0}].", Folder ) );
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

            if (!Exclude.Any())
            {
                Exclude = new string [] { ".svn", "bin", ".gitignore", "lib", "Build", "exslt", "ScormEngineInterface", "FitnesseFixtures" };
            }

            if (exceptionMessages.Any())
                throw new Exception( string.Join( "\n", exceptionMessages.ToArray() ) );

            if (!string.IsNullOrEmpty( Folder ) && Folder[1] == ':')
            {
                if (Library[1] != ':')
                    Library = Path.Combine( Folder, Library );
                if (History[1] != ':')
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
    }
}
