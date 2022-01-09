/*
Copyright (c) 2020 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2020.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PluginMaster
{
    public class PatternMachine
    {
        #region STATES AND TOKENS
        public enum PatternState
        {
            START,
            INDEX,
            OPENING_PARENTHESIS,
            CLOSING_PARENTHESIS,
            COMMA,
            ASTERISK,
            MULTIPLIER,
            ELLIPSIS,
            END
        }

        public class Token
        {
            public readonly PatternState state = PatternState.START;
            protected Token(PatternState state) => this.state = state;
            public static Token START = new Token(PatternState.START);
            public static Token OPENING_PARENTHESIS = new Token(PatternState.OPENING_PARENTHESIS);
            public static Token CLOSING_PARENTHESIS = new Token(PatternState.CLOSING_PARENTHESIS);
            public static Token COMMA = new Token(PatternState.COMMA);
            public static Token ASTERISK = new Token(PatternState.ASTERISK);
            public static Token ELLIPSIS = new Token(PatternState.ELLIPSIS);
            public static Token END = new Token(PatternState.END);
        }

        public class IntToken : Token
        {
            public readonly int value = -1;
            public IntToken(int value, PatternState state) : base(state) => this.value = value;
        }

        public class MultiplierToken : IntToken
        {
            private int _count = 0;
            public int count => _count;
            public MultiplierToken(int value) : base(value, PatternState.MULTIPLIER) { }
            public int IncreaseCount() => ++_count;
            public void Reset() => _count = 0;
        }
        #endregion
        #region VALIDATE
        public enum ValidationResult
        {
            VALID,
            EMPTY,
            INDEX_OUT_OF_RANGE,
            MISPLACED_PERIOD,
            MISPLACED_ASTERISK,
            MISPLACED_COMMA,
            UNPAIRED_PARENTHESIS,
            EMPTY_PARENTHESIS,
            INVALID_MULTIPLIER,
            INVALID_CHARACTER
        }

        public static ValidationResult Validate(string frecuencyPattern, int lastIndex, out Token[] tokens)
        {
            tokens = null;
            frecuencyPattern = frecuencyPattern.Replace(" ", "");
            if (frecuencyPattern == string.Empty) return ValidationResult.EMPTY;
            var validCharactersRemoved = Regex.Replace(frecuencyPattern, @"[\d,.*()]", "");
            if (validCharactersRemoved != string.Empty) return ValidationResult.INVALID_CHARACTER;
            var validBracketsRemoved = Regex.Replace(frecuencyPattern, @"\((?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)", "");
            if (Regex.Match(validBracketsRemoved, @"\(|\)").Success) return ValidationResult.UNPAIRED_PARENTHESIS;
            if (Regex.Match(frecuencyPattern, @"\(\)").Success) return ValidationResult.EMPTY_PARENTHESIS;
            var validMultiplicationsRemoved = Regex.Replace(frecuencyPattern, @"(\d+|\))\*\d+", "");
            if(Regex.Match(validMultiplicationsRemoved, @"\*").Success) return ValidationResult.MISPLACED_ASTERISK;
            var validCommasRemoved = Regex.Replace(frecuencyPattern, @"(\)|\d+)(,(\(|\d+))+", "");
            if (Regex.Match(validCommasRemoved, @",").Success) return ValidationResult.MISPLACED_COMMA;
            var validDotsRemoved = Regex.Replace(frecuencyPattern, @"(\d|\))\.\.\.(?!.)", "");
            if (Regex.Match(validDotsRemoved, @"\.").Success) return ValidationResult.MISPLACED_PERIOD;
            var matches = Regex.Matches(frecuencyPattern, @"\d+|[(),*]|\.\.\.");
            var tokenList = new List<Token>();
            tokenList.Add(Token.START);
            foreach(Match match in matches)
            {
                if (match.Value == "(") tokenList.Add(Token.OPENING_PARENTHESIS);
                else if (match.Value == ")") tokenList.Add(Token.CLOSING_PARENTHESIS);
                else if (match.Value == ",") tokenList.Add(Token.COMMA);
                else if (match.Value == "*") tokenList.Add(Token.ASTERISK);
                else if (match.Value == "...")
                {
                    if(tokenList.Last() is MultiplierToken) return ValidationResult.MISPLACED_PERIOD;
                    tokenList.Add(Token.ELLIPSIS);
                }
                else
                {
                    var value = int.Parse(match.Value);
                    var state = tokenList.Count > 0 && tokenList.Last() == Token.ASTERISK ? PatternState.MULTIPLIER : PatternState.INDEX;
                    if (state == PatternState.INDEX && value > lastIndex) return ValidationResult.INDEX_OUT_OF_RANGE;
                    else if (state == PatternState.MULTIPLIER && value < 2) return ValidationResult.INVALID_MULTIPLIER;
                    tokenList.Add(state == PatternState.INDEX ? new IntToken(value, state) : new MultiplierToken(value));
                }
            }
            tokenList.Add(Token.END);
            tokens = tokenList.ToArray();
            return ValidationResult.VALID;
        }
        #endregion
        #region MACHINE
        private Token[] _tokens = null;
        private int _tokenIndex = 0;
        private Stack<int> _parenthesisStack = new Stack<int>();
        private int _lastParenthesis = -1;

        public PatternMachine(Token[] tokens) => _tokens = tokens;

        public void SetTokens(Token[] tokens)
        {
            if (Enumerable.SequenceEqual(tokens, _tokens)) return;
            _tokens = tokens;
        }

        public void Reset()
        {
            _tokenIndex = 0;
            foreach(var token in _tokens) if(token is MultiplierToken) (token as MultiplierToken).Reset();
        }

        public int nextIndex
        {
            get
            {
                if (_tokenIndex == -1) return -1;
                var currentState = _tokens[_tokenIndex].state;
                if (currentState == PatternState.END) return -1;
                ++_tokenIndex;
                var nextToken = _tokens[_tokenIndex];
                switch(nextToken.state)
                {
                    case PatternState.INDEX:
                        return (nextToken as IntToken).value;
                    case PatternState.OPENING_PARENTHESIS:
                        _parenthesisStack.Push(_tokenIndex);
                        break;
                    case PatternState.CLOSING_PARENTHESIS:
                        _lastParenthesis = _parenthesisStack.Pop();
                        break;
                    case PatternState.MULTIPLIER:
                        var mult = nextToken as MultiplierToken;
                        if (mult.IncreaseCount() < mult.value) _tokenIndex = currentState == PatternState.CLOSING_PARENTHESIS ? _lastParenthesis : _tokenIndex - 3;
                        break;
                    case PatternState.ELLIPSIS:
                        _tokenIndex = currentState == PatternState.CLOSING_PARENTHESIS ? _lastParenthesis - 1 : _tokenIndex - 2;
                        break;
                    case PatternState.END:
                        return -1;
                    default:
                        break;
                }
                return nextIndex;
            }
        }
        #endregion
    }
}