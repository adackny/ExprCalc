using System.Collections.Generic;

namespace ExprCalc.Trees.Abstract
{
    public class ExpressionsProgram : AstNode
    {
        public void Add(ExprNode expression) => Expressions.Add(expression);

        public List<ExprNode> Expressions { get; } = new List<ExprNode>();

        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitProgram(this);
    }
}
