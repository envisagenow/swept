//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class XmlPort_SourceCatalog_Tests
    {
        private XmlPort port;

        [SetUp]
        public void given_a_port()
        {
            port = new XmlPort();
        }

        [Test]
        public void SourceFile_ToXmlText()
        {
            SourceFile file = new SourceFile( "foo.cs" );

            string serializedFile = port.ToText( file );
            string expectedFile =
@"    <SourceFile Name='foo.cs'>
    </SourceFile>
";
            Assert.AreEqual( expectedFile, serializedFile );
        }

        [Test]
        public void SourceFileCatalog_ToXMLText()
        {
            SourceFileCatalog fileCat = new SourceFileCatalog();
            SourceFile blue_cs = fileCat.Fetch( "blue.cs" );

            string text = port.ToText( fileCat );
            string answer =
@"<SourceFileCatalog>
    <SourceFile Name='blue.cs'>
    </SourceFile>
</SourceFileCatalog>";
            Assert.AreEqual( answer, text );
        }

        private SourceFileCatalog get_testing_SourceFileCatalog()
        {
            string catalogText =
@"<SweptProjectData>
<SourceFileCatalog>
    <SourceFile Name='bar.cs'>
    </SourceFile>
    <SourceFile Name='foo.cs'>
    </SourceFile>
</SourceFileCatalog>
</SweptProjectData>";

            return port.SourceFileCatalog_FromText( catalogText );
        }

        [Test]
        public void SourceFileCatalog_FromXmlText_gets_all_Files()
        {
            SourceFileCatalog cat = get_testing_SourceFileCatalog();
            Assert.AreEqual( 2, cat.Files.Count );

            SourceFile bar_cs = cat.Files[0];
            Assert.AreEqual( "bar.cs", bar_cs.Name );
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
    </SourceFile>
    <SourceFile Name='SymbolReplacerTests\ReplacerTests.cs'>
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
        }
    }
}
