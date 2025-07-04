using ExprCalc.Trees.Abstract;

namespace ExprCalc.Trees
{
    public interface IVisitor<T>
    {
        T VisitProgram(ExpressionsProgram program);
        T VisitBinaryOperator(BinaryExpr binaryOperator);
        T VisitUnaryOperator(UnaryExpr unaryOperator);
        T VisitCallExpr(CallExpr callExpr);
        T VisitNumericConst(NumericConst constant);
        T VisitStringConst(StringConst constant);
        T VisitVariable(VariableExpr variable);
        T VisitVariableReference(VariableReferenceExpr variable);
    }
}
