//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using NUnit.Framework;

namespace swept.Tests
{

    public class HollaPublisher
    {
        public event EventHandler RaiseHolla;

        public void ShoutOut()
        {
            if (RaiseHolla != null) RaiseHolla(this, new EventArgs());
        }
    }

    public class HollaListener
    {
        public int HollaCount = 0;

        public void HandleHolla(object sender, EventArgs args)
        {
            HollaCount++;
        }
    }

    [TestFixture]
    public class EventExperimentTests
    {
        [Test]
        public void CanCreatePublisherAndListener()
        {
            //  We start off not having received the event.
            HollaListener l = new HollaListener();
            Assert.AreEqual(0, l.HollaCount);

            //  Raising the event doesn't affect unsubscribed listeners.
            HollaPublisher p = new HollaPublisher();
            p.ShoutOut();
            Assert.AreEqual(0, l.HollaCount);

            //  After subscribing, HandleHolla is called when the event occurs.
            p.RaiseHolla += l.HandleHolla;
            p.ShoutOut();
            Assert.AreEqual(1, l.HollaCount);

            //  Multiple subscriptions (even to the same object's event handler) are all called by an event.
            p.RaiseHolla += l.HandleHolla;
            p.ShoutOut();
            Assert.AreEqual(3, l.HollaCount);

            //  Unsubscribing removes only one handler at a time, even if there are duplicate subscriptions.
            p.RaiseHolla -= l.HandleHolla;
            p.ShoutOut();
            Assert.AreEqual(4, l.HollaCount);

            //  Unsubscribing a non-subscriber is no error.
            HollaListener notListening = new HollaListener();
            p.RaiseHolla -= notListening.HandleHolla;
            p.ShoutOut();
            Assert.AreEqual(5, l.HollaCount);
        }

        [Test]
        public void CanAttachLambdas()
        {
            int firstLambdaCount = 0;
            EventHandler h = (sender, args) => firstLambdaCount++;

            //  Lambdas can be subscribed as usual
            HollaPublisher p = new HollaPublisher();
            p.RaiseHolla += h;
            p.ShoutOut();
            Assert.AreEqual(1, firstLambdaCount);

            p.RaiseHolla -= h;
            p.ShoutOut();
            Assert.AreEqual(1, firstLambdaCount);
        }

    
    }
}
