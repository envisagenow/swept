//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 3.3.1.7705
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// $ANTLR 3.3.1.7705 ..\\..\\DSL\\ChangeRule.g 2018-04-02 15:28:31

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


//  Parser not CLS compliant.  I know.  Shh.
#pragma warning disable 3021
using System;
using System.Text.RegularExpressions;


using System.Collections.Generic;
using Antlr.Runtime;

namespace swept.DSL
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.3.1.7705")]
[System.CLSCompliant(false)]
public partial class ChangeRuleParser : Antlr.Runtime.Parser
{
	internal static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "AND", "BARE_WORD", "DIFFERENCE", "EscapeSequence", "FILE", "FILE_LANGUAGE", "FILE_NAME", "LINES_MATCH", "LINE_COMMENT", "NOT", "OR", "REGEX_MODIFIERS", "STRING_BODY_DQ", "STRING_BODY_RQ", "STRING_BODY_SQ", "STRING_LITERAL", "WS", "'('", "')'"
	};
	public const int EOF=-1;
	public const int AND=4;
	public const int BARE_WORD=5;
	public const int DIFFERENCE=6;
	public const int EscapeSequence=7;
	public const int FILE=8;
	public const int FILE_LANGUAGE=9;
	public const int FILE_NAME=10;
	public const int LINES_MATCH=11;
	public const int LINE_COMMENT=12;
	public const int NOT=13;
	public const int OR=14;
	public const int REGEX_MODIFIERS=15;
	public const int STRING_BODY_DQ=16;
	public const int STRING_BODY_RQ=17;
	public const int STRING_BODY_SQ=18;
	public const int STRING_LITERAL=19;
	public const int WS=20;
	public const int T__21=21;
	public const int T__22=22;

	// delegates
	// delegators

	public ChangeRuleParser( ITokenStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public ChangeRuleParser(ITokenStream input, RecognizerSharedState state)
		: base(input, state)
	{

		OnCreated();
	}
		

	public override string[] TokenNames { get { return ChangeRuleParser.tokenNames; } }
	public override string GrammarFileName { get { return "..\\..\\DSL\\ChangeRule.g"; } }


	private NodeFactory factory = new NodeFactory();


	partial void OnCreated();
	partial void EnterRule(string ruleName, int ruleIndex);
	partial void LeaveRule(string ruleName, int ruleIndex);

	#region Rules

	partial void EnterRule_expression();
	partial void LeaveRule_expression();

	// $ANTLR start "expression"
	// ..\\..\\DSL\\ChangeRule.g:33:8: public expression returns [ISubquery sq] : lhs= and_exp (op= ( OR | DIFFERENCE ) rhs= and_exp )* ;
	[GrammarRule("expression")]
	public ISubquery expression()
	{
		EnterRule_expression();
		EnterRule("expression", 1);
		TraceIn("expression", 1);
		ISubquery sq = default(ISubquery);

		IToken op = default(IToken);
		ISubquery lhs = default(ISubquery);
		ISubquery rhs = default(ISubquery);

		try { DebugEnterRule(GrammarFileName, "expression");
		DebugLocation(33, 1);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:34:2: (lhs= and_exp (op= ( OR | DIFFERENCE ) rhs= and_exp )* )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:34:4: lhs= and_exp (op= ( OR | DIFFERENCE ) rhs= and_exp )*
			{
			DebugLocation(34, 7);
			PushFollow(Follow._and_exp_in_expression110);
			lhs=and_exp();
			PopFollow();

			DebugLocation(34, 16);
			 sq = lhs; 
			DebugLocation(34, 31);
			// ..\\..\\DSL\\ChangeRule.g:34:31: (op= ( OR | DIFFERENCE ) rhs= and_exp )*
			try { DebugEnterSubRule(1);
			while (true)
			{
				int alt1=2;
				try { DebugEnterDecision(1, false);
				int LA1_0 = input.LA(1);

				if ((LA1_0==DIFFERENCE||LA1_0==OR))
				{
					alt1 = 1;
				}


				} finally { DebugExitDecision(1); }
				switch ( alt1 )
				{
				case 1:
					DebugEnterAlt(1);
					// ..\\..\\DSL\\ChangeRule.g:34:32: op= ( OR | DIFFERENCE ) rhs= and_exp
					{
					DebugLocation(34, 34);
					op=(IToken)input.LT(1);
					if (input.LA(1)==DIFFERENCE||input.LA(1)==OR)
					{
						input.Consume();
						state.errorRecovery=false;
					}
					else
					{
						MismatchedSetException mse = new MismatchedSetException(null,input);
						DebugRecognitionException(mse);
						throw mse;
					}

					DebugLocation(34, 56);
					PushFollow(Follow._and_exp_in_expression127);
					rhs=and_exp();
					PopFollow();

					DebugLocation(34, 65);
					 sq = factory.Get( sq, op, rhs ); 

					}
					break;

				default:
					goto loop1;
				}
			}

			loop1:
				;

			} finally { DebugExitSubRule(1); }


			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("expression", 1);
			LeaveRule("expression", 1);
			LeaveRule_expression();
		}
		DebugLocation(35, 1);
		} finally { DebugExitRule(GrammarFileName, "expression"); }
		return sq;

	}
	// $ANTLR end "expression"


	partial void EnterRule_and_exp();
	partial void LeaveRule_and_exp();

	// $ANTLR start "and_exp"
	// ..\\..\\DSL\\ChangeRule.g:38:8: public and_exp returns [ISubquery sq] : lhs= unary (op= AND rhs= unary )* ;
	[GrammarRule("and_exp")]
	public ISubquery and_exp()
	{
		EnterRule_and_exp();
		EnterRule("and_exp", 2);
		TraceIn("and_exp", 2);
		ISubquery sq = default(ISubquery);

		IToken op = default(IToken);
		ISubquery lhs = default(ISubquery);
		ISubquery rhs = default(ISubquery);

		try { DebugEnterRule(GrammarFileName, "and_exp");
		DebugLocation(38, 1);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:39:2: (lhs= unary (op= AND rhs= unary )* )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:39:4: lhs= unary (op= AND rhs= unary )*
			{
			DebugLocation(39, 7);
			PushFollow(Follow._unary_in_and_exp163);
			lhs=unary();
			PopFollow();

			DebugLocation(39, 14);
			 sq = lhs; 
			DebugLocation(39, 29);
			// ..\\..\\DSL\\ChangeRule.g:39:29: (op= AND rhs= unary )*
			try { DebugEnterSubRule(2);
			while (true)
			{
				int alt2=2;
				try { DebugEnterDecision(2, false);
				int LA2_0 = input.LA(1);

				if ((LA2_0==AND))
				{
					alt2 = 1;
				}


				} finally { DebugExitDecision(2); }
				switch ( alt2 )
				{
				case 1:
					DebugEnterAlt(1);
					// ..\\..\\DSL\\ChangeRule.g:39:30: op= AND rhs= unary
					{
					DebugLocation(39, 32);
					op=(IToken)Match(input,AND,Follow._AND_in_and_exp170); 
					DebugLocation(39, 40);
					PushFollow(Follow._unary_in_and_exp174);
					rhs=unary();
					PopFollow();

					DebugLocation(39, 47);
					 sq = factory.Get( sq, op, rhs ); 

					}
					break;

				default:
					goto loop2;
				}
			}

			loop2:
				;

			} finally { DebugExitSubRule(2); }


			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("and_exp", 2);
			LeaveRule("and_exp", 2);
			LeaveRule_and_exp();
		}
		DebugLocation(40, 1);
		} finally { DebugExitRule(GrammarFileName, "and_exp"); }
		return sq;

	}
	// $ANTLR end "and_exp"


	partial void EnterRule_unary();
	partial void LeaveRule_unary();

	// $ANTLR start "unary"
	// ..\\..\\DSL\\ChangeRule.g:44:1: unary returns [ISubquery sq] : (op= ( FILE | NOT ) rhs= unary |rhs= atom );
	[GrammarRule("unary")]
	private ISubquery unary()
	{
		EnterRule_unary();
		EnterRule("unary", 3);
		TraceIn("unary", 3);
		ISubquery sq = default(ISubquery);

		IToken op = default(IToken);
		ISubquery rhs = default(ISubquery);

		try { DebugEnterRule(GrammarFileName, "unary");
		DebugLocation(44, 1);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:45:2: (op= ( FILE | NOT ) rhs= unary |rhs= atom )
			int alt3=2;
			try { DebugEnterDecision(3, false);
			int LA3_0 = input.LA(1);

			if ((LA3_0==FILE||LA3_0==NOT))
			{
				alt3 = 1;
			}
			else if ((LA3_0==BARE_WORD||(LA3_0>=FILE_LANGUAGE && LA3_0<=LINES_MATCH)||LA3_0==21))
			{
				alt3 = 2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 3, 0, input);
				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(3); }
			switch (alt3)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:45:4: op= ( FILE | NOT ) rhs= unary
				{
				DebugLocation(45, 6);
				op=(IToken)input.LT(1);
				if (input.LA(1)==FILE||input.LA(1)==NOT)
				{
					input.Consume();
					state.errorRecovery=false;
				}
				else
				{
					MismatchedSetException mse = new MismatchedSetException(null,input);
					DebugRecognitionException(mse);
					throw mse;
				}

				DebugLocation(45, 23);
				PushFollow(Follow._unary_in_unary240);
				rhs=unary();
				PopFollow();

				DebugLocation(45, 30);
				 sq = factory.Get( op, rhs ); 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:46:4: rhs= atom
				{
				DebugLocation(46, 7);
				PushFollow(Follow._atom_in_unary249);
				rhs=atom();
				PopFollow();

				DebugLocation(46, 13);
				 sq = rhs; 

				}
				break;

			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("unary", 3);
			LeaveRule("unary", 3);
			LeaveRule_unary();
		}
		DebugLocation(47, 1);
		} finally { DebugExitRule(GrammarFileName, "unary"); }
		return sq;

	}
	// $ANTLR end "unary"


	partial void EnterRule_atom();
	partial void LeaveRule_atom();

	// $ANTLR start "atom"
	// ..\\..\\DSL\\ChangeRule.g:49:8: public atom returns [ISubquery sq] : (q= query | '(' q= expression ')' );
	[GrammarRule("atom")]
	public ISubquery atom()
	{
		EnterRule_atom();
		EnterRule("atom", 4);
		TraceIn("atom", 4);
		ISubquery sq = default(ISubquery);

		ISubquery q = default(ISubquery);

		try { DebugEnterRule(GrammarFileName, "atom");
		DebugLocation(49, 1);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:50:2: (q= query | '(' q= expression ')' )
			int alt4=2;
			try { DebugEnterDecision(4, false);
			int LA4_0 = input.LA(1);

			if ((LA4_0==BARE_WORD||(LA4_0>=FILE_LANGUAGE && LA4_0<=LINES_MATCH)))
			{
				alt4 = 1;
			}
			else if ((LA4_0==21))
			{
				alt4 = 2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 4, 0, input);
				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(4); }
			switch (alt4)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:50:4: q= query
				{
				DebugLocation(50, 5);
				PushFollow(Follow._query_in_atom270);
				q=query();
				PopFollow();

				DebugLocation(50, 12);
				 sq = q; 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:51:4: '(' q= expression ')'
				{
				DebugLocation(51, 4);
				Match(input,21,Follow._21_in_atom277); 
				DebugLocation(51, 9);
				PushFollow(Follow._expression_in_atom281);
				q=expression();
				PopFollow();

				DebugLocation(51, 21);
				Match(input,22,Follow._22_in_atom283); 
				DebugLocation(51, 25);
				 sq = q; 

				}
				break;

			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("atom", 4);
			LeaveRule("atom", 4);
			LeaveRule_atom();
		}
		DebugLocation(52, 1);
		} finally { DebugExitRule(GrammarFileName, "atom"); }
		return sq;

	}
	// $ANTLR end "atom"


	partial void EnterRule_query();
	partial void LeaveRule_query();

	// $ANTLR start "query"
	// ..\\..\\DSL\\ChangeRule.g:57:8: public query returns [ISubquery sq] : (op= ( FILE_NAME | LINES_MATCH ) r= regex | (op= FILE_LANGUAGE )? BARE_WORD );
	[GrammarRule("query")]
	public ISubquery query()
	{
		EnterRule_query();
		EnterRule("query", 5);
		TraceIn("query", 5);
		ISubquery sq = default(ISubquery);

		IToken op = default(IToken);
		IToken BARE_WORD1 = default(IToken);
		Regex r = default(Regex);

		try { DebugEnterRule(GrammarFileName, "query");
		DebugLocation(57, 1);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:58:2: (op= ( FILE_NAME | LINES_MATCH ) r= regex | (op= FILE_LANGUAGE )? BARE_WORD )
			int alt6=2;
			try { DebugEnterDecision(6, false);
			int LA6_0 = input.LA(1);

			if (((LA6_0>=FILE_NAME && LA6_0<=LINES_MATCH)))
			{
				alt6 = 1;
			}
			else if ((LA6_0==BARE_WORD||LA6_0==FILE_LANGUAGE))
			{
				alt6 = 2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 6, 0, input);
				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(6); }
			switch (alt6)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:58:4: op= ( FILE_NAME | LINES_MATCH ) r= regex
				{
				DebugLocation(58, 6);
				op=(IToken)input.LT(1);
				if ((input.LA(1)>=FILE_NAME && input.LA(1)<=LINES_MATCH))
				{
					input.Consume();
					state.errorRecovery=false;
				}
				else
				{
					MismatchedSetException mse = new MismatchedSetException(null,input);
					DebugRecognitionException(mse);
					throw mse;
				}

				DebugLocation(58, 34);
				PushFollow(Follow._regex_in_query361);
				r=regex();
				PopFollow();

				DebugLocation(58, 41);
				 sq = factory.GetQuery( op, r ); 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:59:4: (op= FILE_LANGUAGE )? BARE_WORD
				{
				DebugLocation(59, 6);
				// ..\\..\\DSL\\ChangeRule.g:59:6: (op= FILE_LANGUAGE )?
				int alt5=2;
				try { DebugEnterSubRule(5);
				try { DebugEnterDecision(5, false);
				int LA5_0 = input.LA(1);

				if ((LA5_0==FILE_LANGUAGE))
				{
					alt5 = 1;
				}
				} finally { DebugExitDecision(5); }
				switch (alt5)
				{
				case 1:
					DebugEnterAlt(1);
					// ..\\..\\DSL\\ChangeRule.g:59:6: op= FILE_LANGUAGE
					{
					DebugLocation(59, 6);
					op=(IToken)Match(input,FILE_LANGUAGE,Follow._FILE_LANGUAGE_in_query370); 

					}
					break;

				}
				} finally { DebugExitSubRule(5); }

				DebugLocation(59, 22);
				BARE_WORD1=(IToken)Match(input,BARE_WORD,Follow._BARE_WORD_in_query373); 
				DebugLocation(59, 32);
				 sq = factory.GetQuery( op, (BARE_WORD1!=null?BARE_WORD1.Text:null) ); 

				}
				break;

			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("query", 5);
			LeaveRule("query", 5);
			LeaveRule_query();
		}
		DebugLocation(60, 1);
		} finally { DebugExitRule(GrammarFileName, "query"); }
		return sq;

	}
	// $ANTLR end "query"


	partial void EnterRule_regex();
	partial void LeaveRule_regex();

	// $ANTLR start "regex"
	// ..\\..\\DSL\\ChangeRule.g:62:1: regex returns [Regex rex] : STRING_LITERAL ( BARE_WORD )? ;
	[GrammarRule("regex")]
	private Regex regex()
	{
		EnterRule_regex();
		EnterRule("regex", 6);
		TraceIn("regex", 6);
		Regex rex = default(Regex);

		IToken STRING_LITERAL2 = default(IToken);
		IToken BARE_WORD3 = default(IToken);

		try { DebugEnterRule(GrammarFileName, "regex");
		DebugLocation(62, 1);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:63:2: ( STRING_LITERAL ( BARE_WORD )? )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:63:4: STRING_LITERAL ( BARE_WORD )?
			{
			DebugLocation(63, 4);
			STRING_LITERAL2=(IToken)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_regex391); 
			DebugLocation(63, 19);
			// ..\\..\\DSL\\ChangeRule.g:63:19: ( BARE_WORD )?
			int alt7=2;
			try { DebugEnterSubRule(7);
			try { DebugEnterDecision(7, false);
			int LA7_0 = input.LA(1);

			if ((LA7_0==BARE_WORD))
			{
				alt7 = 1;
			}
			} finally { DebugExitDecision(7); }
			switch (alt7)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:63:19: BARE_WORD
				{
				DebugLocation(63, 19);
				BARE_WORD3=(IToken)Match(input,BARE_WORD,Follow._BARE_WORD_in_regex393); 

				}
				break;

			}
			} finally { DebugExitSubRule(7); }

			DebugLocation(63, 30);
			 rex = factory.GetRegex( (STRING_LITERAL2!=null?STRING_LITERAL2.Text:null), (BARE_WORD3!=null?BARE_WORD3.Text:null) ); 

			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("regex", 6);
			LeaveRule("regex", 6);
			LeaveRule_regex();
		}
		DebugLocation(64, 1);
		} finally { DebugExitRule(GrammarFileName, "regex"); }
		return rex;

	}
	// $ANTLR end "regex"
	#endregion Rules


	#region Follow sets
	private static class Follow
	{
		public static readonly BitSet _and_exp_in_expression110 = new BitSet(new ulong[]{0x4042UL});
		public static readonly BitSet _set_in_expression117 = new BitSet(new ulong[]{0x202F20UL});
		public static readonly BitSet _and_exp_in_expression127 = new BitSet(new ulong[]{0x4042UL});
		public static readonly BitSet _unary_in_and_exp163 = new BitSet(new ulong[]{0x12UL});
		public static readonly BitSet _AND_in_and_exp170 = new BitSet(new ulong[]{0x202F20UL});
		public static readonly BitSet _unary_in_and_exp174 = new BitSet(new ulong[]{0x12UL});
		public static readonly BitSet _set_in_unary230 = new BitSet(new ulong[]{0x202F20UL});
		public static readonly BitSet _unary_in_unary240 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _atom_in_unary249 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _query_in_atom270 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _21_in_atom277 = new BitSet(new ulong[]{0x202F20UL});
		public static readonly BitSet _expression_in_atom281 = new BitSet(new ulong[]{0x400000UL});
		public static readonly BitSet _22_in_atom283 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_query351 = new BitSet(new ulong[]{0x80000UL});
		public static readonly BitSet _regex_in_query361 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _FILE_LANGUAGE_in_query370 = new BitSet(new ulong[]{0x20UL});
		public static readonly BitSet _BARE_WORD_in_query373 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_regex391 = new BitSet(new ulong[]{0x22UL});
		public static readonly BitSet _BARE_WORD_in_regex393 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace swept.DSL
