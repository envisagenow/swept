//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using swept;
using swept.DSL;

namespace swept.Tests
{
    [TestFixture]
    public class GathererTests
    {

        private const string FILEONE = @"c:\work\one.cs";
        private const string FILETWO = @"c:\work\two.cs";
        private const string FILETHREE = @"c:\work\three.cs";

        [Test]
        public void Empty_inputs_lead_to_empty_results()
        {
            List<Rule> changes = new List<Rule>();
            List<string> files = new List<string>();
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( changes, files, storage );

            var results = gatherer.GetRuleTasks();

            Assert.That( results, Is.Not.Null );
            Assert.That( results.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void Empty_Change_list_leads_to_empty_results()
        {
            List<Rule> changes = new List<Rule>();
            List<string> files = new List<string> { FILEONE };
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( changes, files, storage );

            var results = gatherer.GetRuleTasks();

            Assert.That( results, Is.Not.Null );
            Assert.That( results.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void Gatherer_loads_all_files_in_list()
        {
            List<string> files = new List<string> { FILEONE, FILETWO };
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( new List<Rule>(), files, storage );

            var results = gatherer.GetRuleTasks();

            Assert.That( storage.DidLoad( FILEONE ) );
            Assert.That( storage.DidLoad( FILETWO ) );
            Assert.IsFalse( storage.DidLoad( @"c:\work\three.cs" ) );
        }

        [Test]
        public void Gatherer_one_change_one_file_leads_to_one_result()
        {
            List<Rule> changes = new List<Rule>();
            Rule change = new Rule() { Subquery = new QueryLanguageNode { Language = FileLanguage.CSharp } };
            changes.Add( change );
            List<string> files = new List<string> { FILEONE };
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( changes, files, storage );

            Dictionary<Rule, FileTasks> results = gatherer.GetRuleTasks();

            Assert.That( results.Count, Is.EqualTo( 1 ) );
            Assert.That( results.Keys.First(), Is.SameAs( change ) );

            var fileMatches = results[change];
            var match = fileMatches[fileMatches.Keys.First()];
            Assert.That( match.DoesMatch );
        }


        [Test]
        public void Gatherer_one_change_two_files_leads_to_one_key_with_list_of_two()
        {
            List<Rule> changes = new List<Rule>();
            Rule change = new Rule() { Subquery = new QueryFileNameNode( "one" ) };
            changes.Add( change );
            List<string> files = new List<string> { FILEONE, FILETWO };
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( changes, files, storage );

            var results = gatherer.GetRuleTasks();

            Assert.That( results.Count, Is.EqualTo( 1 ) );
            Assert.That( results.Keys.First(), Is.SameAs( change ) );

            var fileMatches = results[change];
            Assert.That( fileMatches.Count, Is.EqualTo( 2 ) );
        }

        [Test]
        public void Gatherer_when_file_fails_to_load_it_is_removed_from_list()
        {
            List<Rule> rules = new List<Rule>();
            Rule rule = new Rule() { Subquery = new QueryFileNameNode( "one" ) };
            rules.Add( rule );
            List<string> files = new List<string> { FILEONE, FILETWO };
            MockStorageAdapter storage = new MockStorageAdapter();
            storage.FilesToFailToLoad.Add( FILEONE );
            Gatherer gatherer = new Gatherer( rules, files, storage );

            var results = gatherer.GetRuleTasks();

            Assert.That( results.Count, Is.EqualTo( 1 ) );
            Assert.That( results.Keys.First(), Is.SameAs( rule ) );

            var fileMatches = results[rule];
            Assert.That( fileMatches.Count, Is.EqualTo( 1 ) );
            Assert.That( fileMatches.Keys.First().Name, Is.EqualTo( FILETWO ) );
        }

        [Test]
        public void Gatherer_two_changes_one_file_leads_to_two_results()
        {
            List<Rule> changes = new List<Rule>();
            Rule change1 = new Rule() { Subquery = new QueryContentNode( "woof" ) };
            Rule change2 = new Rule() { Subquery = new QueryFileNameNode( "one" ) };
            changes.Add( change1 );
            changes.Add( change2 );
            List<string> files = new List<string> { FILEONE };
            MockStorageAdapter storage = new MockStorageAdapter();
            Gatherer gatherer = new Gatherer( changes, files, storage );

            var results = gatherer.GetRuleTasks();

            Assert.That( results.Count, Is.EqualTo( 2 ) );
            Assert.That( results.Keys.ElementAt( 0 ), Is.SameAs( change1 ) );
            Assert.That( results.Keys.ElementAt( 1 ), Is.SameAs( change2 ) );
            Assert.That( results[change1].Count, Is.EqualTo( 1 ) );
            Assert.That( results[change2].Count, Is.EqualTo( 1 ) );
            //Assert.That( results[change1][0].SourceFile.Name, Is.SameAs( FILEONE ) );
            //Assert.That( results[change2][0].SourceFile.Name, Is.SameAs( FILEONE ) );
        }
    }
}
