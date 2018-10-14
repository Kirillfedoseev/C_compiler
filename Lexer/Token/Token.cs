
using Compiler.Syntaxing;
using Compiler.Units;
using System.Linq;

namespace Compiler
{
    /// <summary>
    /// Storage class for tokens
    /// </summary>
    public class Token
    {
        public TokenType Type { get; set; }

        public int Line { get; set; }

        public int Position { get; set; }

        public string Value { get; set; }

        public Token(TokenType type, int line, int postion,string value = "")
        {
            Type = type;
            Line = line;
            Position = postion;
            Value = value;
        }
        static Dependency[] atr =
        {
            new Dependency("EOI", "Eof_input", TokenType.End_of_input, false, false, false, -1, NodeType.None),
            new Dependency("*", "Op_multiply", TokenType.Op_multiply, false, true, false, 13, NodeType.Mul),
            new Dependency("/", "Op_divide", TokenType.Op_divide, false, true, false, 13, NodeType.Div),
            new Dependency("%", "Op_mod", TokenType.Op_mod, false, true, false, 13, NodeType.Mod),
            new Dependency("+", "Op_add", TokenType.Op_add, false, true, false, 12, NodeType.Add),
            new Dependency("-", "Op_subtract", TokenType.Op_subtract, false, true, false, 12, NodeType.Sub),
            new Dependency("-", "Op_negate", TokenType.Op_negate, false, false, true, 14, NodeType.Negate),
            new Dependency("!", "Op_not", TokenType.Op_not, false, false, true, 14, NodeType.Not),
            new Dependency("<", "Op_less", TokenType.Op_less, false, true, false, 10, NodeType.Lss),
            new Dependency("<=", "Op_lessequal", TokenType.Op_lessequal, false, true, false, 10, NodeType.Leq),
            new Dependency(">", "Op_greater", TokenType.Op_greater, false, true, false, 10, NodeType.Gtr),
            new Dependency(">=", "Op_greaterequal", TokenType.Op_greaterequal, false, true, false, 10, NodeType.Geq),
            new Dependency("==", "Op_equal", TokenType.Op_equal, false, true, false, 9, NodeType.Eql),
            new Dependency("!=", "Op_notequal", TokenType.Op_notequal, false, true, false, 9, NodeType.Neq),
            new Dependency("=", "Op_assign", TokenType.Op_assign, false, false, false, -1, NodeType.Assign),
            new Dependency("&&", "Op_and", TokenType.Op_and, false, true, false, 5, NodeType.And),
            new Dependency("||", "Op_or", TokenType.Op_or, false, true, false, 4, NodeType.Or),
            new Dependency("if", "Keyword_if", TokenType.Keyword_if, false, false, false, -1, NodeType.If),
            new Dependency("else", "Keyword_else", TokenType.Keyword_else, false, false, false, -1, NodeType.None),
            new Dependency("while", "Keyword_while", TokenType.Keyword_while, false, false, false, -1, NodeType.While),
            new Dependency("print", "Keyword_print", TokenType.Keyword_print, false, false, false, -1, NodeType.None),
            new Dependency("putc", "Keyword_putc", TokenType.Keyword_putc, false, false, false, -1, NodeType.None),
            new Dependency("(", "LeftParen", TokenType.LeftParen, false, false, false, -1, NodeType.None),
            new Dependency(")", "RightParen", TokenType.RightParen, false, false, false, -1, NodeType.None),
            new Dependency("{", "LeftBrace", TokenType.LeftBrace, false, false, false, -1, NodeType.None),
            new Dependency("}", "RightBrace", TokenType.RightBrace, false, false, false, -1, NodeType.None),
            new Dependency(";", "Semicolon", TokenType.Semicolon, false, false, false, -1, NodeType.None),
            new Dependency(",", "Comma", TokenType.Comma, false, false, false, -1, NodeType.None),
            new Dependency("Ident", "Identifier", TokenType.Identifier, false, false, false, -1, NodeType.Ident),
            new Dependency("Integer literal", "Integer", TokenType.Integer, false, false, false, -1,NodeType.Integer),
            new Dependency("String literal", "String", TokenType.String, false, false, false, -1, NodeType.String)
        };

        public static TokenType TypeByName(string name)
        {
            // return internal version of name

            return atr.First(n => n.enum_text == name).tok;
        }

        public static Dependency DependencyByType(TokenType type)
        {
            return atr.FirstOrDefault(n => n.tok == type);
        }

        public override string ToString()
        {
            return Type == TokenType.Integer || 
                   Type == TokenType.Identifier ||
                   Type == TokenType.Include || 
                   Type == TokenType.Char ||
                   Type == TokenType.Float
                ? $"{Line,-5}  {Position,-5}   {Type.ToString(),-14}     {Value}"
                : (Type == TokenType.String
                    ? $"{Line,-5}  {Position,-5}   {Type.ToString(),-14}     \"{Value.Replace("\n", "\\n")}\""
                    : $"{Line,-5}  {Position,-5}   {Type.ToString(),-14}");
        }
    }
}