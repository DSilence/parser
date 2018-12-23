﻿using System.Text;

namespace GraphQLParser
{
    public class Lexer : ILexer
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public Token Lex(ISource source)
        {
            return this.Lex(source, 0);
        }

        public Token Lex(ISource source, int start)
        {
            var context = new LexerContext(source, start, _stringBuilder);
                return context.GetToken();
        }
    }
}