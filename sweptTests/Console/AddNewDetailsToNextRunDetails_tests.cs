using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class AddNewDetailsToNextRunDetails_tests
    {
        [Test]
        public void Can_add_empty_new_details()
        {
            RunDetails nextDetails = new RunDetails();
            Assert.That(nextDetails.DateTime, Is.EqualTo(DateTime.MinValue));
            var ruleTasks = new RuleTasks();
            var runTime = DateTime.Now;

            nextDetails.AddThisRun( ruleTasks, runTime );

            Assert.That(nextDetails.Files.Count(), Is.EqualTo(0));
            Assert.That(nextDetails.DateTime, Is.EqualTo(runTime));
        }

        [Test]
        public void one_RuleTask_will_be_added()
        {
            var runTime = DateTime.Now;
            RunDetails nextDetails = new RunDetails();

            var ruleTasks = new RuleTasks();
            var rule = new Rule { ID = "Req 15" };
            var fileTasks = new FileTasks();
            fileTasks.Add(new SourceFile("foo.cs"), new LineMatch(14, 20, 318));
            ruleTasks.Add(rule, fileTasks);


            nextDetails.AddThisRun(ruleTasks, runTime);

            Assert.That(nextDetails.DateTime, Is.EqualTo(runTime));

            Assert.That(nextDetails.Files.Count(), Is.EqualTo(1));
            Assert.That(nextDetails.Files[0].Rules.Count(), Is.EqualTo(1));
            Assert.That(nextDetails.Files[0].Rules[0].Is, Is.EqualTo(3));
            Assert.That(nextDetails.Files[0].Rules[0].ID, Is.EqualTo("Req 15"));
        }

        //  when we have a detailRule already in place, the ruletask will add information to the existing one instead of adding a new one
        [Test]
        public void needs_short_name()
        {
            var runTime = DateTime.Now;
            RunDetails nextDetails = new RunDetails();

            var ruleTasks = new RuleTasks();
            var rule = new Rule { ID = "Req 15" };
            var fileTasks = new FileTasks();
            fileTasks.Add(new SourceFile("foo.cs"), new LineMatch(14, 20, 318));
            ruleTasks.Add(rule, fileTasks);


            nextDetails.AddThisRun(ruleTasks, runTime);

            Assert.That(nextDetails.DateTime, Is.EqualTo(runTime));

            Assert.That(nextDetails.Files.Count(), Is.EqualTo(1));
            Assert.That(nextDetails.Files[0].Rules.Count(), Is.EqualTo(1));
            Assert.That(nextDetails.Files[0].Rules[0].Is, Is.EqualTo(3));
            Assert.That(nextDetails.Files[0].Rules[0].ID, Is.EqualTo("Req 15"));
        }


    }
}
