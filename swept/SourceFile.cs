//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace swept
{
    public class SourceFile
    {
        public SourceFile( string name )
        {
            Name = name;
            LineIndices = new List<int>();
            Content = string.Empty;

            string fileExt = Path.GetExtension( name );
            Language = SourceFile.FileLanguageFromExtension( fileExt );
        }

        public FileLanguage Language;
        public List<int> LineIndices { get; private set; }
        public string Name;

        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                LineIndices = GetLineIndices( _content );
            }
        }

        // TODO: consider another place for this
        private static Dictionary<string, FileLanguage> extensionLanguage;
        private static Dictionary<string, FileLanguage> ExtensionLanguage
        {
            get
            {
                if( extensionLanguage == null )
                {
                    extensionLanguage = new Dictionary<string, FileLanguage>();
                    extensionLanguage[".cs"] = FileLanguage.CSharp;
                    extensionLanguage[".css"] = FileLanguage.CSS;
                    extensionLanguage[".html"] = FileLanguage.HTML;
                    extensionLanguage[".aspx"] = FileLanguage.HTML;
                    extensionLanguage[".ascx"] = FileLanguage.HTML;
                    extensionLanguage[".asp"] = FileLanguage.HTML;
                    extensionLanguage[".htm"] = FileLanguage.HTML;
                    extensionLanguage[".master"] = FileLanguage.HTML;
                    extensionLanguage[".js"] = FileLanguage.JavaScript;
                    extensionLanguage[".csproj"] = FileLanguage.Project;
                    extensionLanguage[".vbproj"] = FileLanguage.Project;
                    extensionLanguage[".sln"] = FileLanguage.Solution;
                    extensionLanguage[".vb"] = FileLanguage.VBNet;
                    extensionLanguage[".xsl"] = FileLanguage.XSLT;
                    extensionLanguage[".xslt"] = FileLanguage.XSLT;
                    extensionLanguage[""] = FileLanguage.None;
                }
                return extensionLanguage;
            }
        }

        // TODO: Check periodically that this is correct
        public bool Equals( SourceFile file )
        {
            return Name.Equals( file.Name );
        }

        // TODO: Larger issue with Clone(), Equals(file), and ilk:  As class grows,
        //  they become out of date, or perhaps they just _look_ like it, since added
        //  members will not always be wanted in their implementations.  What practices
        //  or conventions will help keep them up to date, and make it prima facie
        //  clear that they're up to date?
        //  One way would be to have attributes for the states applied to the members
        //  themselves--whether they're in or out for each of these member-surface methods.
        //  That documents the intent near the members, though using those attributes
        //  to _make it happen_ is another, larger task.  Also unlikely to be performant,
        //  if reflective, in enough cases.

        public static FileLanguage FileLanguageFromExtension( string fileExt )
        {
            string lowerExt = fileExt.ToLower();
            if (ExtensionLanguage.ContainsKey( lowerExt ))
                return ExtensionLanguage[lowerExt];

            return FileLanguage.Unknown;
        }

        internal List<int> GetLineIndices( string content )
        {
            //list of newline indexes
            Regex lineCatcher = new Regex( "\n", RegexOptions.Multiline );
            MatchCollection lineMatches = lineCatcher.Matches( content );

            var indices = new List<int>();
            foreach (Match match in lineMatches)
            {
                indices.Add( match.Index );
            }

            return indices;
        }
    }
}
