using ExprCalc.Stages.Lexical;

namespace CompilerTests;

public class LexerTests
{
    [Fact]
    public void TestNamesAndNumbers()
    {
        string text = "Hello 123asd1 456.7";

        var scanner = new Lexer(text);
        var helloToken = scanner.NextToken();
        var number123Token = scanner.NextToken();
        var asd1Token = scanner.NextToken();
        var number456_7Token = scanner.NextToken();

        Assert.Equal(TokenType.NAME, helloToken.TokenType);
        Assert.Equal("Hello", helloToken.Lexeme);

        Assert.Equal(TokenType.NUMBER, number123Token.TokenType);
        Assert.Equal("123", number123Token.Lexeme);

        Assert.Equal(TokenType.NAME, asd1Token.TokenType);
        Assert.Equal("asd1", asd1Token.Lexeme);

        Assert.Equal(TokenType.NUMBER, number456_7Token.TokenType);
        Assert.Equal("456.7", number456_7Token.Lexeme);
    }

    [Fact]
    public void TestStrings()
    {
        string text = "Hello \"123asd1\" \"456.7\"";

        var scanner = new Lexer(text);
        var helloToken = scanner.NextToken();
        var string123asd1Token = scanner.NextToken();
        var string456_7Token = scanner.NextToken();

        Assert.Equal(TokenType.NAME, helloToken.TokenType);
        Assert.Equal("Hello", helloToken.Lexeme);

        Assert.Equal(TokenType.STRING, string123asd1Token.TokenType);
        Assert.Equal("123asd1", string123asd1Token.Lexeme);


        Assert.Equal(TokenType.STRING, string456_7Token.TokenType);
        Assert.Equal("456.7", string456_7Token.Lexeme);
    }

    [Fact]
    public void TestComparisonOperators()
    {
        string text = "= >= <= <> < >";

        List<(TokenType kind, string lex)> expected = new List<(TokenType kind, string lex)>
        {
            (TokenType.EQUAL, "="),
            (TokenType.GREATER_EQUAL, ">="),
            (TokenType.LESS_EQUAL, "<="),
            (TokenType.UNEQUAL, "<>"),
            (TokenType.LESS, "<"),
            (TokenType.GREATER, ">"),
            (TokenType.EOF, ""),
        };

        var scanner = new Lexer(text);

        foreach (var (kind, lex) in expected)
        {
            var token = scanner.NextToken();
            Assert.Equal(lex, token.Lexeme);
            Assert.Equal(kind, token.TokenType);
        }
    }

    [Fact]
    public void TestComparisonOfObjects()
    {
        string text = """
            a = 2
            123 <>456
            5>= 2
            """;

        List<(TokenType kind, string lex)> expected = new List<(TokenType kind, string lex)>
        {
            (TokenType.NAME, "a"),
            (TokenType.EQUAL, "="),
            (TokenType.NUMBER, "2"),

            (TokenType.NUMBER, "123"),
            (TokenType.UNEQUAL, "<>"),
            (TokenType.NUMBER, "456"),

            (TokenType.NUMBER, "5"),
            (TokenType.GREATER_EQUAL, ">="),
            (TokenType.NUMBER, "2"),

            (TokenType.EOF, ""),
        };

        var scanner = new Lexer(text);

        foreach (var (kind, lex) in expected)
        {
            var token = scanner.NextToken();
            Assert.Equal(lex, token.Lexeme);
            Assert.Equal(kind, token.TokenType);
        }
    }

    [Fact]
    public void TestLogicExpressions()
    {
        string text = """
            a = 2 AND b = 1
            123 <>456 OR asd = 2
            NOT 5>= 2
            """;

        List<(TokenType kind, string lex)> expected = new List<(TokenType kind, string lex)>
        {
            (TokenType.NAME, "a"),
            (TokenType.EQUAL, "="),
            (TokenType.NUMBER, "2"),
            (TokenType.AND, "AND"),
            (TokenType.NAME, "b"),
            (TokenType.EQUAL, "="),
            (TokenType.NUMBER, "1"),

            (TokenType.NUMBER, "123"),
            (TokenType.UNEQUAL, "<>"),
            (TokenType.NUMBER, "456"),
            (TokenType.OR, "OR"),
            (TokenType.NAME, "asd"),
            (TokenType.EQUAL, "="),
            (TokenType.NUMBER, "2"),

            (TokenType.NOT, "NOT"),
            (TokenType.NUMBER, "5"),
            (TokenType.GREATER_EQUAL, ">="),
            (TokenType.NUMBER, "2"),

            (TokenType.EOF, ""),
        };

        var scanner = new Lexer(text);

        foreach (var (kind, lex) in expected)
        {
            var token = scanner.NextToken();
            Assert.Equal(lex, token.Lexeme);
            Assert.Equal(kind, token.TokenType);
        }
    }
    
    [Fact]
    public void TestAllTokens()
    {
        string text = """
            a = 2 AND b = 1
            123 <>456 OR asd = 2
            NOT 5>= 2
            , (123) :=
            """;

        List<(TokenType kind, string lex)> expected = new List<(TokenType kind, string lex)>
        {
            (TokenType.NAME, "a"),
            (TokenType.EQUAL, "="),
            (TokenType.NUMBER, "2"),
            (TokenType.AND, "AND"),
            (TokenType.NAME, "b"),
            (TokenType.EQUAL, "="),
            (TokenType.NUMBER, "1"),
            
            (TokenType.NUMBER, "123"),
            (TokenType.UNEQUAL, "<>"),
            (TokenType.NUMBER, "456"),
            (TokenType.OR, "OR"),
            (TokenType.NAME, "asd"),
            (TokenType.EQUAL, "="),
            (TokenType.NUMBER, "2"),

            (TokenType.NOT, "NOT"),
            (TokenType.NUMBER, "5"),
            (TokenType.GREATER_EQUAL, ">="),
            (TokenType.NUMBER, "2"),


            (TokenType.COMMA, ","),

            (TokenType.OPEN_PAREN, "("),
            (TokenType.NUMBER, "123"),
            (TokenType.CLOSE_PAREN, ")"),

            (TokenType.ASSIGN, ":="),

            (TokenType.EOF, ""),
        };
        
        var scanner = new Lexer(text);

        foreach (var (kind, lex) in expected)
        {
            var token = scanner.NextToken();
            Assert.Equal(lex, token.Lexeme);
            Assert.Equal(kind, token.TokenType);
        }
    }
}
