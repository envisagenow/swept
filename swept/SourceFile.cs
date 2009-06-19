//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System;

namespace swept
{
    public class SourceFile
    {
        public FileLanguage Language;
        public string Name;
        internal List<Completion> Completions;

        public SourceFile( string name )
        {
            Name = name;

            string fileExt = Path.GetExtension( name );
            Language = SourceFile.FileLanguageFromExtension( fileExt );
            Completions = new List<Completion>();
        }

        private static Dictionary<string, FileLanguage> extensionLanguage = null;
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

        //TODO: refactor into SourceFile.Clone( file )
        public void CopyCompletionsFrom( SourceFile workingFile )
        {
            Completions.Clear();
            workingFile.Completions.ForEach( c => Completions.Add( c.Clone() ) );
        }

        public void AdjustCompletionFrom(Task alteredTask)
        {
            if (alteredTask.Completed)
                Completions.Add(new Completion(alteredTask.ID));
            else
                Completions.RemoveAll(c => c.ChangeID == alteredTask.ID);
        }

        public void AddNewCompletion( string changeID )
        {
            Completions.Add( new Completion( changeID ) );
        }


        #region Serialization
        public static SourceFile FromNode( XmlNode xmlNode )
        {
            if( xmlNode == null )
                throw new Exception( "Can't create a null source file." );

            if( xmlNode.Attributes["Name"] == null )
                throw new Exception( "A SourceFile node must have a Name attribute.  Please add one." );

            SourceFile file = new SourceFile( xmlNode.Attributes["Name"].Value );
            foreach( XmlNode completionNode in xmlNode.SelectNodes( "Completion" ) )
            {
                Completion comp = Completion.FromNode( completionNode );
                file.Completions.Add( comp );
            }
            return file;
        }

        //FUTURE: Sort Completions when writing them out
        public string ToXmlText()
        {
            string elementLabel = "SourceFile";
            string xmlText = String.Format( "    <{0} Name='{1}'>\r\n", elementLabel, Name );
            Completions.ForEach( c => xmlText += c.ToXmlText() );
            xmlText += String.Format( "    </{0}>\r\n", elementLabel );

            return xmlText;
        }
        #endregion Serialization
    }
}
