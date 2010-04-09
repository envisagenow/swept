//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System.Collections.Generic;
using System.IO;
using System;

namespace swept
{
    public class SourceFile
    {
        public FileLanguage Language;
        public string Name;
        public string Content;
        public int TaskCount;
        public bool IsRemoved { get; set; }

        public SourceFile( string name )
        {
            Name = name;
            Content = string.Empty;

            string fileExt = Path.GetExtension( name );
            Language = SourceFile.FileLanguageFromExtension( fileExt );
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
                    extensionLanguage[""] = FileLanguage.None;
                }
                return extensionLanguage;
            }
        }


        public static FileLanguage FileLanguageFromExtension( string fileExt )
        {
            if( ExtensionLanguage.ContainsKey( fileExt ) )
                return ExtensionLanguage[fileExt];

            return FileLanguage.Unknown;
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
    }
}
