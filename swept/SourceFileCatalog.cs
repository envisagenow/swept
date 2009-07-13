//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System;

namespace swept
{
    public class SourceFileCatalog
    {
        internal List<SourceFile> Files;
        public ChangeCatalog ChangeCatalog;

        public SourceFileCatalog()
        {
            Files = new List<SourceFile>();
        }

        public static SourceFileCatalog Clone( SourceFileCatalog parent )
        {
            SourceFileCatalog newCatalog = new SourceFileCatalog();

            newCatalog.ChangeCatalog = parent.ChangeCatalog;
            foreach (SourceFile file in parent.Files)
            {
                SourceFile newSourceFile = new SourceFile(file.Name);
                newSourceFile.CopyCompletionsFrom(file);
                newCatalog.Files.Add(newSourceFile);
            }

            return newCatalog;
        }

        public void Add(SourceFile file)
        {
            if (Files.Exists(sf => sf.Name == file.Name))
                return;
            
            Files.Add(file);
        }

        public void Delete( string fileName )
        {
            SourceFile file = Find( fileName );
            if( file == null ) return;
            Files.Remove( file );
        }

        public void Rename(string oldName, string newName)
        {
            SourceFile file = Fetch(oldName);
            file.Name = newName;
        }

        public bool Equals(SourceFileCatalog catalog)
        {
            if (Files.Count != catalog.Files.Count)
                return false;

            int UpperBound = Files.Count;

            for (int i = 0; i < UpperBound; i++)
            {
                if (!Files[i].Equals(catalog.Files[i]))
                    return false;    
            }

            return true;
        }

        private SourceFile Find( string name )
        {
            return Files.Find( f => f.Name == name );
        }

        internal SourceFile Fetch( string name )
        {
            SourceFile foundFile = Find( name );

            if( foundFile == null )
            {
                foundFile = new SourceFile( name );
                Files.Add( foundFile );
            }

            return foundFile;
        }

    }
}
