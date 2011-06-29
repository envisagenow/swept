//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Text.RegularExpressions;

namespace swept.DSL
{
    public class QueryFileNameNode : ISubquery
    {
        public string NamePattern { get; internal set; }

        public ClauseMatch Answer( SourceFile file )
        {
            return new FileMatch( Regex.IsMatch( file.Name, NamePattern ) );
        }
    }
}
