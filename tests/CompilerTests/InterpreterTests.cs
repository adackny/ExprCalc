using ExprCalc.Runtime;
using ExprCalc.Stages.CodeGen;
using ExprCalc.Stages.Lexical;
using ExprCalc.Symbols;
using ExprCalc.Trees.Abstract;

namespace CompilerTests;

public class InterpreterTests
{
    [Fact]
    public void TestSimpleSum()
    {
        // A := 2.4
        // R := A + 3.5

        // LOADC 0
        // STORE 0
        // LOAD 0
        // LOADC 1
        // ADD
        // STORE 1
        // HALT

        var ARef = new VariableReferenceExpr(new Token(TokenType.NAME, "A", 0, 0));
        var Val = new NumericConst(new Token(TokenType.NUMBER, "2.4", 0, 0));
        var assignment = new BinaryExpr(new Token(TokenType.ASSIGN, ":=", 0, 0), ARef, Val);

        var A = new VariableExpr(new Token(TokenType.NAME, "A", 0, 0));
        var B = new NumericConst(new Token(TokenType.NUMBER, "3.5", 0, 0));

        var binaryExpr = new BinaryExpr(new Token(TokenType.PLUS, "+", 0, 0), A, B);

        var RRef = new VariableReferenceExpr(new Token(TokenType.NAME, "R", 0, 0));
        var resultAssign = new BinaryExpr(new Token(TokenType.ASSIGN, ":=", 0, 0), RRef, binaryExpr);

        var program = new ExpressionsProgram();
        program.Add(assignment);
        program.Add(resultAssign);

        var symbolsTable = new SymbolsTable(new List<Symbol>());
        symbolsTable.Define(new ConstSymbol("2.4", SymbolsTable.Number, 2.4m));

        var aSymbol = new VariableSymbol("A");
        aSymbol.Type = SymbolsTable.Number;
        symbolsTable.Define(aSymbol);

        symbolsTable.Define(new ConstSymbol("3.5", SymbolsTable.Number, 3.5m));

        var rSymbol = new VariableSymbol("R");
        rSymbol.Type = SymbolsTable.Number;
        symbolsTable.Define(rSymbol);

        var codeGen = new CodeGenerator(symbolsTable);
        program.Accept(codeGen);

        var interpreter = new Interpreter();
        interpreter.Load(codeGen.Code, codeGen.Data, codeGen.ConstPool);
        interpreter.Run();

        Assert.Equal(5.9m, interpreter.Memory[1]);
    }
}
