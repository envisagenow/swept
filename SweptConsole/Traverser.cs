//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
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

        public Traverser( Arguments args, IStorageAdapter storageAdapter )
        {
            _storageAdapter = storageAdapter;
            _args = args;
            WhiteListPattern = @"\.(cs|as[cmp]x|x?html?|xml|txt|xslt?|css|js|vb|(cs|vb)proj|sln)$";
        }

        public string WhiteListPattern { get; set; }

        public IEnumerable<string> GetProjectFiles()
        {
            try
            {
                List<string> projectFiles = new List<string>();
                //string lineText;
                //if (_args.Piping)
                //{
                //    while ((lineText = Console.In.ReadLine()) != null)
                //    {
                //        // TODO: Cleanup - Generalize/unplug - rough -- this code should be put into SVN reader.
                //        if (lineText.Length > 9)
                //        {
                //            char firstChar = lineText[0];
                //            if (firstChar == 'D')
                //                continue; // Deleted file- skip
                //            else if (firstChar == '?')
                //                continue; // Not version controlled - skip

                //            // assume - 'A' or 'M' for add or modify
                //            string fileName = Path.Combine( _args.Folder, lineText.Substring( 8 ) );
                //            projectFiles.Add( fileName );
                //        }
                //        else { } //TODO: handle non-status lines more methodically
                //    }
                //}
                //else
                //{
                //    ListFilesInFolder( projectFiles, _args.Folder );
                //}

                ListFilesInFolder( projectFiles, _args.Folder );
                return projectFiles;
            }
            catch (IOException ioex)
            {
                if (ioex.Message.StartsWith( "Could not find a part of the path " ))
                {
                    var msg = string.Format( 
                        "{0}{1}Perhaps you expected a different working folder than [{2}]?{1}That can be controlled from the command line with the 'folder:' argument.", 
                        ioex.Message, Environment.NewLine, _args.Folder );
                    throw new Exception( msg, ioex );
                }
                throw;
            }
        }

        private void ListFilesInFolder( List<string> projectFiles, string folder )
        {
            if (FolderIsExcluded( folder )) return;
            if (FolderIsExcluded( Path.GetFileName( folder ))) return;

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
                string escapedExclusion = exclusion.Replace( "\\", "\\\\" ) + "$";
                if (Regex.IsMatch( folder, escapedExclusion ))
                    return true;
            }
            return false;
        }
    }
}