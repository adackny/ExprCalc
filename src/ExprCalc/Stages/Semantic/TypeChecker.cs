using System;
using System.Collections.Generic;
using ExprCalc.Cross;
using ExprCalc.Stages.Lexical;
using ExprCalc.Symbols;
using ExprCalc.Trees;
using ExprCalc.Trees.Abstract;

namespace ExprCalc.Stages.Semantic
{
    public class TypeChecker : IVisitor<IType>
    {
        private readonly string _sourceInput;
        private readonly SymbolsTable _symbolsTable;
        private readonly List<CompilerError> _errors = new List<CompilerError>();

        public TypeChecker(SymbolsTable symbolsTable, string sourceInput)
        {
            _symbolsTable = symbolsTable;
            _sourceInput = sourceInput;
        }

        private void AddError(AstNode node, string message)
        {
            var tokensCollector = new TokensCollector();
            var tokens = node.Accept(tokensCollector);

            _errors.Add(new CompilerError(tokens, message, _sourceInput));
            throw new Exception("Type checker error");
        }

        public List<CompilerError> Errors => _errors;

        public IType VisitBinaryOperator(BinaryExpr binaryOperator)
        {
            var rightType = binaryOperator.Right.Accept(this);

            if (binaryOperator.Operator.TokenType == TokenType.ASSIGN)
            {
                var variableRef = binaryOperator.Left as VariableReferenceExpr;
                var variableSymbol = _symbolsTable.Resolve(variableRef.Name.Lexeme) as VariableSymbol;
                variableSymbol.Type = (PrimitiveType)rightType;

                return rightType;
            }

            var leftType = binaryOperator.Left.Accept(this);

            if (leftType.Name != rightType.Name)
            {
                AddError(binaryOperator,
                    "Los tipos de datos no encajan.");
                return SymbolsTable.Undefined;
            }

            return rightType;
        }

        public IType VisitCallExpr(CallExpr callExpr)
        {
            var symbol = _symbolsTable.Resolve(callExpr.FuncName.Lexeme) as ExternalFunctionSymbol;
            var funcType = symbol?.Type;

            if (funcType == null)
            {
                AddError(callExpr, $"Undefined function '{callExpr.FuncName.Lexeme}'");
                return SymbolsTable.Undefined;
            }

            for (int i = 0; i < callExpr.Arguments.Count; i++)
            {
                var argExpr = callExpr.Arguments[i];
                var functionArgType = funcType.Args[i];
                var type = argExpr.Accept(this);
                if (type.Name != functionArgType.Name)
                {
                    return SymbolsTable.Undefined;
                }
            }

            return funcType.Ret;
        }

        public IType VisitNumericConst(NumericConst constant)
        {
            var constSymbol = _symbolsTable.Resolve(constant.Value.Lexeme) as ConstSymbol;
            return constSymbol.Type;
        }

        public IType VisitProgram(ExpressionsProgram program)
        {
            foreach (var expr in program.Expressions)
                expr.Accept(this);
            return null;
        }

        public IType VisitStringConst(StringConst constant)
        {
            var constSymbol = _symbolsTable.Resolve(constant.Value.Lexeme) as ConstSymbol;
            return constSymbol.Type;
        }

        public IType VisitUnaryOperator(UnaryExpr unaryOperator)
        {
            var type = unaryOperator.Operand.Accept(this);
            if (type == null)
            {
                return SymbolsTable.Undefined;
            }
            return type;
        }

        public IType VisitVariable(VariableExpr variable)
        {
            var variableSymbol = _symbolsTable.Resolve(variable.Name.Lexeme) as VariableSymbol;
            var type = variableSymbol.Type;

            if (type == null)
                return SymbolsTable.Undefined;
            return type;
        }

        public IType VisitVariableReference(VariableReferenceExpr variable)
        {
            var variableSymbol = _symbolsTable.Resolve(variable.Name.Lexeme) as VariableSymbol;
            var type = variableSymbol.Type;
            
            if (type == null)
                return SymbolsTable.Undefined;
            return type;
        }
    }
}
