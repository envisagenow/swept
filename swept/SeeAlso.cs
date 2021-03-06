﻿//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept
{
    public enum TargetType
    {
        URL,
        File,
        SVN
    }

    public class SeeAlso: IEquatable<SeeAlso>
    {
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

        public SeeAlso Clone()
        {
            return new SeeAlso
            {
                Description = Description,
                Target = Target,
                TargetType = TargetType,
                Commit = Commit
            };
        }

        public override bool Equals( object obj )
        {
            SeeAlso other = obj as SeeAlso;
            return Equals( other );
        }

        public bool Equals( SeeAlso other )
        {
            if (other == null) return false;

            bool match = Description == other.Description;
            match = match && Target == other.Target;
            match = match && TargetType == other.TargetType;
            match = match && Commit == other.Commit;

            return match;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var field in new[] { Description, Target, Commit })
            {
                if (field == null)
                {
                    hash++;  //  so (null, X) hashes differently than (X, null)
                    continue;
                }
                hash = hash * 23 + field.GetHashCode();  // overflow into negative is fine.
            }
            return hash;
        }

    }

}
