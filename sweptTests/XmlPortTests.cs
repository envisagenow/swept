//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
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


        // TODO--0.N: "Note that a 'Not' operator on a top level change won't do what you want.  Make an enclosed 'Not' filter instead."

        // TODO--0.3: strip out the serialization
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
