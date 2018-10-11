using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Compiler
{

    /// <summary>
    /// Lexer for C
    /// </summary>
    public class Lexer
    {
        #region Constants
        // character classes 
        private const string _letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";

        private const string _numbers = "0123456789";

        private const string _identifier = _letters + _numbers;

        private const string _whitespace = " \t\n\r";

        // mappings from string keywords to token type 
        private readonly Dictionary<string, TokenType> _keywordTokenTypeMap = new Dictionary<string, TokenType>()
        {
            {"if", TokenType.Keyword_if},
            {"else", TokenType.Keyword_else},
            { "switch", TokenType.Keyword_switch},
            {"case", TokenType.Keyword_case},

            { "do", TokenType.Keyword_do},
            {"while", TokenType.Keyword_while},
            { "for", TokenType.Keyword_for},

            { "return", TokenType.Keyword_return},
            {"continue", TokenType.Keyword_continue},
            {"goto", TokenType.Keyword_goto},
            {"break", TokenType.Keyword_break},

            {"print", TokenType.Keyword_print},
            {"putc", TokenType.Keyword_putc},
            {"#include", TokenType.Include},
            {"auto", TokenType.Keyword_auto},

            { "double", TokenType.Keyword_double},
            { "int", TokenType.Keyword_int},
            { "long", TokenType.Keyword_long},
            { "char", TokenType.Keyword_char},
            { "float", TokenType.Keyword_float},
            { "short", TokenType.Keyword_short},

            {"struct", TokenType.Keyword_struct},
            {"enum", TokenType.Keyword_enum},
            {"register", TokenType.Keyword_register},
            {"typedef", TokenType.Keyword_typedef},

            {"extern", TokenType.Keyword_extern},
            {"void", TokenType.Keyword_void},
            {"static", TokenType.Keyword_static},

            {"volatile", TokenType.Keyword_volatile},
            {"const", TokenType.Keyword_const},
            {"unsigned", TokenType.Keyword_unsigned},
            {"signed", TokenType.Keyword_signed},
            {"sizeof", TokenType.Keyword_sizeof},
            {"union", TokenType.Keyword_union},
            {"default", TokenType.Keyword_default},


        };

        // mappings from simple operators to token type
        private readonly Dictionary<string, TokenType> _oneOperatorsMap = new Dictionary<string, TokenType>()
        {
            {"+", TokenType.Op_add},
            {"-", TokenType.Op_subtract},
            {"*", TokenType.Op_multiply},
            {"/", TokenType.Op_divide},
            {"%", TokenType.Op_mod},
            {"=", TokenType.Op_assign},
            {"<", TokenType.Op_less},
            {">", TokenType.Op_greater},
            {"!", TokenType.Op_not},
            {"|", TokenType.Op_bit_or},
            {"~", TokenType.Op_bit_not},
            {"^", TokenType.Op_bit_xor},       
            {"&", TokenType.Op_bit_and},
        };

        private readonly Dictionary<string, TokenType> _twoOperatorsMap = new Dictionary<string, TokenType>()
        {
            {"<=", TokenType.Op_lessequal},
            {">=", TokenType.Op_greaterequal},
            {"==", TokenType.Op_equal},
            {"!=", TokenType.Op_notequal},
            {"&&", TokenType.Op_and},
            {"||", TokenType.Op_or},       
            {"<<", TokenType.Op_bit_left_shift},
            {">>", TokenType.Op_bit_right_shift},
            {"+=", TokenType.Op_add_assign},
            {"-=", TokenType.Op_sub_assign},
            {"*=", TokenType.Op_multiply_assign},
            {"&=", TokenType.Op_bit_and_assign},
            {"|=", TokenType.Op_bit_or_assign},
            {"^=", TokenType.Op_bit_xor_assign},
            {"<<=", TokenType.Op_bit_left_shift_assign},
            {">>=", TokenType.Op_bit_right_shift_assign},
        };

        private readonly Dictionary<string, TokenType> _threeOperatorsMap = new Dictionary<string, TokenType>()
        {            
            {"<<=", TokenType.Op_bit_left_shift_assign},
            {">>=", TokenType.Op_bit_right_shift_assign},
        };

        private readonly Dictionary<string, TokenType> _oneOperatorsPreIdMap = new Dictionary<string, TokenType>()
        {
            {"*", TokenType.Op_pointer},
            {"&", TokenType.Op_reference},
        };

        private readonly Dictionary<string, TokenType> _twoOperatorsPreIdMap = new Dictionary<string, TokenType>()
        {
            {"++", TokenType.Op_suf_inc},
            {"--", TokenType.Op_suf_dec},
        };

        private readonly Dictionary<string, TokenType> _twoOperatorsPostIdMap = new Dictionary<string, TokenType>()
        {
            {"++", TokenType.Op_pre_inc},
            {"--", TokenType.Op_pre_dec},

        };

        // mappings from simple operators to token type
        private readonly Dictionary<string, TokenType> _delimitersMap = new Dictionary<string, TokenType>()
        {
            {"(", TokenType.LeftParen},
            {")", TokenType.RightParen},
            {"{", TokenType.LeftBrace},
            {"}", TokenType.RightBrace},
            {";", TokenType.Semicolon},
            {",", TokenType.Comma},
            {".", TokenType.Op_struct_reference},
            {"[", TokenType.LeftSquareBracket},
            {"]", TokenType.RightSquareBracket},
        };



        private int _line = 1;
        private int _position = 1;
        private string _input;

        public string CurrentCharacter
        {
            get
            {
                try
                {
                    return _input.Substring(0, 1);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return "";
                }
            }
        }

        public List<Token> Tokens { get; } = new List<Token>();
        
        #endregion 

        public String MakeAnalyze(String input)
        {
            List<Token> tokens = Scan(input);
            StringBuilder sb = new StringBuilder();
            foreach (Token token in tokens)
                sb.Append(token + "\n");
            return sb.ToString();
        }


        
        /// <summary>
        /// Pattern matching using first & follow matching
        /// </summary>
        /// <param name="recogniseClass">String of step that identifies the token type
        /// or the exact match the be made if exact:true</param>
        /// <param name="matchClass">String of step to match against remaining target step</param>
        /// <param name="tokenType">Type of token the match represents.</param>
        /// <param name="notNextClass">Optional class of step that cannot follow the match</param>
        /// <param name="maxLen">Optional maximum length of token value</param>
        /// <param name="exact">Denotes whether recogniseClass represents an exact match or class match. 
        /// Default: false</param>
        /// <param name="discard">Denotes whether the token is kept or discarded. Default: false</param>
        /// <param name="offset">Optiona line position offset to account for discarded _tokens</param>
        /// <returns>Boolean indicating if a match was made </returns>
        private bool Match(string recogniseClass, string matchClass, TokenType tokenType,
            string notNextClass = null, int maxLen = Int32.MaxValue, bool exact = false,
            bool discard = false, int offset = 0)
        {

            // if we've hit the end of the file, there's no more matching to be done
            if (CurrentCharacter == "") return false;

            // store _current_ line and position so that our vectors point at the start of each token
            int line = _line;
            int position = _position;

            // special case exact _tokens to avoid needing to worry about backtracking
            if (exact)
            {
                if (!_input.StartsWith(recogniseClass)) return false;

                if (!discard) Tokens.Add(new Token(tokenType, line, position - offset, recogniseClass));
                MoveNext(recogniseClass.Length);
                return true;
            }

            // first match - denotes the token type usually
            if (!recogniseClass.Contains(CurrentCharacter)) return false;

            string tokenValue = CurrentCharacter;
            MoveNext();

            // follow match while we haven't exceeded maxLen and there are still step
            // in the code stream
            while ((matchClass ?? "").Contains(CurrentCharacter) && tokenValue.Length <= maxLen &&
                   CurrentCharacter != "")
            {
                tokenValue += CurrentCharacter;
                MoveNext();
            }

            // ensure that any incompatible step are not next to the token
            // eg 42fred is invalid, and neither recognized as a number nor an identifier.
            // _letters would be the notNextClass
            if (notNextClass != null && notNextClass.Contains(CurrentCharacter))
                Error("Unrecognised character: " + CurrentCharacter, _line, _position);

            // only add _tokens to the stack that aren't marked as discard - don't want
            // things like open and close quotes/comments

            if (discard) return true;

            Token token = new Token(tokenType, line, position - offset, tokenValue);
            Tokens.Add(token);

            return true;
        }

        /// <summary>
        /// Tokenise the input code 
        /// </summary>
        /// <returns>List of Tokens</returns>
        private List<Token> Scan(string input)
        {
            _input = input;
            int pos = _position;
            int line = _line;
            while (CurrentCharacter != (""))
            {
                // match whitespace
                Match(_whitespace, _whitespace, TokenType.None, discard: true);

                // match integers
                MatchNumber();

                MatchIncludes();

                MatchKeysAndIds();

                MatchStrings();

                MatchCharacter();

                if (MatchComments()) continue;


                MatchSet(_threeOperatorsMap,3);

                MatchSet(_twoOperatorsMap, 2);
                MatchSetPreId(_twoOperatorsPreIdMap, 2);

                MatchSet(_oneOperatorsMap, 1);

                MatchSet(_delimitersMap);

                if (pos == _position && line == _line)
                {
                    Error("Unresolved symbol: " + CurrentCharacter,_line,_position);
                    MoveNext();
                }
            }

            // end of file token
            Tokens.Add(new Token(TokenType.End_of_input, _line + 1, 1));

            return Tokens;
        }


        /// <summary>
        /// Match identifiers and keywords
        /// </summary>
        private void MatchKeysAndIds()
        {
            if (!Match(_letters, _identifier, TokenType.Identifier)) return;
            Token match = Tokens.Last();
            if (_keywordTokenTypeMap.ContainsKey(match.Value))
                match.Type = _keywordTokenTypeMap[match.Value];
            else CheckAfterId();
        }


        /// <summary>
        /// match string similarly to comments without allowing newlines
        /// this token doesn't get discarded though
        /// </summary>
        private void MatchStrings()
        {

            if (!Match("\"", null, TokenType.String, discard: true)) return;
            string value = "";
            int position = _position;
            while (!Match("\"", null, TokenType.String, discard: true))
            {
                // not allowed newlines in strings
                if (CurrentCharacter == "\n")
                    Error(
                        "End-of-line while scanning string literal. Closing string character not found before end-of-line",
                        _line, _position);
                // end of file reached before finding end of string
                if (CurrentCharacter == "")
                    Error("End-of-file while scanning string literal. Closing string character not found",
                        _line, _position);

                value += CurrentCharacter;

                // deal with escape sequences - we only accept newline (\n)
                if (value.Length >= 2)
                {
                    string lastCharacters = value.Substring(value.Length - 2, 2);
                    if (lastCharacters[0] == '\\')
                        if (lastCharacters[1] == 'n') value = value.Substring(0, value.Length - 2) + "\n";                 
                }
                MoveNext();
            }

            Tokens.Add(new Token(TokenType.String, _line, position - 1, value));
        }

        /// <summary>
        /// Match includes in the code
        /// </summary>
        private void MatchIncludes()
        {
            Regex regex = new Regex("^#include[\\s]+<[\\S]+>");
            Match match = regex.Match(_input);
            if (!match.Success) return;

            string s = match.Value;
            regex = new Regex("<[\\S]+>");
            match = regex.Match(s);
            
            Tokens.Add(new Token(TokenType.Include,_line,_position, match.Value.Substring(1, match.Value.Length - 2)));
            MoveNext(s.Length);
        }

        /// <summary>
        /// match string numbers
        /// </summary>
        private void MatchNumber()
        {

            Regex regex_float = new Regex("^[0-9]*[.][0-9]+");
            Regex regex_int = new Regex("^[0-9][0-9]*");

            Match match = regex_float.Match(_input);
            string s = match.Value;
            if (match.Success)
            {
                Tokens.Add(new Token(TokenType.Float, _line, _position, s));
                MoveNext(s.Length);
                return;
            }

            match = regex_int.Match(_input);
            s = match.Value;

            if (match.Success)
            {
                Tokens.Add(new Token(TokenType.Integer, _line, _position, s));
                MoveNext(s.Length);
                return;
            }


        }


        /// <summary>
        /// match string literals
        /// </summary>
        private void MatchCharacter()
        {
            Regex regex = new Regex("^\'[\\S|\\s]\'|^(\'\\\\n\')|^(\'\\\\\\\\\')");    
            Match match = regex.Match(_input);
            if (!match.Success) return;

            string s = match.Value;
            Tokens.Add(new Token(TokenType.Char, _line, _position, s));
            MoveNext(s.Length);   
            
        }


        /// <summary>
        /// match comments by checking for starting token, then advancing 
        /// until closing token is matched
        /// </summary>
        private bool MatchComments()
        { 
            Regex regex = new Regex(@"^(\/\/[^\n]*\n)|^(\/\*(.|[\n])*?\*\/)");
            Match match = regex.Match(_input);
            if (!match.Success) return false;

            string s = match.Value;
            MoveNext(s.Length);

            return true;
        }


        /// <summary>
        /// Match set of exact tokens, like operators, keywords, or delimeters
        /// the main properties are fixed length and special meaning in C (keywords)
        /// </summary>
        private void MatchSet(Dictionary<string, TokenType> operatorsMap,int maxLen=Int32.MaxValue)
        {
            foreach (KeyValuePair<string, TokenType> pair in operatorsMap)
                Match(pair.Key, null, pair.Value, exact: true, maxLen: maxLen);
        }

        /// <summary>
        /// Match set of difficult operator which must be after Ids
        /// </summary>
        private void MatchSetPreId(Dictionary<string, TokenType> operatorsMap, int maxLen = Int32.MaxValue)
        {
            foreach (KeyValuePair<string, TokenType> pair in operatorsMap)
                Match(pair.Key, null, pair.Value, exact: true, maxLen: maxLen);
        }

        /// <summary>
        /// Check on difficult operators which must be before Ids
        /// </summary>
        private void CheckAfterId()
        {
            int count = Tokens.Count;
            if (count >= 2 && Tokens.Last().Type == TokenType.Identifier &&
                (Tokens[count-1].Position - Tokens[count - 2].Position) == Tokens[count - 2].Value.Length) 
            {
                switch (Tokens[count - 2].Type)
                {
                    case TokenType.Op_multiply: Tokens[count - 2].Type = TokenType.Op_pointer;break;
                    case TokenType.Op_bit_and: Tokens[count - 2].Type = TokenType.Op_reference;break;
                }
            }

            if (count >= 2 && Tokens[count - 2].Type == TokenType.Op_suf_inc && Tokens[count - 1].Type == TokenType.Identifier && Tokens[count - 1].Position - Tokens[count - 2].Position == 2)
            {
                Tokens[count - 2].Type = TokenType.Op_pre_inc;
            }

            if (count >= 2 && Tokens[count - 2].Type == TokenType.Op_suf_dec && Tokens[count - 1].Type == TokenType.Identifier && Tokens[count - 1].Position - Tokens[count - 2].Position == 2)
            {
                Tokens[count - 2].Type = TokenType.Op_pre_dec;
            }
        }


        /// <summary>
        /// Advance the cursor forward given number of characters
        /// </summary>
        /// <param name="step">Value of step to MoveNext</param>
        private void MoveNext(int step = 1)
        {
            try
            {
                for (int i = 0; i < step; i++)
                {
                    // reset position when there is a newline
                    if (CurrentCharacter.Equals("\n"))
                    {
                        _position = 0;
                        _line++;
                    }

                    _input = _input.Remove(0,1);
                    _position ++;
                }
                
            }
            catch (ArgumentOutOfRangeException)
            {
                _input = "";
            }
        }


        /// <summary>
        /// Outputs Error message to the console and exits 
        /// </summary>
        /// <param name="message">Error message to display to user</param>
        /// <param name="line">Line Error occurred on</param>
        /// <param name="position">Line column that the Error occurred at</param>
        private void Error(string message, int line, int position)
        {
            // output Error to the console and exit
            Tokens.Add(new Token(TokenType.Error, line, position - 1, message));
        }



    }
}