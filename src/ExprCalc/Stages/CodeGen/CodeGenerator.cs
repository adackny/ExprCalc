using System;
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
        private readonly List<byte> _code = new List<byte>();
        private readonly List<object> _constPool = new List<object>();
        private readonly List<object> _data = new List<object>();
        private readonly SymbolsTable _symbolsTable;

        private readonly Dictionary<TokenType, byte> _binaryOperatorsOpcodes = new Dictionary<TokenType, byte>
        {
            { TokenType.PLUS, Opcodes.ADD },
            { TokenType.MINUS, Opcodes.SUB },
            { TokenType.STAR, Opcodes.MUL },
            { TokenType.AND, Opcodes.AND },
            { TokenType.OR, Opcodes.OR },
            { TokenType.ASSIGN, Opcodes.STORE }
        };

        private readonly Dictionary<TokenType, byte> _unaryOperatorsOpcodes = new Dictionary<TokenType, byte>
        {
            { TokenType.MINUS, Opcodes.NEG },
            { TokenType.NOT, Opcodes.NOT }
        };

        public CodeGenerator(SymbolsTable symbolsTable)
        {
            _symbolsTable = symbolsTable;
        }

        public List<byte> Code => _code;
        public List<object> Data => _data;
        public List<object> ConstPool => _constPool;

        public AstNode VisitBinaryOperator(BinaryExpr binaryOperator)
        {
            if (binaryOperator.Operator.TokenType == TokenType.ASSIGN)
            {
                binaryOperator.Right.Accept(this);
                VariableReferenceExpr variable = binaryOperator.Left.Accept(this) as VariableReferenceExpr;

                _code.Add(Opcodes.STORE);
                var symbol = _symbolsTable.Resolve(variable.Name.Lexeme) as VariableSymbol;
                AddImm2Bytes(symbol.Address);

                return binaryOperator;
            }

            binaryOperator.Left.Accept(this);
            binaryOperator.Right.Accept(this);
            _code.Add(_binaryOperatorsOpcodes[binaryOperator.Operator.TokenType]);

            return binaryOperator;
        }

        public AstNode VisitCallExpr(CallExpr callExpr)
        {
            foreach (var arg in callExpr.Arguments)
                arg.Accept(this);

            // agregar la funcion al const pool
            var symbol = _symbolsTable.Resolve(callExpr.FuncName.Lexeme) as ExternalFunctionSymbol;
            if (symbol.Address == -1)
            {
                symbol.Address = (short)_constPool.Count;
                _constPool.Add(symbol);
            }

            _code.Add(Opcodes.CALL);
            AddImm2Bytes(symbol.Address);

            return callExpr;
        }

        public AstNode VisitNumericConst(NumericConst constant)
        {
            var symbol = _symbolsTable.Resolve(constant.Value.Lexeme.ToString()) as ConstSymbol;
            if (symbol.Address == -1)
            {
                symbol.Address = (short)_constPool.Count;
                _constPool.Add(symbol.Value);
            }

            _code.Add(Opcodes.LOADC);
            AddImm2Bytes(symbol.Address);

            return constant;
        }

        public AstNode VisitStringConst(StringConst constant)
        {
            var symbol = _symbolsTable.Resolve(constant.Value.Lexeme) as ConstSymbol;
            if (symbol.Address == -1)
            {
                symbol.Address = (short)_constPool.Count;
                _constPool.Add(symbol.Value);
            }

            _code.Add(Opcodes.LOADC);
            AddImm2Bytes(symbol.Address);

            return constant;
        }

        public AstNode VisitProgram(ExpressionsProgram program)
        {
            foreach (var expr in program.Expressions)
                expr.Accept(this);

            _code.Add(Opcodes.HALT);
            return program;
        }

        public AstNode VisitUnaryOperator(UnaryExpr unaryOperator)
        {
            unaryOperator.Operand.Accept(this);
            _code.Add(_unaryOperatorsOpcodes[unaryOperator.Operator.TokenType]);
            return unaryOperator;
        }

        public AstNode VisitVariable(VariableExpr variable)
        {
            var symbol = _symbolsTable.Resolve(variable.Name.Lexeme) as VariableSymbol;

            if (symbol is ExternalVariableSymbol externalVariableSymbol)
            {
                externalVariableSymbol.Address = (short)_constPool.Count;
                _constPool.Add(externalVariableSymbol.Value);

                _code.Add(Opcodes.LOADC);
                AddImm2Bytes(symbol.Address);

                return variable;
            }

            _code.Add(Opcodes.LOAD);
            AddImm2Bytes(symbol.Address);

            return variable;
        }

        public AstNode VisitVariableReference(VariableReferenceExpr variable)
        {
            var symbol = _symbolsTable.Resolve(variable.Name.Lexeme) as VariableSymbol;
            if (symbol.Address == -1)
            {
                symbol.Address = (short)_data.Count;
                if (symbol.Type.Equals(SymbolsTable.Number))
                {
                    _data.Add(new decimal(0));
                }
                else if (symbol.Type.Equals(SymbolsTable.String))
                {
                    _data.Add(string.Empty);
                }
            }

            return variable;
        }
        
        private void AddImm2Bytes(short imm)
        {
            _code.Add((byte)((imm & 0xFF00) >> 8));
            _code.Add((byte)(imm & 0x00FF));
        }
    }
}
