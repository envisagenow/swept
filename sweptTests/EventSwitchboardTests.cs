//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class EventSwitchboardTests
    {
        private EventSwitchboard _switchboard;
        private MockEventListener _listener;

        [SetUp]
        public void SetUp()
        {
            _switchboard = new EventSwitchboard();
            _listener = new MockEventListener();

            var subscriber = new Subscriber();
            subscriber.Subscribe( _switchboard, _listener );
        }

        [Test]
        public void Listener_gets_path_to_opened_solutions()
        {
            string openedPath = @"new\location\of.sln";
            _switchboard.Raise_SolutionOpened( openedPath );
            Assert.That( openedPath, Is.EqualTo( _listener.SolutionOpened_args.Name ) );

            openedPath = @"another\place\holding.sln";
            _switchboard.Raise_SolutionOpened( openedPath );
            Assert.That( openedPath, Is.EqualTo( _listener.SolutionOpened_args.Name ) );
        }

        [Test]
        public void Listener_gets_name_and_content_of_opened_files()
        {
            string openedFile = "thing.cs";
            string openedFileContent = "// silly code";
            _switchboard.Raise_FileOpened( openedFile, openedFileContent );
            Assert.That( openedFile, Is.EqualTo( _listener.FileOpened_args.Name ) );
            Assert.That( openedFileContent, Is.EqualTo( _listener.FileOpened_args.Content ) );
        }

        [Test]
        public void Listener_gets_name_of_closed_files()
        {
            string closedFile = "thing.cs";
            _switchboard.Raise_FileClosing( closedFile );
            Assert.That( closedFile, Is.EqualTo( _listener.FileClosed_args.Name ) );
        }

        [Test]
        public void Listener_is_alerted_to_closed_solutions()
        {
            _switchboard.Raise_SolutionClosed();
            Assert.That( _listener.SolutionClosed_args, Is.Not.Null );
        }
    }
}
