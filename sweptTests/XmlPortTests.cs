//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
// TODO--0.3, DC: replace the hand serialization with XElement code
// using System.Xml.Linq;
using NUnit.Framework;
using System.Xml;

namespace swept.Tests
{
    [TestFixture]
    public class XmlPortTests
    {
        private XmlPort port;

        [SetUp]
        public void given_a_port()
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
        public void CompoundFilter_ToXmlText()
        {
            CompoundFilter filter = new CompoundFilter();

            string serializedFilter = port.ToText( filter );
            string expectedFilter =
@"    <Filter>
    </Filter>
";
            Assert.AreEqual( expectedFilter, serializedFilter );
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
            Assert.AreEqual( 2, cat.Files.Count );

            SourceFile bar_cs = cat.Files[0];
            Assert.AreEqual( "bar.cs", bar_cs.Name );
            Assert.That( bar_cs.Completions.Count, Is.EqualTo( 2 ) );
        }


        private SourceFileCatalog get_full_library_SourceFileCatalog()
        {
            string catalogText =
@"<SweptProjectData>
<ChangeCatalog>
    <Change ID='007' Description='Bond.  James Bond.' Language='CSharp' />
    <Change ID='P-01' Description='Possible task' Language='CSharp' />
    <Change ID='Hard1' Description='This is hard' Language='CSharp' />
</ChangeCatalog>
<SourceFileCatalog>
    <SourceFile Name='SymbolReplacer\Replacer.cs'>
        <Completion ID='007' />
    </SourceFile>
    <SourceFile Name='SymbolReplacerTests\ReplacerTests.cs'>
        <Completion ID='P-01' />
    </SourceFile>
</SourceFileCatalog>
</SweptProjectData>";

            return port.SourceFileCatalog_FromText( catalogText );
        }
        [Test]
        public void SourceFileCatalog_From_demo_gets_all_Files()
        {
            SourceFileCatalog cat = get_full_library_SourceFileCatalog();
            Assert.AreEqual( 2, cat.Files.Count );

            SourceFile replacer_cs = cat.Files[0];
            Assert.That( replacer_cs.Completions.Count, Is.EqualTo( 1 ) );
        }

        [Test]
        public void SourceFileCatalog_FromXmlText_gets_all_Completions()
        {
            SourceFileCatalog cat = get_testing_SourceFileCatalog();
            SourceFile bar_cs = cat.Files[0];
            Assert.AreEqual( 2, bar_cs.Completions.Count );
            Assert.AreEqual( "AB2", bar_cs.Completions[1].ChangeID );
        }

        // TODO: public void Removed_Changes_are_saved()
        // TODO: public void Removed_Changes_are_loaded()

        [Test]
        public void Can_serialize_full_Change_ToXml()
        {
            Change change = new Change {
                ID = "Uno",
                Description = "Eliminate profanity from error messages.",
                Language = FileLanguage.CSharp,
                Subpath = @"utilities\error_handling",
                NamePattern = "messages",
            };

            string xmlText = port.ToText(change);

            string expectedXml = string.Format(
                "    <Change ID='{0}' Description='{1}' Language='{2}' Subpath='{3}' NamePattern='{4}' />{5}", 
                change.ID, change.Description, change.Language, change.Subpath, change.NamePattern, 
                Environment.NewLine
            );

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

        // TODO: public void Changes_are_sorted()
        // TODO: public void SourceFiles_are_sorted()
        // TODO: public void Completions_are_sorted()

        #region Compound filters from XML
        [Test]
        public void basic_nested()
        {
            string filter_text =
@"<Filter ID='parent'>
    <Filter ID='child'>
    </Filter>
</Filter>
";
            CompoundFilter filter = port.CompoundFilter_FromText( filter_text );
            Assert.That( filter.ID, Is.EqualTo( "parent" ) );
            Assert.That( filter.Children.Count, Is.EqualTo( 1 ) );
            Assert.That( filter.Children.Count, Is.EqualTo( 1 ) );
        }
        #endregion


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
