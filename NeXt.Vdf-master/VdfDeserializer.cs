using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace NeXt.Vdf
{
    /// <summary>
    /// Deserializes Vdf formatted text into a VdfValue
    /// </summary>
    public class VdfDeserializer
    {
        /// <summary>
        /// Creates a VdfDeserializer object
        /// </summary>
        /// <param name="vdfText">the text to deserialize</param>
        private VdfDeserializer(string vdfText)
        {
            _vdfText = vdfText;
        }

        /// <summary>
        /// Creates a vdfdeserializer object
        /// </summary>
        /// <param name="filePath">path to the file to deserialize</param>
        /// <returns></returns>
        public static VdfDeserializer FromFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                return new VdfDeserializer(reader.ReadToEnd());
            }
        }

        private readonly string _vdfText;

        private CharacterType GetCharType(char c)
        {
            switch (c)
            {
                case '\n': return CharacterType.Newline;
                case '\r':
                case '\t':
                case ' ':
                {
                    return CharacterType.Whitespace;
                }

                case '{': return CharacterType.TableOpen;
                case '}': return CharacterType.TableClose;
                case '\\': return CharacterType.EscapeChar;
                case '/': return CharacterType.CommentDelimiter;
                case '"': return CharacterType.SequenceDelimiter;
                default: return CharacterType.Char;
            }
        }


        /// <summary>
        /// gets the character a single escape char represents
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private char GetUnescapedChar(char c)
        {
            switch (c)
            {
                case 'n': return '\n';
                case 't': return '\t';
                default: return c;
            }
        }

        /// <summary>
        /// returns the next full text (or as much as possible if incomplete)
        /// </summary>
        /// <param name="s">text to search in</param>
        /// <param name="startindex">first index to look at</param>
        /// <param name="endIndex">the index the returned string ended at</param>
        /// <param name="fullStop">true if the string was over, false if it was incomplete</param>
        /// <returns></returns>
        private string GetTextToDelimiter(string s, int startindex, out int endIndex, out bool fullStop)
        {
            fullStop = true;
            var openEscape = false;
            var escapedSequence = GetCharType(s[startindex]) == CharacterType.SequenceDelimiter;

            var sb = new StringBuilder();
            for (var i = startindex; i < s.Length; i++)
            {
                var c = s[i];
                switch (GetCharType(c))
                {
                    case CharacterType.SequenceDelimiter:
                    {
                        if (!openEscape && escapedSequence && i > startindex)
                        {
                            endIndex = i + 1;
                            return sb.ToString();
                        }

                        if (!escapedSequence)
                        {
                            throw new UnexpectedCharacterException("Non-Escape sequences cannot contain sequence delimiters", c);
                        }

                        if (openEscape)
                        {
                            sb.Append(GetUnescapedChar(c));
                            openEscape = false;
                        }

                        break;
                    }

                    case CharacterType.Whitespace:
                    {
                        if (escapedSequence)
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            endIndex = i;
                            return sb.ToString();
                        }

                        break;
                    }

                    case CharacterType.EscapeChar:
                    {
                        if (openEscape)
                        {
                            sb.Append(GetUnescapedChar(c));
                        }

                        openEscape = !openEscape;
                        break;
                    }

                    default:
                    {
                        if (openEscape)
                        {
                            sb.Append(GetUnescapedChar(c));
                            openEscape = false;
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        break;
                    }
                }
            }

            endIndex = s.Length;
            if (escapedSequence)
            {
                if (GetCharType(s[s.Length - 1]) != CharacterType.SequenceDelimiter)
                {
                    fullStop = false;
                }
                else
                {
                    var c = 0;
                    for (var i = s.Length - 2; i >= 0 && GetCharType(s[i]) == CharacterType.EscapeChar; i--)
                    {
                        c++;
                    }

                    fullStop = c % 2 == 0;
                }
            }

            return sb.ToString();
        }

        private Token _startedToken;
        private bool _unclosedLine;

        private void HandleUnclosedLine(Action<Token> callback, string line)
        {
            var text = GetTextToDelimiter("\"" + line, 0, out var endindex, out var isEnd);
            if (!isEnd)
            {
                _startedToken.Content += text;
                _unclosedLine = true;
            }
            else
            {
                _unclosedLine = false;
                callback(_startedToken);
                if (endindex < line.Length)
                {
                    HandleLine(callback, line.Substring(endindex).Trim());
                }
            }
        }

        private void HandleLine(Action<Token> callback, string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            var ct = GetCharType(line[0]);
            switch (ct)
            {
                case CharacterType.TableOpen:
                {
                    callback(new Token() {Type = TokenType.TableStart, Content = line[0].ToString()});
                    break;
                }

                case CharacterType.TableClose:
                {
                    callback(new Token() {Type = TokenType.TableEnd, Content = line[0].ToString()});
                    break;
                }

                case CharacterType.CommentDelimiter:
                {
                    if (line.Length < 2 || GetCharType(line[1]) != CharacterType.CommentDelimiter)
                    {
                        throw new UnexpectedCharacterException("Single comment delimiter is not allowed", line[0]);
                    }

                    callback(new Token {Type = TokenType.Comment, Content = line});
                    break;
                }

                default:
                {
                    var text = GetTextToDelimiter(line, 0, out var endindex, out var isEnd);
                    if (!isEnd)
                    {
                        _startedToken = new Token() {Type = TokenType.String, Content = text};
                        _unclosedLine = true;
                    }
                    else
                    {
                        callback(new Token
                        {
                            Type = TokenType.String, Content = text
                        });
                        if (endindex < line.Length)
                        {
                            HandleLine(callback, line.Substring(endindex).Trim());
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Tokenizes the VdfFormatted string into a list of tokens
        /// </summary>
        /// <param name="s">the string to tokenize</param>
        /// <returns>the token list</returns>
        private List<Token> Tokenize(string s)
        {
            var result = new List<Token>();

            var lines = s.Split('\n').Select((v) => v.Trim());

            foreach (var line in lines)
            {
                if (_unclosedLine)
                {
                    HandleUnclosedLine(result.Add, line);
                }
                else
                {
                    HandleLine(result.Add, line);
                }
            }

            return result;
        }

        /// <summary>
        /// Deserializes the Vdf string into a VdfValue
        /// </summary>
        /// <returns></returns>
        public VdfValue Deserialize()
        {
            if (_vdfText == null)
            {
                throw new ArgumentNullException();
            }

            if (_vdfText.Length < 1)
            {
                throw new ArgumentException("s cannot be empty ");
            }

            var tokens = Tokenize(_vdfText);

            if (tokens.Count < 1)
            {
                throw new ArgumentException("no tokens found in string");
            }

            VdfValue root = null;
            VdfTable current = null;
            var comments = new List<string>();

            string name = null;

            foreach (var token in tokens)
            {
                if (token.Type == TokenType.Comment)
                {
                    comments.Add(token.Content.Substring(2));
                    continue;
                }


                if (root == null)
                {
                    if (token.Type == TokenType.String)
                    {
                        if (name != null)
                        {
                            return new VdfString(name, token.Content);
                        }

                        name = token.Content;
                    }
                    else if (token.Type == TokenType.TableStart)
                    {
                        root = new VdfTable(name);
                        if (comments.Count > 0)
                        {
                            foreach (var comment in comments)
                            {
                                root.Comments.Add(comment);
                            }

                            comments.Clear();
                        }

                        current = root as VdfTable;
                        name = null;
                    }
                    else
                    {
                        throw new VdfDeserializationException("Invalid format: First token was not a string");
                    }

                    continue;
                }

                if (name != null)
                {
                    VdfValue v;
                    if (token.Type == TokenType.String)
                    {
                        if (int.TryParse(token.Content, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                        {
                            v = new VdfInteger(name, i);
                        }
                        else if (double.TryParse(token.Content, NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
                        {
                            v = new VdfDouble(name, d);
                        }
                        else
                        {
                            v = new VdfString(name, token.Content);
                        }

                        if (comments.Count > 0)
                        {
                            foreach (var comment in comments)
                            {
                                v.Comments.Add(comment);
                            }

                            comments.Clear();
                        }

                        name = null;
                        current.Add(v);
                    }
                    else if (token.Type == TokenType.TableStart)
                    {
                        v = new VdfTable(name);
                        if (comments.Count > 0)
                        {
                            foreach (var comment in comments)
                            {
                                v.Comments.Add(comment);
                            }

                            comments.Clear();
                        }

                        current.Add(v);
                        name = null;
                        current = v as VdfTable;
                    }
                }
                else
                {
                    if (token.Type == TokenType.String)
                    {
                        name = token.Content;
                    }
                    else if (token.Type == TokenType.TableEnd)
                    {
                        current = current.Parent as VdfTable;
                    }
                    else
                    {
                        throw new VdfDeserializationException("Invalid Format: a name was needed but not found");
                    }
                }
            }

            if (current != null)
            {
                throw new VdfDeserializationException("Invalid format: unclosed table");
            }

            return root;
        }
    }
}