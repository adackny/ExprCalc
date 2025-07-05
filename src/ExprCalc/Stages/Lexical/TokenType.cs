namespace ExprCalc.Stages.Lexical
{
    public enum TokenType
    {
        EOF, INVALID, PLUS, MINUS, STAR, SLASH, ASSIGN,

        EQUAL, UNEQUAL, LESS, GREATER, LESS_EQUAL, GREATER_EQUAL,

        NOT, AND, OR,

        NAME, NUMBER, STRING,

        OPEN_PAREN, CLOSE_PAREN, COMMA
    }
}
