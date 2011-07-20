//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Text.RegularExpressions;

namespace swept.DSL
{
    public class QueryFileNameNode : ISubquery
    {
        public Regex Pattern { get; internal set; }

        public QueryFileNameNode( string pattern ) : this( new Regex( pattern )) { }
        public QueryFileNameNode( Regex pattern )
        {
            Pattern = pattern;
        }

        public ClauseMatch Answer( SourceFile file )
        {
            return new FileMatch( Pattern.IsMatch( file.Name ) );
        }
    }
}
