//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

namespace swept
{
    [CoverageExclude( "Wrapper around Windows GUI" )]
    public class UserGUIAdapter : IUserAdapter
    {
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

        //public void MoveStudioCursorToLocation( int lineNumber )
        //{
        //    throw new NotImplementedException( String.Format("Soon...  But I got line number [{0}].", lineNumber ) );
        //}

        public void ShowSeeAlso( SeeAlso seeAlso )
        {
            switch (seeAlso.TargetType)
            {
            case TargetType.URL:
                Process.Start( seeAlso.Target );
                break;

            case TargetType.SVN:
                // TODO: Show SeeAlso SVN:  bring back two local files into temp directory, fire up svndiff...
                break;

            case TargetType.File:
                // TODO: Show SeeAlso File:  switch VS IDE to display chosen file at line number
                break;

            default:
                throw new Exception( string.Format( "Don't know how to show a SeeAlso Target of type [{0}].", seeAlso.TargetType ) );
            }
        }

    }
}
