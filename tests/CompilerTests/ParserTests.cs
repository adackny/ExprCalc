using ExprCalc.Stages.Lexical;
using ExprCalc.Stages.Syntax;
using ExprCalc.Symbols;
using ExprCalc.Trees.Abstract;

namespace CompilerTests;

public class ParserTests
{
    [Fact]
    public void TestBasicProgram()
    {
        var text = """
            p1 = 123 + 3*B
            p2 = 456
            r := p1 + p2
            """;

        var parser = new Parser(new Lexer(text), new SymbolsTable([]));

        AstNode program = parser.Parse();
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void TestBasicProgramWithError()
    {
        var text = """
            r := p1 +
            """;

        var parser = new Parser(new Lexer(text), new SymbolsTable([]));

        AstNode program = parser.Parse();

        var error = """
            r := p1 +
                     ^
                     Se esperaba una expresión
            """;
        Assert.Equal(error, parser.Errors[0].Message);
    }

    [Fact]
    public void TestInvalidFuncCallWithError()
    {
        var text = """
            p1 + Asd(Qwe,)
            """;

        var parser = new Parser(new Lexer(text), new SymbolsTable([]));

        AstNode program = parser.Parse();

        var error = """
            p1 + Asd(Qwe,)
                         ^
                         Se esperaba una expresión
            """;
        Assert.Equal(error, parser.Errors[0].Message);
    }


    [Fact]
    public void TestInvalidAssignment()
    {
        var text = """
            123, := 123
            """;

        var parser = new Parser(new Lexer(text), new SymbolsTable([]));

        AstNode program = parser.Parse();

        var error = """
            123, := 123
               ^
               Se esperaba una expresión
            """;
        Assert.Equal(error, parser.Errors[0].Message);
    }

    [Fact]
    public void TestExpressionWithStrings()
    {
        var text = """
            a := "Hello"
            """;

        var parser = new Parser(new Lexer(text), new SymbolsTable([]));

        AstNode program = parser.Parse();
        Assert.Empty(parser.Errors);
    }
}
