using ExprCalc.Stages.Lexical;

namespace ExprCalc.Trees.Abstract
{
    public class NumericConst : ExprNode
    {
        public NumericConst(Token value)
        {
            Value = value;
        }

        public Token Value { get; }

        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitNumericConst(this);
    }
}
