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
        internal List<Completion> Completions;
        public bool IsRemoved { get; set; }

        public SourceFile( string name )
        {
            Name = name;

            string fileExt = Path.GetExtension( name );
            Language = SourceFile.FileLanguageFromExtension( fileExt );
            Completions = new List<Completion>();
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

        public void MarkCompleted( string changeID )
        {
            if( !Completions.Exists( c => c.ChangeID == changeID ) )
                Completions.Add( new Completion( changeID ) );
        }

        public SourceFile Clone()
        {
            SourceFile file = new SourceFile( Name );

            file.CopyCompletionsFrom( this );
            
            return file;
        }

        //TODO--0.3, DC: refactor into SourceFile.Clone( file )
        public void CopyCompletionsFrom( SourceFile workingFile )
        {
            Completions.Clear();
            workingFile.Completions.ForEach( c => Completions.Add( c.Clone() ) );
        }

        public void AdjustCompletionFrom(Task alteredTask)
        {
            Completions.RemoveAll( c => c.ChangeID == alteredTask.ID );
            if (alteredTask.Completed)
                Completions.Add(new Completion(alteredTask.ID));
        }

        public void AddNewCompletion( string changeID )
        {
            Completions.Add( new Completion( changeID ) );
        }

        public bool Equals(SourceFile file)
        {
            if( Name != file.Name ) return false;

            if (Completions.Count != file.Completions.Count)
                return false;

            int UpperBound = Completions.Count;

            for (int i = 0; i < UpperBound; i++)
            {
                if (!Completions[i].ChangeID.Equals(file.Completions[i].ChangeID))
                    return false;
            }

            return true;
        }
    }
}
