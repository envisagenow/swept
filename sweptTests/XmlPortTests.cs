﻿//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Xml.Linq;
using NUnit.Framework;
using System.Xml;

namespace swept.Tests
{
    [TestFixture]
    public class XmlPortTests
    {
        private XmlPort port;

        [SetUp]
        public void CanCreate()
        {
            port = new XmlPort();
        }

        [Test]
        public void Completion_ToXmlText()
        {
            Completion comp = new Completion("cr001");
            Assert.AreEqual("        <Completion ID='cr001' />\r\n", port.ToText(comp));
        }

        [Test]
        public void Completion_ToXmlText_ShouldEscapeIDs()
        {
            Completion comp = new Completion("cr<un'escaped>");
            Assert.AreEqual("        <Completion ID='cr<un\'escaped>' />\r\n", port.ToText(comp));
        }

        [Test]
        public void SourceFile_ToXmlText()
        {
            SourceFile file = new SourceFile("foo.cs");
            Assert.AreEqual(0, file.Completions.Count);
            file.MarkCompleted("13");
            Assert.AreEqual(1, file.Completions.Count);

            string serializedFile = port.ToText(file);
            string expectedFile =
@"    <SourceFile Name='foo.cs'>
        <Completion ID='13' />
    </SourceFile>
";
            Assert.AreEqual(expectedFile, serializedFile);
        }

        [Test]
        public void SourceFileCatalog_ToXMLText()
        {
            SourceFileCatalog fileCat = new SourceFileCatalog();
            SourceFile blue_cs = fileCat.Fetch("blue.cs");

            blue_cs.Completions.Add(new Completion("id11"));

            string text = port.ToText(fileCat);
            string answer =
@"<SourceFileCatalog>
    <SourceFile Name='blue.cs'>
        <Completion ID='id11' />
    </SourceFile>
</SourceFileCatalog>";
            Assert.AreEqual(answer, text);
        }

        private SourceFileCatalog get_testing_SourceFileCatalog()
        {
            string catalogText =
@"<SweptProjectData>
<SourceFileCatalog>
    <SourceFile Name='bar.cs'>
        <Completion ID='AB1' />
        <Completion ID='AB2' />
    </SourceFile>
    <SourceFile Name='foo.cs'>
        <Completion ID='anotherID' />
    </SourceFile>
</SourceFileCatalog>
</SweptProjectData>";

            return port.SourceFileCatalog_FromText(catalogText);
        }


        [Test]
        public void SourceFileCatalog_FromXmlText_gets_all_Files()
        {
            SourceFileCatalog cat = get_testing_SourceFileCatalog();
            Assert.AreEqual(2, cat.Files.Count);

            SourceFile bar_cs = cat.Files[0];
            Assert.AreEqual("bar.cs", bar_cs.Name);
        }

        [Test]
        public void SourceFileCatalog_FromXmlText_gets_all_Completions()
        {
            SourceFileCatalog cat = get_testing_SourceFileCatalog();
            SourceFile bar_cs = cat.Files[0];
            Assert.AreEqual( 2, bar_cs.Completions.Count );
            Assert.AreEqual( "AB2", bar_cs.Completions[1].ChangeID );
        }

        [Test, Ignore]
        public void SourceFileCatalog_FromXmlText_gets_IsRemoved()
        {
            // TODO: SourceFileCatalog_FromXmlText_gets_IsRemoved()
            // TODO: IsRemoved_gets_serialized
        }

        [Test]
        public void Can_serialize_Change_ToXml()
        {
            Change change = new Change("Uno", "Eliminate profanity from error messages.", FileLanguage.CSharp);

            string xmlText = port.ToText(change);

            string expectedXml = "    <Change ID='Uno' Description='Eliminate profanity from error messages.' Language='CSharp' />" + Environment.NewLine;

            Assert.AreEqual(expectedXml, xmlText);
        } 

        [Test]
        public void Can_serialize_ToXml()
        {
            ChangeCatalog cat = new ChangeCatalog();
            cat.Add(new Change("1", "Desc", FileLanguage.CSharp));
            string expectedXml =
@"<ChangeCatalog>
    <Change ID='1' Description='Desc' Language='CSharp' />
</ChangeCatalog>";

            Assert.AreEqual(expectedXml, port.ToText(cat));
        }

        //[Test]
        //public void ChangeCatalog_from_XML_gets_changes()
        //{
        //    
        //}

        #region Exception testing
        [Test, ExpectedException(ExpectedMessage = "Document must have a <SourceFileCatalog> node.  Please supply one.")]
        public void IncorrectRootNodeThrows()
        {
            port.SourceFileCatalog_FromText("<IncorrectRoot><other /></IncorrectRoot>");
        }

        [Test, ExpectedException(ExpectedMessage = "A SourceFile node must have a Name attribute.  Please add one.")]
        public void SourceFileNodeMissingNameThrows()
        {
            port.SourceFileCatalog_FromText("<SweptProjectData><SourceFileCatalog><SourceFile /></SourceFileCatalog></SweptProjectData>");
        }


        [Test, ExpectedException( ExpectedMessage = "Text [asdflkj] was not valid XML.  Please check its contents.  Details: Data at the root level is invalid. Line 1, position 1." )]
        public void SourceFileCatalog_from_InvalidXML_Throws()
        {
            port.SourceFileCatalog_FromText( "asdflkj" );
        }

        [Test, ExpectedException( ExpectedMessage = "Document must have a <ChangeCatalog> node.  Please supply one." )]
        public void ChangeCatalog_from_IncorrectXML_Throws()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml( "<x>eek!</x>" );
            port.ChangeCatalog_FromXmlDocument( xml );
        }

        #endregion
    }
}
