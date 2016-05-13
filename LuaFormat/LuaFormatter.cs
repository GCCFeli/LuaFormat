using Irony.Parsing;
using Lua.LanguageService.Grammar;
using Lua.LanguageService.Grammar.Ast;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Lua.LanguageService.Format
{
    class LuaFormatter
    {
        public static LuaFormatter _instance;

        private Parser Parser;

        private Dictionary<SourceLocation, LuaTokenInfo> tokenMap;

        private static HashSet<string> Operators = new HashSet<string>
        {
            "+", "-", "*", "/", "%", "^", "==", "~=", "<=", ">=", "<", ">", "=", "..", "and", "or", "not", "#", ".", ":"
        };

        private static HashSet<string> LeftBrackets = new HashSet<string>
        {
            "(", "{", "[",
        };

        private static HashSet<string> RightBrackets = new HashSet<string>
        {
            ")", "}", "]",
        };

        private static HashSet<string> Delimiters = new HashSet<string>
        {
            ";", ","
        };

        private static HashSet<string> Keywords = new HashSet<string>
        {
            "break", "do", "else", "elseif", "end", "for", "function", "if", "in", "local", "repeat", "return", "then", "until", "while"
        };

        public static LuaFormatter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LuaFormatter();
                }
                return _instance;
            }
        }

        private LuaFormatter()
        {
        }

        public string DoFormatting(string text)
        {
            string output = "";

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (text != null)
            {
                output = ExectueFormat(text);
            }
            stopwatch.Stop();

            return output;
        }

        private void InitParser()
        {
            Parser = LuaGrammar.Instance.GetParser(ParseMode.File);
            Parser.Context.Options = ParseOptions.AnalyzeCode;
        }

        private string ExectueFormat(string text)
        {
            StringBuilder output = new StringBuilder();

            InitParser();
            var pt = Parser.Parse(text);

            if (Parser.Context.HasErrors)
            {
                throw new Exception("Cannot formet code due to syntax error");
            }

            tokenMap = new Dictionary<SourceLocation, LuaTokenInfo>();

            var indentState = new LuaIndentState(text);

            Traverse(indentState, pt.Root, 0);

            var depthTable = indentState.BuildDepthTable(pt.Root);

            InitParser();
            Parser.Scanner.VsSetSource(text, 0);

            bool lastLineHasToken = false;
            int lastLine = 0;
            Token lastToken = null;
            Token lastLastToken = null;
            StringBuilder lastLineBuilder = new StringBuilder();
            List<string> lineCache = new List<string>();

            while (true)
            {
                int s = 0;
                Token token = Parser.Scanner.VsReadToken(ref s);
                if (token == null)
                {
                    break;
                }

                SourceLocation sl = new SourceLocation(token.Location.Position, token.Location.Line - 1, token.Location.Column);
                LuaTokenInfo tokenInfo = null;
                if (tokenMap.ContainsKey(sl))
                {
                    tokenInfo = tokenMap[sl];
                }

                LuaTokenInfo lastTokenInfo = null;
                if (lastToken != null)
                {
                    SourceLocation lastSl = new SourceLocation(lastToken.Location.Position, lastToken.Location.Line - 1, lastToken.Location.Column);

                    if (tokenMap.ContainsKey(lastSl))
                    {
                        lastTokenInfo = tokenMap[lastSl];
                    }
                }

                if (sl.Line != lastLine)
                {
                    if (!lastLineHasToken)
                    {
                        using (StringReader sr = new StringReader(lastLineBuilder.ToString()))
                        {
                            while (true)
                            {
                                string cacheLine = sr.ReadLine();
                                if (cacheLine == null)
                                {
                                    break;
                                }
                                lineCache.Add(cacheLine);
                            }
                        }
                    }
                    else
                    {
                        Write(output, depthTable, lastLine, lastLineBuilder, lineCache);

                        lineCache.Clear();
                    }

                    for (int i = lastLine + lineCache.Count + 1; i < sl.Line; i++)
                    {
                        lineCache.Add("");
                    }

                    lastLineHasToken = false;
                    lastLine = sl.Line;
                    lastLineBuilder.Clear();
                    lastToken = null;
                    lastLastToken = null;
                }

                if (tokenInfo != null)
                {
                    lastLineHasToken = true;
                }

                AppendSpace(lastLineBuilder, lastToken, lastTokenInfo, token);

                lastLineBuilder.Append(token.Text);
                lastLastToken = lastToken;
                lastToken = token;
            }

            Write(output, depthTable, lastLine, lastLineBuilder, lineCache);

            return output.ToString();
        }

        private static void Write(StringBuilder output, int[] depthTable, int lastLine, StringBuilder lastLineBuilder, List<string> lineCache)
        {
            int indent = depthTable[lastLine];

            foreach (string line in lineCache)
            {
                WriteIndent(output, indent);
                output.AppendLine(line.Trim());
            }

            WriteIndent(output, indent);

            output.AppendLine(lastLineBuilder.ToString().TrimEnd());
        }

        private static void WriteIndent(StringBuilder output, int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                output.Append("    ");
            }
        }

        private void AppendSpace(StringBuilder lastLineBuilder, Token lastToken, LuaTokenInfo lastTokenInfo, Token token)
        {
            LuaTokenType lastTokenType = LuaTokenType.None;
            if (lastToken != null) lastTokenType = GetTokenType(lastToken);
            LuaTokenType tokenType = GetTokenType(token);

            if (lastTokenType == LuaTokenType.None)
            {
                // don't append space
            }
            else if (lastTokenType == LuaTokenType.LeftBracket)
            {
                // don't append space
            }
            else if (lastTokenType == LuaTokenType.RightBracket)
            {
                if (tokenType == LuaTokenType.Keyword)
                {
                    AppendSpace(lastLineBuilder);
                }
                else if (tokenType == LuaTokenType.Operator)
                {
                    if (token.Text == "." || token.Text == ":")
                    {

                    }
                    else
                    {
                        AppendSpace(lastLineBuilder);
                    }
                }
                // don't append space
            }
            else if (lastTokenType == LuaTokenType.Delimiter)
            {
                AppendSpace(lastLineBuilder);
            }
            else if (lastTokenType == LuaTokenType.Identifier)
            {
                if (tokenType == LuaTokenType.LeftBracket)
                {
                    ;
                }
                else if (tokenType == LuaTokenType.RightBracket)
                {
                    ;
                }
                else if (tokenType == LuaTokenType.Delimiter)
                {

                }
                else if (token.Text == "." || token.Text == ":")
                {

                }
                else
                {
                    AppendSpace(lastLineBuilder);
                }
            }
            else if (lastTokenType == LuaTokenType.Keyword)
            {
                if (tokenType == LuaTokenType.LeftBracket)
                {
                    if (lastToken.Text != "function")
                    {
                        AppendSpace(lastLineBuilder);
                    }
                }
                else if (tokenType == LuaTokenType.RightBracket)
                {
                    if (lastToken.Text != "end")
                    {
                        AppendSpace(lastLineBuilder);
                    }
                }
                else if (tokenType == LuaTokenType.Delimiter)
                {

                }
                else if (token.Text == "." || token.Text == ":")
                {

                }
                else
                {
                    AppendSpace(lastLineBuilder);
                }
            }
            else if (lastTokenType == LuaTokenType.Operator)
            {
                if (lastToken.Text == "#" || lastToken.Text == "." || lastToken.Text == ":")
                {
                    // don't append sapce
                }
                else if (lastTokenInfo.IsUnOp == true && lastToken.Text != "not")
                {
                    // don't append space
                }
                else
                {
                    AppendSpace(lastLineBuilder);
                }
            }
            else
            {
                throw new Exception("error token type");
            }
        }

        private void AppendSpace(StringBuilder sb)
        {
            if (sb.Length == 0 || sb.Length > 0 && sb[sb.Length - 1] != ' ')
            {
                sb.Append(' ');
            }
        }

        private LuaTokenType GetTokenType(Token token)
        {
            if (Delimiters.Contains(token.Text))
                return LuaTokenType.Delimiter;
            if (LeftBrackets.Contains(token.Text))
                return LuaTokenType.LeftBracket;
            if (RightBrackets.Contains(token.Text))
                return LuaTokenType.RightBracket;
            if (Operators.Contains(token.Text))
                return LuaTokenType.Operator;
            if (Keywords.Contains(token.Text))
                return LuaTokenType.Keyword;
            return LuaTokenType.Identifier;
        }

        private void Traverse(LuaIndentState indentState, ParseTreeNode node, int level)
        {
            if (node.Token != null)
            {
                if (tokenMap.ContainsKey(node.Token.Location) == false)
                {
                    tokenMap.Add(node.Token.Location, new LuaTokenInfo { Token = node.Token });
                }
            }
            else
            {
                IAstNode astNode = node.AstNode as IAstNode;
                if (astNode != null)
                {
                    astNode.Indent(indentState, tokenMap);
                }
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    Traverse(indentState, node.ChildNodes[i], level + 1);
                }
            }
        }
    }
}
