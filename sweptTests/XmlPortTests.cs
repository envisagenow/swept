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
        public void SourceFileCatalog_ToText()
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

        // TODO--0.N: "Note that a 'Not' operator on a top level change won't do what you want.  Make an enclosed 'Not' filter instead."

        [Test]
        public void basic_ChangeCatalog_ToXml()
        {
            ChangeCatalog cat = new ChangeCatalog();
            cat.Add( new Change { ID = "1", Description = "Desc", Language = FileLanguage.CSharp } );
            Assert.That( port.ToText( cat ), Is.EqualTo( 
@"<ChangeCatalog>
    <Change ID='1' Description='Desc' Language='CSharp' />
</ChangeCatalog>" ) );
        }

        [Test]
        public void saved_Changes_are_sorted_by_ID()
        {
            ChangeCatalog cat = new ChangeCatalog();
            cat.Add( new Change { ID = "003", Description = "A", Language = FileLanguage.CSharp } );
            cat.Add( new Change { ID = "002", Description = "B", Language = FileLanguage.CSS } );
            cat.Add( new Change { ID = "004", Description = "C", Language = FileLanguage.HTML } );
            cat.Add( new Change { ID = "001", Description = "D", Language = FileLanguage.JavaScript } );
            Assert.That( port.ToText( cat ), Is.EqualTo(
@"<ChangeCatalog>
    <Change ID='001' Description='D' Language='JavaScript' />
    <Change ID='002' Description='B' Language='CSS' />
    <Change ID='003' Description='A' Language='CSharp' />
    <Change ID='004' Description='C' Language='HTML' />
</ChangeCatalog>" ) );
        }


        [Test]
        public void saved_SourceFiles_are_sorted_on_file_fqName()
        {
            SourceFileCatalog cat = new SourceFileCatalog();
            cat.Add( new SourceFile( "a.cs" ) );
            cat.Add( new SourceFile( "d.cs" ) );
            cat.Add( new SourceFile( "b.cs" ) );
            cat.Add( new SourceFile( "c.cs" ) );

            Assert.That( port.ToText( cat ), Is.EqualTo(
@"<SourceFileCatalog>
    <SourceFile Name='a.cs'>
    </SourceFile>
    <SourceFile Name='b.cs'>
    </SourceFile>
    <SourceFile Name='c.cs'>
    </SourceFile>
    <SourceFile Name='d.cs'>
    </SourceFile>
</SourceFileCatalog>" ) );
        }

        [Test]
        public void saved_Completions_are_sorted_on_ID()
        {
            var sourceFile = new SourceFile( "a.cs" );
            sourceFile.AddNewCompletion( "003" );
            sourceFile.AddNewCompletion( "002" );
            sourceFile.AddNewCompletion( "004" );
            sourceFile.AddNewCompletion( "001" );
            SourceFileCatalog cat = new SourceFileCatalog();
            cat.Add( sourceFile );

            Assert.That( port.ToText( cat ), Is.EqualTo(
@"<SourceFileCatalog>
    <SourceFile Name='a.cs'>
        <Completion ID='001' />
        <Completion ID='002' />
        <Completion ID='003' />
        <Completion ID='004' />
    </SourceFile>
</SourceFileCatalog>" ) );
        }

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
