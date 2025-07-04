using System;
using System.Collections.Generic;

namespace ExprCalc.Stages.Lexical
{
    public class Lexer
    {
        private readonly string _input;
        private int _currentPosition;
        private readonly State _startState;
        private int _line = 0;
        private int _column = 0;

        private readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>
        {
            { "AND", TokenType.AND },
            { "OR", TokenType.OR },
            { "NOT", TokenType.NOT },
        };


        public Lexer(string input)
        {
            _input = input;
            _startState = new State("start", TokenType.INVALID);
            CreateNameState(_startState);
            CreateNumberState(_startState);
            CreateComparisonStates(_startState);
            CreateStringState(_startState);

            _startState.Transition(new State("add", TokenType.PLUS, true), (str, i) => str[i] == '+');
            _startState.Transition(new State("sub", TokenType.MINUS, true), (str, i) => str[i] == '-');
            _startState.Transition(new State("mult", TokenType.STAR, true), (str, i) => str[i] == '*');
            _startState.Transition(new State("div", TokenType.DIV, true), (str, i) => str[i] == '/');

            _startState.Transition(new State("openParenthesis", TokenType.OPEN_PAREN, true), (str, i) => str[i] == '(');
            _startState.Transition(new State("closeParenthesis", TokenType.CLOSE_PAREN, true), (str, i) => str[i] == ')');
            _startState.Transition(new State("comma", TokenType.COMMA, true), (str, i) => str[i] == ',');
        }

        public string SourceInput => _input;

        public Token NextToken()
        {
            ConsumeWhiteSpaces();

            if (_currentPosition >= _input.Length)
                return new Token(TokenType.EOF, string.Empty, _line, _column);

            var (state, lexeme) = Evaluator.Recognize(_startState, _input, _currentPosition);

            if (state == null)
                return new Token(TokenType.INVALID, string.Empty, _line, _column);

            if (!state.IsFinal)
                return new Token(TokenType.INVALID, lexeme, _line, _column);

            var tokenType = state.TokenType;
            if (_keywords.ContainsKey(lexeme))
                tokenType = _keywords[lexeme];
            
            var tokenLexeme = lexeme.Trim('\"');

            var token = new Token(tokenType, tokenLexeme, _line, _column);
            _currentPosition += lexeme.Length;
            _column += lexeme.Length;
            return token;
        }

        private void ConsumeWhiteSpaces()
        {
            while (_currentPosition < _input.Length && char.IsWhiteSpace(_input[_currentPosition]))
            {
                if (_input[_currentPosition] == '\n' || _currentPosition + 2 < _input.Length && _input[_currentPosition] == '\r' && _input[_currentPosition] == '\n')
                {
                    _line++;
                    _column = 0;
                }
                else
                {
                    _column++;
                }
                _currentPosition++;
            }
        }

        private void CreateNameState(State startState)
        {
            var nameQ0 = new State("nameQ0", TokenType.NAME, true);
            var nameQ1 = new State("nameQ1", TokenType.NAME, true);

            startState.Transition(nameQ0, (str, i) => char.IsLetter(str[i]) || str[i] == '_');
            nameQ0.Transition(nameQ1, (str, i) => char.IsLetter(str[i]) || char.IsDigit(str[i]) || str[i] == '_');
            nameQ1.Transition(nameQ1, (str, i) => char.IsLetter(str[i]) || char.IsDigit(str[i]) || str[i] == '_');
        }

        private void CreateNumberState(State startState)
        {
            var numberQ0 = new State("numberQ0", TokenType.NUMBER, true);
            var numberQ1 = new State("numberQ1", TokenType.NUMBER);
            var numberQ2 = new State("numberQ1", TokenType.NUMBER, true);

            startState.Transition(numberQ0, (str, i) => char.IsDigit(str[i]));
            numberQ0.Transition(numberQ0, (str, i) => char.IsDigit(str[i]));
            numberQ0.Transition(numberQ1, (str, i) => i + 1 < str.Length && str[i] == '.' && char.IsDigit(str[i + 1]));
            numberQ1.Transition(numberQ2, (str, i) => char.IsDigit(str[i]));
        }

        public void CreateStringState(State startState)
        {
            var stringQ0 = new State("stringQ0", TokenType.STRING);
            var stringQ1 = new State("stringQ1", TokenType.STRING);
            var stringQ2 = new State("stringQ2", TokenType.STRING, true);

            startState.Transition(stringQ0, (str, i) => str[i] == '"');
            stringQ0.Transition(stringQ1, (str, i) => str[i] != '\n' && str[i] != '\r');
            stringQ1.Transition(stringQ1, (str, i) => str[i] != '"' && str[i] != '\n' && str[i] != '\r');
            stringQ1.Transition(stringQ2, (str, i) => str[i] == '"');
        }

        private void CreateComparisonStates(State startState)
        {
            var less = new State("less", TokenType.LESS, true);
            var equals = new State("equals", TokenType.EQUAL, true);
            var greater = new State("greater", TokenType.GREATER, true);

            var assignQ0 = new State("assignQ0", TokenType.ASSIGN);
            var assignQ1 = new State("assignQ1", TokenType.ASSIGN, true);

            var lessOrEquals = new State("lessOrEquals", TokenType.LESS_EQUAL, true);
            var greaterOrEquals = new State("lessOrEquals", TokenType.GREATER_EQUAL, true);
            var notEquals = new State("notEquals", TokenType.UNEQUAL, true);


            startState.Transition(less, (str, i) => str[i] == '<');
            less.Transition(lessOrEquals, (str, i) => str[i] == '=');
            less.Transition(notEquals, (str, i) => str[i] == '>');

            startState.Transition(equals, (str, i) => str[i] == '=');

            startState.Transition(greater, (str, i) => str[i] == '>');
            greater.Transition(greaterOrEquals, (str, i) => str[i] == '=');

            startState.Transition(assignQ0, (str, i) => str[i] == ':');
            assignQ0.Transition(assignQ1, (str, i) => str[i] == '=');
        }
    }

    public static class Evaluator
    {
        public static (State state, string lexeme) Recognize(State startState, string input, int startIndex)
        {
            State finalState = null;
            State next = startState;
            int currentIndex = startIndex;

            while (currentIndex < input.Length && next != null)
            {
                next = next.Next(input, currentIndex);
                if (next == null)
                    break;
                
                if (next.IsFinal)
                    finalState = next;
                currentIndex++;
            }
            var lexeme = input.Substring(startIndex, currentIndex - startIndex);

            return (finalState, lexeme);
        }
    }

    public class State
    {

        public enum ExecStatus { }
        private readonly List<(State state, Func<string, int, bool> condition)> _nextStates = new List<(State state, Func<string, int, bool> condition)>();

        public State(string name, TokenType tokenType, bool isFinal = false) => (Name, TokenType, IsFinal) = (name, tokenType, isFinal);

        public void Transition(State next, Func<string, int, bool> condition) => _nextStates.Add((next, condition));

        public string Name { get; }
        public TokenType TokenType { get; set; }
        public bool IsFinal { get; }

        public State Next(string input, int index)
        {
            foreach (var (state, condition) in _nextStates)
            {
                if (condition(input, index))
                    return state;
            }
            return null;
        }
    }
}
