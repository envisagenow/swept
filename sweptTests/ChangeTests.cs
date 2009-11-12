//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeTests
    {
        // TODO: figure out how to keep ChangeTests up to date as attributes are added...
        [Test]
        public void CanCreateChange()
        {
            const string newID = "NEW";
            const string newDescription = "Brand new";

            const string path = @"somewhere";
            const string namePattern = "problematic.code();";
            const FileLanguage newLanguage = FileLanguage.CSharp;

            const string problematicContent = "problematic.code();";

            Change change = new Change { 
                ID = newID, Description = newDescription, 
                Language = newLanguage, Subpath = path, NamePattern = namePattern,
                ContentPattern = problematicContent
            };

            Assert.AreEqual( newID, change.ID );
            Assert.AreEqual( newDescription, change.Description );
            Assert.AreEqual( newLanguage, change.Language );
        }
    }
}
