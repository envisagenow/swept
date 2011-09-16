//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
grammar ChangeRule;

options {
	language = CSharp3;
}

@lexer::header {
#pragma warning disable 3021
}
@header {
#pragma warning disable 3021
using System;
using System.Text.RegularExpressions;
}

@lexer::namespace {swept.DSL}
@parser::namespace {swept.DSL}
@lexer::members {
const int HIDDEN = Hidden;
}

@parser::members {
private NodeFactory factory = new NodeFactory();
}

OR:				'||' | 'or' ;
DIFFERENCE:		'-'  | 'except' ;
public expression returns [ISubquery sq]
	:	lhs=and_exp { $sq = lhs; } (op=(OR | DIFFERENCE) rhs=and_exp { $sq = factory.Get( sq, op, rhs ); })*
	;

AND:			'&&' | 'and' ;
public and_exp returns [ISubquery sq]
	:	lhs=unary { $sq = lhs; } (op=AND rhs=unary { $sq = factory.Get( sq, op, rhs ); })*
	;

FILE:			'file.has' | 'f.h' | '*' ;
NOT:			'not'			   | '!' ;
unary returns [ISubquery sq]
	:	op=(FILE | NOT) rhs=unary { $sq = factory.Get( op, rhs ); }
	|	rhs=atom { $sq = rhs; }
	;

public atom returns [ISubquery sq]
	:	q=query { $sq = q; }
	|	'(' q=expression ')' { $sq = q; }
	;

FILE_NAME:		'file.name'		| 'f.n' | '@' ;
LINES_MATCH:	'lines.match'	| 'l.m' | '~' ;
FILE_LANGUAGE:	'file.language'	| 'f.l' | '^' ;
public query returns [ISubquery sq]
	:	op=(FILE_NAME | LINES_MATCH) r=regex { $sq = factory.GetQuery( op, r ); }
	|	op=FILE_LANGUAGE LANGUAGE { $sq = factory.GetQuery( op, $LANGUAGE.text ); }
	;
	
regex returns [Regex rex]
	:	STRING_LITERAL REGEX_MODIFIERS? { $rex = factory.GetRegex( $STRING_LITERAL.text, $REGEX_MODIFIERS.text ); }
	;

LANGUAGE:
	( 'CSharp' | 'CSS' | 'HTML' | 'JavaScript' | 'Project' | 'Solution' | 'VBNet' | 'XSLT' | 'Unknown' ) ;


//	----------------------

STRING_LITERAL
    :  '"' STRING_BODY_DQ '"' { $text = $STRING_BODY_DQ.text; } 
    |  '/' STRING_BODY_RQ '/' { $text = $STRING_BODY_RQ.text; } 
    |  '\'' STRING_BODY_SQ '\'' { $text = $STRING_BODY_SQ.text; } 
    ;

fragment STRING_BODY_DQ
    :  ( '\\"' | ~( '"' ) )*
    ;

fragment STRING_BODY_RQ
    :  ( '\\/' | ~( '/' ) )*
    ;

fragment STRING_BODY_SQ
    :  ( '\\\'' | ~( '\'' ) )*
    ;

REGEX_MODIFIERS
	:	('i' | 's' | 'w')+
	;

DECIMAL_LITERAL : ('0'..'9')+ ;

fragment
EscapeSequence
    :   '\\' ('b'|'t'|'n'|'f'|'r'|'\\')
    ;

WS  :  (' '|'\r'|'\t'|'\u000C'|'\n') {$channel=HIDDEN;}
    ;

LINE_COMMENT
    : '//' ~('\n'|'\r')* '\r'? '\n' {$channel=HIDDEN;}
    ;
