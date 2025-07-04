namespace ExprCalc.Trees.Abstract
{
    public abstract class AstNode
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}
