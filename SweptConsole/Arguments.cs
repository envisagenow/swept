using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace swept
{
    public class Arguments
    {
        public Arguments( string[] args )
        {


            foreach (string s in args)
            {
                string[] pair = null;

                if (!s.Contains( ':' ))
                    throw new ArgumentException( String.Format( "Don't understand the argument [{0}].", s ) );

                pair= s.Split( ':' );


                switch (pair[0])
                {
                    case "library":
                        Library = pair[1];
                        break;
                    case "folder":
                        Folder = pair[1];
                        if (pair.Length > 2)
                            Folder += ":" + pair[2];
                        break;
                    case "exclude":
                        Exclude = pair[1].Split(',');
                        break;
                    case null:
                    case "":
                    default:
                        throw new ArgumentException( String.Format( "Don't recognize the argument [{0}].", pair[0] ) );
                }
            }
        }

        public string Library { get; private set; }
        public string Folder { get; private set; }
        public IEnumerable<string> Exclude { get; private set; }
    }
}
