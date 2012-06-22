//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using Antlr.Runtime;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace swept.DSL
{
    /// <summary> Used by the ANTLR-generated parser to create CST nodes </summary>
    public class NodeFactory
    {
        public ISubquery Get( IToken op, ISubquery rhs )
        {
            switch (op.Type)
            {
            case ChangeRuleLexer.NOT:
                return new OpNegationNode( op.Text, rhs );

            case ChangeRuleLexer.FILE:
                return new OpFileScopeNode( op.Text, rhs );

            default:
                throw new NotImplementedException( string.Format( 
                    "Swept factory has not learneed how to make a unary expression with operator [{0}].", OperatorName( op.Type ) ) );
            }
        }

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
                throw new NotImplementedException( string.Format( 
                    "Swept factory has not learned how to make a binary expression with operator [{0}].", OperatorName( op.Type ) ) );
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
                throw new NotImplementedException( string.Format( 
                    "Swept factory has not learned how to make a query from operator [{0}] followed by regex /{1}/.", OperatorName( op.Type ), pattern ) );
            }
        }

        public ISubquery GetQuery( IToken op, string text )
        {
            switch (op.Type)
            {
            case ChangeRuleLexer.FILE_LANGUAGE:
                if (!Enum.IsDefined( typeof( FileLanguage ), text ))
                {
                    throw new ArgumentException( String.Format(
                        "Swept core has not learned how to recognize files of language [{0}].", text ) );
                }

                return new QueryLanguageNode { Language = (FileLanguage)Enum.Parse( typeof( FileLanguage ), text ) };

            default:
                throw new NotImplementedException( string.Format(
                    "Swept factory has not learned how to make a query from operator [{0}] followed by text \"{1}\".", OperatorName(op.Type), text ) );
            }
        }

        Dictionary<int, string> _nameOfOpType = null;
        public string OperatorName( int opType )
        {
            if (_nameOfOpType == null)
            {
                // if and as the grammar changes, the lexer consts may change.  Update as necessary.
                _nameOfOpType = new Dictionary<int, string>();
                _nameOfOpType[ChangeRuleLexer.EOF] = "EOF";
                _nameOfOpType[ChangeRuleLexer.AND] = "AND";
                _nameOfOpType[ChangeRuleLexer.BARE_WORD] = "BARE_WORD";
                _nameOfOpType[ChangeRuleLexer.DIFFERENCE] = "DIFFERENCE";
                _nameOfOpType[ChangeRuleLexer.EscapeSequence] = "EscapeSequence";
                _nameOfOpType[ChangeRuleLexer.FILE] = "FILE";
                _nameOfOpType[ChangeRuleLexer.FILE_LANGUAGE] = "FILE_LANGUAGE";
                _nameOfOpType[ChangeRuleLexer.FILE_NAME] = "FILE_NAME";
                _nameOfOpType[ChangeRuleLexer.LINES_MATCH] = "LINES_MATCH";
                _nameOfOpType[ChangeRuleLexer.LINE_COMMENT] = "LINE_COMMENT";
                _nameOfOpType[ChangeRuleLexer.NOT] = "NOT";
                _nameOfOpType[ChangeRuleLexer.OR] = "OR";
                _nameOfOpType[ChangeRuleLexer.REGEX_MODIFIERS] = "REGEX_MODIFIERS";
                _nameOfOpType[ChangeRuleLexer.STRING_BODY_DQ] = "STRING_BODY_DQ";
                _nameOfOpType[ChangeRuleLexer.STRING_BODY_RQ] = "STRING_BODY_RQ";
                _nameOfOpType[ChangeRuleLexer.STRING_BODY_SQ] = "STRING_BODY_SQ";
                _nameOfOpType[ChangeRuleLexer.STRING_LITERAL] = "STRING_LITERAL";
                _nameOfOpType[ChangeRuleLexer.WS] = "WS";
                _nameOfOpType[ChangeRuleLexer.T__21] = "T__21";
                _nameOfOpType[ChangeRuleLexer.T__22] = "T__22";
            }

            return _nameOfOpType[opType];
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
