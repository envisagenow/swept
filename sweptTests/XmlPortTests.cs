//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
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
        private ChangeCatalog getChangeCatalog( string changeText )
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml( changeText );
            var port = new XmlPort();
            return port.ChangeCatalog_FromXmlDocument( xml );
        }

        [Test]
        public void Populates_FailMode_from_attribute()
        {
            var cat = getChangeCatalog( "<SweptProjectData><ChangeCatalog><Change ID='this' FailMode='Any'> ^CSharp </Change></ChangeCatalog></SweptProjectData>" );
            List<Change> changes = cat.GetSortedChanges();
            Assert.That( changes.Count, Is.EqualTo( 1 ) );

            Assert.That( changes[0].BuildFail, Is.EqualTo( BuildFailMode.Any ) );
        }

        [Test]
        public void Populates_FailOver_Limit_from_attribute()
        {
            var cat = getChangeCatalog( "<SweptProjectData><ChangeCatalog><Change ID='this' FailMode='Over' Limit='2'> ^CSharp </Change></ChangeCatalog></SweptProjectData>" );
            List<Change> changes = cat.GetSortedChanges();
            Assert.That( changes.Count, Is.EqualTo( 1 ) );

            Assert.That( changes[0].BuildFail, Is.EqualTo( BuildFailMode.Over ) );
            Assert.That( changes[0].BuildFailOverLimit, Is.EqualTo( 2 ) );
        }

        [Test]
        public void ChangeCatalog_from_IncorrectXML_Throws()
        {
            var ex = Assert.Throws<Exception>( () => getChangeCatalog( "<x>eek!</x>" ) );
            Assert.That( ex.Message, Is.EqualTo( "Document must have a <ChangeCatalog> node inside a <SweptProjectData> node.  Please supply one." ) );
        }

        [Test]
        public void FailMode_parse_gives_clear_exception_on_invalid_value()
        {
            string badMode = "<SweptProjectData><ChangeCatalog><Change ID='this' FailMode='Fake'> ^CSharp </Change></ChangeCatalog></SweptProjectData>";
            var ex = Assert.Throws<Exception>( () => getChangeCatalog( badMode ) );
            Assert.That( ex.Message, Is.EqualTo( "Change ID [this] has an unknown FailMode value [Fake]." ) );
        }

        [Test]
        public void FailOver_Limit_gripes_when_missing_or_gibberish()
        {
            string badLimit = "<SweptProjectData><ChangeCatalog><Change ID='this' FailMode='Over' Limit='asfdfds'> ^CSharp </Change></ChangeCatalog></SweptProjectData>";
            Exception ex = Assert.Throws<Exception>( () => getChangeCatalog( badLimit ) );
            Assert.That( ex.Message, Is.EqualTo( "Found no integer 'Limit' for Change ID [this]." ) );
        }
    }
}
