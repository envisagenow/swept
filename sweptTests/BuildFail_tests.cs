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

            int returnCode = checker.Check( results );

            Assert.That( returnCode, Is.EqualTo( 0 ) );
        }


        [Test]
        public void When_we_do_violate_a_BuildFail_Any_Change_we_do_fail()
        {
            Change change = new Change() { ID = "Faily", BuildFail = BuildFailMode.Any };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            SourceFile failedSource = new SourceFile( "Faily.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int>() );

            sourceClauseMatch.Add( failedSource, failedClause );
        }
    }

    public class FailChecker
    {
        public int Check( Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> changes )
        {
            int returnCode = 0;

            //foreach (Change c in changes)
            //{
            //   c.
            //}

            return returnCode;
        }
    }
}
