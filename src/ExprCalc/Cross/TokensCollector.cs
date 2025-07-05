using System.Collections.Generic;
using ExprCalc.Stages.Lexical;
using ExprCalc.Trees;
using ExprCalc.Trees.Abstract;

namespace ExprCalc.Cross
{
    public class TokensCollector : IVisitor<List<Token>>
    {
        public List<Token> VisitAssignExpr(AssignExpr assignExpr)
        {
            var tokens = new List<Token>();

            var leftTokens = assignExpr.Left.Accept(this);
            var rightTokens = assignExpr.Right.Accept(this);
            tokens.AddRange(leftTokens);
            tokens.Add(assignExpr.Operator);
            tokens.AddRange(rightTokens);

            return tokens;
        }

        public List<Token> VisitBinaryExpr(BinaryExpr binaryExpr)
        {
            var tokens = new List<Token>();

            var leftTokens = binaryExpr.Left.Accept(this);
            var rightTokens = binaryExpr.Right.Accept(this);
            tokens.AddRange(leftTokens);
            tokens.Add(binaryExpr.Operator);
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

        public List<Token> VisitUnaryExpr(UnaryExpr unaryExpr)
        {
            var tokens = new List<Token>
            {
                unaryExpr.Operator
            };

            tokens.AddRange(unaryExpr.Operand.Accept(this));

            return tokens;
        }

        public List<Token> VisitVariable(VariableExpr variable)
        {
            return new List<Token> { variable.Name };
        }
    }
}
