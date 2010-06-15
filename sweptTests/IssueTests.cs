using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class IssueTests
    {
    	[Test]
        public void Issue_has_a_Change_a_SourceFile_and()
        {
            SourceFile myFile = new SourceFile( "foo.cs" );
            Change myChange = new Change();

            var issue = new Issue() { SourceFile = myFile, Change = myChange };

            Assert.That( issue.SourceFile, Is.EqualTo( myFile ) );
            Assert.That( issue.Change, Is.EqualTo( myChange ) );
            Assert.That( issue.HasFileLevelError, Is.False );
            Assert.That( issue.HasAtLeastOneMatchError, Is.False );

            IList<int> matchErrorLineNumbers = issue.GetMatchLineNumbers();
            Assert.That( matchErrorLineNumbers, Is.Not.Null );
            Assert.That( matchErrorLineNumbers.Count, Is.EqualTo( 0 ) );
        }
    }
}
