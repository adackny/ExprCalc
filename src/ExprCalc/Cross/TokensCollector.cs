using System.Collections.Generic;
using ExprCalc.Stages.Lexical;
using ExprCalc.Trees;
using ExprCalc.Trees.Abstract;

namespace ExprCalc.Cross
{
    public class TokensCollector : IVisitor<List<Token>>
    {
        public List<Token> VisitBinaryOperator(BinaryExpr binaryOperator)
        {
            var tokens = new List<Token>();

            var leftTokens = binaryOperator.Left.Accept(this);
            var rightTokens = binaryOperator.Right.Accept(this);
            tokens.AddRange(leftTokens);
            tokens.Add(binaryOperator.Operator);
            tokens.AddRange(rightTokens);
            return tokens;
        }

        public List<Token> VisitCallExpr(CallExpr callExpr)
        {
            var tokens = new List<Token>
            {
                callExpr.FuncName
            };
            
            foreach (var arg in callExpr.Arguments)
                tokens.AddRange(arg.Accept(this));

            return tokens;
        }

        public List<Token> VisitNumericConst(NumericConst constant)
        {
            return new List<Token> { constant.Value };
        }

        public List<Token> VisitProgram(ExpressionsProgram program)
        {
            var tokens = new List<Token>();

            foreach (var expr in program.Expressions)
                tokens.AddRange(expr.Accept(this));

            return tokens;
        }

        public List<Token> VisitStringConst(StringConst constant)
        {
            return new List<Token> { constant.Value };
        }

        public List<Token> VisitUnaryOperator(UnaryExpr unaryOperator)
        {
            var tokens = new List<Token>
            {
                unaryOperator.Operator
            };

            tokens.AddRange(unaryOperator.Operand.Accept(this));

            return tokens;
        }

        public List<Token> VisitVariable(VariableExpr variable)
        {
            return new List<Token> { variable.Name };
        }

        public List<Token> VisitVariableReference(VariableReferenceExpr variable)
        {
            return new List<Token> { variable.Name };
        }
    }
}
