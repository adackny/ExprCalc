using System;

namespace ExprCalc.Stages.Lexical
{
    public static class TokenTypeExtensions
    {
        public static bool IsComparisonOperator(this TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.EQUAL:
                case TokenType.UNEQUAL:
                case TokenType.LESS:
                case TokenType.LESS_EQUAL:
                case TokenType.GREATER:
                case TokenType.GREATER_EQUAL:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsLogicOperator(this TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.AND:
                case TokenType.OR:
                case TokenType.NOT:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsArithmeticOperator(this TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.PLUS:
                case TokenType.MINUS:
                case TokenType.STAR:
                case TokenType.SLASH:
                    return true;
                default:
                    return false;
            }
        }
    }
}
