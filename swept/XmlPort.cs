//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;

namespace swept
{
    internal class XmlPort
    {
        XmlPort_CompoundFilter _filterPort;

        public XmlPort()
        {
            _filterPort = new XmlPort_CompoundFilter();
        }

        public ChangeCatalog ChangeCatalog_FromXmlDocument( XmlDocument doc )
        {
            XmlNode node = doc.SelectSingleNode( "SweptProjectData/ChangeCatalog" );

            if( node == null )
                throw new Exception( "Document must have a <ChangeCatalog> node.  Please supply one." );

            return ChangeCatalog_FromNode( node );
        }


        public ChangeCatalog ChangeCatalog_FromNode( XmlNode node )
        {
            ChangeCatalog cat = new ChangeCatalog();

            XmlNodeList changes = node.SelectNodes( "Change" );
            foreach( XmlNode changeNode in changes )
            {
                Change change = (Change)_filterPort.CompoundFilter_FromNode( changeNode );

                cat.Add( change );
            }

            return cat;
        }
    }
}
