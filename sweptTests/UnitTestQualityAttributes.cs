using System;

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
    public class ExemplaryAttribute : UnitTestQualityAttribute
    {
        public ExemplaryAttribute(string reason) : base(reason) { }
    }

    //  For when a test breaks expected normal usage to do its job.
    public class WoefulAttribute : UnitTestQualityAttribute
    {
        public WoefulAttribute(string reason) : base(reason) { }
    }

    //  Is this gilding the lily?  How can something be notably unexciting?
    //  It could at least show that we've considered it neither exemplary nor woeful.
    public class OkayAttribute : UnitTestQualityAttribute
    {
        public OkayAttribute(string reason) : base(reason) { }
    }

}
