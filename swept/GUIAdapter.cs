//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Windows.Forms;

namespace swept
{
    public class GUIAdapter : IGUIAdapter
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
            MessageBox.Show(
                "The XML in the Swept library is invalid.  Swept will shut down now.", 
                "", MessageBoxButtons.OK );
        }

        public void DebugMessage( string message )
        {
            MessageBox.Show( message );
        }
    }
}
