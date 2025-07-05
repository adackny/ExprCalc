using ExprCalc.Runtime;
using ExprCalc.Stages.CodeGen;
using ExprCalc.Stages.Lexical;
using ExprCalc.Symbols;
using ExprCalc.Trees.Abstract;

namespace CompilerTests;

public class CodeGenTests
{
    [Fact]
    public void TestSimpleSum()
    {
        var ARef = new VariableExpr(new Token(TokenType.NAME, "A", 0, 0));
        var Val = new NumericConst(new Token(TokenType.NUMBER, "2.4", 0, 0));
        var assignment = new AssignExpr(new Token(TokenType.ASSIGN, ":=", 0, 0), ARef, Val);

        var A = new VariableExpr(new Token(TokenType.NAME, "A", 0, 0));
        var B = new NumericConst(new Token(TokenType.NUMBER, "3.5", 0, 0));

        var binaryExpr = new BinaryExpr(new Token(TokenType.PLUS, "+", 0, 0), A, B);

        var RRef = new VariableExpr(new Token(TokenType.NAME, "R", 0, 0));
        var resultAssign = new AssignExpr(new Token(TokenType.ASSIGN, ":=", 0, 0), RRef, binaryExpr);

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

        Assert.Equal(OpcodesBuilder.LOADC(0), codeGen.Code[0]);
    }
}
