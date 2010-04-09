using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace swept
{
    public class Traverser
    {
        IStorageAdapter StorageAdapter;
        Arguments Args;

        public Traverser( Arguments args, IStorageAdapter storageAdapter )
        {
            StorageAdapter = storageAdapter;
            Args = args;
            WhiteListPattern = @"\.(cs|aspx|ascx|x?html?|xml|txt|xslt?|css)$";
        }

        public string WhiteListPattern
        {
            get;
            set;
        }

        public IEnumerable<string> GetProjectFiles()
        {
            List<string> projectFiles = new List<string>();

            ListFilesInFolder( projectFiles, Args.Folder );

            return projectFiles;
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