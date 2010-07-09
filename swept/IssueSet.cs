using System;
using System.Collections.Generic;

namespace swept
{
    public class IssueSet
    {
        private List<int> _matchList;
        public IssueSet( Change change, SourceFile file )
        {
            Change = change;
            SourceFile = file;

            change.DoesMatch(file);
            _matchList = Change._matchList;
        }
                
        public SourceFile SourceFile { get; set; }
        public Change Change { get; set; }

        public IList<int> MatchLineNumbers
        {
            get
            {
                return _matchList;
            }
        }
    }
}
