using System.Collections.Generic;
using ExprCalc.Stages.Lexical;

namespace ExprCalc.Trees.Abstract
{
    public class CallExpr : ExprNode
    {
        public CallExpr(Token funcName, List<ExprNode> arguments)
        {
            FuncName = funcName;
            Arguments = arguments;
        }

        public Token FuncName { get; }
        public List<ExprNode> Arguments { get; }

        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitCallExpr(this);
    }
}
