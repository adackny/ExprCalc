using System;
using System.Collections.Generic;
using System.Globalization;
using ExprCalc.Cross;
using ExprCalc.Stages.Lexical;
using ExprCalc.Symbols;
using ExprCalc.Trees.Abstract;

namespace ExprCalc.Stages.Syntax
{
    public class Parser
    {
        private readonly Lexer _input;
        private readonly CircularBuffer<Token> _lookahead = new CircularBuffer<Token>(2);
        private readonly SymbolsTable _symbolsTable;

        public Parser(Lexer input, SymbolsTable symbolsTable)
        {
            _input = input;
            _symbolsTable = symbolsTable;
            _lookahead.Poke(input.NextToken());
            _lookahead.Poke(input.NextToken());
        }

        public List<CompilerError> Errors { get; } = new List<CompilerError>();

        private void AddError(string message)
        {
            Errors.Add(new CompilerError(new List<Token> { LT(0) }, message, _input.SourceInput));
            throw new Exception("Parsing error");
        }

        private Token LT(int i) => _lookahead[i];
        private TokenType LA(int i) => _lookahead[i].TokenType;

        private void Consume() => _lookahead.Poke(_input.NextToken());

        private bool Match(TokenType tokenType)
        {
            if (_lookahead.Peek(0).TokenType == tokenType)
            {
                Consume();
                return true;
            }
            return false;
        }

        public ExpressionsProgram Parse()
        {
            var program = new ExpressionsProgram();

            try
            {
                if (LA(0) == TokenType.EOF)
                    AddError("Programa está vacío");

                while (LA(0) != TokenType.EOF)
                {
                    program.Add(Expression());
                }
            }
            catch { }

            return program;
        }

        private ExprNode Expression()
        {
            if (LA(0) == TokenType.NAME && LA(1) == TokenType.ASSIGN)
            {
                var variableRefToken = LT(0);
                var assignOperator = LT(1);
                Consume(); // left variable expression
                Consume(); // := token
                _symbolsTable.Define(new VariableSymbol(variableRefToken.Lexeme, isDefined: true));
                var variableReference = new VariableReferenceExpr(variableRefToken);
                var expression = OrExpr();
                return new BinaryExpr(assignOperator, variableReference, expression);
            }

            return OrExpr();
        }

        private ExprNode OrExpr()
        {
            ExprNode left = AndExpr();

            while (LA(0) == TokenType.OR)
            {
                var orToken = LT(0);
                Consume();
                ExprNode right = AndExpr();
                left = new BinaryExpr(orToken, left, right);
            }

            return left;
        }

        private ExprNode AndExpr()
        {
            ExprNode left = ComparisonExpr();


            while (LA(0) == TokenType.AND)
            {
                var andToken = LT(0);
                Consume();
                ExprNode right = ComparisonExpr();
                left = new BinaryExpr(andToken, left, right);
            }

            return left;
        }

        private ExprNode ComparisonExpr()
        {
            ExprNode left = SumExpr();


            while (LA(0).IsComparisonOperator())
            {
                var op = LT(0);
                Consume();
                ExprNode right = SumExpr();
                left = new BinaryExpr(op, left, right); ;
            }

            return left;
        }

        private ExprNode SumExpr()
        {
            ExprNode left = MultExpr();

            while (LA(0) == TokenType.PLUS || LA(0) == TokenType.MINUS)
            {
                var op = LT(0);
                Consume();
                ExprNode right = MultExpr();
                left = new BinaryExpr(op, left, right);
            }

            return left;
        }

        private ExprNode MultExpr()
        {
            ExprNode left = UnaryExpr();

            while (LA(0) == TokenType.STAR || LA(0) == TokenType.DIV)
            {
                var op = LT(0);
                Consume();
                ExprNode right = UnaryExpr();
                left = new BinaryExpr(op, left, right); ;
            }

            return left;
        }

        private ExprNode UnaryExpr()
        {
            ExprNode operand = null;
            while (LA(0) == TokenType.NOT || LA(0) == TokenType.MINUS)
            {
                var op = LT(0);
                Consume();
                operand = Operand();
                operand = new UnaryExpr(op, operand);
            }

            if (operand == null)
                return Operand();
            return operand;
        }

        private ExprNode Operand()
        {
            if (LA(0) == TokenType.NUMBER)
            {
                return Number();
            }
            else if (LA(0) == TokenType.STRING)
            {
                return String();
            }
            else if (LA(0) == TokenType.NAME)
            {
                if (LA(1) == TokenType.OPEN_PAREN)
                    return CallExpr();

                return Name();
            }
            else if (LA(0) == TokenType.OPEN_PAREN)
            {
                Consume();
                var expr = Expression();

                if (!Match(TokenType.CLOSE_PAREN))
                    AddError("Se esperaba )");

                return expr;
            }

            AddError("Se esperaba una expresión");
            return null;
        }

        private ExprNode CallExpr()
        {
            if (LA(0) != TokenType.NAME)
                AddError("Se esperaba un nombre de función");

            var token = LT(0);
            Consume();

            if (!Match(TokenType.OPEN_PAREN))
                AddError("Se esperaba (");

            var args = new List<ExprNode>();
            while (LA(0) != TokenType.CLOSE_PAREN)
            {
                if (args.Count > 0 && !Match(TokenType.COMMA))
                    AddError("Se esperaba ','");

                var arg = Expression();
                args.Add(arg);
            }

            if (!Match(TokenType.CLOSE_PAREN))
                AddError("Se esperaba )");

            return new CallExpr(token, args);
        }

        private VariableExpr Name()
        {
            if (LA(0) != TokenType.NAME)
                AddError("Se esperaba un nombre de variable");

            var token = LT(0);
            Consume();

            _symbolsTable.Define(new VariableSymbol(token.Lexeme));
            return new VariableExpr(token);
        }

        private NumericConst Number()
        {
            if (LA(0) != TokenType.NUMBER)
                AddError("Se esperaba una constante numérica");

            var token = LT(0);
            Consume();

            _symbolsTable.Define(new ConstSymbol(token.Lexeme, SymbolsTable.Number, decimal.Parse(token.Lexeme, CultureInfo.InvariantCulture)));
            return new NumericConst(token);
        }

        private StringConst String()
        {
            if (LA(0) != TokenType.STRING)
                AddError("Se esperaba una constante de texto");

            var token = LT(0);
            Consume();

            _symbolsTable.Define(new ConstSymbol(token.Lexeme, SymbolsTable.String, token.Lexeme));
            return new StringConst(token);
        }
    }
}
