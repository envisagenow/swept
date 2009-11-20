//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;

namespace swept
{
    public interface IUserAdapter
    {
        bool KeepChangeHistory( Change historicalChange );
        bool KeepSourceFileHistory( SourceFile historicalFile );
        void BadXmlInExpectedLibrary( string libraryPath, XmlException exception );
        void DebugMessage( string message );
        void ShowSeeAlso( SeeAlso seeAlso );
    }
}
