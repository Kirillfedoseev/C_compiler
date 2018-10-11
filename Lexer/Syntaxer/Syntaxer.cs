using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Compiler.Syntaxing
{
    public class Syntaxer
    {
        public String MakeAnalyze(String input)
        {
            tokens = new Stack<string>(input.Split('\n').Reverse());
            
            return prt_ast(parse());
        }

        public enum TokenType
        {
            tk_EOI,
            tk_Mul,
            tk_Div,
            tk_Mod,
            tk_Add,
            tk_Sub,
            tk_Negate,
            tk_Not,
            tk_Lss,
            tk_Leq,
            tk_Gtr,
            tk_Geq,
            tk_Eql,
            tk_Neq,
            tk_Assign,
            tk_And,
            tk_Or,
            tk_If,
            tk_Else,
            tk_While,
            tk_Print,
            tk_Putc,
            tk_Lparen,
            tk_Rparen,
            tk_Lbrace,
            tk_Rbrace,
            tk_Semi,
            tk_Comma,
            tk_Ident,
            tk_Integer,
            tk_String
        };

        public enum NodeType
        {
            None,
            nd_Ident,
            nd_String,
            nd_Integer,
            nd_Sequence,
            nd_If,
            nd_Prtc,
            nd_Prts,
            nd_Prti,
            nd_While,
            nd_Assign,
            nd_Negate,
            nd_Not,
            nd_Mul,
            nd_Div,
            nd_Mod,
            nd_Add,
            nd_Sub,
            nd_Lss,
            nd_Leq,
            nd_Gtr,
            nd_Geq,
            nd_Eql,
            nd_Neq,
            nd_And,
            nd_Or
        }


        public class tok_s
        {
            public TokenType tok;
            public int err_ln;
            public int err_col;
            public string text; /* ident or string literal or integer value */
        }


        class Tree
        {
            public NodeType node_type;
            public Tree left;
            public Tree right;
            public string value;

            //leaf
            public Tree(NodeType node_type, string value)
            {
                this.node_type = node_type;
                this.value = value;
            }

            public Tree(NodeType node_type, Tree left, Tree right)
            {
                this.node_type = node_type;
                this.left = left;
                this.right = right;
            }

        }


        // dependency: Ordered by tok, must remain in same order as TokenType enum
        public class dependency
        {
            public string text, enum_text;
            public TokenType tok;
            public bool right_associative, is_binary, is_unary;
            public int precedence;
            public NodeType node_type;

            public dependency(string text, string enum_text, TokenType tok, bool right_associative, bool is_binary,
                bool is_unary, int precedence, NodeType node_type)
            {
                this.text = text;
                this.enum_text = enum_text;
                this.tok = tok;
                this.right_associative = right_associative;
                this.is_binary = is_binary;
                this.is_unary = is_unary;
                this.precedence = precedence;
                this.node_type = node_type;
            }
        }

        dependency[] atr =
        {
            new dependency("EOI", "End_of_input", TokenType.tk_EOI, false, false, false, -1, NodeType.None),
            new dependency("*", "Op_multiply", TokenType.tk_Mul, false, true, false, 13, NodeType.nd_Mul),
            new dependency("/", "Op_divide", TokenType.tk_Div, false, true, false, 13, NodeType.nd_Div),
            new dependency("%", "Op_mod", TokenType.tk_Mod, false, true, false, 13, NodeType.nd_Mod),
            new dependency("+", "Op_add", TokenType.tk_Add, false, true, false, 12, NodeType.nd_Add),
            new dependency("-", "Op_subtract", TokenType.tk_Sub, false, true, false, 12, NodeType.nd_Sub),
            new dependency("-", "Op_negate", TokenType.tk_Negate, false, false, true, 14, NodeType.nd_Negate),
            new dependency("!", "Op_not", TokenType.tk_Not, false, false, true, 14, NodeType.nd_Not),
            new dependency("<", "Op_less", TokenType.tk_Lss, false, true, false, 10, NodeType.nd_Lss),
            new dependency("<=", "Op_lessequal", TokenType.tk_Leq, false, true, false, 10, NodeType.nd_Leq),
            new dependency(">", "Op_greater", TokenType.tk_Gtr, false, true, false, 10, NodeType.nd_Gtr),
            new dependency(">=", "Op_greaterequal", TokenType.tk_Geq, false, true, false, 10, NodeType.nd_Geq),
            new dependency("==", "Op_equal", TokenType.tk_Eql, false, true, false, 9, NodeType.nd_Eql),
            new dependency("!=", "Op_notequal", TokenType.tk_Neq, false, true, false, 9, NodeType.nd_Neq),
            new dependency("=", "Op_assign", TokenType.tk_Assign, false, false, false, -1, NodeType.nd_Assign),
            new dependency("&&", "Op_and", TokenType.tk_And, false, true, false, 5, NodeType.nd_And),
            new dependency("||", "Op_or", TokenType.tk_Or, false, true, false, 4, NodeType.nd_Or),
            new dependency("if", "Keyword_if", TokenType.tk_If, false, false, false, -1, NodeType.nd_If),
            new dependency("else", "Keyword_else", TokenType.tk_Else, false, false, false, -1, NodeType.None),
            new dependency("while", "Keyword_while", TokenType.tk_While, false, false, false, -1, NodeType.nd_While),
            new dependency("print", "Keyword_print", TokenType.tk_Print, false, false, false, -1, NodeType.None),
            new dependency("putc", "Keyword_putc", TokenType.tk_Putc, false, false, false, -1, NodeType.None),
            new dependency("(", "LeftParen", TokenType.tk_Lparen, false, false, false, -1, NodeType.None),
            new dependency(")", "RightParen", TokenType.tk_Rparen, false, false, false, -1, NodeType.None),
            new dependency("{", "LeftBrace", TokenType.tk_Lbrace, false, false, false, -1, NodeType.None),
            new dependency("}", "RightBrace", TokenType.tk_Rbrace, false, false, false, -1, NodeType.None),
            new dependency(";", "Semicolon", TokenType.tk_Semi, false, false, false, -1, NodeType.None),
            new dependency(",", "Comma", TokenType.tk_Comma, false, false, false, -1, NodeType.None),
            new dependency("Ident", "Identifier", TokenType.tk_Ident, false, false, false, -1, NodeType.nd_Ident),
            new dependency("Integer literal", "Integer", TokenType.tk_Integer, false, false, false, -1,
                NodeType.nd_Integer),
            new dependency("String literal", "String", TokenType.tk_String, false, false, false, -1, NodeType.nd_String)
        };

        string[] Display_nodes =
        {
            "Identifier", "String", "Integer", "Sequence", "If", "Prtc",
            "Prts", "Prti", "While", "Assign", "Negate", "Not", "Multiply", "Divide", "Mod",
            "Add", "Subtract", "Less", "LessEqual", "Greater", "GreaterEqual", "Equal",
            "NotEqual", "And", "Or"
        };

        static tok_s tok;
 
        TokenType get_enum(string name)
        {
            // return internal version of name

            return atr.First(n => n.enum_text == name).tok;

        }

        private Stack<string> tokens;


        tok_s gettok()
        {
            tok_s tok = new tok_s();
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);

            string[] token_parts = regex.Replace(tokens.Pop().Trim(), " ").Split();

            tok.err_ln = Convert.ToInt32(token_parts[0]);
            tok.err_col = Convert.ToInt32(token_parts[1]);
            tok.tok = get_enum(token_parts[2]);
            tok.text = token_parts.Length == 4 ? token_parts[3] : null;

            return tok;
        }


        void expect(string msg, TokenType s)
        {
            if (tok.tok == s)
            {
                tok = gettok();
                return;
            }

            //todo error(tok.err_ln, tok.err_col, "%s: Expecting '%s', found '%s'\n", msg, atr[s].text, atr[tok.tok].text);
        }

        Tree expr(int p)
        {
            Tree x = null, node;
            TokenType op;

            switch (tok.tok)
            {
                case TokenType.tk_Lparen:
                    x = paren_expr();
                    break;
                case TokenType.tk_Sub:
                case TokenType.tk_Add:
                    op = tok.tok;
                    tok = gettok();
                    node = expr(atr.First(n => n.tok == TokenType.tk_Negate).precedence);
                    x = (op == TokenType.tk_Sub) ? new Tree(NodeType.nd_Negate, node, null) : node;
                    break;
                case TokenType.tk_Not:
                    tok = gettok();
                    x = new Tree(NodeType.nd_Not, expr(atr.First(n => n.tok == TokenType.tk_Not).precedence), null);
                    break;
                case TokenType.tk_Ident:
                    x = new Tree(NodeType.nd_Ident, tok.text);
                    tok = gettok();
                    break;
                case TokenType.tk_Integer:
                    x = new Tree(NodeType.nd_Integer, tok.text);
                    tok = gettok();
                    break;
                //default:
                //error(tok.err_ln, tok.err_col, "Expecting a primary, found: %s\n", atr[tok.tok].text);
            }

            //todo dictionary
            dependency current = atr.First(n => n.tok == tok.tok);
            while (current.is_binary && current.precedence >= p)
            {
                tok = gettok();

                int q = current.precedence;
                if (!current.right_associative)
                    q++;

                node = expr(q);
                x = new Tree(current.node_type, x, node);

                current = atr.First(n => n.tok == tok.tok);

            }

            return x;
        }

        Tree paren_expr()
        {
            expect("paren_expr", TokenType.tk_Lparen);
            Tree t = expr(0);
            expect("paren_expr", TokenType.tk_Rparen);
            return t;
        }

        Tree stmt()
        {
            Tree t = null, v, e, s, s2;

            switch (tok.tok)
            {
                case TokenType.tk_If:
                    tok = gettok();
                    e = paren_expr();
                    s = stmt();
                    s2 = null;
                    if (tok.tok == TokenType.tk_Else)
                    {
                        tok = gettok();
                        s2 = stmt();
                    }

                    t = new Tree(NodeType.nd_If, e, new Tree(NodeType.nd_If, s, s2));
                    break;
                case TokenType.tk_Putc:
                    tok = gettok();
                    e = paren_expr();
                    t = new Tree(NodeType.nd_Prtc, e, null);
                    expect("Putc", TokenType.tk_Semi);
                    break;
                case TokenType.tk_Print: /* print '(' expr {',' expr} ')' */
                    tok = gettok();
                    for (expect("Print", TokenType.tk_Lparen);; expect("Print", TokenType.tk_Comma))
                    {
                        if (tok.tok == TokenType.tk_String)
                        {
                            e = new Tree(NodeType.nd_Prts, new Tree(NodeType.nd_String, tok.text), null);
                            tok = gettok();
                        }
                        else
                            e = new Tree(NodeType.nd_Prti, expr(0), null);

                        t = new Tree(NodeType.nd_Sequence, t, e);

                        if (tok.tok != TokenType.tk_Comma)
                            break;
                    }

                    expect("Print", TokenType.tk_Rparen);
                    expect("Print", TokenType.tk_Semi);
                    break;
                case TokenType.tk_Semi:
                    tok = gettok();
                    break;
                case TokenType.tk_Ident:
                    v = new Tree(NodeType.nd_Ident, tok.text);
                    tok = gettok();
                    expect("assign", TokenType.tk_Assign);
                    e = expr(0);
                    t = new Tree(NodeType.nd_Assign, v, e);
                    expect("assign", TokenType.tk_Semi);
                    break;
                case TokenType.tk_While:
                    tok = gettok();
                    e = paren_expr();
                    s = stmt();
                    t = new Tree(NodeType.nd_While, e, s);
                    break;
                case TokenType.tk_Lbrace: /* {stmt} */
                    for (expect("Lbrace", TokenType.tk_Lbrace);
                        tok.tok != TokenType.tk_Rbrace && tok.tok != TokenType.tk_EOI;)
                        t = new Tree(NodeType.nd_Sequence, t, stmt());
                    expect("Lbrace", TokenType.tk_Rbrace);
                    break;
                case TokenType.tk_EOI:
                    break;                   
               // default(): error(tok.err_ln, tok.err_col, "expecting start of statement, found '%s'\n", atr[tok.tok].text);
            }

            return t;
        }

        Tree parse()
        {
            Tree t = null;

            tok = gettok();
            do
            {
                t = new Tree(NodeType.nd_Sequence, t, stmt());
            } while (t != null && tok.tok != TokenType.tk_EOI && tokens.Count != 0);

            return t;
        }

        string prt_ast(Tree t)
        {
            StringBuilder sb = new StringBuilder("");
            if (t == null)
                sb.Append(";\n");
            else
            {
                sb.Append(String.Format(t.node_type.ToString(), "%-14s "));
                if (t.node_type == NodeType.nd_Ident || t.node_type == NodeType.nd_Integer ||
                    t.node_type == NodeType.nd_String)
                {
                    sb.Append(t.value + "\n");
                }
                else
                {
                    sb.Append("\n");
                    sb.Append(prt_ast(t.left));
                    sb.Append(prt_ast(t.right));
                }
            }

            return sb.ToString();
        }



    }
}