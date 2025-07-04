using ExprCalc.Stages.Lexical;

namespace ExprCalc.Trees.Abstract
{
    public class BinaryExpr : ExprNode
    {
        public BinaryExpr(Token op, ExprNode left, ExprNode right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public Token Operator { get; }
        public ExprNode Left { get; }
        public ExprNode Right { get; }

        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinaryOperator(this);
    }
}
