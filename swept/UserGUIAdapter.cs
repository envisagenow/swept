//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Windows.Forms;
using System.Xml;

namespace swept
{
    public class UserGUIAdapter : IUserAdapter
    {
        public bool KeepChangeHistory( Change historicalChange )
        {
            //TODO--0.3:  Better message
            DialogResult keepChangeHistory = MessageBox.Show( "This change existed in the past.  Should I keep its history?" );
            return (keepChangeHistory == DialogResult.Yes);
        }

        public bool KeepSourceFileHistory( SourceFile historicalFile )
        {
            //TODO--0.3:  Better message
            DialogResult keepFileHistory = MessageBox.Show( "This source file existed in the past.  Shall I keep its history?" );
            return (keepFileHistory == DialogResult.Yes);
        }

        public void BadXmlInExpectedLibrary( string libraryPath, XmlException exception )
        {
            MessageBox.Show( string.Format( 
                "Invalid XML in the Swept library at {0}: {1}", libraryPath, exception.Message ), 
                "", MessageBoxButtons.OK );
        }

        // TODO--0.N: a sensible debug framework
        public void DebugMessage( string message )
        {
            //MessageBox.Show( message );
        }

        public void ShowSeeAlso( SeeAlso seeAlso )
        {
            throw new Exception( String.Format( "See also not implemented:  [{0}].", seeAlso.Description ) );
            //if (! string.IsNullOrEmpty(seeAlso.URL))
            //    System.Diagnostics.Process.Start( seeAlso.URL );
        }

    }
}
