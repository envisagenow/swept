//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class SubscriberTests
    {
        Subscriber _subscriber;
        MockEventListener _listener;
        EventSwitchboard _switchboard;

        [SetUp]
        public void CanSubscribe()
        {
            _subscriber = new Subscriber();
            Assert.That( _subscriber.HasSubscribed, Is.False );

            _listener = new MockEventListener();
            _switchboard = new EventSwitchboard();
            _subscriber.Subscribe( _switchboard, _listener );

            Assert.That( _subscriber.HasSubscribed );
        }

        //  TODO:  A way to break automatically?
        #region Events
        [Test]
        public void SolutionOpened_is_connected()
        {
            Assert.That( _listener.SolutionOpened_args, Is.Null );
            _switchboard.Raise_SolutionOpened( "my.sln" );
            Assert.That( _listener.SolutionOpened_args, Is.Not.Null );
        }

        [Test]
        public void SolutionClosed_is_connected()
        {
            Assert.That( _listener.SolutionClosed_args, Is.Null );
            _switchboard.Raise_SolutionClosed();
            Assert.That( _listener.SolutionClosed_args, Is.Not.Null );
        }

        [Test]
        public void FileOpened_is_connected()
        {
            Assert.That( _listener.FileOpened_args, Is.Null );
            _switchboard.Raise_FileOpened( "file.cs", string.Empty );
            Assert.That( _listener.FileOpened_args, Is.Not.Null );
        }

        [Test]
        public void FileClosing_is_connected()
        {
            Assert.That( _listener.FileClosed_args, Is.Null );
            _switchboard.Raise_FileClosing( "file.cs" );
            Assert.That( _listener.FileClosed_args, Is.Not.Null );
        }
        #endregion

        [Test]
        public void Unsubscribe_does()
        {
            _subscriber.Unsubscribe();

            Assert.That( _subscriber.HasSubscribed, Is.False );

            _switchboard.Raise_SolutionOpened( "my.sln" );
            Assert.That( _listener.SolutionOpened_args, Is.Null );
            
            _switchboard.Raise_SolutionClosed();
            Assert.That( _listener.SolutionClosed_args, Is.Null );

            _switchboard.Raise_FileOpened( "file.cs", string.Empty );
            Assert.That( _listener.FileOpened_args, Is.Null );

            _switchboard.Raise_FileClosing( "file.cs" );
            Assert.That( _listener.FileClosed_args, Is.Null );
        }

        [Test]
        public void Resubscribe_throws()
        {
            TestDelegate resub = () => _subscriber.Subscribe( _switchboard, _listener );

            var ex = Assert.Throws<Exception>( resub );

            Assert.That( ex.Message, Is.EqualTo( Subscriber.MSG_Resubscribe_Exception ) );
        }

        [Test]
        public void Double_Unsubscribe_throws()
        {
            TestDelegate unsub = () => _subscriber.Unsubscribe();

            unsub();
            var ex = Assert.Throws<Exception>( unsub );

            Assert.That( ex.Message, Is.EqualTo( Subscriber.MSG_Unsubscribe_Exception ) );
        }

        //  Unsubscribe then resubscribe happens to work, under this (13 Aug 2011)
        //  implementation, but it's not a requirement--let's not test it.
    }
}
