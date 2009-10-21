//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.Tests
{
    class TestPreparer
    {
        public MockUserAdapter MockGUI;
        public MockStorageAdapter MockFS;

        public TestPreparer()
        {
            MockGUI = new MockUserAdapter();
            MockFS = new MockStorageAdapter();
        }

        public void ShiftStarterToMocks( Starter starter )
        {
            starter.TaskWindow._GUIAdapter = MockGUI;
            starter.Librarian._userAdapter = MockGUI;
            starter.Librarian._storageAdapter = MockFS;
        }
    }
}
