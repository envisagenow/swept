//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.IO;

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

        public Arguments( string[] args, IStorageAdapter fileSystem, TextWriter writer )
        {
            Library = string.Empty;
            Folder = string.Empty;
            Exclude = new List<string>();

            if (args.Length == 0)
            {
                writer.Write( UsageMessage );
                return;
            }

            foreach (string s in args)
            {
                string[] tokens = null;

                if (!s.Contains( ":" ))
                    throw new Exception( String.Format( "Don't understand the input [{0}].", s ) );

                tokens = s.Split( ':' );

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
                        Exclude = tokens[1].Split(',');
                        break;
                    case null:
                    case "":
                    default:
                        throw new Exception( String.Format( "Don't recognize the argument [{0}].", tokens[0] ) );
                }
            }

            if( string.IsNullOrEmpty( Library ))
                throw new Exception( "The [library] argument is required." );

            if (String.IsNullOrEmpty( Folder ))
            {
                Folder = fileSystem.GetCWD();
            }

            if ( Folder[1] == ':' && Library[1] != ':')
            {
                Library = Path.Combine( Folder, Library );
            }
        }
    }
}
