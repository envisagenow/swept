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
    public class AddRuleTasksToRunChanges_tests
    {
        [Test]
        public void Can_add_empty_new_details()
        {
            var runChanges = new RunChanges();
            Assert.That(runChanges.DateTime, Is.EqualTo(DateTime.MinValue));
            var ruleTasks = new RuleTasks();
            var runTime = DateTime.Now;

            runChanges.AddRuleTasks( ruleTasks, runTime );

            Assert.That(runChanges.Files.Count(), Is.EqualTo(0));
            Assert.That(runChanges.DateTime, Is.EqualTo(runTime));
        }

        [Test]
        public void RuleTask_without_matching_FileChange_will_add_one()
        {
            var runTime = DateTime.Now;
            var runChanges = new RunChanges();

            var ruleTasks = new RuleTasks();
            var rule = new Rule { ID = "Req 15" };
            var fileTasks = new FileTasks();
            fileTasks.Add(new SourceFile("foo.cs"), new LineMatch(14, 20, 318));
            ruleTasks.Add(rule, fileTasks);


            runChanges.AddRuleTasks(ruleTasks, runTime);


            Assert.That(runChanges.DateTime, Is.EqualTo(runTime));

            Assert.That(runChanges.Files.Count(), Is.EqualTo(1));
            Assert.That(runChanges.Files[0].Rules.Count(), Is.EqualTo(1));
            Assert.That(runChanges.Files[0].Rules[0].Is, Is.EqualTo(3));
            Assert.That(runChanges.Files[0].Rules[0].ID, Is.EqualTo("Req 15"));
        }

        [Test]
        public void RuleTask_without_matching_RuleChange_will_add_one()
        {
            var runTime = DateTime.Now;
            var runChanges = new RunChanges();
            var fileChange = new FileChange { Name = "foo.cs" };
            fileChange.Rules.Add(new RuleChange { ID = "Sys 22", Was = 2 });
            runChanges.Files.Add(fileChange);

            var ruleTasks = new RuleTasks();
            var rule = new Rule { ID = "Req 15" };
            var fileTasks = new FileTasks();
            fileTasks.Add(new SourceFile("foo.cs"), new LineMatch(14, 20, 318));
            ruleTasks.Add(rule, fileTasks);


            runChanges.AddRuleTasks(ruleTasks, runTime);


            Assert.That(runChanges.DateTime, Is.EqualTo(runTime));
            Assert.That(runChanges.Files.Count(), Is.EqualTo(1));

            var fileFoo = runChanges.Files[0];
            Assert.That(fileFoo.Rules.Count(), Is.EqualTo(2));

            var rule22 = fileFoo.Rules[0];
            Assert.That(rule22.ID, Is.EqualTo("Sys 22"));
            Assert.That(rule22.Is, Is.EqualTo(0));
            Assert.That(rule22.Was, Is.EqualTo(2));

            var rule15 = fileFoo.Rules[1];
            Assert.That(rule15.ID, Is.EqualTo("Req 15"));
            Assert.That(rule15.Is, Is.EqualTo(3));
            Assert.That(rule15.Was, Is.EqualTo(0));
        }


        //  Adding RuleTasks for a file/rule already in place will edit instead of add
        [Test]
        public void RuleTask_matching_FileChange_and_RuleChange_will_update_it()
        {
            var runTime = DateTime.Now;
            var runChanges = new RunChanges();
            var fileChange = new FileChange { Name = "foo.cs" };
            fileChange.Rules.Add(new RuleChange { ID = "Req 15", Was = 2 });
            runChanges.Files.Add(fileChange);

            var ruleTasks = new RuleTasks();
            var rule = new Rule { ID = "Req 15" };
            var fileTasks = new FileTasks();
            fileTasks.Add(new SourceFile("foo.cs"), new LineMatch(14, 20, 318));
            ruleTasks.Add(rule, fileTasks);


            runChanges.AddRuleTasks(ruleTasks, runTime);


            Assert.That(runChanges.DateTime, Is.EqualTo(runTime));
            Assert.That(runChanges.Files.Count(), Is.EqualTo(1));

            var fileFoo = runChanges.Files[0];
            Assert.That(fileFoo.Changed);
            Assert.That(fileFoo.Rules.Count(), Is.EqualTo(1));

            var rule15 = fileFoo.Rules[0];
            Assert.That(rule15.Is, Is.EqualTo(3));
            Assert.That(rule15.Was, Is.EqualTo(2));
            Assert.That(rule15.ID, Is.EqualTo("Req 15"));
        }


    }
}
