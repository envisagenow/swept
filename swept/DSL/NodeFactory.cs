//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using Antlr.Runtime;

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

        public ISubquery GetQuery( IToken op, string match )
        {
            switch (op.Type)
            {
            case ChangeRuleLexer.FILE_LANGUAGE:
                return new QueryLanguageNode { Language = (FileLanguage)Enum.Parse( typeof( FileLanguage ), match ) }; 
                
            case ChangeRuleLexer.FILE_NAME:
                return new QueryFileNameNode { NamePattern = match };

            case ChangeRuleLexer.LINES_MATCH:
                return new QueryContentNode { ContentPattern = match };

            default:
                throw new NotImplementedException( string.Format( "Don't know how to create a [{0}] query.", op.Type ) );
            }
        }
    }
}
