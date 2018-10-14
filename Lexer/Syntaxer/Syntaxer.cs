using System.Collections.Generic;
using System.Linq;
using Compiler.Units;
using Compiler.Units.SimpleUnits;
using Newtonsoft.Json;

namespace Compiler.Syntaxing
{
    public class Syntaxer
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
        public string MakeAnalyze(List<Token> input)
        {
            input.Reverse();
            Tree tree = ParseTokens(new Stack<Token>(input));

            var jset = new JsonSerializerSettings();
            jset.Formatting = Formatting.Indented;
            return JsonConvert.SerializeObject(tree, jset);
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
            do
            {
                Current = tokens.Pop();
                tree = new Tree(NodeType.Sequence, tree, Statement());
            } while (tree != null && Current.Type != TokenType.End_of_input && tokens.Count != 0);

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
                case TokenType.LeftParen:
                    x = ParentExpression();
                    break;
                case TokenType.Op_subtract:
                case TokenType.Op_add:
                    op = Current.Type;
                    Current = Tokens.Pop();
                    node = Expression(Token.DependencyByType(TokenType.Op_negate).precedence);
                    x = (op == TokenType.Op_subtract) ? new Tree(NodeType.Negate, node, null) : node;
                    break;
                case TokenType.Op_not:
                    Current = Tokens.Pop();
                    x = new Tree(NodeType.Not, Expression(Token.DependencyByType(TokenType.Op_not).precedence), null);
                    break;
                case TokenType.Identifier:
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
                case TokenType.Keyword_if:
                    Current = Tokens.Pop();
                    e = ParentExpression();
                    s = Statement();
                    s2 = null;
                    if (Current.Type == TokenType.Keyword_else)
                    {
                        Current = Tokens.Pop();
                        s2 = Statement();
                    }

                    tree = new Tree(NodeType.If, e, new Tree(NodeType.If, s, s2));
                    break;
                case TokenType.Keyword_putc:
                    Current = Tokens.Pop();
                    e = ParentExpression();
                    tree = new Tree(NodeType.Prtc, e, null);
                    IsExpected("Putc", TokenType.Semicolon);
                    break;
                case TokenType.Keyword_print: /* print '(' Expression {',' Expression} ')' */
                    Current = Tokens.Pop();
                    for (IsExpected("Print", TokenType.LeftParen); ; IsExpected("Print", TokenType.Comma))
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

                    IsExpected("Print", TokenType.RightParen);
                    IsExpected("Print", TokenType.Semicolon);
                    break;
                case TokenType.Semicolon:
                    Current = Tokens.Pop();
                    break;
                case TokenType.Identifier:
                    v = new Tree(NodeType.Ident, Current.Value);
                    Current = Tokens.Pop();
                    IsExpected("assign", TokenType.Op_assign);
                    e = Expression(0);
                    tree = new Tree(NodeType.Assign, v, e);
                    IsExpected("assign", TokenType.Semicolon);
                    break;
                case TokenType.Keyword_while:
                    Current = Tokens.Pop();
                    e = ParentExpression();
                    s = Statement();
                    tree = new Tree(NodeType.While, e, s);
                    break;
                case TokenType.LeftBrace: /* {Statement} */
                    for (IsExpected("Lbrace", TokenType.LeftBrace);
                        Current.Type != TokenType.RightBrace && Current.Type != TokenType.End_of_input;)
                        tree = new Tree(NodeType.Sequence, tree, Statement());
                    IsExpected("Lbrace", TokenType.RightBrace);
                    break;
                case TokenType.End_of_input:
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
            IsExpected("ParentExpression", TokenType.LeftParen);
            Tree t = Expression(0);
            IsExpected("ParentExpression", TokenType.RightParen);
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
    }
}