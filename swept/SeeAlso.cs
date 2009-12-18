﻿using System;
using System.Collections.Generic;
using System.Collections;

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

        public SeeAlso Clone()
        {
            return new SeeAlso
            {
                Description = this.Description,
                Target = this.Target,
                TargetType = this.TargetType,
                Commit = this.Commit
            };
        }

        public override bool Equals( object obj )
        {
            SeeAlso compareTo = obj as SeeAlso;
            if (compareTo == null) return false;

            bool match = Description == compareTo.Description;
            match = match && Target == compareTo.Target;
            match = match && TargetType == compareTo.TargetType;
            match = match && Commit == compareTo.Commit;

            return match;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var field in new[] { Description, Target, Commit })
            {
                if (field == null) continue;
                hash = hash * 23 + field.GetHashCode();
            }
            return hash;
        }

    }

}