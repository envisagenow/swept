//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace swept
{
    public class Traverser
    {
        private readonly IStorageAdapter _storageAdapter;
        private readonly Arguments _args;
        private String currentWorkingDirectory;

        public Traverser( Arguments args, IStorageAdapter storageAdapter )
        {
            _storageAdapter = storageAdapter;
            _args = args;
            WhiteListPattern = @"\.(cs|as[chmp]x|x?html?|master|xml|txt|xslt?|css|js|vb|(cs|vb)proj|sln|sql|catalog|swf|fla|ocx|jar|java|applet|template)$";
            currentWorkingDirectory = _storageAdapter.GetCWD();
        }

        public string WhiteListPattern { get; set; }

        public IEnumerable<string> GetFilesToScan()
        {
            string nippedFolder = _args.Folder.StartsWith( currentWorkingDirectory )
                ? _args.Folder.Substring(currentWorkingDirectory.Length)
                : _args.Folder;

            try
            {
                List<string> projectFiles = new List<string>();
                ListFilesInFolder( projectFiles, nippedFolder);
                return projectFiles;
            }
            catch (IOException ioex)
            {
                if (ioex.Message.StartsWith( "Could not find a part of the path " ))
                {
                    var msg = string.Format( 
                        "{0}{1}Perhaps you expected a different working folder than [{2}]?{1}That can be controlled from the command line with the 'folder:' argument.", 
                        ioex.Message, Environment.NewLine, nippedFolder );
                    throw new Exception( msg, ioex );
                }
                throw;
            }
        }

        private void ListFilesInFolder( List<string> projectFiles, string folder )
        {
            if (FolderIsExcluded( folder )) return;

            foreach (string file in _storageAdapter.GetFilesInFolder( folder ))
            {
                string fullyQualifiedFileName = Path.Combine( folder, file );
                if( Regex.IsMatch( file, WhiteListPattern, RegexOptions.IgnoreCase ))
                    projectFiles.Add( fullyQualifiedFileName );
            }

            IEnumerable<string> children = _storageAdapter.GetFoldersInFolder( folder );
            foreach (string childFolder in children )
            {
                string fullPath = Path.Combine( folder, childFolder );
                ListFilesInFolder( projectFiles, fullPath );
            }
        }

        private bool FolderIsExcluded( string folder )
        {
            foreach (string exclusion in _args.Exclude)
            {
                string tunedExclusion = exclusion.StartsWith(currentWorkingDirectory)
                    ? exclusion.Substring(currentWorkingDirectory.Length)
                    : exclusion;
                tunedExclusion = tunedExclusion.Replace( "\\", "\\\\" ) + "$";

                if (Regex.IsMatch( folder, tunedExclusion ))
                    return true;
            }
            return false;
        }
    }
}