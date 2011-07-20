//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace swept.DSL
{
    public class QueryContentNode : ISubquery
    {
        public Regex Pattern { get; internal set; }

        public QueryContentNode( string pattern ) : this( new Regex( pattern ) ) { }
        public QueryContentNode( Regex pattern )
        {
            Pattern = pattern;
        }

        public ClauseMatch Answer( SourceFile file )
        {
            var matchList = new LineMatch( new List<int>() );
            if (string.IsNullOrEmpty( Pattern.ToString() ))
                return matchList;

            foreach (Match match in Pattern.Matches( file.Content ))
            {
                int line = lineNumberOfMatch( match.Index, file.LineIndices );
                matchList.Lines.Add( line );
            }

            return matchList;
        }

        // IMPROVE: Surely there's a better way to do this?  :/
        // E. g., binary search or co-iterating or some better API...
        private int lineNumberOfMatch( int matchStartPosition, List<int> lineStartPositions )
        {
            int currentLineNumber = 1;
            foreach (int lineStartPosition in lineStartPositions)
            {
                if (lineStartPosition >= matchStartPosition) break;
                currentLineNumber++;
            }

            return currentLineNumber;
        }
    }
}
