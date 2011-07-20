//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
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
        public string Name;

        public List<int> LineIndices { get; private set; }

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

        private static Dictionary<string, FileLanguage> extensionLanguage;
        private static Dictionary<string, FileLanguage> ExtensionLanguage
        {
            get
            {
                if( extensionLanguage == null )
                {
                    extensionLanguage = new Dictionary<string, FileLanguage>();
                    extensionLanguage[".cs"] = FileLanguage.CSharp;
                    extensionLanguage[".vb"] = FileLanguage.VBNet;
                    extensionLanguage[".html"] = FileLanguage.HTML;
                    extensionLanguage[".aspx"] = FileLanguage.HTML;
                    extensionLanguage[".ascx"] = FileLanguage.HTML;
                    extensionLanguage[".asp"] = FileLanguage.HTML;
                    extensionLanguage[".htm"] = FileLanguage.HTML;
                    extensionLanguage[".js"] = FileLanguage.JavaScript;
                    extensionLanguage[".css"] = FileLanguage.CSS;
                    extensionLanguage[".xsl"] = FileLanguage.XSLT;
                    extensionLanguage[".xslt"] = FileLanguage.XSLT;
                    extensionLanguage[".csproj"] = FileLanguage.Project;
                    extensionLanguage[".vbproj"] = FileLanguage.Project;
                    extensionLanguage[".sln"] = FileLanguage.Solution;
                    extensionLanguage[""] = FileLanguage.None;
                }
                return extensionLanguage;
            }
        }

        public SourceFile Clone()
        {
            SourceFile file = new SourceFile( Name );

            // TODO: clone other attrs?  Figure out how to automagic this.
            
            return file;
        }

        public bool Equals(SourceFile file)
        {
            if( Name != file.Name ) return false;
            // TODO: expand equality?  Automagic?

            return true;
        }

        public static FileLanguage FileLanguageFromExtension( string fileExt )
        {
            if (ExtensionLanguage.ContainsKey( fileExt ))
                return ExtensionLanguage[fileExt];

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
