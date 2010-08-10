using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class IssueSet_Tests
    {
        [Test]
        public void Can_create_line_IssueSet()
        {
            Clause clause = new Clause();
            SourceFile fooFile = new SourceFile( "foo.cs" );
            IssueSet set = new IssueSet( clause, fooFile, ClauseMatchScope.Line, new List<int>() { 1 });
            Assert.That( set, Is.Not.Null );
            Assert.That( set.Clause, Is.EqualTo( clause ) );
            Assert.That( set.SourceFile, Is.EqualTo( fooFile ) );
            Assert.That( set.MatchScope, Is.EqualTo( ClauseMatchScope.Line ) );
            Assert.That( set.MatchLineNumbers[0], Is.EqualTo( 1 ) );

            Assert.That( set.DoesMatch );
        }

        [Test]
        public void Can_clone_IssueSet()
        {
            Clause clause = new Clause();
            SourceFile fooFile = new SourceFile( "foo.cs" );
            
            IssueSet set = new IssueSet( clause, fooFile, ClauseMatchScope.Line, new List<int>() { 1 });

            IssueSet clone = new IssueSet(set);

            Assert.That( clone, Is.Not.Null );
            Assert.That( clone.Clause, Is.EqualTo( clause ) );
            Assert.That( clone.SourceFile, Is.EqualTo( fooFile ) );
            Assert.That( clone.MatchScope, Is.EqualTo( ClauseMatchScope.Line ) );
            Assert.That( clone.MatchLineNumbers[0], Is.EqualTo( 1 ) );

            Assert.That( set.DoesMatch );
        }

        // TODO: Unite, Intersect, Subtract tests
    }            

}
