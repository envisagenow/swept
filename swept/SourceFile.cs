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
        // TODO: consider another place for this
        private static Dictionary<string, FileLanguage> ExtensionLanguage;

        public SourceFile( string name )
        {
            Name = name;
            LineIndices = new List<int>();
            Content = string.Empty;

            string fileExt = Path.GetExtension( name );
            Language = SourceFile.FileLanguageFromExtension( fileExt );
        }

        static SourceFile()
        {
            ExtensionLanguage = new Dictionary<string, FileLanguage>();
            ExtensionLanguage[".cs"] = FileLanguage.CSharp;
            ExtensionLanguage[".cshtml"] = FileLanguage.CSHTML;
            ExtensionLanguage[".css"] = FileLanguage.CSS;
            ExtensionLanguage[".scss"] = FileLanguage.CSS;
            ExtensionLanguage[".html"] = FileLanguage.HTML;
            ExtensionLanguage[".aspx"] = FileLanguage.HTML;
            ExtensionLanguage[".ascx"] = FileLanguage.HTML;
            ExtensionLanguage[".asp"] = FileLanguage.HTML;
            ExtensionLanguage[".htm"] = FileLanguage.HTML;
            ExtensionLanguage[".master"] = FileLanguage.HTML;
            ExtensionLanguage[".js"] = FileLanguage.JavaScript;
            ExtensionLanguage[".csproj"] = FileLanguage.Project;
            ExtensionLanguage[".vbproj"] = FileLanguage.Project;
            ExtensionLanguage[".sln"] = FileLanguage.Solution;
            ExtensionLanguage[".vb"] = FileLanguage.VBNet;
            ExtensionLanguage[".xsl"] = FileLanguage.XSLT;
            ExtensionLanguage[".xslt"] = FileLanguage.XSLT;
            ExtensionLanguage[".sql"] = FileLanguage.SQL;
            ExtensionLanguage[""] = FileLanguage.None;
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
