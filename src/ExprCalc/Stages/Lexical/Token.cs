namespace ExprCalc.Stages.Lexical
{
    public class Token
    {
        public Token(TokenType kind, string lexeme, int line, int column)
        {
            TokenType = kind;
            Lexeme = lexeme;
            Line = line;
            Column = column;
        }

        public TokenType TokenType { get; }
        public string Lexeme { get; }
        public int Line { get; }
        public int Column { get; }
        // public int SourcePosition { get; }
    }
}
