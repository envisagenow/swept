//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;

namespace swept.Tests
{
    class TestPreparer
    {
        public MockGUIAdapter MockGUI;
        public MockFSAdapter MockFS;

        public TestPreparer()
        {
            MockGUI = new MockGUIAdapter();
            MockFS = new MockFSAdapter();
        }

        public void ShiftStarterToMocks( Starter starter )
        {
            starter.TaskWindow._GUIAdapter = MockGUI;
            starter.Librarian._GUIAdapter = MockGUI;
            starter.Librarian._FSAdapter = MockFS;
        }
    }
}
