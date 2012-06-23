//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;

namespace swept
{
    public class RuleCatalog
    {
        internal List<Rule> _rules;

        public RuleCatalog()
        {
            _rules = new List<Rule>();
        }

        public List<Rule> GetSortedRules()
        {
            _rules.Sort( (left, right) => left.ID.CompareTo(right.ID) );
            return _rules;
        }

        public List<Rule> GetRulesForFile( SourceFile file )
        {
            return _rules.FindAll( rule => rule.GetMatches( file ).DoesMatch );
        }

        public void Add( Rule rule )
        {
            if( _rules.Exists( c => c.ID == rule.ID) )
                throw new Exception( string.Format( "There is already a rule with the ID [{0}].", rule.ID ) );
            _rules.Add( rule );
        }
    }
}
