using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Compiler.Syntaxing
{
    public partial class Syntaxer
    {
        

        /// <summary>
        /// Proccessing stack of tokens
        /// </summary>
        private Stack<Token> Tokens { get; set; }
        

        /// <summary>
        /// Current token in proccessing
        /// </summary>
        private Token Current { get; set; }

        /// <summary>
        /// Make syntaxis analyze of input string from lexer
        /// </summary>
        /// <param name="input">string from lexer</param>
        /// <returns>AST tree in string</returns>
        public string MakeAnalyze(string input)
        {
            return ParseTokens(StringToTokensStack(input)).ToString();
        }

        /// <summary>
        /// Parse input token Stack to AST tree
        /// </summary>
        /// <param name="tokens">input token, where head is first lexical token</param>
        /// <returns></returns>
        private Tree ParseTokens(Stack<Token> tokens)
        {
            Tree tree = null;
            Tokens = tokens;
            Current = tokens.Pop();
            do   
                tree = new Tree(NodeType.Sequence, tree, Statement());
            while (tree != null && Current.Type != TokenType.EOI && tokens.Count != 0);

            Tokens = null;
            Current = null;

            return tree;
        }
       
        
        /// <summary>
        /// Parse Expression
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Tree Expression(int p)
        {
            Tree x = null, node;
            TokenType op;

            switch (Current.Type)
            {
                case TokenType.Lparen:
                    x = ParentExpression();
                    break;
                case TokenType.Sub:
                case TokenType.Add:
                    op = Current.Type;
                    Current = Tokens.Pop();
                    node = Expression(Token.DependencyByType(TokenType.Negate).precedence);
                    x = (op == TokenType.Sub) ? new Tree(NodeType.Negate, node, null) : node;
                    break;
                case TokenType.Not:
                    Current = Tokens.Pop();
                    x = new Tree(NodeType.Not, Expression(Token.DependencyByType(TokenType.Not).precedence), null);
                    break;
                case TokenType.Ident:
                    x = new Tree(NodeType.Ident, Current.Value);
                    Current = Tokens.Pop();
                    break;
                case TokenType.Integer:
                    x = new Tree(NodeType.Integer, Current.Value);
                    Current = Tokens.Pop();
                    break;
                    //default:
                    //error(Type.Line, Type.Position, "Expecting a primary, found: %tokenType\n", atr[Type.Type].Value);
            }

            //todo dictionary
            Dependency current = Token.DependencyByType(Current.Type);
            while (current.is_binary && current.precedence >= p)
            {
                Current = Tokens.Pop();

                int q = current.precedence;
                if (!current.right_associative)
                    q++;

                node = Expression(q);
                x = new Tree(current.node_type, x, node);

                current = Token.DependencyByType(Current.Type);

            }

            return x;
        }
        
        
        /// <summary>
        /// Parse statement
        /// </summary>
        /// <returns></returns>
        private Tree Statement()
        {
            Tree tree = null, v, e, s, s2;

            switch (Current.Type)
            {
                case TokenType.If:
                    Current = Tokens.Pop();
                    e = ParentExpression();
                    s = Statement();
                    s2 = null;
                    if (Current.Type == TokenType.Else)
                    {
                        Current = Tokens.Pop();
                        s2 = Statement();
                    }

                    tree = new Tree(NodeType.If, e, new Tree(NodeType.If, s, s2));
                    break;
                case TokenType.Putc:
                    Current = Tokens.Pop();
                    e = ParentExpression();
                    tree = new Tree(NodeType.Prtc, e, null);
                    IsExpected("Putc", TokenType.Semi);
                    break;
                case TokenType.Print: /* print '(' Expression {',' Expression} ')' */
                    Current = Tokens.Pop();
                    for (IsExpected("Print", TokenType.Lparen); ; IsExpected("Print", TokenType.Comma))
                    {
                        if (Current.Type == TokenType.String)
                        {
                            e = new Tree(NodeType.Prts, new Tree(NodeType.String, Current.Value), null);
                            Current = Tokens.Pop();
                        }
                        else
                            e = new Tree(NodeType.Prti, Expression(0), null);

                        tree = new Tree(NodeType.Sequence, tree, e);

                        if (Current.Type != TokenType.Comma)
                            break;
                    }

                    IsExpected("Print", TokenType.Rparen);
                    IsExpected("Print", TokenType.Semi);
                    break;
                case TokenType.Semi:
                    Current = Tokens.Pop();
                    break;
                case TokenType.Ident:
                    v = new Tree(NodeType.Ident, Current.Value);
                    Current = Tokens.Pop();
                    IsExpected("assign", TokenType.Assign);
                    e = Expression(0);
                    tree = new Tree(NodeType.Assign, v, e);
                    IsExpected("assign", TokenType.Semi);
                    break;
                case TokenType.While:
                    Current = Tokens.Pop();
                    e = ParentExpression();
                    s = Statement();
                    tree = new Tree(NodeType.While, e, s);
                    break;
                case TokenType.Lbrace: /* {Statement} */
                    for (IsExpected("Lbrace", TokenType.Lbrace);
                        Current.Type != TokenType.Rbrace && Current.Type != TokenType.EOI;)
                        tree = new Tree(NodeType.Sequence, tree, Statement());
                    IsExpected("Lbrace", TokenType.Rbrace);
                    break;
                case TokenType.EOI:
                    break;
                    // default(): error(Type.Line, Type.Position, "expecting start of statement, found '%tokenType'\n", atr[Type.Type].Value);
            }

            return tree;
        }


        /// <summary>
        /// Parse expression in parantesise
        /// </summary>
        /// <returns></returns>
        private Tree ParentExpression()
        {
            IsExpected("ParentExpression", TokenType.Lparen);
            Tree t = Expression(0);
            IsExpected("ParentExpression", TokenType.Rparen);
            return t;
        }


        /// <summary>
        /// Is Curent Token of type tokenType 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="tokenType">Token type</param>
        private void IsExpected(string msg, TokenType tokenType)
        {
            if (Current.Type == tokenType)
            {
                Current = Tokens.Pop();
                return;
            }
             
            //todo error(Type.Line, Type.Position, "%tokenType: Expecting '%tokenType', found '%tokenType'\n", msg, atr[tokenType].Value, atr[Type.Type].Value);
        }


        /// <summary>
        /// Parse string output from Lexer to Tokens Stack
        /// </summary>
        /// <param name="input">Massive</param>
        /// <returns>Stack, where the first line is head, aka reverse input</returns>
        private Stack<Token> StringToTokensStack(string input)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            string[] lines = input.Split('\n');
            Stack<Token> tokens = new Stack<Token>(lines.Length);

            for (int i = lines.Length - 1; i >= 0; i--)
            {
                try
                {
                    string[] token_parts = regex.Replace(lines[i].Trim(), " ").Split();
                    Token token = new Token(Token.TypeByName(token_parts[2]), int.Parse(token_parts[0]), int.Parse(token_parts[1]),
                        token_parts.Length == 4 ? token_parts[3] : null);

                    Tokens.Push(token);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Problem with input Current line " + i + " ! \n" + e);
                }

            }
            return tokens;
        }


    }
}