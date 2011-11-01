using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class BuildFail_tests
    {
        [Test]
        public void Empty_inputs_do_not_fail()
        {

            FailChecker checker = new FailChecker();

            var results = new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>();

            var problems = checker.Check( results );

            Assert.That( problems.Count(), Is.EqualTo( 0 ) );
        }


        [Test]
        public void When_we_do_violate_a_BuildFail_Any_Change_we_do_fail()
        {
            Change change = new Change() { ID = "644", Description = "Major problem!", BuildFail = BuildFailMode.Any };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );

            sourceClauseMatch[failedSource] = failedClause;

            var checker = new FailChecker();

            var changes = new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>();
            changes[change] = sourceClauseMatch;

            var problems = checker.Check( changes );

            Assert.That( problems.Count(), Is.EqualTo( 1 ) );
            Assert.That( problems[0], Is.EqualTo( "Rule [644] has been violated, and it breaks the build if there are any violations." ) );
        }

        [Test]
        public void When_we_do_violate_a_BuildFail_None_Change_we_do_not_fail()
        {
            Change change = new Change() { ID = "644", Description="Not a problem.", BuildFail = BuildFailMode.None };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );

            sourceClauseMatch[failedSource] = failedClause;

            var checker = new FailChecker();

            var changes = new Dictionary< Change, Dictionary<SourceFile, ClauseMatch> >();
            changes[change] = sourceClauseMatch;

            var problems = checker.Check( changes );

            Assert.That( problems.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_we_violate_multiple_BuildFail_Any_Changes_we_report_all_failures()
        {
            Change change = new Change() { ID = "191", Description = "Major problem!", BuildFail = BuildFailMode.Any };
            Change change2 = new Change() { ID = "200", Description = "Major problem!", BuildFail = BuildFailMode.Any };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch2 = new Dictionary<SourceFile, ClauseMatch>();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );

            sourceClauseMatch[failedSource] = failedClause;

            SourceFile failedSource2 = new SourceFile( "some_other_file.cs" );
            ClauseMatch failedClause2 = new LineMatch( new List<int> { 23, 65, 81 } );

            sourceClauseMatch2[failedSource2] = failedClause2;

            var checker = new FailChecker();

            var changes = new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>();
            changes[change] = sourceClauseMatch;
            changes[change2] = sourceClauseMatch2;

            var problems = checker.Check( changes );

            Assert.That( problems.Count(), Is.EqualTo( 2 ) );
            Assert.That( problems[0], Is.EqualTo( "Rule [191] has been violated, and it breaks the build if there are any violations." ) );
            Assert.That( problems[1], Is.EqualTo( "Rule [200] has been violated, and it breaks the build if there are any violations." ) );
        }

        [Test]
        public void When_we_exceed_BuildFail_Over_limit_we_fail()
        {
            Change change = new Change() { ID = "191", Description = "Major problem!", 
                BuildFail = BuildFailMode.Over, BuildFailOverLimit = 2 };
            Change change2 = new Change() { ID = "200", Description = "Major problem!", 
                BuildFail = BuildFailMode.Over, BuildFailOverLimit = 2 };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch2 = new Dictionary<SourceFile, ClauseMatch>();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );

            sourceClauseMatch[failedSource] = failedClause;

            SourceFile failedSource2 = new SourceFile( "some_other_file.cs" );
            ClauseMatch failedClause2 = new LineMatch( new List<int> { 23, 65, 81 } );

            sourceClauseMatch2[failedSource2] = failedClause2;

            var checker = new FailChecker();

            var changes = new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>();
            changes[change] = sourceClauseMatch;
            changes[change2] = sourceClauseMatch2;

            var problems = checker.Check( changes );

            Assert.That( problems.Count(), Is.EqualTo( 1 ) );
            Assert.That( problems[0], Is.EqualTo( "Rule [200] has been violated [3] times, and it breaks the build if there are over [2] violations." ) );
        }
    }

    public class FailChecker
    {
        public List<string> Check( Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> changes )
        {
            var problems = new List<string>();

            foreach (var change in changes.Keys)
            {
                if (change.BuildFail == BuildFailMode.Any)
                {
                    string problemText = string.Format( "Rule [{0}] has been violated, and it breaks the build if there are any violations.", change.ID );
                    problems.Add( problemText );
                }
                else if (change.BuildFail == BuildFailMode.Over)
                {
                    int violationCount = 0;
                    foreach (var sourcefile in changes[change].Keys)
                    {
                        violationCount += changes[change][sourcefile].Count;
                    }
                    if (violationCount > change.BuildFailOverLimit)
                    {
                        string problemText = string.Format( "Rule [{0}] has been violated [{1}] times, and it breaks the build if there are over [{2}] violations.", change.ID, violationCount, change.BuildFailOverLimit );
                        problems.Add( problemText );
                    }
                }
            }

            return problems;
        }
    }
}
