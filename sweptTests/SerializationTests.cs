//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using swept;
using System.IO;

namespace swept.Tests
{
    //  There are two major Swept data files per solution:  The Change and Source File catalogs.
    //  The Change Catalog holds things the team wants to improve in this .sln
    //  The Source File Catalog holds all the completions, grouped per file.

    [TestFixture]
    public class SerializationTests
    {
        private const string VB_ID1 = "T1";
        private const string CS_ID2 = "T2";
        private const string VB_ID3 = "T3";


        [Test]
        public void SourceFileCatalog_FromXmlText_gets_all_Files()
        {
            SourceFileCatalog cat = get_testing_SourceFileCatalog();
            Assert.AreEqual(2, cat.Files.Count);

            SourceFile bar_cs = cat.Files[0];
            Assert.AreEqual( "bar.cs", bar_cs.Name );
        }

        [Test]
        public void SourceFileCatalog_FromXmlText_gets_all_Completions()
        {
            SourceFileCatalog cat = get_testing_SourceFileCatalog();
            SourceFile bar_cs = cat.Files[0];
            Assert.AreEqual(2, bar_cs.Completions.Count);
            Assert.AreEqual( "AB2", bar_cs.Completions[1].ChangeID );
        }

        private static SourceFileCatalog get_testing_SourceFileCatalog()
        {
            string catalogText =
@"<SourceFileCatalog>
    <SourceFile Name='bar.cs'>
        <Completion ID='AB1' />
        <Completion ID='AB2' />
    </SourceFile>
    <SourceFile Name='foo.cs'>
        <Completion ID='anotherID' />
    </SourceFile>
</SourceFileCatalog>";

            return SourceFileCatalog.FromXmlText(catalogText);
        }

        [Test]
        public void Completion_ToXmlText()
        {
            Completion comp = new Completion( "cr001" );
            string text = comp.ToXmlText();
            Assert.AreEqual( "        <Completion ID='cr001' />\r\n", text );
        }

        [Test]
        public void Completion_ToXmlText_ShouldEscapeIDs()
        {
            Completion comp = new Completion( "cr<un'escaped>" );
            Assert.AreEqual( "        <Completion ID='cr<un\'escaped>' />\r\n", comp.ToXmlText() );
        }

        [Test]
        public void SourceFile_ToXmlText()
        {
            SourceFile file = new SourceFile( "foo.cs" );
            Assert.AreEqual( 0, file.Completions.Count );
            file.MarkCompleted( "13" );
            Assert.AreEqual( 1, file.Completions.Count );

            string serializedFile = file.ToXmlText();
            string expectedFile =
@"    <SourceFile Name='foo.cs'>
        <Completion ID='13' />
    </SourceFile>
";
            Assert.AreEqual( expectedFile, serializedFile );
        }

        [Test]
        public void SourceFileCatalog_ToXMLText()
        {
            SourceFileCatalog fileCat = new SourceFileCatalog();
            SourceFile blue_cs = fileCat.Fetch( "blue.cs" );

            blue_cs.Completions.Add( new Completion( "id11" ) );

            string text = fileCat.ToXmlText();
            string answer =
@"<SourceFileCatalog>
    <SourceFile Name='blue.cs'>
        <Completion ID='id11' />
    </SourceFile>
</SourceFileCatalog>";
            Assert.AreEqual( answer, text );
        }

        #region Exception testing
        [Test, ExpectedException( ExpectedMessage="Document must have a <SourceFileCatalog> root node.  Please supply one." )]
        public void IncorrectRootNodeThrows()
        {
            SourceFileCatalog.FromXmlText( "<IncorrectRoot><other /></IncorrectRoot>" );
        }

        [Test, ExpectedException( ExpectedMessage="Can't create a null source file." )]
        public void NullSourceFileThrows()
        {
            SourceFile.FromNode( null );
        }

        [Test, ExpectedException( ExpectedMessage="A SourceFile node must have a Name attribute.  Please add one." )]
        public void SourceFileNodeMissingNameThrows()
        {
            SourceFileCatalog.FromXmlText( "<SourceFileCatalog><SourceFile /></SourceFileCatalog>" );
        }


        [Test, ExpectedException( ExpectedMessage="Text [asdflkj] was not valid XML.  Please check its contents.  Details: Data at the root level is invalid. Line 1, position 1." )]
        public void InvalidXMLThrows()
        {
            SourceFileCatalog.FromXmlText( "asdflkj" );
        }

        #endregion
    }
}
