//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
grammar ChangeRule;

options {
	language = CSharp3;
//	backtrack = true;
}

@lexer::header {
#pragma warning disable 3021
}
@header {
#pragma warning disable 3021
using System;
}

@lexer::namespace {swept.DSL}
@parser::namespace {swept.DSL}
@lexer::members {
const int HIDDEN = Hidden;
}

@members {
private NodeFactory factory = new NodeFactory();
}

OR: ('||' | 'or') ;
DIFFERENCE: ('-' | 'except') ;
public expression returns [ISubquery sq]
	:	lhs=and_exp { $sq = lhs; } (op=(OR | DIFFERENCE) rhs=and_exp { $sq = factory.Get( lhs, op, rhs ); })?
	;

AND: ('&&' | 'and') ;
public and_exp returns [ISubquery sq]
	:	lhs=atom { $sq = lhs; } (op=AND rhs=atom { $sq = factory.Get( lhs, op, rhs ); })?
	;
	
public atom returns [ISubquery sq]
	:	q=direct_query { $sq = q; }
	|	'(' q=expression ')' { $sq = q; }
	;

FILE_NAME:		'file.name'		| 'f.n' | '@' ;
LINES_MATCH:	'lines.match'	| 'l.m' | '~' ;
FILE_LANGUAGE:	'file.language'	| 'f.l' | '^' ;
public direct_query returns [ISubquery sq]
	:	op=(FILE_NAME | LINES_MATCH) regex { $sq = factory.GetQuery( op, $regex.text ); }
	|	op=FILE_LANGUAGE LANGUAGE { $sq = factory.GetQuery( op, $LANGUAGE.text ); }
	;
	
regex
	:	STRING_LITERAL
	;

LANGUAGE:
	( 'CSharp' | 'HTML' | 'JavaScript' | 'CSS' | 'XSLT' | 'VBNet' | 'Project' | 'Solution' | 'Unknown' ) ;


//	----------------------

IDENTIFIER
	:	LETTER (LETTER|'0'..'9')*
	;
	
fragment
LETTER
	:	'$'
	|	'A'..'Z'
	|	'a'..'z'
	|	'_'
	;

CHARACTER_LITERAL
    :   '\'' ( EscapeSequence | ~('\''|'\\') ) '\''
    ;

STRING_LITERAL
    :  '"' STRING_BODY_DQ '"' { $text = $STRING_BODY_DQ.text; } 
    |  '\'' STRING_BODY_SQ '\'' { $text = $STRING_BODY_SQ.text; } 
    ;

fragment STRING_BODY_DQ
    :  ( EscapeSequence | ~( '\\' | '"' ) )*
    ;

fragment STRING_BODY_SQ
    :  ( EscapeSequence | ~( '\\' | '\'' ) )*
    ;

DECIMAL_LITERAL : ('0'..'9')+ ;

fragment
EscapeSequence
    :   '\\' ('b'|'t'|'n'|'f'|'r'|'\"'|'\''|'\\')
    ;

WS  :  (' '|'\r'|'\t'|'\u000C'|'\n') {$channel=HIDDEN;}
    ;

LINE_COMMENT
    : '//' ~('\n'|'\r')* '\r'? '\n' {$channel=HIDDEN;}
    ;
