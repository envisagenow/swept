using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace swept
{
    public class Traverser
    {
        IStorageAdapter StorageAdapter;
        Arguments Args;

        public Traverser(Arguments args, IStorageAdapter storageAdapter)
        {
            StorageAdapter = storageAdapter;
            Args = args;
        }


        public IEnumerable<string> GetProjectFiles()
        {
            List<string> projectFiles = new List<string>();

            foreach (string folder in new[] { Args.Folder })
            {
                if (FolderIsExcluded( folder )) continue;
                projectFiles.AddRange( StorageAdapter.GetFilesInFolder( folder ) );
            }
            

            return projectFiles;
        }

        private bool FolderIsExcluded( string folder )
        {
            return Args.Exclude.Contains( folder );
        }
    }
}
