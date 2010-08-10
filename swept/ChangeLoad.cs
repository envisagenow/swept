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
                    union.IssueSets[issueSet.SourceFile].UniteLines( issueSet.MatchLineNumbers );
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
                    IssueSet intersectingSet = new IssueSet( issueSet.Clause, issueSet.SourceFile, new List<int>( issueSet.MatchLineNumbers ) );
                    intersectingSet.IntersectLines( matchingSet.MatchLineNumbers );
                    if (intersectingSet.MatchLineNumbers.Any())
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
                IssueSet subtractingSet = new IssueSet( issueSet.Clause, issueSet.SourceFile, new List<int>( issueSet.MatchLineNumbers ) );
                if (other.IssueSets.ContainsKey( issueSet.SourceFile ))
                {
                    var matchingSet = other.IssueSets[issueSet.SourceFile];
                    subtractingSet.SubtractLines( matchingSet.MatchLineNumbers );
                }
                if (subtractingSet.MatchLineNumbers.Any())
                {
                    subtraction.IssueSets.Add( issueSet.SourceFile, subtractingSet );
                }
            }

            return subtraction;
        }
    }
}
