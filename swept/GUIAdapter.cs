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
            //TODO:  Better message
            DialogResult result = MessageBox.Show( "This change existed in the past.  Should I keep its history?" );
            return (result == DialogResult.Yes);
        }

        public bool KeepSourceFileHistory( SourceFile historicalFile )
        {
            //TODO:  Better message
            DialogResult result = MessageBox.Show( "This source file existed in the past.  Shall I keep its history?" );
            return (result == DialogResult.Yes);
        }

        public bool BadXmlInExpectedLibrary( string libraryPath )
        {
            //TODO:  Work out choices
            DialogResult result = MessageBox.Show(
                "The XML in the Swept library is invalid.  Eventually this dialog will let you choose an approach to handling that.", 
                "", MessageBoxButtons.OKCancel );
            return (result == DialogResult.OK);
        }

        public void DebugMessage( string message )
        {
            MessageBox.Show( message );
        }
    }
}
