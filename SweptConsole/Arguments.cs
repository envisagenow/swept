//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace swept
{
    public enum PipeSource { None, SVN }

    public class Arguments
    {
        public string Library { get; private set; }
        public string Folder { get; private set; }
        public IEnumerable<string> Exclude { get; private set; }
        public bool Piping { get; private set; }
        public PipeSource PipeSource { get; private set; }

        public string Output { get; private set; }

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
                return @"SweptConsole version 0.3, Swept core version 0.4.2
Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
This software is open source, MIT license.  See the file LICENSE for details.
";
            }
        }

        public Arguments( string[] args, IStorageAdapter storageAdapter, TextWriter writer )
        {
            Library = string.Empty;
            Folder = string.Empty;
            Exclude = new List<string>();
            Output = string.Empty;

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
                        writer.Write( VersionMessage );
                        return;

                    case "usage":
                    case "help":
                    case "h":
                    case "/?":
                        writer.Write( UsageMessage );
                        return;

                    default:
                        exceptionMessages.Add( String.Format( "Don't understand the input [{0}].  Try 'sweptconsole h' for help with arguments.", s ) );
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
                case "folder":
                    Folder = tokens[1];
                    break;
                case "exclude":
                    Exclude = tokens[1].Split( ',' );
                    break;
                case "output":
                    Output = tokens[1];
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

            if (!Exclude.Any())
            {
                Exclude = new string [] { ".svn", "bin", ".gitignore", "lib", "Build", "exslt", "ScormEngineInterface", "FitnesseFixtures" };
            }

            if (exceptionMessages.Any())
                throw new Exception( string.Join( "\n", exceptionMessages.ToArray() ) );

            if (Folder[1] == ':' && Library[1] != ':')
            {
                Library = Path.Combine( Folder, Library );
            }
        }
    }
}
