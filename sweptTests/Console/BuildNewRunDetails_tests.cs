using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{

    [TestFixture]
    public class BuildNewRunDetails_tests
    {
        [Test]
        public void Can_initialize_empty_run_details()
        {
            RunDetails oldDetails = new RunDetails { RunNumber = 17 };

            RunDetails result = oldDetails.InitializeNextRunDetails();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(oldDetails));
            Assert.That(result.Files.Count(), Is.EqualTo(0));
            Assert.That(result.RunNumber, Is.EqualTo(18));
        }

        [Test]
        public void Initializing_run_details_moves_is_to_was()
        {
            RunDetails oldDetails = new RunDetails();
            var fileFoo = new DetailFile { Name = "foo.cs" };
            fileFoo.Rules.Add(new DetailRule { ID = "aa04", Was = 88, Is = 53 });
            oldDetails.Files.Add(fileFoo);

            RunDetails nextDetails = oldDetails.InitializeNextRunDetails();

            Assert.That(nextDetails, Is.Not.Null);

            Assert.That(nextDetails.Files.Count(), Is.EqualTo(1));

            var nextFileFoo = nextDetails.Files[0];
            Assert.That(nextFileFoo.Rules.Count(), Is.EqualTo(1));

            var nextRuleAa04 = nextFileFoo.Rules[0];
            Assert.That(nextRuleAa04.Was, Is.EqualTo(53));
        }

        [Test]
        public void Initializing_run_details_brings_all_files_and_rules()
        {
            RunDetails oldDetails = new RunDetails();
            var fileFoo = new DetailFile { Name = "foo.cs" };
            fileFoo.Rules.Add(new DetailRule { ID = "aa04", Was = 88, Is = 53 });
            oldDetails.Files.Add(fileFoo);

            var fileBar = new DetailFile { Name = "bar.cs" };
            fileBar.Rules.Add(new DetailRule { ID = "aa04", Was = 88, Is = 11 });
            fileBar.Rules.Add(new DetailRule { ID = "aa99", Was = 3, Is = 9 });
            oldDetails.Files.Add(fileBar);

            RunDetails nextDetails = oldDetails.InitializeNextRunDetails();

            Assert.That(nextDetails, Is.Not.Null);

            Assert.That(nextDetails.Files.Count(), Is.EqualTo(2));

            var nextFileFoo = nextDetails.Files[0];
            Assert.That(nextFileFoo.Rules.Count(), Is.EqualTo(1));

            var nextRule = nextFileFoo.Rules[0];
            Assert.That(nextRule.Was, Is.EqualTo(53));

            var nextFileBar = nextDetails.Files[1];
            Assert.That(nextFileBar.Rules.Count(), Is.EqualTo(2));

            nextRule = nextFileBar.Rules[0];
            Assert.That(nextRule.Was, Is.EqualTo(11));

            nextRule = nextFileBar.Rules[1];
            Assert.That(nextRule.Was, Is.EqualTo(9));
            Assert.That(nextRule.Is, Is.EqualTo(0));
        }


        [Test]
        public void Next_run_details_removes_rules_with_no_historic_problems()
        {
            RunDetails oldDetails = new RunDetails();

            var fileBar = new DetailFile { Name = "bar.cs" };
            fileBar.Rules.Add(new DetailRule { ID = "aa99", Was = 3, Is = 0 });
            fileBar.Rules.Add(new DetailRule { ID = "aa04", Was = 88, Is = 11 });
            oldDetails.Files.Add(fileBar);

            RunDetails nextDetails = oldDetails.InitializeNextRunDetails();

            Assert.That(nextDetails.Files.Count(), Is.EqualTo(1));

            var nextFileBar = nextDetails.Files[0];
            Assert.That(nextFileBar.Rules.Count(), Is.EqualTo(1));

            var nextRule = nextFileBar.Rules[0];
            Assert.That(nextRule.ID, Is.EqualTo("aa04"));
            Assert.That(nextRule.Was, Is.EqualTo(11));
            Assert.That(nextRule.Is, Is.EqualTo(0));
        }


        [Test]
        public void Next_run_details_removes_files_with_no_historic_problems()
        {
            RunDetails oldDetails = new RunDetails();
            var fileFoo = new DetailFile { Name = "foo.cs" };
            fileFoo.Rules.Add(new DetailRule { ID = "aa04", Was = 88, Is = 0 });
            fileFoo.Rules.Add(new DetailRule { ID = "aa99", Was = 3, Is = 0 });
            oldDetails.Files.Add(fileFoo);

            var fileBar = new DetailFile { Name = "bar.cs" };
            fileBar.Rules.Add(new DetailRule { ID = "aa04", Was = 88, Is = 11 });
            fileBar.Rules.Add(new DetailRule { ID = "aa99", Was = 3, Is = 0 });
            oldDetails.Files.Add(fileBar);

            RunDetails nextDetails = oldDetails.InitializeNextRunDetails();

            Assert.That(nextDetails, Is.Not.Null);

            Assert.That(nextDetails.Files.Count(), Is.EqualTo(1));

            var nextFileBar = nextDetails.Files[0];
            Assert.That(nextFileBar.Name, Is.EqualTo("bar.cs"));
        }
    }
}
