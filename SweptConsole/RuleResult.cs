//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class RuleResult
    {
        public string ID;
        public int TaskCount;
        private int _threshold = -1;
        public int Threshold
        {
            get
            {
                if (_threshold < 0)
                {
                    switch (FailOn)
                    {
                    case RuleFailOn.Any:
                        _threshold = 0;
                        break;

                    case RuleFailOn.None:
                    case RuleFailOn.Increase:
                        _threshold = int.MaxValue;
                        break;

                    default:
                        throw new Exception(String.Format("Don't know the case [{0}].", FailOn));
                    }
                }
                return _threshold;
            }
            set
            {
                _threshold = value;
            }
        }
        public bool Breaking;
        public RuleFailOn FailOn;
        public string Description;
    }
}
