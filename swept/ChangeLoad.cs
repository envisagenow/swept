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

        public ChangeLoad Union( ChangeLoad other )
        {
            var union = new ChangeLoad();

            foreach (IssueSet issueSet in IssueSets.Values)
            {
                union.IssueSets.Add( issueSet.SourceFile, issueSet );
            }

            foreach (IssueSet issueSet in other.IssueSets.Values)
            {
                if (union.IssueSets.ContainsKey( issueSet.SourceFile ))
                {
                    //unify line numbers
                    //union.IssueSets[issueSet.SourceFile].UniteLines( issueSet.Matches );
#warning ChangeLoad union punted
                    //union.IssueSets[issueSet.SourceFile] = union.IssueSets[issueSet.SourceFile].Union( issueSet );
                }
                else
                    union.IssueSets.Add( issueSet.SourceFile, issueSet );
            }

            return union;
        }

        public ChangeLoad Intersection( ChangeLoad other )
        {
            var intersection = new ChangeLoad();

            foreach (IssueSet issueSet in IssueSets.Values)
            {
                if (other.IssueSets.ContainsKey( issueSet.SourceFile ))
                {
                    var matchingSet = other.IssueSets[issueSet.SourceFile];
                    IssueSet intersectingSet = new IssueSet( issueSet );
#warning ChangeLoad intersection punted
                    //intersectingSet.IntersectLines( matchingSet.Matches );
                    if (intersectingSet.Matches.Any())
                    {
                        intersection.IssueSets.Add( issueSet.SourceFile, intersectingSet );
                    }
                }
            }

            return intersection;
        }

        public ChangeLoad Subtraction( ChangeLoad other )
        {
            var subtraction = new ChangeLoad();

            foreach (IssueSet issueSet in IssueSets.Values)
            {
                IssueSet subtractingSet = new IssueSet( issueSet );
                if (other.IssueSets.ContainsKey( issueSet.SourceFile ))
                {
                    var matchingSet = other.IssueSets[issueSet.SourceFile];
#warning ChangeLoad subtraction punted
                    //subtractingSet.SubtractLines( matchingSet.Matches );
                }
                if (subtractingSet.Matches.Any())
                {
                    subtraction.IssueSets.Add( issueSet.SourceFile, subtractingSet );
                }
            }

            return subtraction;
        }
    }
}
