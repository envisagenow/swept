//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;

namespace swept
{
    [CoverageExclude( "Wrapper around file system" )]
    public class StorageAdapter : IStorageAdapter
    {
        static internal string emptyCatalogText = 
@"<SweptProjectData>
<RuleCatalog>
</RuleCatalog>
</SweptProjectData>";

        static internal XDocument emptyCatalogDoc
        {
            get
            {
                return XDocument.Parse( emptyCatalogText );
            }
        }

        public XDocument LoadLibrary( string libraryPath )
        {
            if (!File.Exists( libraryPath ))
            {
                return emptyCatalogDoc;
            }

            using (XmlTextReader reader = new XmlTextReader( libraryPath ))
            {
                try
                {
                    return XDocument.Load( reader );
                }
                catch (XmlException xe)
                {
                    throw new Exception( string.Format( "Swept opened file [{0}] for its library, which was not valid XML.  Please check its contents.\n  Details: {1}", libraryPath, xe.Message ) );
                }
            }
        }

        public string GetCWD()
        {
            return Environment.CurrentDirectory;
        }

        public IEnumerable<string> GetFilesInFolder( string folder )
        {
            return Directory.GetFiles( folder );
        }

        public IEnumerable<string> GetFilesInFolder( string folder, string searchPattern )
        {
            return Directory.GetFiles( folder, searchPattern );
        }

        public IEnumerable<string> GetFoldersInFolder( string folder )
        {
            return Directory.GetDirectories( folder );
        }

        public SourceFile LoadFile( string fileName )
        {
            try
            {
                SourceFile file = new SourceFile( fileName );
                file.Content = File.ReadAllText( fileName );
                return file;
            }
            catch
            {
                Console.Out.WriteLine( String.Format( "LoadFile( {0} ) failed.", fileName ) );
                return null;
            }
        }

        public void SaveRunChanges(XDocument runChanges, string fileName)
        {
            runChanges.Save(fileName);
        }

        public void SaveRunHistory(XDocument runHistory, string fileName)
        {
            runHistory.Save( fileName );
        }

        public XDocument LoadRunHistory( string historyPath )
        {
            var historyText = File.ReadAllText( historyPath );
            return XDocument.Parse( historyText );
        }

        public XDocument LoadChangeSet( string changeSetPath )
        {
            var changeText = File.ReadAllText( changeSetPath );
            return XDocument.Parse( changeText );
        }

        public TextWriter GetOutputWriter( string outputLocation )
        {
            if (string.IsNullOrEmpty( outputLocation ))
                return Console.Out;

            return new StreamWriter( File.Create(outputLocation) );
        }
    }
}
