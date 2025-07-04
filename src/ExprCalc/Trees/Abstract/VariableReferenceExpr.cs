using ExprCalc.Stages.Lexical;
using ExprCalc.Symbols;

namespace ExprCalc.Trees.Abstract
{
    public class VariableReferenceExpr : ExprNode
    {
        public VariableReferenceExpr(Token name) => Name = name;

        public Token Name { get; }

        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitVariableReference(this);
    }
}
