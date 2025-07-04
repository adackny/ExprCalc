
NO TOCAR!!
<expressionsProgram> -> <expression>* Eof
<expression> -> <orExpr> ':=' <orExpr>

<orExpr> -> <andExpr> ('OR' <andExpr>)*
<andExpr> -> <comparisonExpr> ('AND' <comparisonExpr>)*
<comparisonExpr> -> <sumExpr> ( ('=' | '<>' | '<=' | '>=') <sumExpr> )?

<sumExpr> -> <multExpr> (('+' | '-') <multExpr> )*
<multExpr> -> <unaryExpr> ( ('*'| '/') <unaryExpr> )*

<unaryExpr> -> ('-' | 'NOT')? <operand>
<operand> -> NUMBER | STRING | NAME | <callExpr> | '(' <expression> ')'
<callExpr> -> NAME '(' <expression> ( ',' <expression>)* ')'
