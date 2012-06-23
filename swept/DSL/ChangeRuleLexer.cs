//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 3.3.1.7705
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// $ANTLR 3.3.1.7705 ..\\..\\DSL\\ChangeRule.g 2012-06-23 15:37:24

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


#pragma warning disable 3021


using System.Collections.Generic;
using Antlr.Runtime;

namespace swept.DSL
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.3.1.7705")]
[System.CLSCompliant(false)]
public partial class ChangeRuleLexer : Antlr.Runtime.Lexer
{
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

	const int HIDDEN = Hidden;


    // delegates
    // delegators

	public ChangeRuleLexer()
	{
		OnCreated();
	}

	public ChangeRuleLexer(ICharStream input )
		: this(input, new RecognizerSharedState())
	{
	}

	public ChangeRuleLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state)
	{


		OnCreated();
	}
	public override string GrammarFileName { get { return "..\\..\\DSL\\ChangeRule.g"; } }


	partial void OnCreated();
	partial void EnterRule(string ruleName, int ruleIndex);
	partial void LeaveRule(string ruleName, int ruleIndex);

	partial void EnterRule_T__21();
	partial void LeaveRule_T__21();

	// $ANTLR start "T__21"
	[GrammarRule("T__21")]
	private void mT__21()
	{
		EnterRule_T__21();
		EnterRule("T__21", 1);
		TraceIn("T__21", 1);
		try
		{
			int _type = T__21;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:15:7: ( '(' )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:15:9: '('
			{
			DebugLocation(15, 9);
			Match('('); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__21", 1);
			LeaveRule("T__21", 1);
			LeaveRule_T__21();
		}
	}
	// $ANTLR end "T__21"

	partial void EnterRule_T__22();
	partial void LeaveRule_T__22();

	// $ANTLR start "T__22"
	[GrammarRule("T__22")]
	private void mT__22()
	{
		EnterRule_T__22();
		EnterRule("T__22", 2);
		TraceIn("T__22", 2);
		try
		{
			int _type = T__22;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:16:7: ( ')' )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:16:9: ')'
			{
			DebugLocation(16, 9);
			Match(')'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__22", 2);
			LeaveRule("T__22", 2);
			LeaveRule_T__22();
		}
	}
	// $ANTLR end "T__22"

	partial void EnterRule_OR();
	partial void LeaveRule_OR();

	// $ANTLR start "OR"
	[GrammarRule("OR")]
	private void mOR()
	{
		EnterRule_OR();
		EnterRule("OR", 3);
		TraceIn("OR", 3);
		try
		{
			int _type = OR;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:29:3: ( '||' | 'or' )
			int alt1=2;
			try { DebugEnterDecision(1, false);
			int LA1_0 = input.LA(1);

			if ((LA1_0=='|'))
			{
				alt1 = 1;
			}
			else if ((LA1_0=='o'))
			{
				alt1 = 2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 1, 0, input);
				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(1); }
			switch (alt1)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:29:8: '||'
				{
				DebugLocation(29, 8);
				Match("||"); 


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:29:15: 'or'
				{
				DebugLocation(29, 15);
				Match("or"); 


				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("OR", 3);
			LeaveRule("OR", 3);
			LeaveRule_OR();
		}
	}
	// $ANTLR end "OR"

	partial void EnterRule_DIFFERENCE();
	partial void LeaveRule_DIFFERENCE();

	// $ANTLR start "DIFFERENCE"
	[GrammarRule("DIFFERENCE")]
	private void mDIFFERENCE()
	{
		EnterRule_DIFFERENCE();
		EnterRule("DIFFERENCE", 4);
		TraceIn("DIFFERENCE", 4);
		try
		{
			int _type = DIFFERENCE;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:30:11: ( '-' | 'except' )
			int alt2=2;
			try { DebugEnterDecision(2, false);
			int LA2_0 = input.LA(1);

			if ((LA2_0=='-'))
			{
				alt2 = 1;
			}
			else if ((LA2_0=='e'))
			{
				alt2 = 2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 2, 0, input);
				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(2); }
			switch (alt2)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:30:14: '-'
				{
				DebugLocation(30, 14);
				Match('-'); 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:30:21: 'except'
				{
				DebugLocation(30, 21);
				Match("except"); 


				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("DIFFERENCE", 4);
			LeaveRule("DIFFERENCE", 4);
			LeaveRule_DIFFERENCE();
		}
	}
	// $ANTLR end "DIFFERENCE"

	partial void EnterRule_AND();
	partial void LeaveRule_AND();

	// $ANTLR start "AND"
	[GrammarRule("AND")]
	private void mAND()
	{
		EnterRule_AND();
		EnterRule("AND", 5);
		TraceIn("AND", 5);
		try
		{
			int _type = AND;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:35:4: ( '&&' | 'and' )
			int alt3=2;
			try { DebugEnterDecision(3, false);
			int LA3_0 = input.LA(1);

			if ((LA3_0=='&'))
			{
				alt3 = 1;
			}
			else if ((LA3_0=='a'))
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
				// ..\\..\\DSL\\ChangeRule.g:35:8: '&&'
				{
				DebugLocation(35, 8);
				Match("&&"); 


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:35:15: 'and'
				{
				DebugLocation(35, 15);
				Match("and"); 


				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("AND", 5);
			LeaveRule("AND", 5);
			LeaveRule_AND();
		}
	}
	// $ANTLR end "AND"

	partial void EnterRule_FILE();
	partial void LeaveRule_FILE();

	// $ANTLR start "FILE"
	[GrammarRule("FILE")]
	private void mFILE()
	{
		EnterRule_FILE();
		EnterRule("FILE", 6);
		TraceIn("FILE", 6);
		try
		{
			int _type = FILE;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:40:5: ( 'file.has' | 'f.h' | '*' )
			int alt4=3;
			try { DebugEnterDecision(4, false);
			int LA4_0 = input.LA(1);

			if ((LA4_0=='f'))
			{
				int LA4_1 = input.LA(2);

				if ((LA4_1=='i'))
				{
					alt4 = 1;
				}
				else if ((LA4_1=='.'))
				{
					alt4 = 2;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 4, 1, input);
					DebugRecognitionException(nvae);
					throw nvae;
				}
			}
			else if ((LA4_0=='*'))
			{
				alt4 = 3;
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
				// ..\\..\\DSL\\ChangeRule.g:40:9: 'file.has'
				{
				DebugLocation(40, 9);
				Match("file.has"); 


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:40:22: 'f.h'
				{
				DebugLocation(40, 22);
				Match("f.h"); 


				}
				break;
			case 3:
				DebugEnterAlt(3);
				// ..\\..\\DSL\\ChangeRule.g:40:30: '*'
				{
				DebugLocation(40, 30);
				Match('*'); 

				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("FILE", 6);
			LeaveRule("FILE", 6);
			LeaveRule_FILE();
		}
	}
	// $ANTLR end "FILE"

	partial void EnterRule_NOT();
	partial void LeaveRule_NOT();

	// $ANTLR start "NOT"
	[GrammarRule("NOT")]
	private void mNOT()
	{
		EnterRule_NOT();
		EnterRule("NOT", 7);
		TraceIn("NOT", 7);
		try
		{
			int _type = NOT;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:41:4: ( 'not' | '!' )
			int alt5=2;
			try { DebugEnterDecision(5, false);
			int LA5_0 = input.LA(1);

			if ((LA5_0=='n'))
			{
				alt5 = 1;
			}
			else if ((LA5_0=='!'))
			{
				alt5 = 2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 5, 0, input);
				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(5); }
			switch (alt5)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:41:8: 'not'
				{
				DebugLocation(41, 8);
				Match("not"); 


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:41:21: '!'
				{
				DebugLocation(41, 21);
				Match('!'); 

				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("NOT", 7);
			LeaveRule("NOT", 7);
			LeaveRule_NOT();
		}
	}
	// $ANTLR end "NOT"

	partial void EnterRule_FILE_NAME();
	partial void LeaveRule_FILE_NAME();

	// $ANTLR start "FILE_NAME"
	[GrammarRule("FILE_NAME")]
	private void mFILE_NAME()
	{
		EnterRule_FILE_NAME();
		EnterRule("FILE_NAME", 8);
		TraceIn("FILE_NAME", 8);
		try
		{
			int _type = FILE_NAME;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:52:10: ( 'file.name' | 'f.n' | '@' )
			int alt6=3;
			try { DebugEnterDecision(6, false);
			int LA6_0 = input.LA(1);

			if ((LA6_0=='f'))
			{
				int LA6_1 = input.LA(2);

				if ((LA6_1=='i'))
				{
					alt6 = 1;
				}
				else if ((LA6_1=='.'))
				{
					alt6 = 2;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 6, 1, input);
					DebugRecognitionException(nvae);
					throw nvae;
				}
			}
			else if ((LA6_0=='@'))
			{
				alt6 = 3;
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
				// ..\\..\\DSL\\ChangeRule.g:52:13: 'file.name'
				{
				DebugLocation(52, 13);
				Match("file.name"); 


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:52:28: 'f.n'
				{
				DebugLocation(52, 28);
				Match("f.n"); 


				}
				break;
			case 3:
				DebugEnterAlt(3);
				// ..\\..\\DSL\\ChangeRule.g:52:36: '@'
				{
				DebugLocation(52, 36);
				Match('@'); 

				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("FILE_NAME", 8);
			LeaveRule("FILE_NAME", 8);
			LeaveRule_FILE_NAME();
		}
	}
	// $ANTLR end "FILE_NAME"

	partial void EnterRule_LINES_MATCH();
	partial void LeaveRule_LINES_MATCH();

	// $ANTLR start "LINES_MATCH"
	[GrammarRule("LINES_MATCH")]
	private void mLINES_MATCH()
	{
		EnterRule_LINES_MATCH();
		EnterRule("LINES_MATCH", 9);
		TraceIn("LINES_MATCH", 9);
		try
		{
			int _type = LINES_MATCH;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:53:12: ( 'lines.match' | 'l.m' | '~' )
			int alt7=3;
			try { DebugEnterDecision(7, false);
			int LA7_0 = input.LA(1);

			if ((LA7_0=='l'))
			{
				int LA7_1 = input.LA(2);

				if ((LA7_1=='i'))
				{
					alt7 = 1;
				}
				else if ((LA7_1=='.'))
				{
					alt7 = 2;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 7, 1, input);
					DebugRecognitionException(nvae);
					throw nvae;
				}
			}
			else if ((LA7_0=='~'))
			{
				alt7 = 3;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 7, 0, input);
				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(7); }
			switch (alt7)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:53:14: 'lines.match'
				{
				DebugLocation(53, 14);
				Match("lines.match"); 


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:53:30: 'l.m'
				{
				DebugLocation(53, 30);
				Match("l.m"); 


				}
				break;
			case 3:
				DebugEnterAlt(3);
				// ..\\..\\DSL\\ChangeRule.g:53:38: '~'
				{
				DebugLocation(53, 38);
				Match('~'); 

				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("LINES_MATCH", 9);
			LeaveRule("LINES_MATCH", 9);
			LeaveRule_LINES_MATCH();
		}
	}
	// $ANTLR end "LINES_MATCH"

	partial void EnterRule_FILE_LANGUAGE();
	partial void LeaveRule_FILE_LANGUAGE();

	// $ANTLR start "FILE_LANGUAGE"
	[GrammarRule("FILE_LANGUAGE")]
	private void mFILE_LANGUAGE()
	{
		EnterRule_FILE_LANGUAGE();
		EnterRule("FILE_LANGUAGE", 10);
		TraceIn("FILE_LANGUAGE", 10);
		try
		{
			int _type = FILE_LANGUAGE;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:54:14: ( 'file.language' | 'f.l' | '^' )
			int alt8=3;
			try { DebugEnterDecision(8, false);
			int LA8_0 = input.LA(1);

			if ((LA8_0=='f'))
			{
				int LA8_1 = input.LA(2);

				if ((LA8_1=='i'))
				{
					alt8 = 1;
				}
				else if ((LA8_1=='.'))
				{
					alt8 = 2;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 8, 1, input);
					DebugRecognitionException(nvae);
					throw nvae;
				}
			}
			else if ((LA8_0=='^'))
			{
				alt8 = 3;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 8, 0, input);
				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(8); }
			switch (alt8)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:54:16: 'file.language'
				{
				DebugLocation(54, 16);
				Match("file.language"); 


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:54:34: 'f.l'
				{
				DebugLocation(54, 34);
				Match("f.l"); 


				}
				break;
			case 3:
				DebugEnterAlt(3);
				// ..\\..\\DSL\\ChangeRule.g:54:42: '^'
				{
				DebugLocation(54, 42);
				Match('^'); 

				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("FILE_LANGUAGE", 10);
			LeaveRule("FILE_LANGUAGE", 10);
			LeaveRule_FILE_LANGUAGE();
		}
	}
	// $ANTLR end "FILE_LANGUAGE"

	partial void EnterRule_STRING_LITERAL();
	partial void LeaveRule_STRING_LITERAL();

	// $ANTLR start "STRING_LITERAL"
	[GrammarRule("STRING_LITERAL")]
	private void mSTRING_LITERAL()
	{
		EnterRule_STRING_LITERAL();
		EnterRule("STRING_LITERAL", 11);
		TraceIn("STRING_LITERAL", 11);
		try
		{
			int _type = STRING_LITERAL;
			int _channel = DefaultTokenChannel;
			CommonToken STRING_BODY_DQ1 = default(CommonToken);
			CommonToken STRING_BODY_RQ2 = default(CommonToken);
			CommonToken STRING_BODY_SQ3 = default(CommonToken);

			// ..\\..\\DSL\\ChangeRule.g:71:5: ( '\"' STRING_BODY_DQ '\"' | '/' STRING_BODY_RQ '/' | '\\'' STRING_BODY_SQ '\\'' )
			int alt9=3;
			try { DebugEnterDecision(9, false);
			switch (input.LA(1))
			{
			case '\"':
				{
				alt9 = 1;
				}
				break;
			case '/':
				{
				alt9 = 2;
				}
				break;
			case '\'':
				{
				alt9 = 3;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 9, 0, input);
					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(9); }
			switch (alt9)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:71:8: '\"' STRING_BODY_DQ '\"'
				{
				DebugLocation(71, 8);
				Match('\"'); 
				DebugLocation(71, 12);
				int STRING_BODY_DQ1Start187 = CharIndex;
				int STRING_BODY_DQ1StartLine187 = Line;
				int STRING_BODY_DQ1StartCharPos187 = CharPositionInLine;
				mSTRING_BODY_DQ(); 
				STRING_BODY_DQ1 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, STRING_BODY_DQ1Start187, CharIndex-1);
				STRING_BODY_DQ1.Line = STRING_BODY_DQ1StartLine187;
				STRING_BODY_DQ1.CharPositionInLine = STRING_BODY_DQ1StartCharPos187;
				DebugLocation(71, 27);
				Match('\"'); 
				DebugLocation(71, 31);
				 Text = (STRING_BODY_DQ1!=null?STRING_BODY_DQ1.Text:null); 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// ..\\..\\DSL\\ChangeRule.g:72:8: '/' STRING_BODY_RQ '/'
				{
				DebugLocation(72, 8);
				Match('/'); 
				DebugLocation(72, 12);
				int STRING_BODY_RQ2Start203 = CharIndex;
				int STRING_BODY_RQ2StartLine203 = Line;
				int STRING_BODY_RQ2StartCharPos203 = CharPositionInLine;
				mSTRING_BODY_RQ(); 
				STRING_BODY_RQ2 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, STRING_BODY_RQ2Start203, CharIndex-1);
				STRING_BODY_RQ2.Line = STRING_BODY_RQ2StartLine203;
				STRING_BODY_RQ2.CharPositionInLine = STRING_BODY_RQ2StartCharPos203;
				DebugLocation(72, 27);
				Match('/'); 
				DebugLocation(72, 31);
				 Text = (STRING_BODY_RQ2!=null?STRING_BODY_RQ2.Text:null); 

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// ..\\..\\DSL\\ChangeRule.g:73:8: '\\'' STRING_BODY_SQ '\\''
				{
				DebugLocation(73, 8);
				Match('\''); 
				DebugLocation(73, 13);
				int STRING_BODY_SQ3Start219 = CharIndex;
				int STRING_BODY_SQ3StartLine219 = Line;
				int STRING_BODY_SQ3StartCharPos219 = CharPositionInLine;
				mSTRING_BODY_SQ(); 
				STRING_BODY_SQ3 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, STRING_BODY_SQ3Start219, CharIndex-1);
				STRING_BODY_SQ3.Line = STRING_BODY_SQ3StartLine219;
				STRING_BODY_SQ3.CharPositionInLine = STRING_BODY_SQ3StartCharPos219;
				DebugLocation(73, 28);
				Match('\''); 
				DebugLocation(73, 33);
				 Text = (STRING_BODY_SQ3!=null?STRING_BODY_SQ3.Text:null); 

				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("STRING_LITERAL", 11);
			LeaveRule("STRING_LITERAL", 11);
			LeaveRule_STRING_LITERAL();
		}
	}
	// $ANTLR end "STRING_LITERAL"

	partial void EnterRule_STRING_BODY_DQ();
	partial void LeaveRule_STRING_BODY_DQ();

	// $ANTLR start "STRING_BODY_DQ"
	[GrammarRule("STRING_BODY_DQ")]
	private void mSTRING_BODY_DQ()
	{
		EnterRule_STRING_BODY_DQ();
		EnterRule("STRING_BODY_DQ", 12);
		TraceIn("STRING_BODY_DQ", 12);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:77:5: ( ( '\\\\\"' |~ ( '\"' ) )* )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:77:8: ( '\\\\\"' |~ ( '\"' ) )*
			{
			DebugLocation(77, 8);
			// ..\\..\\DSL\\ChangeRule.g:77:8: ( '\\\\\"' |~ ( '\"' ) )*
			try { DebugEnterSubRule(10);
			while (true)
			{
				int alt10=3;
				try { DebugEnterDecision(10, false);
				int LA10_0 = input.LA(1);

				if ((LA10_0=='\\'))
				{
					int LA10_2 = input.LA(2);

					if ((LA10_2=='\"'))
					{
						alt10 = 1;
					}

					else
					{
						alt10 = 2;
					}

				}
				else if (((LA10_0>='\u0000' && LA10_0<='!')||(LA10_0>='#' && LA10_0<='[')||(LA10_0>=']' && LA10_0<='\uFFFF')))
				{
					alt10 = 2;
				}


				} finally { DebugExitDecision(10); }
				switch ( alt10 )
				{
				case 1:
					DebugEnterAlt(1);
					// ..\\..\\DSL\\ChangeRule.g:77:10: '\\\\\"'
					{
					DebugLocation(77, 10);
					Match("\\\""); 


					}
					break;
				case 2:
					DebugEnterAlt(2);
					// ..\\..\\DSL\\ChangeRule.g:77:18: ~ ( '\"' )
					{
					DebugLocation(77, 18);
					input.Consume();


					}
					break;

				default:
					goto loop10;
				}
			}

			loop10:
				;

			} finally { DebugExitSubRule(10); }


			}

		}
		finally
		{
			TraceOut("STRING_BODY_DQ", 12);
			LeaveRule("STRING_BODY_DQ", 12);
			LeaveRule_STRING_BODY_DQ();
		}
	}
	// $ANTLR end "STRING_BODY_DQ"

	partial void EnterRule_STRING_BODY_RQ();
	partial void LeaveRule_STRING_BODY_RQ();

	// $ANTLR start "STRING_BODY_RQ"
	[GrammarRule("STRING_BODY_RQ")]
	private void mSTRING_BODY_RQ()
	{
		EnterRule_STRING_BODY_RQ();
		EnterRule("STRING_BODY_RQ", 13);
		TraceIn("STRING_BODY_RQ", 13);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:81:5: ( ( '\\\\/' |~ ( '/' ) )* )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:81:8: ( '\\\\/' |~ ( '/' ) )*
			{
			DebugLocation(81, 8);
			// ..\\..\\DSL\\ChangeRule.g:81:8: ( '\\\\/' |~ ( '/' ) )*
			try { DebugEnterSubRule(11);
			while (true)
			{
				int alt11=3;
				try { DebugEnterDecision(11, false);
				int LA11_0 = input.LA(1);

				if ((LA11_0=='\\'))
				{
					int LA11_2 = input.LA(2);

					if ((LA11_2=='/'))
					{
						alt11 = 1;
					}

					else
					{
						alt11 = 2;
					}

				}
				else if (((LA11_0>='\u0000' && LA11_0<='.')||(LA11_0>='0' && LA11_0<='[')||(LA11_0>=']' && LA11_0<='\uFFFF')))
				{
					alt11 = 2;
				}


				} finally { DebugExitDecision(11); }
				switch ( alt11 )
				{
				case 1:
					DebugEnterAlt(1);
					// ..\\..\\DSL\\ChangeRule.g:81:10: '\\\\/'
					{
					DebugLocation(81, 10);
					Match("\\/"); 


					}
					break;
				case 2:
					DebugEnterAlt(2);
					// ..\\..\\DSL\\ChangeRule.g:81:18: ~ ( '/' )
					{
					DebugLocation(81, 18);
					input.Consume();


					}
					break;

				default:
					goto loop11;
				}
			}

			loop11:
				;

			} finally { DebugExitSubRule(11); }


			}

		}
		finally
		{
			TraceOut("STRING_BODY_RQ", 13);
			LeaveRule("STRING_BODY_RQ", 13);
			LeaveRule_STRING_BODY_RQ();
		}
	}
	// $ANTLR end "STRING_BODY_RQ"

	partial void EnterRule_STRING_BODY_SQ();
	partial void LeaveRule_STRING_BODY_SQ();

	// $ANTLR start "STRING_BODY_SQ"
	[GrammarRule("STRING_BODY_SQ")]
	private void mSTRING_BODY_SQ()
	{
		EnterRule_STRING_BODY_SQ();
		EnterRule("STRING_BODY_SQ", 14);
		TraceIn("STRING_BODY_SQ", 14);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:85:5: ( ( '\\\\\\'' |~ ( '\\'' ) )* )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:85:8: ( '\\\\\\'' |~ ( '\\'' ) )*
			{
			DebugLocation(85, 8);
			// ..\\..\\DSL\\ChangeRule.g:85:8: ( '\\\\\\'' |~ ( '\\'' ) )*
			try { DebugEnterSubRule(12);
			while (true)
			{
				int alt12=3;
				try { DebugEnterDecision(12, false);
				int LA12_0 = input.LA(1);

				if ((LA12_0=='\\'))
				{
					int LA12_2 = input.LA(2);

					if ((LA12_2=='\''))
					{
						alt12 = 1;
					}

					else
					{
						alt12 = 2;
					}

				}
				else if (((LA12_0>='\u0000' && LA12_0<='&')||(LA12_0>='(' && LA12_0<='[')||(LA12_0>=']' && LA12_0<='\uFFFF')))
				{
					alt12 = 2;
				}


				} finally { DebugExitDecision(12); }
				switch ( alt12 )
				{
				case 1:
					DebugEnterAlt(1);
					// ..\\..\\DSL\\ChangeRule.g:85:10: '\\\\\\''
					{
					DebugLocation(85, 10);
					Match("\\'"); 


					}
					break;
				case 2:
					DebugEnterAlt(2);
					// ..\\..\\DSL\\ChangeRule.g:85:19: ~ ( '\\'' )
					{
					DebugLocation(85, 19);
					input.Consume();


					}
					break;

				default:
					goto loop12;
				}
			}

			loop12:
				;

			} finally { DebugExitSubRule(12); }


			}

		}
		finally
		{
			TraceOut("STRING_BODY_SQ", 14);
			LeaveRule("STRING_BODY_SQ", 14);
			LeaveRule_STRING_BODY_SQ();
		}
	}
	// $ANTLR end "STRING_BODY_SQ"

	partial void EnterRule_REGEX_MODIFIERS();
	partial void LeaveRule_REGEX_MODIFIERS();

	// $ANTLR start "REGEX_MODIFIERS"
	[GrammarRule("REGEX_MODIFIERS")]
	private void mREGEX_MODIFIERS()
	{
		EnterRule_REGEX_MODIFIERS();
		EnterRule("REGEX_MODIFIERS", 15);
		TraceIn("REGEX_MODIFIERS", 15);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:89:2: ( ( 'i' | 's' | 'w' )+ )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:89:4: ( 'i' | 's' | 'w' )+
			{
			DebugLocation(89, 4);
			// ..\\..\\DSL\\ChangeRule.g:89:4: ( 'i' | 's' | 'w' )+
			int cnt13=0;
			try { DebugEnterSubRule(13);
			while (true)
			{
				int alt13=2;
				try { DebugEnterDecision(13, false);
				int LA13_0 = input.LA(1);

				if ((LA13_0=='i'||LA13_0=='s'||LA13_0=='w'))
				{
					alt13 = 1;
				}


				} finally { DebugExitDecision(13); }
				switch (alt13)
				{
				case 1:
					DebugEnterAlt(1);
					// ..\\..\\DSL\\ChangeRule.g:
					{
					DebugLocation(89, 4);
					input.Consume();


					}
					break;

				default:
					if (cnt13 >= 1)
						goto loop13;

					EarlyExitException eee13 = new EarlyExitException( 13, input );
					DebugRecognitionException(eee13);
					throw eee13;
				}
				cnt13++;
			}
			loop13:
				;

			} finally { DebugExitSubRule(13); }


			}

		}
		finally
		{
			TraceOut("REGEX_MODIFIERS", 15);
			LeaveRule("REGEX_MODIFIERS", 15);
			LeaveRule_REGEX_MODIFIERS();
		}
	}
	// $ANTLR end "REGEX_MODIFIERS"

	partial void EnterRule_BARE_WORD();
	partial void LeaveRule_BARE_WORD();

	// $ANTLR start "BARE_WORD"
	[GrammarRule("BARE_WORD")]
	private void mBARE_WORD()
	{
		EnterRule_BARE_WORD();
		EnterRule("BARE_WORD", 16);
		TraceIn("BARE_WORD", 16);
		try
		{
			int _type = BARE_WORD;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:93:2: ( ( 'a' .. 'z' | 'A' .. 'Z' )+ )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:93:5: ( 'a' .. 'z' | 'A' .. 'Z' )+
			{
			DebugLocation(93, 5);
			// ..\\..\\DSL\\ChangeRule.g:93:5: ( 'a' .. 'z' | 'A' .. 'Z' )+
			int cnt14=0;
			try { DebugEnterSubRule(14);
			while (true)
			{
				int alt14=2;
				try { DebugEnterDecision(14, false);
				int LA14_0 = input.LA(1);

				if (((LA14_0>='A' && LA14_0<='Z')||(LA14_0>='a' && LA14_0<='z')))
				{
					alt14 = 1;
				}


				} finally { DebugExitDecision(14); }
				switch (alt14)
				{
				case 1:
					DebugEnterAlt(1);
					// ..\\..\\DSL\\ChangeRule.g:
					{
					DebugLocation(93, 5);
					input.Consume();


					}
					break;

				default:
					if (cnt14 >= 1)
						goto loop14;

					EarlyExitException eee14 = new EarlyExitException( 14, input );
					DebugRecognitionException(eee14);
					throw eee14;
				}
				cnt14++;
			}
			loop14:
				;

			} finally { DebugExitSubRule(14); }


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("BARE_WORD", 16);
			LeaveRule("BARE_WORD", 16);
			LeaveRule_BARE_WORD();
		}
	}
	// $ANTLR end "BARE_WORD"

	partial void EnterRule_EscapeSequence();
	partial void LeaveRule_EscapeSequence();

	// $ANTLR start "EscapeSequence"
	[GrammarRule("EscapeSequence")]
	private void mEscapeSequence()
	{
		EnterRule_EscapeSequence();
		EnterRule("EscapeSequence", 17);
		TraceIn("EscapeSequence", 17);
		try
		{
			// ..\\..\\DSL\\ChangeRule.g:101:5: ( '\\\\' ( 'b' | 't' | 'n' | 'f' | 'r' | '\\\\' ) )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:101:9: '\\\\' ( 'b' | 't' | 'n' | 'f' | 'r' | '\\\\' )
			{
			DebugLocation(101, 9);
			Match('\\'); 
			DebugLocation(101, 14);
			if (input.LA(1)=='\\'||input.LA(1)=='b'||input.LA(1)=='f'||input.LA(1)=='n'||input.LA(1)=='r'||input.LA(1)=='t')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("EscapeSequence", 17);
			LeaveRule("EscapeSequence", 17);
			LeaveRule_EscapeSequence();
		}
	}
	// $ANTLR end "EscapeSequence"

	partial void EnterRule_WS();
	partial void LeaveRule_WS();

	// $ANTLR start "WS"
	[GrammarRule("WS")]
	private void mWS()
	{
		EnterRule_WS();
		EnterRule("WS", 18);
		TraceIn("WS", 18);
		try
		{
			int _type = WS;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:103:5: ( ( ' ' | '\\r' | '\\t' | '\\u000C' | '\\n' ) )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:103:8: ( ' ' | '\\r' | '\\t' | '\\u000C' | '\\n' )
			{
			DebugLocation(103, 8);
			if ((input.LA(1)>='\t' && input.LA(1)<='\n')||(input.LA(1)>='\f' && input.LA(1)<='\r')||input.LA(1)==' ')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}

			DebugLocation(103, 38);
			_channel=HIDDEN;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("WS", 18);
			LeaveRule("WS", 18);
			LeaveRule_WS();
		}
	}
	// $ANTLR end "WS"

	partial void EnterRule_LINE_COMMENT();
	partial void LeaveRule_LINE_COMMENT();

	// $ANTLR start "LINE_COMMENT"
	[GrammarRule("LINE_COMMENT")]
	private void mLINE_COMMENT()
	{
		EnterRule_LINE_COMMENT();
		EnterRule("LINE_COMMENT", 19);
		TraceIn("LINE_COMMENT", 19);
		try
		{
			int _type = LINE_COMMENT;
			int _channel = DefaultTokenChannel;
			// ..\\..\\DSL\\ChangeRule.g:107:5: ( '//' (~ ( '\\n' | '\\r' ) )* ( '\\r' )? '\\n' )
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:107:7: '//' (~ ( '\\n' | '\\r' ) )* ( '\\r' )? '\\n'
			{
			DebugLocation(107, 7);
			Match("//"); 

			DebugLocation(107, 12);
			// ..\\..\\DSL\\ChangeRule.g:107:12: (~ ( '\\n' | '\\r' ) )*
			try { DebugEnterSubRule(15);
			while (true)
			{
				int alt15=2;
				try { DebugEnterDecision(15, false);
				int LA15_0 = input.LA(1);

				if (((LA15_0>='\u0000' && LA15_0<='\t')||(LA15_0>='\u000B' && LA15_0<='\f')||(LA15_0>='\u000E' && LA15_0<='\uFFFF')))
				{
					alt15 = 1;
				}


				} finally { DebugExitDecision(15); }
				switch ( alt15 )
				{
				case 1:
					DebugEnterAlt(1);
					// ..\\..\\DSL\\ChangeRule.g:
					{
					DebugLocation(107, 12);
					input.Consume();


					}
					break;

				default:
					goto loop15;
				}
			}

			loop15:
				;

			} finally { DebugExitSubRule(15); }

			DebugLocation(107, 26);
			// ..\\..\\DSL\\ChangeRule.g:107:26: ( '\\r' )?
			int alt16=2;
			try { DebugEnterSubRule(16);
			try { DebugEnterDecision(16, false);
			int LA16_0 = input.LA(1);

			if ((LA16_0=='\r'))
			{
				alt16 = 1;
			}
			} finally { DebugExitDecision(16); }
			switch (alt16)
			{
			case 1:
				DebugEnterAlt(1);
				// ..\\..\\DSL\\ChangeRule.g:107:26: '\\r'
				{
				DebugLocation(107, 26);
				Match('\r'); 

				}
				break;

			}
			} finally { DebugExitSubRule(16); }

			DebugLocation(107, 32);
			Match('\n'); 
			DebugLocation(107, 37);
			_channel=HIDDEN;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("LINE_COMMENT", 19);
			LeaveRule("LINE_COMMENT", 19);
			LeaveRule_LINE_COMMENT();
		}
	}
	// $ANTLR end "LINE_COMMENT"

	public override void mTokens()
	{
		// ..\\..\\DSL\\ChangeRule.g:1:8: ( T__21 | T__22 | OR | DIFFERENCE | AND | FILE | NOT | FILE_NAME | LINES_MATCH | FILE_LANGUAGE | STRING_LITERAL | BARE_WORD | WS | LINE_COMMENT )
		int alt17=14;
		try { DebugEnterDecision(17, false);
		try
		{
			alt17 = dfa17.Predict(input);
		}
		catch (NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
			throw;
		}
		} finally { DebugExitDecision(17); }
		switch (alt17)
		{
		case 1:
			DebugEnterAlt(1);
			// ..\\..\\DSL\\ChangeRule.g:1:10: T__21
			{
			DebugLocation(1, 10);
			mT__21(); 

			}
			break;
		case 2:
			DebugEnterAlt(2);
			// ..\\..\\DSL\\ChangeRule.g:1:16: T__22
			{
			DebugLocation(1, 16);
			mT__22(); 

			}
			break;
		case 3:
			DebugEnterAlt(3);
			// ..\\..\\DSL\\ChangeRule.g:1:22: OR
			{
			DebugLocation(1, 22);
			mOR(); 

			}
			break;
		case 4:
			DebugEnterAlt(4);
			// ..\\..\\DSL\\ChangeRule.g:1:25: DIFFERENCE
			{
			DebugLocation(1, 25);
			mDIFFERENCE(); 

			}
			break;
		case 5:
			DebugEnterAlt(5);
			// ..\\..\\DSL\\ChangeRule.g:1:36: AND
			{
			DebugLocation(1, 36);
			mAND(); 

			}
			break;
		case 6:
			DebugEnterAlt(6);
			// ..\\..\\DSL\\ChangeRule.g:1:40: FILE
			{
			DebugLocation(1, 40);
			mFILE(); 

			}
			break;
		case 7:
			DebugEnterAlt(7);
			// ..\\..\\DSL\\ChangeRule.g:1:45: NOT
			{
			DebugLocation(1, 45);
			mNOT(); 

			}
			break;
		case 8:
			DebugEnterAlt(8);
			// ..\\..\\DSL\\ChangeRule.g:1:49: FILE_NAME
			{
			DebugLocation(1, 49);
			mFILE_NAME(); 

			}
			break;
		case 9:
			DebugEnterAlt(9);
			// ..\\..\\DSL\\ChangeRule.g:1:59: LINES_MATCH
			{
			DebugLocation(1, 59);
			mLINES_MATCH(); 

			}
			break;
		case 10:
			DebugEnterAlt(10);
			// ..\\..\\DSL\\ChangeRule.g:1:71: FILE_LANGUAGE
			{
			DebugLocation(1, 71);
			mFILE_LANGUAGE(); 

			}
			break;
		case 11:
			DebugEnterAlt(11);
			// ..\\..\\DSL\\ChangeRule.g:1:85: STRING_LITERAL
			{
			DebugLocation(1, 85);
			mSTRING_LITERAL(); 

			}
			break;
		case 12:
			DebugEnterAlt(12);
			// ..\\..\\DSL\\ChangeRule.g:1:100: BARE_WORD
			{
			DebugLocation(1, 100);
			mBARE_WORD(); 

			}
			break;
		case 13:
			DebugEnterAlt(13);
			// ..\\..\\DSL\\ChangeRule.g:1:110: WS
			{
			DebugLocation(1, 110);
			mWS(); 

			}
			break;
		case 14:
			DebugEnterAlt(14);
			// ..\\..\\DSL\\ChangeRule.g:1:113: LINE_COMMENT
			{
			DebugLocation(1, 113);
			mLINE_COMMENT(); 

			}
			break;

		}

	}


	#region DFA
	DFA17 dfa17;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa17 = new DFA17(this, SpecialStateTransition17);
	}

	private class DFA17 : DFA
	{
		private const string DFA17_eotS =
			"\x4\xFFFF\x1\x13\x1\xFFFF\x1\x13\x1\xFFFF\x2\x13\x1\xFFFF\x1\x13\x2\xFFFF"+
			"\x1\x13\x6\xFFFF\x1\x3\x3\x13\x1\xFFFF\x2\x13\x1\x11\x1\x13\x1\x7\x1"+
			"\x13\x1\xC\x1\x13\x1\xFFFF\x4\x13\x1\xFFFF\x1\x13\x1\x5";
		private const string DFA17_eofS =
			"\x2A\xFFFF";
		private const string DFA17_minS =
			"\x1\x9\x3\xFFFF\x1\x72\x1\xFFFF\x1\x78\x1\xFFFF\x1\x6E\x1\x2E\x1\xFFFF"+
			"\x1\x6F\x2\xFFFF\x1\x2E\x3\xFFFF\x1\x0\x2\xFFFF\x1\x41\x1\x63\x1\x64"+
			"\x1\x6C\x1\x68\x1\x74\x1\x6E\x1\x0\x1\x65\x1\x41\x1\x65\x1\x41\x1\x65"+
			"\x1\xFFFF\x1\x70\x1\x2E\x1\x73\x1\x74\x1\x68\x1\x2E\x1\x41";
		private const string DFA17_maxS =
			"\x1\x7E\x3\xFFFF\x1\x72\x1\xFFFF\x1\x78\x1\xFFFF\x1\x6E\x1\x69\x1\xFFFF"+
			"\x1\x6F\x2\xFFFF\x1\x69\x3\xFFFF\x1\xFFFF\x2\xFFFF\x1\x7A\x1\x63\x1\x64"+
			"\x1\x6C\x1\x6E\x1\x74\x1\x6E\x1\xFFFF\x1\x65\x1\x7A\x1\x65\x1\x7A\x1"+
			"\x65\x1\xFFFF\x1\x70\x1\x2E\x1\x73\x1\x74\x1\x6E\x1\x2E\x1\x7A";
		private const string DFA17_acceptS =
			"\x1\xFFFF\x1\x1\x1\x2\x1\x3\x1\xFFFF\x1\x4\x1\xFFFF\x1\x5\x2\xFFFF\x1"+
			"\x6\x1\xFFFF\x1\x7\x1\x8\x1\xFFFF\x1\x9\x1\xA\x1\xB\x1\xFFFF\x1\xC\x1"+
			"\xD\xD\xFFFF\x1\xE\x7\xFFFF";
		private const string DFA17_specialS =
			"\x12\xFFFF\x1\x0\x9\xFFFF\x1\x1\xD\xFFFF}>";
		private static readonly string[] DFA17_transitionS =
			{
				"\x2\x14\x1\xFFFF\x2\x14\x12\xFFFF\x1\x14\x1\xC\x1\x11\x3\xFFFF\x1\x7"+
				"\x1\x11\x1\x1\x1\x2\x1\xA\x2\xFFFF\x1\x5\x1\xFFFF\x1\x12\x10\xFFFF\x1"+
				"\xD\x1A\x13\x3\xFFFF\x1\x10\x2\xFFFF\x1\x8\x3\x13\x1\x6\x1\x9\x5\x13"+
				"\x1\xE\x1\x13\x1\xB\x1\x4\xB\x13\x1\xFFFF\x1\x3\x1\xFFFF\x1\xF",
				"",
				"",
				"",
				"\x1\x15",
				"",
				"\x1\x16",
				"",
				"\x1\x17",
				"\x1\x19\x3A\xFFFF\x1\x18",
				"",
				"\x1\x1A",
				"",
				"",
				"\x1\xF\x3A\xFFFF\x1\x1B",
				"",
				"",
				"",
				"\x2F\x11\x1\x1C\xFFD0\x11",
				"",
				"",
				"\x1A\x13\x6\xFFFF\x1A\x13",
				"\x1\x1D",
				"\x1\x1E",
				"\x1\x1F",
				"\x1\xA\x3\xFFFF\x1\x10\x1\xFFFF\x1\xD",
				"\x1\x20",
				"\x1\x21",
				"\x0\x22",
				"\x1\x23",
				"\x1A\x13\x6\xFFFF\x1A\x13",
				"\x1\x24",
				"\x1A\x13\x6\xFFFF\x1A\x13",
				"\x1\x25",
				"",
				"\x1\x26",
				"\x1\x27",
				"\x1\x28",
				"\x1\x29",
				"\x1\xA\x3\xFFFF\x1\x10\x1\xFFFF\x1\xD",
				"\x1\xF",
				"\x1A\x13\x6\xFFFF\x1A\x13"
			};

		private static readonly short[] DFA17_eot = DFA.UnpackEncodedString(DFA17_eotS);
		private static readonly short[] DFA17_eof = DFA.UnpackEncodedString(DFA17_eofS);
		private static readonly char[] DFA17_min = DFA.UnpackEncodedStringToUnsignedChars(DFA17_minS);
		private static readonly char[] DFA17_max = DFA.UnpackEncodedStringToUnsignedChars(DFA17_maxS);
		private static readonly short[] DFA17_accept = DFA.UnpackEncodedString(DFA17_acceptS);
		private static readonly short[] DFA17_special = DFA.UnpackEncodedString(DFA17_specialS);
		private static readonly short[][] DFA17_transition;

		static DFA17()
		{
			int numStates = DFA17_transitionS.Length;
			DFA17_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA17_transition[i] = DFA.UnpackEncodedString(DFA17_transitionS[i]);
			}
		}

		public DFA17( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 17;
			this.eot = DFA17_eot;
			this.eof = DFA17_eof;
			this.min = DFA17_min;
			this.max = DFA17_max;
			this.accept = DFA17_accept;
			this.special = DFA17_special;
			this.transition = DFA17_transition;
		}

		public override string Description { get { return "1:1: Tokens : ( T__21 | T__22 | OR | DIFFERENCE | AND | FILE | NOT | FILE_NAME | LINES_MATCH | FILE_LANGUAGE | STRING_LITERAL | BARE_WORD | WS | LINE_COMMENT );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition17(DFA dfa, int s, IIntStream _input)
	{
		IIntStream input = _input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA17_18 = input.LA(1);

				s = -1;
				if ((LA17_18=='/')) {s = 28;}

				else if (((LA17_18>='\u0000' && LA17_18<='.')||(LA17_18>='0' && LA17_18<='\uFFFF'))) {s = 17;}

				if (s >= 0) return s;
				break;
			case 1:
				int LA17_28 = input.LA(1);

				s = -1;
				if (((LA17_28>='\u0000' && LA17_28<='\uFFFF'))) {s = 34;}

				else s = 17;

				if (s >= 0) return s;
				break;
		}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 17, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
 
	#endregion

}

} // namespace swept.DSL
