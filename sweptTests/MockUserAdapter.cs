//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;

namespace swept.Tests
{
    class MockUserAdapter : IUserAdapter
    {
        public bool KeepHistoricalResponse;

        internal List<string> messages;

        public MockUserAdapter()
        {
            messages = new List<string>();
        }

        #region IDialogPresenter Members

        public bool KeepChangeHistory( Change historicalChange )
        {
            return KeepHistoricalResponse;
        }

        public bool KeepSourceFileHistory( SourceFile historicalFile )
        {
            return KeepHistoricalResponse;
        }

        public void BadXmlInExpectedLibrary( string libraryPath )
        {
            // TODO--0.2: throw exception based on switch
            // ...then write tests that catch that the addin is shut down correctly.
        }

        public void DebugMessage( string message )
        {
            messages.Add( message );
        }

        #endregion
    }
}
