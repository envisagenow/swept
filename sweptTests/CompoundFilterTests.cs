//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using swept;
using System.Collections.Generic;

namespace swept.Tests
{

    [TestFixture]
    public class CompoundFilterTests
    {
        // current representation   
        //<Change ID="Persistence 1" Description="Move persistence code into persisters" Language="CSharp" />

        /*  Goal representation:

<Change ID="Persistence 1" Description="Move persistence code into persisters">

    <When ID="C#" Language="CSharp" />
    
    <AndNot ID="Permitted_Files">
        <When NamesContaining="(Persister|Service)" />
        <Or NamesContaining="XADR" />
    </AndNot>
    
    <And ID="Uses_Persistence_Directly">
        <When FileContaining="XADR" />
        <Or FileContaining="Hibernate" />
        <Or FileContaining="Oracle" />
    </And>

</Change>
        */

        #region Compound matching
        [Test]
        public void Chilren_get_matched()
        {
            var child1 = new CompoundFilter { Language = FileLanguage.CSharp };
            var child2 = new CompoundFilter { NamePattern = "blue" };

            var filter = new CompoundFilter { };
            filter.Children.Add( child1 );
            filter.Children.Add( child2 );

            Assert.IsFalse( filter.Matches( new SourceFile( "my.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( "blue.html" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "blue.cs" ) ) );
        }

        [Test]
        public void Or_filter()
        {
            var child1 = new CompoundFilter { Language = FileLanguage.CSharp };
            var child2 = new CompoundFilter { NamePattern = "blue", Operator = FilterOperator.Or };

            var filter = new CompoundFilter { };
            filter.Children.Add( child1 );
            filter.Children.Add( child2 );

            Assert.IsTrue( filter.Matches( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "blue.html" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "blue.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( "bluuue.css" ) ) );
        }


        [Test]
        public void Not_language_filter_passes_MISmatches_only()
        {
            var filter = new CompoundFilter
            {
                ID = "Not language",
                Description = "Relevant to everything except C# files.",
                Language = FileLanguage.CSharp,
                Operator = FilterOperator.Not,
            };

            Assert.IsFalse( filter.Matches( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "my.html" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "my.unknownextension" ) ) );
        }

        #region wed tests
        [Test]
        public void NotFilter_inverts_decision()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "no_tests",
                Description = "Skip the test files",
                NamePattern = "tests",
                Operator = FilterOperator.Not
            };

            Assert.IsTrue( filter.Matches( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( @"Tests.cs" ) ) );
        }

        [Test]
        public void Filter_passes_depending_on_internal_filters()
        {
            CompoundFilter child = new CompoundFilter
            {
                ID = "Tests",
                Description = "Give me just the unit tests.",
                NamePattern = "tests"
            };

            CompoundFilter filter = new CompoundFilter
            {
                Children = new List<CompoundFilter> { child }
            };

            Assert.IsFalse( filter.Matches( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"Tests.cs" ) ) );
        }



        [Test]
        public void either_of_or_filter_passing_passes()
        {
            CompoundFilter one = new CompoundFilter
            {
                ID = "one",
                Description = "files named one",
                NamePattern = "one"
            };

            CompoundFilter two = new CompoundFilter
            {
                ID = "two",
                Description = "files named two",
                NamePattern = "two",
                Operator = FilterOperator.Or,
            };

            CompoundFilter filter = new CompoundFilter
            {
                Children = new List<CompoundFilter> { one, two }
            };

            Assert.IsTrue( filter.Matches( new SourceFile( @"my_one.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"my_two.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( @"my_three.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"my_one_two.cs" ) ) );
        }
#endregion

        #endregion

        #region Simple filter functionality

        [Test]
        public void empty_filter_matches_any_file()
        {
            var filter = new CompoundFilter();
            var file = new SourceFile( @"\path\file.ext" );
            Assert.That( filter.Matches( file ) );
        }

        [Test]
        public void language_filter_passes_all_when_set_to_None()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "no language",
                Description = "Relevant to files of all languages.",
                Language = FileLanguage.None
            };

            Assert.IsTrue( filter.Matches( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "my.html" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "my.unknownextension" ) ) );
        }

        [Test]
        public void language_filter_passes_matches_only()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "set language",
                Description = "Relevant to C# files.",
                Language = FileLanguage.CSharp
            };

            Assert.IsTrue( filter.Matches( new SourceFile( "my.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( "my.html" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( "my.unknownextension" ) ) );
        }

        [Test]
        public void subpath_filter_passes_all_when_empty()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "no subpath",
                Description = "Relevant to files in all locations.",
                Subpath = ""
            };

            Assert.IsTrue( filter.Matches( new SourceFile( @"my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"specified\subpath\my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"specified\subpath\and\deeper\my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"another\subpath\my.cs" ) ) );
        }

        [Test]
        public void subpath_filter_passes_matches_only()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "specified subpath",
                Description = "Relevant to files in one subtree.",
                Subpath = @"specified\subpath"
            };

            Assert.IsFalse( filter.Matches( new SourceFile( @"my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"specified\subpath\my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"specified\subpath\and\deeper\my.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( @"another\subpath\my.cs" ) ) );
        }

        [Test]
        public void name_pattern_filter_passes_all_when_empty()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "no name pattern",
                Description = "Relevant to files of all names.",
                NamePattern = ""
            };

            Assert.IsTrue( filter.Matches( new SourceFile( @"myCode.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"Tests.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"myTests.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"my_tests.js" ) ) );
        }

        [Test]
        public void name_pattern_filter_passes_matches_only()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "no name pattern",
                Description = "Relevant to files of all names.",
                NamePattern = "tests"
            };

            Assert.IsFalse( filter.Matches( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"Tests.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"myTests.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"my_tests.js" ) ) );
        }

        #endregion

        #region File Content Criteria
        #endregion


    }

}
