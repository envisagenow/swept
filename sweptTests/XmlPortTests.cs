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


        // TODO--0.N: "Note that a 'Not' operator on a top level change won't do what you want.
        // Make an enclosed 'Not' filter instead."


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
