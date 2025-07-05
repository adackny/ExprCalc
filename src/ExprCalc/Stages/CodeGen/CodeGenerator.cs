using System.Collections.Generic;
using ExprCalc.Runtime;
using ExprCalc.Stages.Lexical;
using ExprCalc.Symbols;
using ExprCalc.Trees;
using ExprCalc.Trees.Abstract;

namespace ExprCalc.Stages.CodeGen
{
    public class CodeGenerator : IVisitor<AstNode>
    {
        private readonly List<ByteCode> _code = new List<ByteCode>();
        private readonly List<object> _constPool = new List<object>();
        private readonly List<object> _memory = new List<object>();
        private readonly SymbolsTable _symbolsTable;

        private readonly Dictionary<TokenType, ushort> _binaryOperatorsOpcodes = new Dictionary<TokenType, ushort>
        {
            { TokenType.PLUS, OpcodesBuilder.ADD() },
            { TokenType.MINUS, OpcodesBuilder.SUB() },
            { TokenType.STAR, OpcodesBuilder.MUL() },
            { TokenType.AND, OpcodesBuilder.AND() },
            { TokenType.OR, OpcodesBuilder.OR() },
        };

        private readonly Dictionary<TokenType, ushort> _unaryOperatorsOpcodes = new Dictionary<TokenType, ushort>
        {
            { TokenType.MINUS, OpcodesBuilder.NEG() },
            { TokenType.NOT, OpcodesBuilder.NOT() }
        };

        public CodeGenerator(SymbolsTable symbolsTable)
        {
            _symbolsTable = symbolsTable;
        }

        public List<ByteCode> Code => _code;
        public List<object> Data => _memory;
        public List<object> ConstPool => _constPool;

        public AstNode VisitAssignExpr(AssignExpr assignExpr)
        {
            assignExpr.Right.Accept(this);

            var lvalue = assignExpr.Left as VariableExpr;
            var symbol = _symbolsTable.Resolve(lvalue.Name.Lexeme) as VariableSymbol;

            if (symbol.Address == null)
            {
                symbol.Address = (byte)_memory.Count;

                if (symbol.Type.Name == SymbolsTable.Number.Name)
                    _memory.Add(new decimal(0));
                else if (symbol.Type.Name == SymbolsTable.String.Name)
                    _memory.Add(string.Empty);
                else
                    _memory.Add(null);
            }

            _code.Add(OpcodesBuilder.STORE(symbol.Address.Value));

            return assignExpr;
        }

        public AstNode VisitBinaryExpr(BinaryExpr binaryExpr)
        {
            binaryExpr.Left.Accept(this);
            binaryExpr.Right.Accept(this);
            _code.Add(_binaryOperatorsOpcodes[binaryExpr.Operator.TokenType]);

            return binaryExpr;
        }

        public AstNode VisitCallExpr(CallExpr callExpr)
        {
            foreach (var arg in callExpr.Arguments)
                arg.Accept(this);

            var symbol = _symbolsTable.Resolve(callExpr.FuncName.Lexeme) as ExternalFunctionSymbol;
            if (symbol.ConstPoolIndex == null)
            {
                symbol.ConstPoolIndex = (byte)_constPool.Count;
                _constPool.Add(symbol);
            }

            _code.Add(OpcodesBuilder.CALL(symbol.ConstPoolIndex.Value));

            return callExpr;
        }

        public AstNode VisitNumericConst(NumericConst constant)
        {
            var symbol = _symbolsTable.Resolve(constant.Value.Lexeme.ToString()) as ConstSymbol;
            if (symbol.ConstPoolIndex == null)
            {
                symbol.ConstPoolIndex = (byte)_constPool.Count;
                _constPool.Add(symbol.Value);
            }

            _code.Add(OpcodesBuilder.LOADC(symbol.ConstPoolIndex.Value));

            return constant;
        }

        public AstNode VisitStringConst(StringConst constant)
        {
            var symbol = _symbolsTable.Resolve(constant.Value.Lexeme) as ConstSymbol;
            if (symbol.ConstPoolIndex == null)
            {
                symbol.ConstPoolIndex = (byte)_constPool.Count;
                _constPool.Add(symbol.Value);
            }

            _code.Add(OpcodesBuilder.LOADC(symbol.ConstPoolIndex.Value));

            return constant;
        }

        public AstNode VisitProgram(ExpressionsProgram program)
        {
            foreach (var expr in program.Expressions)
                expr.Accept(this);

            _code.Add(OpcodesBuilder.HALT());
            return program;
        }

        public AstNode VisitUnaryExpr(UnaryExpr unaryExpr)
        {
            unaryExpr.Operand.Accept(this);
            _code.Add(_unaryOperatorsOpcodes[unaryExpr.Operator.TokenType]);
            return unaryExpr;
        }

        public AstNode VisitVariable(VariableExpr variable)
        {
            var symbol = _symbolsTable.Resolve(variable.Name.Lexeme) as VariableSymbol;

            if (symbol is ExternalVariableSymbol externalVariableSymbol)
            {
                externalVariableSymbol.Address = (byte)_constPool.Count;
                _constPool.Add(externalVariableSymbol.Value);

                _code.Add(OpcodesBuilder.LOADC(symbol.Address.Value));

                return variable;
            }

            _code.Add(OpcodesBuilder.LOAD(symbol.Address.Value));

            return variable;
        }
    }
}
