//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System.Collections.Generic;
using System;

namespace swept
{
    public class SourceFileCatalog
    {
        internal List<SourceFile> Files;
        public ChangeCatalog ChangeCatalog;
        public IGUIAdapter AdaptGUI;

        public SourceFileCatalog()
        {
            Files = new List<SourceFile>();
        }

        public SourceFileCatalog Clone()
        {
            SourceFileCatalog newCatalog = new SourceFileCatalog { ChangeCatalog = ChangeCatalog };

            Files.ForEach( file => newCatalog.Files.Add( file.Clone() ) );

            return newCatalog;
        }

        public void Add( SourceFile file )
        {
            if( Files.Exists( sf => sf.Name == file.Name ) )
                return;

            Files.Add( file );
        }

        public void Remove( string fileName )
        {
            SourceFile file = Find( fileName );
            if( file == null ) return;
            Remove( file );
        }

        public void Remove( SourceFile file )
        {
            file.IsRemoved = true;
        }

        public void Rename( string oldName, string newName )
        {
            SourceFile file = Fetch( oldName );
            file.Name = newName;
        }

        public bool Equals( SourceFileCatalog catalog )
        {
            if( Files.Count != catalog.Files.Count )
                return false;

            int UpperBound = Files.Count;

            for( int i = 0; i < UpperBound; i++ )
            {
                if( !Files[i].Equals( catalog.Files[i] ) )
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

            MaintainHistory( foundFile );

            return foundFile;
        }

        private void MaintainHistory( SourceFile foundFile )
        {
            if( !foundFile.IsRemoved ) return;

            if( !AdaptGUI.KeepSourceFileHistory( foundFile ) )
            {
                foundFile.Completions.Clear();
            }
            foundFile.IsRemoved = false;
        }
    }
}
