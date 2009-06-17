//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System.Text;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeTests
    {
        [Test]
        public void CanCreateChange()
        {
            const string newID = "NEW";
            const string newDescription = "Brand new";
            const FileLanguage newLanguage = FileLanguage.CSharp;

            Change chg = new Change( newID, newDescription, newLanguage );

            Assert.AreEqual( newID, chg.ID );
            Assert.AreEqual( newDescription, chg.Description );
            Assert.AreEqual( newLanguage, chg.Language );
        }

        [Test]
        public void Can_serialize_ToXml()
        {
            Change change = new Change("Uno", "Eliminate profanity from error messages.", FileLanguage.CSharp);

            string xmlText = change.ToXmlText();

            string expectedXml = "    <Change ID='Uno' Description='Eliminate profanity from error messages.' Language='CSharp' />";

            Assert.AreEqual(expectedXml, xmlText);
        }
    }
}
