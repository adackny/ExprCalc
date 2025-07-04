using ExprCalc.Stages.Lexical;
using ExprCalc.Symbols;

namespace ExprCalc.Trees.Abstract
{
    public class VariableExpr : ExprNode
    {
        public VariableExpr(Token name) => Name = name;

        public Token Name { get; }

        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitVariable(this);
    }
}
