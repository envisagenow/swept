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
            return GetSortedRules( new List<string>() );
        }

        public List<Rule> GetSortedRules( List<string> specifiedRules )
        {
            List<Rule> filteredRules = _rules;
            if (specifiedRules.Count > 0)
                filteredRules = _rules.Where( r => specifiedRules.Contains( r.ID, StringComparer.CurrentCultureIgnoreCase ) ).ToList();
            filteredRules.Sort( ( left, right ) => left.ID.CompareTo( right.ID ) );
            return filteredRules;
        }

        public List<Rule> GetRulesForFile( SourceFile file )
        {
            return _rules.FindAll( rule => rule.GetMatches( file ).DoesMatch );
        }

        public void Add( Rule rule )
        {
            var priorRule = _rules.SingleOrDefault( r => r.ID == rule.ID );
            if( priorRule != null )
                throw new Exception( string.Format( "Swept cannot add the rule \"{0}\" with the ID [{1}], the rule \"{2}\" already has that ID.", rule.Description, rule.ID, priorRule.Description ) );
            _rules.Add( rule );
        }
    }
}
