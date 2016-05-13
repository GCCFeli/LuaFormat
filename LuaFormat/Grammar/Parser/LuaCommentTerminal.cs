using Irony.Parsing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lua.LanguageService.Grammar.Parser
{
    class LuaCommentTerminal : Terminal
    {
        public string StartSymbol = "--";

        public LuaCommentTerminal(string name) : base(name, TokenCategory.Comment)
        {
            Priority = 1000;
        }

        public override void Init(GrammarData grammarData)
        {
            base.Init(grammarData);
            base.SetFlag(TermFlags.IsMultiline);
            if (EditorInfo == null)
            {
                EditorInfo = new TokenEditorInfo(TokenType.Comment, TokenColor.Comment, TokenTriggers.None);
            }
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            Token token;
            if (context.VsLineScanState.Value != 0)
            {
                byte tokenSubType = context.VsLineScanState.TokenSubType;
                token = CompleteMatch(context, source, tokenSubType);
            }
            else
            {
                byte commentLevel = 0;
                if (!BeginMatch(context, source, ref commentLevel))
                {
                    return null;
                }
                token = CompleteMatch(context, source, commentLevel);
            }
            if (token != null)
            {
                return token;
            }
            if (context.Mode == ParseMode.VsLineScan)
            {
                return CreateIncompleteToken(context, source);
            }
            return source.CreateErrorToken("Unclosed comment block", new object[0]);
        }

        private Token CreateIncompleteToken(ParsingContext context, ISourceStream source)
        {
            source.PreviewPosition = source.Text.Length;
            Token expr_1D = source.CreateToken(base.OutputTerminal);
            expr_1D.Flags |= TokenFlags.IsIncomplete;
            context.VsLineScanState.TerminalIndex = MultilineIndex;
            return expr_1D;
        }

        private bool BeginMatch(ParsingContext context, ISourceStream source, ref byte commentLevel)
        {
            if (!source.MatchSymbol(StartSymbol, !base.Grammar.CaseSensitive))
            {
                return false;
            }
            int num = source.PreviewPosition + StartSymbol.Length;
            int length = source.Text.Length;
            if (length > num + 1 && source.Text[num] == '[')
            {
                byte b = 1;
                int num2 = num + 255;
                num++;
                while (num < num2 && length > num)
                {
                    if (source.Text[num] == '=')
                    {
                        b += 1;
                        num++;
                    }
                    else
                    {
                        if (source.Text[num] == '[')
                        {
                            commentLevel = b;
                            break;
                        }
                        break;
                    }
                }
            }
            source.PreviewPosition += StartSymbol.Length + (int)commentLevel;
            return true;
        }

        private Token CompleteMatch(ParsingContext context, ISourceStream source, byte commentLevel)
        {
            if (commentLevel == 0)
            {
                char[] anyOf = new char[]
                {
                    '\n',
                    '\r',
                    '\v'
                };
                int num = source.Text.IndexOfAny(anyOf, source.PreviewPosition);
                if (num > 0)
                {
                    source.PreviewPosition = num;
                }
                else
                {
                    source.PreviewPosition = source.Text.Length;
                }
                return source.CreateToken(base.OutputTerminal);
            }
            while (!source.EOF())
            {
                foreach (Match match in Regex.Matches(source.Text.Substring(source.PreviewPosition), "\\](=*)\\]"))
                {
                    if (match.Groups[1].Value.Length == (int)(commentLevel - 1))
                    {
                        source.PreviewPosition += match.Index + match.Length;
                        Token result;
                        if (context.VsLineScanState.Value != 0)
                        {
                            SourceLocation location = default(SourceLocation);
                            location.Position = 0;
                            string text = source.Text.Substring(0, source.PreviewPosition);
                            context.VsLineScanState.Value = 0;
                            result = new Token(this, location, text, null);
                            return result;
                        }
                        result = source.CreateToken(base.OutputTerminal);
                        return result;
                    }
                }
                int previewPosition = source.PreviewPosition;
                source.PreviewPosition = previewPosition + 1;
            }
            context.VsLineScanState.TokenSubType = commentLevel;
            return null;
        }

        public override IList<string> GetFirsts()
        {
            return new string[]
            {
                StartSymbol
            };
        }
    }
}
