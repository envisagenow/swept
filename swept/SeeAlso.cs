using System;
using System.Collections.Generic;

namespace swept
{
    public enum TargetType
    {
        URL,
        File,
        SVN
    }

    public class SeeAlso
    {
        public SeeAlso() { }

        public string Description { get; set; }
        public string Commit { get; set; }

        private string _target;
        public string Target 
        {
            get { return _target; }
            set
            {
                _target = value;
                string[] targetParts = value.Split(':' );
                string prefix = string.Empty;
                if (targetParts.Length > 0)
                {
                    prefix = targetParts[0].ToLower();
                }
            }
        }

        public TargetType TargetType { get; set; }

    }

}
