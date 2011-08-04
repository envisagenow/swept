//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace swept
{
    public class Traverser
    {
        readonly IStorageAdapter StorageAdapter;
        readonly Arguments Args;

        public Traverser( Arguments args, IStorageAdapter storageAdapter )
        {
            StorageAdapter = storageAdapter;
            Args = args;
            WhiteListPattern = @"\.(cs|aspx|ascx|x?html?|xml|txt|xslt?|css)$";
        }

        public string WhiteListPattern { get; set; }

        public IEnumerable<string> GetProjectFiles()
        {
            try
            {
                List<string> projectFiles = new List<string>();

                ListFilesInFolder( projectFiles, Args.Folder );

                return projectFiles;
            }
            catch (IOException ioex)
            {
                if (ioex.Message.StartsWith( "Could not find a part of the path " ))
                {
                    var msg = string.Format( 
                        "{0}{1}Perhaps you expected a different current dir.{1}Perhaps you need to specify a different 'folder:' argument.", 
                        ioex.Message, Environment.NewLine );
                    throw new Exception( msg, ioex );
                }
                throw;
            }
        }

        private void ListFilesInFolder( List<string> projectFiles, string folder )
        {
            if (FolderIsExcluded( folder )) return;
            if (FolderIsExcluded( Path.GetFileName( folder ))) return;

            foreach (string file in StorageAdapter.GetFilesInFolder( folder ))
            {
                string fullyQualifiedFileName = Path.Combine( folder, file );
                if( Regex.IsMatch( file, WhiteListPattern, RegexOptions.IgnoreCase ))
                    projectFiles.Add( fullyQualifiedFileName );
            }

            IEnumerable<string> children = StorageAdapter.GetFoldersInFolder( folder );
            foreach (string childFolder in children )
            {
                string fullPath = Path.Combine( folder, childFolder );
                ListFilesInFolder( projectFiles, fullPath );
            }
        }

        private bool FolderIsExcluded( string folder )
        {
            foreach (string exclusion in Args.Exclude)
            {
                string escapedExclusion = exclusion.Replace( "\\", "\\\\" ) + "$";
                if (Regex.IsMatch( folder, escapedExclusion ))
                    return true;
            }
            return false;
        }
    }
}