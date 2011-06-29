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
        public string ContentPattern { get; internal set; }

        public ClauseMatch Answer( SourceFile file )
        {
            return identifyMatchList( file, ContentPattern );
        }

        public ClauseMatch identifyMatchList( SourceFile file, string pattern )
        {
            var matchList = new LineMatch( new List<int>() );
            if (string.IsNullOrEmpty( pattern ))
                return new LineMatch( new List<int>() );

            // TODO: Add attribute to allow case sensitive matching
            Regex rx = new Regex( pattern, RegexOptions.IgnoreCase );
            MatchCollection matches = rx.Matches( file.Content );

            foreach (Match match in matches)
            {
                int line = lineNumberOfMatch( match.Index, file.LineIndices );
                matchList.Lines.Add( line );
            }

            return matchList;
        }

        // IMPROVE: Surely there's a better way to do this?  :/
        internal int lineNumberOfMatch( int matchStartPosition, List<int> lineStartPositions )
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
