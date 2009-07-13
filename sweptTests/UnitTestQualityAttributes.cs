using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System.Text;
using System;
using System.Xml;

namespace swept.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitTestQualityAttribute : Attribute
    {
        public string Reason;

        public UnitTestQualityAttribute(string reason)
        {
            Reason = reason;
        }
    }

    //  For unit tests that are clear examples of the API, worthy of being a springboard.
    [AttributeUsage(AttributeTargets.Method)]
    public class ExemplaryAttribute : UnitTestQualityAttribute
    {
        public ExemplaryAttribute(string reason) : base(reason) { }
    }

    //  Is this gilding the lily?  How can something be notably unexciting?
    //  It could at least show that we've considered it neither exemplary nor woeful.
    [AttributeUsage(AttributeTargets.Method)]
    public class MehAttribute : UnitTestQualityAttribute
    {
        public MehAttribute(string reason) : base(reason) { }
    }

    //  For unit tests we behave shadily in, to set up or validate, that we don't want to see in the app.
    [AttributeUsage(AttributeTargets.Method)]
    public class WoefulAttribute : UnitTestQualityAttribute
    {
        public WoefulAttribute(string reason) : base(reason) { }
    }

}
