using ExprCalc.Stages.Lexical;
using ExprCalc.Symbols;

namespace ExprCalc.Trees.Abstract
{
    public class StringConst : ExprNode
    {
        public StringConst(Token value)
        {
            Value = value;
        }

        public Token Value { get; }

        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitStringConst(this);
    }
}
