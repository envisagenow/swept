//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;

namespace swept.Tests
{
    class MockGUIAdapter : IGUIAdapter
    {
        public bool KeepHistoricalResponse;

        internal List<string> messages;

        public MockGUIAdapter()
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
