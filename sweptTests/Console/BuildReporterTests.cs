//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Xml.Linq;
using swept;
using swept.DSL;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class BuildReporterTests
    {
        public string empty_report = "<SweptBuildReport />";

        [Test]
        public void No_task_data_creates_empty_report()
        {
            BuildReporter reporter = new BuildReporter();

            string report = reporter.ReportOn( new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>() );

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

            var changes = new Dictionary<Change, Dictionary<SourceFile,ClauseMatch>>();

            var change = new Change
            {
                ID = "HTML 01",
                Subquery = new QueryLanguageNode { Language = FileLanguage.HTML },
                Description = "Improve browser compatibility"
            };

            var bar = new SourceFile( "bar.htm" );

            var fileMatches = new Dictionary<SourceFile, ClauseMatch>();
            fileMatches[bar] = new LineMatch( new List<int> { 1, 12, 123, 1234 } ); //change.Subquery.Answer( bar );
            changes.Add( change, fileMatches );

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

            var changes = new Dictionary<Change, Dictionary<SourceFile,ClauseMatch>>();

            var csharpChange = new Change
            {
                ID = "DomainEvents 01",
                Description = "Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister"
            };

            var csharpFiles = new Dictionary<SourceFile,ClauseMatch>();

            // TODO: sensible flexibility point for generating tests--but .TaskCount is m'lark.
            SourceFile foo = new SourceFile( "foo.cs" ); // { TaskCount = 1 };
            SourceFile goo = new SourceFile( "goo.cs" ); // { TaskCount = 3 };

            csharpFiles[foo] = new FileMatch( true );
            csharpFiles[goo] = new LineMatch( new List<int> { 1, 2, 3 } );

            changes[csharpChange] = csharpFiles;

            var htmlChange = new Change
            {
                ID = "HTML 01",
                Subquery = new QueryLanguageNode { Language = FileLanguage.HTML },
                Description = "Improve browser compatibility across IE versions"
            };

            var htmlFiles = new Dictionary<SourceFile,ClauseMatch>();
            SourceFile bar = new SourceFile( "bar.htm" ); // { TaskCount = 4 };
            SourceFile shmoo = new SourceFile( "shmoo.aspx" ); // { TaskCount = 2 };

            htmlFiles[bar] = new LineMatch( new List<int> { 1, 2, 3, 4 } );
            htmlFiles[shmoo] = new LineMatch( new List<int> { 8, 12 } );

            changes[htmlChange] = htmlFiles;

            BuildReporter reporter = new BuildReporter();
            string report = reporter.ReportOn( changes );

            Assert.That( report, Is.EqualTo( expectedReport.ToString() ) );
        }


    }
}
