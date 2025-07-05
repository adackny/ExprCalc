using ExprCalc.Trees.Abstract;

namespace ExprCalc.Trees
{
    public interface IVisitor<T>
    {
        T VisitProgram(ExpressionsProgram program);
        T VisitAssignExpr(AssignExpr assignExpr);
        T VisitBinaryExpr(BinaryExpr binaryExpr);
        T VisitUnaryExpr(UnaryExpr unaryExpr);
        T VisitCallExpr(CallExpr callExpr);
        T VisitNumericConst(NumericConst constant);
        T VisitStringConst(StringConst constant);
        T VisitVariable(VariableExpr variable);
    }
}
