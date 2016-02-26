using System;

namespace swept
{
    public enum PickDomain { Tag, ID }

    public class Pick
    {
        public PickDomain Domain { get; set; }
        public string Value { get; set; }

    }
}
