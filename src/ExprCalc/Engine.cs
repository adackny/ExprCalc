using System;
using System.Collections.Generic;
using ExprCalc.Cross;
using ExprCalc.Runtime;
using ExprCalc.Stages.CodeGen;
using ExprCalc.Stages.Lexical;
using ExprCalc.Stages.Semantic;
using ExprCalc.Stages.Syntax;
using ExprCalc.Symbols;

namespace ExprCalc
{
    public class Engine
    {
        private readonly Lexer _scanner;
        private readonly Parser _parser;
        private readonly TypeChecker _typeChecker;
        private readonly CodeGenerator _codeGenerator;
        private readonly Interpreter _interpreter;


        public Engine(string input, List<Symbol> builtInSymbols)
        {
            var symbolsTable = new SymbolsTable(builtInSymbols);
            _scanner = new Lexer(input);
            _parser = new Parser(_scanner, symbolsTable);
            _typeChecker = new TypeChecker(symbolsTable, input);
            _codeGenerator = new CodeGenerator(symbolsTable);
            _interpreter = new Interpreter();
        }

        public List<object> Memory => _interpreter.Memory;
        public Stack<object> Operands => _interpreter.Operands;

        public List<CompilerError> Check()
        {
            var errors = new List<CompilerError>();

            try
            {
                var ast = _parser.Parse();
                ast.Accept(_typeChecker);
            }
            catch (Exception) { }
            finally
            {
                errors.AddRange(_parser.Errors);
                errors.AddRange(_typeChecker.Errors);
            }
            return errors;
        }

        public List<CompilerError> Run()
        {
            var errors = new List<CompilerError>();

            try
            {
                var ast = _parser.Parse();
                ast.Accept(_typeChecker);
                ast.Accept(_codeGenerator);

                _interpreter.Load(_codeGenerator.Code, _codeGenerator.Data, _codeGenerator.ConstPool);
                _interpreter.Run();
            }
            catch (Exception) { }
            finally
            {
                errors.AddRange(_parser.Errors);
                errors.AddRange(_typeChecker.Errors);
            }
            return errors;
        }
    }
}
