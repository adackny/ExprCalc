using ExprCalc.Stages.Lexical;

namespace ExprCalc.Trees.Abstract
{
    public class UnaryExpr : ExprNode
    {
        public UnaryExpr(Token op, ExprNode operand)
        {
            Operator = op;
            Operand = operand;
        }

        public Token Operator { get; }
        public ExprNode Operand { get; }

        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
    }
}
