//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;
using swept;
using System.Collections.Generic;
using System.Text;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeCatalogTests
    {
        private ChangeCatalog cat;
        [SetUp]
        public void CanCreate()
        {
            cat = new ChangeCatalog();
        }

        [Test]
        public void CanGetChangesFilteredByLanguage()
        {
            cat.Add(new Change("e1", "Fix 'using' statements", FileLanguage.CSharp));
            cat.Add(new Change("e2", "Upgrade to XHTML", FileLanguage.HTML));
            cat.Add(new Change("e3", "Put <title> on all pages", FileLanguage.HTML));

            List<Change> changes = cat.GetListForLanguage(FileLanguage.HTML);
            List<Change> moreChanges = cat.FindAll(ch => ch.Language == FileLanguage.HTML);
            Assert.IsTrue(changes.TrueForAll(ch => moreChanges.Contains(ch)));
            Assert.AreEqual(moreChanges.Count, changes.Count);

            changes = cat.GetListForLanguage(FileLanguage.CSharp);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("e1", changes[0].ID);
        }

        [Test]
        public void Can_serialize_ToXml()
        {
            cat.Add(new Change("1", "Desc", FileLanguage.CSharp));
            string expectedXml =
@"<ChangeCatalog>
    <Change ID='1' Description='Desc' Language='CSharp' />
</ChangeCatalog>";

            Assert.AreEqual(expectedXml, cat.ToXmlText());
        }
    }
}
