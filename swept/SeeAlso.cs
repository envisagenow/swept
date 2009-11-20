using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace swept
{
    public enum TargetType
    {
        URL,
        File,
        SVN
    }

    public class SeeAlso
    {
        public string Description { get; set; }

        public string Commit { get; set; }

        private string _target;
        public string Target 
        {
            get { return _target; }
            set
            {
                _target = value;
                string[] targetParts = value.Split(':' );
                string prefix = string.Empty;
                if (targetParts.Length > 0)
                {
                    prefix = targetParts[0].ToLower();
                }

                // TODO: svn can work over http protocol, so this won't be enough long-term.  move to deducing type attrib from Target label
                switch (prefix)
                {
                    case "https":
                    case "http":
                        TargetType = TargetType.URL;
                        break;
                    case "file":
                        TargetType = TargetType.File;
                        break;
                    case "svn":
                        TargetType = TargetType.SVN;
                        break;
                    default:
                        throw new ApplicationException( string.Format("Swept doesn't understand the TargetType of [{0}].", prefix) );
                }
            }
        }
        public TargetType TargetType { get; private set; }

        public SeeAlso() { }
        
        public string SVN { get; set; }
        public string ProjectFile { get; set; }
        public string URL { get; set; }
    }

}
