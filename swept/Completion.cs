//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System;

namespace swept
{
    public class Completion
    {
        public string ChangeID;


        public Completion() {}
        public Completion( string changeID )
        {
            ChangeID = changeID;
        }

        public Completion Clone()
        {
            return new Completion( ChangeID );
        }

        #region Serialization
        public static Completion FromNode( XmlNode completion )
        {
            string changeID = completion.Attributes["ID"].Value;
            Completion fileChange = new Completion( changeID );
            return fileChange;
        }

        public string ToXmlText()
        {
            return string.Format( "        <Completion ID='{0}' />\r\n", ChangeID );
        }
        #endregion Serialization
    }
}
