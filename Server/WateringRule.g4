grammar WateringRule;

program: rule* EOF;

cron: (NUM | '*') ';';

rule: expr ';';

expr: logic_expr;

logic_expr: logic_expr (AND | OR) logic_expr
    | NOT logic_expr
    | comp_expr
    | braced_expr
    ;

comp_expr: operand (LT | LE | GT | GE) operand;

braced_expr: LPAREN expr RPAREN;

operand: metric | NUM ;

metric: L | T | M;

LT: '<';
LE: '<=';
GT: '>';
GE: '>=';
AND : 'and' ;
OR : 'or' ;
NOT : 'not' ;
SEMI : ';' ;
LPAREN : '(' ;
RPAREN : ')' ;
NUM : [0-9]+ ('.' [0-9]+)? ;
L: 'L';
T: 'T';
M: 'M';
WS: [ \t\n\r\f]+ -> skip ;