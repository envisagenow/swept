using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace swept
{
    public class ChangeLoad
    {
        public Dictionary<SourceFile, IssueSet> IssueSets;

        public ChangeLoad()
        {
            IssueSets = new Dictionary<SourceFile, IssueSet>();
        }

        public ChangeLoad Union( ChangeLoad rhs )
        {
            var union = new ChangeLoad();

            foreach (IssueSet issueSet in IssueSets.Values)
            {
                union.IssueSets[issueSet.SourceFile] = new IssueSet( issueSet );
            }

            foreach (IssueSet issueSet in rhs.IssueSets.Values)
            {
                if (union.IssueSets.ContainsKey( issueSet.SourceFile ))
                {
                    var matchingSet = union.IssueSets[issueSet.SourceFile];
                    union.IssueSets[issueSet.SourceFile] = issueSet.Union( matchingSet );
                }
                else
                {
                    union.IssueSets[issueSet.SourceFile] = new IssueSet( issueSet );
                }
            }

            return union;
        }

        public ChangeLoad Intersection( ChangeLoad rhs )
        {
            var intersection = new ChangeLoad();

            foreach (IssueSet issueSet in IssueSets.Values)
            {
                if (rhs.IssueSets.ContainsKey( issueSet.SourceFile ))
                {
                    var matchingSet = rhs.IssueSets[issueSet.SourceFile];
                    IssueSet intersectingSet = issueSet.Intersection( matchingSet );
                    if (intersectingSet.Any())
                    {
                        intersection.IssueSets.Add( issueSet.SourceFile, intersectingSet );
                    }
                }
            }

            return intersection;
        }

        public ChangeLoad Subtraction( ChangeLoad rhs )
        {
            var subtraction = new ChangeLoad();

            foreach (IssueSet issueSet in IssueSets.Values)
            {
                if (rhs.IssueSets.ContainsKey( issueSet.SourceFile ))
                {
                    var matchingSet = rhs.IssueSets[issueSet.SourceFile];
                    IssueSet subtractedSet = issueSet.Subtraction( matchingSet );
                    if (subtractedSet.Any())
                    {
                        subtraction.IssueSets.Add( issueSet.SourceFile, subtractedSet );
                    }
                }
                else
                {
                    subtraction.IssueSets.Add( issueSet.SourceFile, new IssueSet( issueSet ) );
                }
            }

            return subtraction;
        }
    }
}
