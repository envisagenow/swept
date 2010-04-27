//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System.Collections.Generic;
using System;
using System.IO;

namespace swept
{
    public class SourceFileCatalog
    {
        internal List<SourceFile> Files;
        public ChangeCatalog ChangeCatalog;
        public IUserAdapter UserAdapter;

        public string SolutionPath { get; set; }

        public SourceFileCatalog()
        {
            Files = new List<SourceFile>();
        }

        public SourceFileCatalog Clone()
        {
            SourceFileCatalog newCatalog = new SourceFileCatalog { ChangeCatalog = ChangeCatalog };

            Files.ForEach( file => newCatalog.Files.Add( file.Clone() ) );
            newCatalog.SolutionPath = SolutionPath;

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
            string relativeFileName = SolutionRelativeName( name );
            SourceFile foundFile = Find( relativeFileName );

            if( foundFile == null )
            {
                foundFile = new SourceFile( relativeFileName );
                Files.Add( foundFile );
            }

            return foundFile;
        }

        internal string SolutionRelativeName( string name )
        {
            string solutionDir = Path.GetDirectoryName( SolutionPath );

            if( string.IsNullOrEmpty( solutionDir ) )
                return name;

            if (name.Length < solutionDir.Length || name.Substring( 0, solutionDir.Length ) != solutionDir)
                return name;

            return name.Substring( solutionDir.Length + 1 );
        }
    }
}
