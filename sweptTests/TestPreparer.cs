//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.Tests
{
    [CoverageExclude]
    class TestPreparer
    {
        public MockUserAdapter MockGUI;
        public MockStorageAdapter MockFS;

        public TestPreparer()
        {
            MockGUI = new MockUserAdapter();
            MockFS = new MockStorageAdapter();
        }

        public void ShiftSweptToMocks( Starter starter )
        {
            // FIX: for testing, this won't work for exceptions.
            starter.Librarian._switchboard = null;
            starter.Librarian._storageAdapter = MockFS;
        }
    }
}
