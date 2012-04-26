//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using swept.DSL;

namespace swept.Tests
{
    [TestFixture]
    public class BuildReporterTests
    {
        BuildReporter reporter = new BuildReporter();

        [SetUp]
        public void SetUp()
        {
            reporter = new BuildReporter();
        }

        [Test]
        public void No_task_data_creates_empty_report()
        {
            string empty_report = "<SweptBuildReport TotalTasks=\"0\" />";

            string report = reporter.ReportOn( new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>() );

            Assert.That( report, Is.EqualTo( empty_report ) );
        }

        [Test]
        public void single_Change_single_SourceFile_report()
        {
            var expectedReport = XDocument.Parse(
@"<SweptBuildReport TotalTasks='4'>
    <Change ID='HTML 01' Description='Improve browser compatibility' TotalTasks='4'>
        <SourceFile Name='bar.htm' TaskCount='4' />
    </Change>
</SweptBuildReport>"
            );

            var changes = new Dictionary<Change, Dictionary<SourceFile,ClauseMatch>>();

            var change = new Change
            {
                ID = "HTML 01",
                Subquery = new QueryLanguageNode { Language = FileLanguage.HTML },
                Description = "Improve browser compatibility"
            };

            var bar = new SourceFile( "bar.htm" );

            var fileMatches = new Dictionary<SourceFile, ClauseMatch>();
            fileMatches[bar] = new LineMatch( new List<int> { 1, 12, 123, 1234 } );
            changes.Add( change, fileMatches );

            string report = reporter.ReportOn( changes );

            Assert.That( report, Is.EqualTo( expectedReport.ToString() ) );
        }

        [Test]
        public void multiple_Change_multiple_SourceFile_report()
        {
            var expectedReport = XDocument.Parse(
@"
<SweptBuildReport TotalTasks='10'>
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

            var csharpChange = new Change
            {
                ID = "DomainEvents 01",
                Description = "Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister"
            };

            SourceFile foo = new SourceFile("foo.cs");
            SourceFile goo = new SourceFile("goo.cs");

            var csharpFiles = new Dictionary<SourceFile, ClauseMatch>();
            csharpFiles[foo] = new FileMatch(true);
            csharpFiles[goo] = new LineMatch(new List<int> { 1, 2, 3 });

            var htmlChange = new Change
            {
                ID = "HTML 01",
                Subquery = new QueryLanguageNode { Language = FileLanguage.HTML },
                Description = "Improve browser compatibility across IE versions"
            };

            SourceFile bar = new SourceFile("bar.htm");
            SourceFile shmoo = new SourceFile("shmoo.aspx");

            var htmlFiles = new Dictionary<SourceFile, ClauseMatch>();
            htmlFiles[bar] = new LineMatch(new List<int> { 1, 2, 3, 4 });
            htmlFiles[shmoo] = new LineMatch(new List<int> { 8, 12 });

            var changes = new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>();
            changes[csharpChange] = csharpFiles;
            changes[htmlChange] = htmlFiles;

            string report = reporter.ReportOn(changes);

            Assert.That(report, Is.EqualTo(expectedReport.ToString()));
        }

        [Test]
        public void Files_with_false_FileMatch_not_added()
        {
            var expectedReport = XDocument.Parse(
@"
<SweptBuildReport TotalTasks='1'>
    <Change 
        ID='DomainEvents 01' 
        Description='Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister'
        TotalTasks='1'>
        
        <SourceFile Name='foo.cs' TaskCount='1' />
    </Change>
</SweptBuildReport>
"
            );

            var csharpChange = new Change
            {
                ID = "DomainEvents 01",
                Description = "Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister"
            };

            SourceFile foo = new SourceFile("foo.cs");
            SourceFile goo = new SourceFile("goo.cs");
            SourceFile boo = new SourceFile("boo.cs");

            var csharpFiles = new Dictionary<SourceFile, ClauseMatch>();
            csharpFiles[foo] = new FileMatch(true);
            csharpFiles[goo] = new FileMatch(false);
            csharpFiles[boo] = new LineMatch( new int[] {} );

            var changes = new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>();
            changes[csharpChange] = csharpFiles;

            string report = reporter.ReportOn(changes);

            Assert.That(report, Is.EqualTo(expectedReport.ToString()));
        }


    }
}
