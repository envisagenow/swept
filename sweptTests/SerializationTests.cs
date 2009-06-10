//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
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
        public void SourceFileCatalogFromXmlText()
        {
            string catalogText = 
@"<SourceFileCatalog>
    <SourceFile Name=""bar.cs"">
        <Completion ID=""AB1"" />
    </SourceFile>
</SourceFileCatalog>";

            SourceFileCatalog cat = SourceFileCatalog.FromXmlText( catalogText );
            Assert.AreEqual( 1, cat.Files.Count );

            SourceFile file = cat.Files[0];
            Assert.AreEqual( "bar.cs", file.Name );
        }


        [Test]
        public void SourceFilePopulateCompletions()
        {
            string catalogText = 
@"<SourceFileCatalog>
    <SourceFile Name=""bar.cs"">
        <Completion ID=""AB1"" />
    </SourceFile>
</SourceFileCatalog>";

            SourceFileCatalog cat = SourceFileCatalog.FromXmlText( catalogText );

            SourceFile file = cat.Files[0];
            Assert.AreEqual( "AB1", file.Completions[0].ChangeID );
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
            Completion comp = new Completion( "cr<unescaped>" );
            Assert.AreEqual( "        <Completion ID='cr<unescaped>' />\r\n", comp.ToXmlText() );
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
        public void CanSerializeToXMLText()
        {
            SourceFileCatalog fileCat = new SourceFileCatalog();
            SourceFile blue = fileCat.FetchFile( "blue.cs" );

            blue.Completions.Add( new Completion( "id11" ) );

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

        [Test, ExpectedException( ExpectedMessage="Can not create a source file without a Name attribute.  Please add one." )]
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
