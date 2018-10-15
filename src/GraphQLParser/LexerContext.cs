using System.Text;

namespace GraphQLParser
{
    using Exceptions;
    using System;

    public ref struct LexerContext
    {
        private int currentIndex;
        private readonly ReadOnlySpan<char> body;
        private readonly ISource _source;
        private readonly StringBuilder stringBuilder;

        public LexerContext(ISource source, int index)
        {
            this.currentIndex = index;
            this.body = source.Body.Span;
            this._source = source;
            stringBuilder = new StringBuilder();
        }

        public Token GetToken()
        {
            if (this.body.IsEmpty)
                return this.CreateEOFToken();

            this.currentIndex = this.GetPositionAfterWhitespace(this.body, this.currentIndex);

            if (this.currentIndex >= this.body.Length)
                return this.CreateEOFToken();

            var code = this.body[this.currentIndex];

            this.ValidateCharacterCode(code);

            var token = this.CheckForPunctuationTokens(code);
            if (!token.IsEmpty)
                return token;

            if (char.IsLetter(code) || code == '_')
                return this.ReadName();

            if (char.IsNumber(code) || code == '-')
                return this.ReadNumber();

            if (code == '"')
                return this.ReadString();

            throw new GraphQLSyntaxErrorException(
                $"Unexpected character {this.ResolveCharName(code, IfUnicodeGetString().ToString())}", _source, this.currentIndex);
        }

        public bool OnlyHexInString(ReadOnlySpan<char> test)
        {
            bool isHex;
            for (var i = 0; i < test.Length; i++)
            {
                var c = test[i];
                isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!isHex)
                {
                    return false;
                }
            }

            return true;
        }

        public Token ReadNumber()
        {
            var isFloat = false;
            var start = this.currentIndex;
            var code = this.body[start];

            if (code == '-')
                code = this.NextCode();

            var nextCode = code == '0'
                ? this.NextCode()
                : this.ReadDigitsFromOwnSource(code);

            if (nextCode >= 48 && nextCode <= 57)
            {
                throw new GraphQLSyntaxErrorException(
                    $"Invalid number, unexpected digit after {code}: \"{nextCode}\"", _source, this.currentIndex);
            }

            code = nextCode;
            if (code == '.')
            {
                isFloat = true;
                code = this.ReadDigitsFromOwnSource(this.NextCode());
            }

            if (code == 'E' || code == 'e')
            {
                isFloat = true;
                code = this.NextCode();
                if (code == '+' || code == '-')
                {
                    code = this.NextCode();
                }

                code = this.ReadDigitsFromOwnSource(code);
            }

            return isFloat ? this.CreateFloatToken(start) : this.CreateIntToken(start);
        }

        public Token ReadString()
        {
            var start = this.currentIndex;
            var value = this.ProcessStringChunks();

            return new Token()
            {
                Kind = TokenKind.STRING,
                Value = value.ToString().AsSpan(),
                Start = start,
                End = this.currentIndex + 1
            };
        }

        private static bool IsValidNameCharacter(char code)
        {
            return code == '_' || char.IsLetterOrDigit(code);
        }

        private StringBuilder AppendCharactersFromLastChunk(StringBuilder value, int chunkStart)
        {
            return value.Append(this.body.Slice(chunkStart, this.currentIndex - chunkStart - 1).ToArray());
        }

        private StringBuilder AppendToValueByCode(StringBuilder value, char code)
        {
            switch (code)
            {
                case '"': value.Append('"'); break;
                case '/': value.Append('/'); break;
                case '\\': value.Append('\\'); break;
                case 'b': value.Append('\b'); break;
                case 'f': value.Append('\f'); break;
                case 'n': value.Append('\n'); break;
                case 'r': value.Append('\r'); break;
                case 't': value.Append('\t'); break;
                case 'u': value.Append(this.GetUnicodeChar()); break;
                default:
                    throw new GraphQLSyntaxErrorException($"Invalid character escape sequence: \\{code}.", _source, this.currentIndex);
            }

            return value;
        }

        private int CharToHex(char code)
        {
            return Convert.ToByte(code.ToString(), 16);
        }

        private void CheckForInvalidCharacters(char code)
        {
            if (code < 0x0020 && code != 0x0009)
            {
                throw new GraphQLSyntaxErrorException(
                    $"Invalid character within String: \\u{((int)code).ToString("D4")}.", _source, this.currentIndex);
            }
        }

        private Token CheckForPunctuationTokens(char code)
        {
            switch (code)
            {
                case '!': return this.CreatePunctuationToken(TokenKind.BANG, 1);
                case '$': return this.CreatePunctuationToken(TokenKind.DOLLAR, 1);
                case '(': return this.CreatePunctuationToken(TokenKind.PAREN_L, 1);
                case ')': return this.CreatePunctuationToken(TokenKind.PAREN_R, 1);
                case '.': return this.CheckForSpreadOperator();
                case ':': return this.CreatePunctuationToken(TokenKind.COLON, 1);
                case '=': return this.CreatePunctuationToken(TokenKind.EQUALS, 1);
                case '@': return this.CreatePunctuationToken(TokenKind.AT, 1);
                case '[': return this.CreatePunctuationToken(TokenKind.BRACKET_L, 1);
                case ']': return this.CreatePunctuationToken(TokenKind.BRACKET_R, 1);
                case '{': return this.CreatePunctuationToken(TokenKind.BRACE_L, 1);
                case '|': return this.CreatePunctuationToken(TokenKind.PIPE, 1);
                case '}': return this.CreatePunctuationToken(TokenKind.BRACE_R, 1);
                default: return default;
            }
        }

        private Token CheckForSpreadOperator()
        {
            var char1 = this.body.Length > this.currentIndex + 1 ? this.body[this.currentIndex + 1] : 0;
            var char2 = this.body.Length > this.currentIndex + 2 ? this.body[this.currentIndex + 2] : 0;

            if (char1 == '.' && char2 == '.')
            {
                return this.CreatePunctuationToken(TokenKind.SPREAD, 3);
            }

            return default;
        }

        private void CheckStringTermination(char code)
        {
            if (code != '"')
            {
                throw new GraphQLSyntaxErrorException("Unterminated string.", _source, this.currentIndex);
            }
        }

        private Token CreateEOFToken()
        {
            return new Token()
            {
                Start = this.currentIndex,
                End = this.currentIndex,
                Kind = TokenKind.EOF
            };
        }

        private Token CreateFloatToken(int start)
        {
            return new Token(start, this.currentIndex, this.body.Slice(start, this.currentIndex - start), TokenKind.FLOAT)
            ;
        }

        private Token CreateIntToken(int start)
        {
            return new Token()
            {
                Kind = TokenKind.INT,
                Start = start,
                End = this.currentIndex,
                Value = this.body.Slice(start, this.currentIndex - start)
            };
        }

        private Token CreateNameToken(int start)
        {
            return new Token()
            {
                Start = start,
                End = this.currentIndex,
                Kind = TokenKind.NAME,
                Value = this.body.Slice(start, this.currentIndex - start)
            };
        }

        private Token CreatePunctuationToken(TokenKind kind, int offset)
        {
            return new Token()
            {
                Start = this.currentIndex,
                End = this.currentIndex + offset,
                Kind = kind,
                Value = null
            };
        }

        private char GetCode()
        {
            return this.IsNotAtTheEndOfQuery()
                ? this.body[this.currentIndex]
                : (char)0;
        }

        private int GetPositionAfterWhitespace(ReadOnlySpan<char> body, int start)
        {
            var position = start;

            while (position < body.Length)
            {
                var code = body[position];
                switch (code)
                {
                    case '\xFEFF': // BOM
                    case '\t': // tab
                    case ' ': // space
                    case '\n': // new line
                    case '\r': // carriage return
                    case ',': // Comma
                        ++position;
                        break;

                    case '#':
                        position = this.WaitForEndOfComment(body, position, code);
                        break;

                    default:
                        return position;
                }
            }

            return position;
        }

        private char GetUnicodeChar()
        {
            var expression = this.body.Slice(this.currentIndex, 5);

            if (!this.OnlyHexInString(expression.Slice(1)))
            {
                throw new GraphQLSyntaxErrorException($"Invalid character escape sequence: \\{expression.ToString()}.", _source, this.currentIndex);
            }

            var character = (char)(
                this.CharToHex(this.NextCode()) << 12 |
                this.CharToHex(this.NextCode()) << 8 |
                this.CharToHex(this.NextCode()) << 4 |
                this.CharToHex(this.NextCode()));

            return character;
        }

        private ReadOnlySpan<char> IfUnicodeGetString()
        {
            return this.body.Length > this.currentIndex + 5 &&
                this.OnlyHexInString(this.body.Slice(this.currentIndex + 2, 4))
                ? this.body.Slice(this.currentIndex, 6)
                : default;
        }

        private bool IsNotAtTheEndOfQuery()
        {
            return this.currentIndex < this.body.Length;
        }

        private char NextCode()
        {
            this.currentIndex++;
            return this.IsNotAtTheEndOfQuery()
                ? this.body[this.currentIndex]
                : (char)0;
        }

        private char ProcessCharacter(StringBuilder value, ref int chunkStart)
        {
            var code = this.GetCode();
            ++this.currentIndex;

            if (code == '\\')
            {
                value = this.AppendToValueByCode(this.AppendCharactersFromLastChunk(value, chunkStart), this.GetCode());

                ++this.currentIndex;
                chunkStart = this.currentIndex;
            }

            return this.GetCode();
        }

        private StringBuilder ProcessStringChunks()
        {
            var chunkStart = ++this.currentIndex;
            var code = this.GetCode();
            var value = stringBuilder.Clear();

            while (this.IsNotAtTheEndOfQuery() && code != 0x000A && code != 0x000D && code != '"')
            {
                this.CheckForInvalidCharacters(code);
                code = this.ProcessCharacter(value, ref chunkStart);
            }

            this.CheckStringTermination(code);
            value.Append(this.body.Slice(chunkStart, this.currentIndex - chunkStart).ToArray());
            return value;
        }

        private int ReadDigits(int start, char firstCode)
        {
            var position = start;
            var code = firstCode;

            if (!char.IsNumber(code))
            {
                throw new GraphQLSyntaxErrorException(
                    $"Invalid number, expected digit but got: {this.ResolveCharName(code)}", _source, this.currentIndex);
            }

            do
            {
                code = ++position < body.Length
                    ? body[position]
                    : (char)0;
            }
            while (char.IsNumber(code));

            return position;
        }

        private char ReadDigitsFromOwnSource(char code)
        {
            this.currentIndex = this.ReadDigits(this.currentIndex, code);
            code = this.GetCode();
            return code;
        }

        private Token ReadName()
        {
            var start = this.currentIndex;
            var code = (char)0;

            do
            {
                this.currentIndex++;
                code = this.GetCode();
            }
            while (this.IsNotAtTheEndOfQuery() && IsValidNameCharacter(code));

            return this.CreateNameToken(start);
        }

        private string ResolveCharName(char code, string unicodeString = null)
        {
            if (code == '\0')
                return "<EOF>";

            if (!string.IsNullOrWhiteSpace(unicodeString))
                return $"\"{unicodeString}\"";

            return $"\"{code}\"";
        }

        private void ValidateCharacterCode(int code)
        {
            if (code < 0x0020 && code != 0x0009 && code != 0x000A && code != 0x000D)
            {
                throw new GraphQLSyntaxErrorException(
                    $"Invalid character \"\\u{code.ToString("D4")}\".", _source, this.currentIndex);
            }
        }

        private int WaitForEndOfComment(ReadOnlySpan<char> body, int position, char code)
        {
            while (++position < body.Length && (code = body[position]) != 0 && (code > 0x001F || code == 0x0009) && code != 0x000A && code != 0x000D)
            {
            }

            return position;
        }
    }
}