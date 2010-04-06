using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace swept
{
    public class Arguments
    {
        public Arguments( string[] args, IStorageAdapter fileSystem )
        {
            Exclude = new List<string>();

            foreach (string s in args)
            {
                string[] tokens = null;

                if (!s.Contains( ':' ))
                    throw new ArgumentException( String.Format( "Don't understand the input [{0}].", s ) );

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
                        throw new ArgumentException( String.Format( "Don't recognize the argument [{0}].", tokens[0] ) );
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

        public string Library { get; private set; }
        public string Folder { get; private set; }
        public IEnumerable<string> Exclude { get; private set; }
    }
}
