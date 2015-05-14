//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2015 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{

    [TestFixture]
    public class BuildNewRunChanges_tests
    {
        [Test]
        public void Can_InitializeNextRun_when_empty()
        {
            RunChanges oldRun = new RunChanges { RunNumber = 17 };

            RunChanges result = oldRun.InitializeNextRun();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(oldRun));
            Assert.That(result.Files.Count(), Is.EqualTo(0));
            Assert.That(result.RunNumber, Is.EqualTo(18));
        }

        [Test]
        public void Next_run_has_Is_values_moved_to_Was()
        {
            RunChanges oldRun = new RunChanges();
            var fileFoo = new FileChange { Name = "foo.cs" };
            fileFoo.Rules.Add(new RuleChange { ID = "aa04", Was = 88, Is = 53 });
            oldRun.Files.Add(fileFoo);

            RunChanges nextRun = oldRun.InitializeNextRun();

            Assert.That(nextRun, Is.Not.Null);

            Assert.That(nextRun.Files.Count(), Is.EqualTo(1));

            var nextFileFoo = nextRun.Files[0];
            Assert.That(nextFileFoo.Rules.Count(), Is.EqualTo(1));

            var nextRuleAa04 = nextFileFoo.Rules[0];
            Assert.That(nextRuleAa04.Was, Is.EqualTo(53));
        }

        [Test]
        public void InitializeNextRun_brings_all_files_and_rules()
        {
            RunChanges oldRun = new RunChanges();
            var fileFoo = new FileChange { Name = "foo.cs" };
            fileFoo.Rules.Add(new RuleChange { ID = "aa04", Was = 88, Is = 53 });
            oldRun.Files.Add(fileFoo);

            var fileBar = new FileChange { Name = "bar.cs" };
            fileBar.Rules.Add(new RuleChange { ID = "aa04", Was = 88, Is = 11 });
            fileBar.Rules.Add(new RuleChange { ID = "aa99", Was = 3, Is = 9 });
            oldRun.Files.Add(fileBar);

            RunChanges nextRun = oldRun.InitializeNextRun();

            Assert.That(nextRun, Is.Not.Null);

            Assert.That(nextRun.Files.Count(), Is.EqualTo(2));

            var nextFileFoo = nextRun.Files[0];
            Assert.That(nextFileFoo.Rules.Count(), Is.EqualTo(1));

            var nextRule = nextFileFoo.Rules[0];
            Assert.That(nextRule.Was, Is.EqualTo(53));

            var nextFileBar = nextRun.Files[1];
            Assert.That(nextFileBar.Rules.Count(), Is.EqualTo(2));

            nextRule = nextFileBar.Rules[0];
            Assert.That(nextRule.Was, Is.EqualTo(11));

            nextRule = nextFileBar.Rules[1];
            Assert.That(nextRule.Was, Is.EqualTo(9));
            Assert.That(nextRule.Is, Is.EqualTo(0));
        }


        [Test]
        public void Next_run_omits_rules_with_no_tasks()
        {
            RunChanges oldRun = new RunChanges();

            var fileBar = new FileChange { Name = "bar.cs" };
            fileBar.Rules.Add(new RuleChange { ID = "aa99", Was = 3, Is = 0 });
            fileBar.Rules.Add(new RuleChange { ID = "aa04", Was = 88, Is = 11 });
            oldRun.Files.Add(fileBar);

            RunChanges nextRun = oldRun.InitializeNextRun();

            Assert.That(nextRun.Files.Count(), Is.EqualTo(1));

            var nextFileBar = nextRun.Files[0];
            Assert.That(nextFileBar.Rules.Count(), Is.EqualTo(1));

            var nextRule = nextFileBar.Rules[0];
            Assert.That(nextRule.ID, Is.EqualTo("aa04"));
            Assert.That(nextRule.Was, Is.EqualTo(11));
            Assert.That(nextRule.Is, Is.EqualTo(0));
        }


        [Test]
        public void Next_run_omits_files_with_no_tasks()
        {
            RunChanges oldRun = new RunChanges();
            var fileFoo = new FileChange { Name = "foo.cs" };
            fileFoo.Rules.Add(new RuleChange { ID = "aa04", Was = 88, Is = 0 });
            fileFoo.Rules.Add(new RuleChange { ID = "aa99", Was = 3, Is = 0 });
            oldRun.Files.Add(fileFoo);

            var fileBar = new FileChange { Name = "bar.cs" };
            fileBar.Rules.Add(new RuleChange { ID = "aa04", Was = 88, Is = 11 });
            fileBar.Rules.Add(new RuleChange { ID = "aa99", Was = 3, Is = 0 });
            oldRun.Files.Add(fileBar);

            RunChanges nextRun = oldRun.InitializeNextRun();

            Assert.That(nextRun, Is.Not.Null);

            Assert.That(nextRun.Files.Count(), Is.EqualTo(1));

            var nextFileBar = nextRun.Files[0];
            Assert.That(nextFileBar.Name, Is.EqualTo("bar.cs"));
        }
    } 
}
