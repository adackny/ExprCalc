using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExprCalc.Stages.Lexical;

namespace ExprCalc.Cross
{

    public class CompilerError
    {
        private readonly List<Token> _tokenSequence;
        private readonly string _message;

        public CompilerError(List<Token> tokenSequence, string message, string sourceInput)
            : this(tokenSequence, message)
        {
            _message = BuildErrorWithUnderline(tokenSequence, sourceInput, message);
        }

        public CompilerError(List<Token> tokenSequence, string message)
        {
            _tokenSequence = tokenSequence;
            _message = message;
        }

        public string Message => _message;

        protected string BuildErrorWithUnderline(
            List<Token> tokenSequence, string sourceInput, string message)
        {
            var nl = System.Environment.NewLine;

            var contextLines = GetContextLines(sourceInput, tokenSequence);
            StringBuilder errorBuilder = UnderlineError(contextLines, message, nl);

            return errorBuilder.ToString();
        }

        private List<(int line, string text, IEnumerable<Token> tokens)> GetContextLines(string sourceInput, List<Token> tokenSequence)
        {
            var startToken = tokenSequence.First();
            var endToken = tokenSequence.Last();
            var result = new List<(int line, string text, IEnumerable<Token> tokens)>();

            var lines = Regex.Split(sourceInput, @"\r?\n");

            var tokenGroups = tokenSequence
                .GroupBy(t => t.Line)
                .ToDictionary(g => g.Key, g => g.ToList());

            for (int i = startToken.Line; i <= endToken.Line; i++)
            {
                result.Add((i, lines[i], tokenGroups[i]));
            }

            return result;
        }

        private StringBuilder UnderlineError(
            List<(int line, string text, IEnumerable<Token> tokens)> contextLines,
            string message,
            string newLine)
        {
            var sb = new StringBuilder();

            for (var k = 0; k < contextLines.Count; k++)
            {
                var (_, text, tokens) = contextLines[k];
                sb.Append(text);
                sb.Append(newLine);

                for (int i = 0; i < tokens.First().Column; i++)
                    sb.Append(' ');

                var first = tokens.First();
                var last = tokens.Last();
                var length = last.Column + last.Lexeme.Length;

                if (length == first.Column)
                    length++;
                
                for (int i = first.Column; i < length; i++)
                    sb.Append('^');

                sb.Append(newLine);
            }

            for (int i = 0; i < contextLines[0].tokens.First().Column; i++)
                sb.Append(' ');

            sb.Append(message);

            return sb;
        }
    }
}
