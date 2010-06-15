using System;
using System.Collections.Generic;

namespace swept
{
    public class Issue
    {
        public Issue()
        {
        }

        public SourceFile SourceFile { get; set; }
        public Change Change { get; set; }

        public bool HasFileLevelError;

        public bool HasAtLeastOneMatchError;

        public IList<int> GetMatchLineNumbers()
        {
            return new List<int>();
        }
    }
}
