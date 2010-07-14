//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace swept
{
    public class Arguments
    {

        public string Library { get; private set; }
        public string Folder { get; private set; }
        public IEnumerable<string> Exclude { get; private set; }

        public bool AreInvalid
        {
            get { return String.IsNullOrEmpty( Folder ); }
        }

        public static string UsageMessage
        {
            get
            {
                return @"SweptConsole usage:
> SweptConsole library:my_solution.swept.library
> SweptConsole folder:c:\work\acadis library:acadis-2008.swept.library exclude:.svn,bin,build
";
            }
        }

        public static object VersionMessage
        {
            get
            {
                return @"SweptConsole version 0.1, Swept core version 0.2.4
Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
This software is open source, MIT license.  See the file LICENSE for details.
";
            }
        }

        public Arguments( string[] args, IStorageAdapter storageAdapter, TextWriter writer )
        {
            Library = string.Empty;
            Folder = string.Empty;
            Exclude = new List<string>();

            if (args.Length == 0)
            {
                writer.Write( UsageMessage );
                return;
            }

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

                    default:
                        exceptionMessages.Add( String.Format( "Don't understand the input [{0}].", s ) );
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

            if (exceptionMessages.Any())
                throw new Exception( string.Join( "\n", exceptionMessages.ToArray() ) );

            if (Folder[1] == ':' && Library[1] != ':')
            {
                Library = Path.Combine( Folder, Library );
            }
        }
    }
}
