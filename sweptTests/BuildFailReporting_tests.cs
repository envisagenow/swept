//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class BuildFailReporting_tests
    {
        private FailChecker _checker;
        private Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> _changeViolations;

        [SetUp]
        public void Setup()
        {
            _checker = new FailChecker();
            _changeViolations = new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>();
        }

        [Test]
        public void Zero_Problems_produces_no_failure_text()
        {
            string failureText = _checker.ReportFailures( new List<string>() );

            Assert.AreEqual( string.Empty, failureText );
        }

        [Test]
        public void Zero_Problems_produces_no_failure_XML()
        {
            string failureXML = _checker.ReportFailureXML( new List<string>() );

            Assert.AreEqual( string.Empty, failureXML );
        }

        [Test]
        public void When_one_failure_occurs_text_is_correct()
        {
            List<string> failures = new List<string>();
            string problemText = "fooblah";
            failures.Add( problemText );
            string failureText = _checker.ReportFailures( failures );

            var expectedFailureMessage = String.Format( "Swept failed due to build breaking rule failure:\n{0}\n", problemText );
            Assert.AreEqual( expectedFailureMessage, failureText );
        }

        [Test]
        public void When_one_failure_occurs_failure_XML_is_correct()
        {
            List<string> failures = new List<string>();
            string problemText = "fooblah";
            failures.Add( problemText );
            string failureXML = _checker.ReportFailureXML( failures );

            var expectedFailureXML = String.Format( "<{0}s><{0}>{1}</{0}></{0}s>", "SweptRuleFailure", problemText );

            Assert.AreEqual( expectedFailureXML, failureXML );
        }

        [Test]
        public void When_multiple_failures_occur_text_is_correct()
        {
            List<string> failures = new List<string>();
            failures.Add( "fail1" );
            failures.Add( "fail2" );

            string problemText = "";
            foreach (string fail in failures)
            {
                problemText += fail + "\n";
            }
            var expectedFailureMessage = String.Format( "Swept failed due to build breaking rule failures:\n{0}", problemText );

            string failureText = _checker.ReportFailures( failures );

            Assert.AreEqual( expectedFailureMessage, failureText );
        }

    }
}
