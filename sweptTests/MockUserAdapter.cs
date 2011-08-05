//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Xml;

namespace swept.Tests
{
    [CoverageExclude]
    class MockUserAdapter : IUserAdapter
    {
        internal List<string> messages;

        public Task DoubleClickedTask { get; private set; }

        public MockUserAdapter()
        {
            messages = new List<string>();
        }

        #region IUserAdapter Members

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
