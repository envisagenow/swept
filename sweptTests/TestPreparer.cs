//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
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
