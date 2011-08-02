//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using Antlr.Runtime;
using System.Text.RegularExpressions;

namespace swept.DSL
{
    /// <summary> Used by the ANTLR-generated parser to create CST nodes </summary>
    public class NodeFactory
    {
        public ISubquery Get( ISubquery lhs, IToken op, ISubquery rhs )
        {
            switch (op.Type)
            {
            case ChangeRuleLexer.AND:
                return new OpIntersectionNode( lhs, op.Text, rhs );

            case ChangeRuleLexer.OR:
                return new OpUnionNode( lhs, op.Text, rhs );

            case ChangeRuleLexer.DIFFERENCE:
                return new OpDifferenceNode( lhs, op.Text, rhs );

            default:
                throw new NotImplementedException( string.Format( "Don't know how to create a [{0}] binary operation.", op.Type ) );
            }
        }

        // TODO: improving errors may require the Token Stream to be passed in...  Even that may not be enough.
        public ISubquery GetQuery( IToken op, Regex pattern )
        {
            switch (op.Type)
            {
            case ChangeRuleLexer.FILE_NAME:
                return new QueryFileNameNode( pattern );

            case ChangeRuleLexer.LINES_MATCH:
                return new QueryContentNode( pattern );

            default:
                throw new NotImplementedException( string.Format( "Don't know how to create a [{0}] query followed by a regex [{1}].", op.Type, pattern ) );
            }
        }

        public ISubquery GetQuery( IToken op, string match )
        {
            switch (op.Type)
            {
            case ChangeRuleLexer.FILE_LANGUAGE:
                if (match == "<missing LANGUAGE>")
                {
                    var ct = op as CommonToken;
                    throw new ArgumentException( string.Format( "Swept doesn't know the language you want, starting at line {0}, char {1}.", ct.Line, ct.CharPositionInLine ) );
                }
                if (!Enum.IsDefined( typeof( FileLanguage ), match ))
                    throw new ArgumentException( String.Format("Swept does not know how to tell if a file is language [{0}] at this time.", match) );
                return new QueryLanguageNode { Language = (FileLanguage)Enum.Parse( typeof( FileLanguage ), match ) };

            default:
                throw new NotImplementedException( string.Format( "Don't know how to create a [{0}] query followed by a string [{1}].", op.Type, match ) );
            }
        }

        public Regex GetRegex( string pattern, string options )
        {
            RegexOptions opts = RegexOptions.Multiline | RegexOptions.Compiled;

            if (!string.IsNullOrEmpty( options ))
            {
                if (options.Contains("i")) opts = RegexOptions.IgnoreCase;
                if (options.Contains("s")) opts |= RegexOptions.Singleline;
                if (options.Contains("w")) opts |= RegexOptions.IgnorePatternWhitespace;
            }

            return new Regex( pattern, opts );
        }
    }
}
