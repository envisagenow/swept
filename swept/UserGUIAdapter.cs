//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Windows.Forms;

namespace swept
{
    public class UserGUIAdapter : IUserAdapter
    {
        public bool KeepChangeHistory( Change historicalChange )
        {
            //TODO--0.3:  Better message
            DialogResult result = MessageBox.Show( "This change existed in the past.  Should I keep its history?" );
            return (result == DialogResult.Yes);
        }

        public bool KeepSourceFileHistory( SourceFile historicalFile )
        {
            //TODO--0.3:  Better message
            DialogResult result = MessageBox.Show( "This source file existed in the past.  Shall I keep its history?" );
            return (result == DialogResult.Yes);
        }

        public void BadXmlInExpectedLibrary( string libraryPath )
        {
            MessageBox.Show( string.Format( 
                "The XML in the Swept library at {0} is invalid.  Swept will shut down now.", libraryPath ), 
                "", MessageBoxButtons.OK );
        }

        public void DebugMessage( string message )
        {
            //MessageBox.Show( message );
        }
    }
}
