using ExprCalc;
using ExprCalc.Runtime;
using ExprCalc.Stages.CodeGen;
using ExprCalc.Stages.Lexical;
using ExprCalc.Stages.Semantic;
using ExprCalc.Stages.Syntax;
using ExprCalc.Symbols;

namespace CompilerTests;

public class FullProcessTests
{
    [Fact]
    public void SimpleSumTest()
    {
        var program = """
            num := 2.4
            R := num + 3.5
            """;

        var lexer = new Lexer(program);
        var symbolsTable = new SymbolsTable([]);
        var parser = new Parser(lexer, symbolsTable);
        var ast = parser.Parse();
        var typeChecker = new TypeChecker(symbolsTable, program);
        ast.Accept(typeChecker);
        var codeGen = new CodeGenerator(symbolsTable);
        ast.Accept(codeGen);
        var interpreter = new Interpreter();
        interpreter.Load(codeGen.Code, codeGen.Data, codeGen.ConstPool);
        interpreter.Run();

        Assert.Equal(5.9m, interpreter.Memory[1]);
    }

    [Fact]
    public void SimpleSumWithExternalVariableTest()
    {
        var program = """
            num := 2.4
            R := num + E
            """;

        var engine = new Engine(program, new List<Symbol>
        {
            new ExternalVariableSymbol("E", SymbolsTable.String, "hello")
        });

        var errors = engine.Run();

        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Formula1Test()
    {
        var program = """
            (M/1000)*P1+(M/1000)*P2+ (M/1000)*P3
            """;

        var symbolsTable = new SymbolsTable(new List<Symbol>
        {
            new ExternalVariableSymbol("M", SymbolsTable.Number, 10_000m),
            new ExternalVariableSymbol("P1", SymbolsTable.Number, 1m),
            new ExternalVariableSymbol("P2", SymbolsTable.Number, 0m),
            new ExternalVariableSymbol("P3", SymbolsTable.Number, 0m),
        });
        var parser = new Parser(new Lexer(program), symbolsTable);

        var ast = parser.Parse();
        var typeChecker = new TypeChecker(symbolsTable, program);
        ast.Accept(typeChecker);
        Assert.Empty(parser.Errors);
        Assert.Empty(typeChecker.Errors);
    }

    [Fact]
    public void Formula2Test()
    {
        var program = """
            (M/1000)*P1(M/1000)*P2+ (M/1000)*P3
            """;

        var symbols = new List<Symbol>
        {
            new ExternalVariableSymbol("M", SymbolsTable.Number, 10_000m),
            new ExternalVariableSymbol("P1", SymbolsTable.Number, 1m),
            new ExternalVariableSymbol("P2", SymbolsTable.Number, 0m),
            new ExternalVariableSymbol("P3", SymbolsTable.Number, 0m),
        };

        var engine = new Engine(program, symbols);
        var errors = engine.Check();
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Formula3Test()
    {
        var program = """
            ((TIME(B)>=0001) AND (TIME(B)<=0659))
            """;

        var timeFuncType = new FunctionType(
            new List<PrimitiveType> { SymbolsTable.Number },
            SymbolsTable.Number);
        var symbols = new List<Symbol>
        {
            timeFuncType,
            new ExternalFunctionSymbol("TIME", timeFuncType, (args) =>
            {
                return null;
            }),
            new ExternalVariableSymbol("B", SymbolsTable.Number, 10_000m),
        };

        var engine = new Engine(program, symbols);
        var errors = engine.Check();
        Assert.Empty(errors);
    }
    
    [Fact]
    public void Formula4Test()
    {
        var program = """
            ((TIME(B)>=0001)(TIME(B)<=0659))
            """;

        var timeFuncType = new FunctionType(
            new List<PrimitiveType> { SymbolsTable.Number },
            SymbolsTable.Number);
        var symbols = new List<Symbol>
        {
            timeFuncType,
            new ExternalFunctionSymbol("TIME", timeFuncType, (args) =>
            {
                return null;
            }),
            new ExternalVariableSymbol("B", SymbolsTable.Number, 10_000m),
        };

        var engine = new Engine(program, symbols);
        var errors = engine.Check();
        Assert.NotEmpty(errors);
    }
}
