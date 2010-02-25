//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Xml;

namespace swept.Tests
{
    class MockUserAdapter : IUserAdapter
    {
        public bool SentBadLibraryMessage;
        public bool KeepHistoricalResponse;

        internal List<string> messages;

        public Task DoubleClickedTask { get; private set; }

        public MockUserAdapter()
        {
            messages = new List<string>();
        }

        #region IUserAdapter Members

        public bool KeepChangeHistory( Change historicalChange )
        {
            return KeepHistoricalResponse;
        }

        public bool KeepSourceFileHistory( SourceFile historicalFile )
        {
            return KeepHistoricalResponse;
        }

        public void BadXmlInExpectedLibrary( string libraryPath, XmlException exception )
        {
            SentBadLibraryMessage = true;
            // TODO--0.3: shut down swept addin smoothly when Library XML is bad.
        }

        public void DebugMessage( string message )
        {
            messages.Add( message );
        }

        public SeeAlso SentSeeAlso = null;
        public void ShowSeeAlso( SeeAlso seeAlso )
        {
            SentSeeAlso = seeAlso;
        }

        #endregion
    }
}
