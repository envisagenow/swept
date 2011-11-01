//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using System.Xml;
using System.Collections.Generic;

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
        public void Populates_BuildFail_from_attribute()
        {
            string changeText = "<SweptProjectData><ChangeCatalog><Change ID='this' BuildFail='Any'> ^CSharp </Change></ChangeCatalog></SweptProjectData>";
            XmlDocument xml = new XmlDocument();
            xml.LoadXml( changeText );

            var cat = port.ChangeCatalog_FromXmlDocument( xml );
            List<Change> changes = cat.GetSortedChanges();
            Assert.That( changes.Count, Is.EqualTo( 1 ) );

            Assert.That( changes[0].BuildFail, Is.EqualTo( BuildFailMode.Any ) );
        }

        [Test]
        public void Populates_BuildFail_gives_clear_exception_on_invalid_value()
        {
            string changeText = "<SweptProjectData><ChangeCatalog><Change ID='this' BuildFail='Fake'> ^CSharp </Change></ChangeCatalog></SweptProjectData>";
            XmlDocument xml = new XmlDocument();
            xml.LoadXml( changeText );

            var ex = Assert.Throws<Exception>( () => port.ChangeCatalog_FromXmlDocument( xml ) );

            Assert.That( ex.Message, Is.EqualTo( "Change ID [this] has an unknown BuildFail value [Fake]." ) );
        }


        #region Exception testing
        [CoverageExclude]
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
