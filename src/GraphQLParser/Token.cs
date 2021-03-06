﻿using System;

namespace GraphQLParser
{
    public enum TokenKind
    {
        UNDEFINED = 0,
        EOF = 1,
        BANG = 2,
        DOLLAR = 3,
        PAREN_L = 4,
        PAREN_R = 5,
        SPREAD = 6,
        COLON = 7,
        EQUALS = 8,
        AT = 9,
        BRACKET_L = 10,
        BRACKET_R = 11,
        BRACE_L = 12,
        PIPE = 13,
        BRACE_R = 14,
        NAME = 15,
        INT = 16,
        FLOAT = 17,
        STRING = 18
    }

    public ref struct Token
    {
        public bool IsEmpty => Kind == TokenKind.UNDEFINED;
        public int End { get; set; }
        public TokenKind Kind { get; set; }
        public int Start { get; set; }
        public ReadOnlySpan<char> Value { get; set; }

        public Token(int start, int end, ReadOnlySpan<char> value, TokenKind kind)
        {
            Start = start;
            End = end;
            Value = value;
            Kind = kind;
        }


        public static string GetTokenKindDescription(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.EOF: return "EOF";
                case TokenKind.BANG: return "!";
                case TokenKind.DOLLAR: return "$";
                case TokenKind.PAREN_L: return "(";
                case TokenKind.PAREN_R: return ")";
                case TokenKind.SPREAD: return "...";
                case TokenKind.COLON: return ":";
                case TokenKind.EQUALS: return "=";
                case TokenKind.AT: return "@";
                case TokenKind.BRACKET_L: return "[";
                case TokenKind.BRACKET_R: return "]";
                case TokenKind.BRACE_L: return "{";
                case TokenKind.PIPE: return "|";
                case TokenKind.BRACE_R: return "}";
                case TokenKind.NAME: return "Name";
                case TokenKind.INT: return "Int";
                case TokenKind.FLOAT: return "Float";
                case TokenKind.STRING: return "String";
            }

            return string.Empty;
        }

        public override string ToString()
        {
            return this.Value != null
                ? $"{GetTokenKindDescription(this.Kind)} \"{new string(this.Value)}\""
                : GetTokenKindDescription(this.Kind);
        }
    }
}