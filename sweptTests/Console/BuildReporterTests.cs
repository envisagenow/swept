//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Xml.Linq;
using swept;

namespace swept.Tests
{
    [TestFixture]
    public class BuildReporterTests
    {
        public string empty_report = "<SweptBuildReport />";

        [Test]
        public void No_task_data_creates_empty_report()
        {
            BuildReporter reporter = new BuildReporter();

            string report = reporter.ReportOn( new Dictionary<Change, List<IssueSet>>() );

            Assert.That( report, Is.EqualTo( empty_report ) );
        }

        [Test]
        public void single_Change_single_SourceFile_report()
        {
            var expectedReport = XDocument.Parse(
@"<SweptBuildReport>
    <Change ID='HTML 01' Description='Improve browser compatibility' TotalTasks='4'>
        <SourceFile Name='bar.htm' TaskCount='4' />
    </Change>
</SweptBuildReport>"
            );

            BuildReporter reporter = new BuildReporter();

            var changes = new Dictionary<Change, List<IssueSet>>();

            var change = new Change
            {
                ID = "HTML 01",
                Language = FileLanguage.HTML,
                Description = "Improve browser compatibility"
            };

            var bar = new SourceFile( "bar.htm" ) { TaskCount = 4 };
            var set = change.GetIssueSet( bar );

            changes.Add( change, new List<IssueSet> { set } );

            string report = reporter.ReportOn( changes );

            Assert.That( report, Is.EqualTo( expectedReport.ToString() ) );
        }

        [Test]
        public void multiple_Change_multiple_SourceFile_report()
        {
            var expectedReport = XDocument.Parse(
@"
<SweptBuildReport>
    <Change 
        ID='DomainEvents 01' 
        Description='Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister'
        TotalTasks='4'>
        
        <SourceFile Name='foo.cs' TaskCount='1' />
        <SourceFile Name='goo.cs' TaskCount='3' />
    </Change>
    <Change 
        ID='HTML 01' 
        Description='Improve browser compatibility across IE versions'
        TotalTasks='6'>

        <SourceFile Name='bar.htm' TaskCount='4' />
        <SourceFile Name='shmoo.aspx' TaskCount='2' />
    </Change>
</SweptBuildReport>
"
            );

            var changes = new Dictionary<Change, List<IssueSet>>();

            var csharpChange = new Change
            {
                ID = "DomainEvents 01",
                Description = "Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister"
            };

            var csharpFiles = new List<IssueSet>();

            SourceFile foo = new SourceFile( "foo.cs" ) { TaskCount = 1 };
            SourceFile goo = new SourceFile( "goo.cs" ) { TaskCount = 3 };
            csharpFiles.Add( csharpChange.GetIssueSet( foo ) );
            csharpFiles.Add( csharpChange.GetIssueSet( goo ) );

            changes.Add( csharpChange, csharpFiles );

            var htmlChange = new Change
            {
                ID = "HTML 01",
                Language = FileLanguage.HTML,
                Description = "Improve browser compatibility across IE versions"
            };

            var htmlFiles = new List<IssueSet>();
            SourceFile bar = new SourceFile( "bar.htm" ) { TaskCount = 4 };
            SourceFile shmoo = new SourceFile( "shmoo.aspx" ) { TaskCount = 2 };
            htmlFiles.Add( htmlChange.GetIssueSet( bar ) );
            htmlFiles.Add( htmlChange.GetIssueSet( shmoo ) );

            changes.Add( htmlChange, htmlFiles );

            BuildReporter reporter = new BuildReporter();
            string report = reporter.ReportOn( changes );

            Assert.That( report, Is.EqualTo( expectedReport.ToString() ) );
        }


    }
}
