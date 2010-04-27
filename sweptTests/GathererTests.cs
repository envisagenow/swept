//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using swept;

namespace swept.Tests
{
    [TestFixture]
    public class GathererTests
    {
        [Test]
        public void Empty_inputs_lead_to_empty_results()
        {
            List<Change> changes = new List<Change>();
            List<string> files = new List<string>();
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( changes, files, storage );

            Dictionary<Change, List<SourceFile>> results = gatherer.GetIssueList();

            Assert.That( results, Is.Not.Null );
            Assert.That( results.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void Empty_Change_list_leads_to_empty_results()
        {
            List<Change> changes = new List<Change>();
            List<string> files = new List<string> { FILEONE };
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( changes, files, storage );

            Dictionary<Change, List<SourceFile>> results = gatherer.GetIssueList();

            Assert.That( results, Is.Not.Null );
            Assert.That( results.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void Gatherer_loads_all_files_in_list()
        {
            List<string> files = new List<string> { FILEONE, FILETWO };
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( new List<Change>(), files, storage );

            Dictionary<Change, List<SourceFile>> results = gatherer.GetIssueList();

            Assert.That( storage.DidLoad( FILEONE ) );
            Assert.That( storage.DidLoad( FILETWO ) );
            Assert.IsFalse( storage.DidLoad( @"c:\work\three.cs" ) );
        }

        [Test]
        public void Gatherer_one_change_one_file_leads_to_one_result()
        {
            List<Change> changes = new List<Change>();
            Change change = new Change();
            changes.Add( change );
            List<string> files = new List<string> { FILEONE };
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( changes, files, storage );

            Dictionary<Change, List<SourceFile>> results = gatherer.GetIssueList();

            Assert.That( results.Count, Is.EqualTo( 1 ) );
            Assert.That( results.Keys.First(), Is.SameAs( change ) );
            Assert.That( results[change].Count, Is.EqualTo( 1 ) );
            Assert.That( results[change][0].Name, Is.SameAs( FILEONE ) );
        }


        [Test]
        public void Gatherer_one_change_two_files_leads_to_one_key_with_list_of_two()
        {
            List<Change> changes = new List<Change>();
            Change change = new Change();
            changes.Add( change );
            List<string> files = new List<string> { FILEONE, FILETWO };
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( changes, files, storage );

            Dictionary<Change, List<SourceFile>> results = gatherer.GetIssueList();

            Assert.That( results.Count, Is.EqualTo( 1 ) );
            Assert.That( results.Keys.First(), Is.SameAs( change ) );
            Assert.That( results[change].Count, Is.EqualTo( 2 ) );
            Assert.That( results[change][0].Name, Is.SameAs( FILEONE ) );
            Assert.That( results[change][1].Name, Is.SameAs( FILETWO ) );
        }

        [Test]
        public void Gatherer_two_changes_one_file_leads_to_two_results()
        {
            List<Change> changes = new List<Change>();
            Change change1 = new Change();
            Change change2 = new Change();
            changes.Add( change1 );
            changes.Add( change2 );
            List<string> files = new List<string> { FILEONE };
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( changes, files, storage );

            Dictionary<Change, List<SourceFile>> results = gatherer.GetIssueList();

            Assert.That( results.Count, Is.EqualTo( 2 ) );
            Assert.That( results.Keys.ElementAt( 0 ), Is.SameAs( change1 ) );
            Assert.That( results.Keys.ElementAt( 1 ), Is.SameAs( change2 ) );
            Assert.That( results[change1].Count, Is.EqualTo( 1 ) );
            Assert.That( results[change2].Count, Is.EqualTo( 1 ) );
            Assert.That( results[change1][0].Name, Is.SameAs( FILEONE ) );
            Assert.That( results[change2][0].Name, Is.SameAs( FILEONE ) );
        }

        private const string FILEONE = @"c:\work\one.cs";
        private const string FILETWO = @"c:\work\two.cs";
    }
}
