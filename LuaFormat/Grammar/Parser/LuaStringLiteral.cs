using Irony.Parsing;

namespace Lua.LanguageService.Grammar.Parser
{
    class LuaStringLiteral : StringLiteral
    {
        public LuaStringLiteral(string name) : base(name)
        {
            base.AddStartEnd("'", StringOptions.AllowsAllEscapes);
            base.AddStartEnd("\"", StringOptions.AllowsAllEscapes);
        }

        protected override bool ReadBody(ISourceStream source, CompoundTerminalBase.CompoundTokenDetails details)
        {
            int num = source.Text.IndexOf('\n', source.PreviewPosition);
            if (num > -1 && source.Text[num - 1] == '\\')
            {
                details.Flags += 4;
            }
            return base.ReadBody(source, details);
        }

        protected override string HandleSpecialEscape(string segment, CompoundTerminalBase.CompoundTokenDetails details)
        {
            if (string.IsNullOrEmpty(segment))
            {
                return string.Empty;
            }
            char c = segment[0];
            if (c <= '\\')
            {
                if (c <= '\'')
                {
                    if (c != '"' && c != '\'')
                    {
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '0':
                        case '1':
                        case '2':
                            {
                                bool flag = false;
                                if (segment.Length >= 3)
                                {
                                    string arg_A2_0 = segment.Substring(0, 3);
                                    int num = 0;
                                    flag = int.TryParse(arg_A2_0, out num);
                                }
                                if (!flag)
                                {
                                    details.Error = "Invalid escape sequence: \000 must be a valid number.";
                                }
                                break;
                            }
                        default:
                            if (c != '\\')
                            {
                            }
                            break;
                    }
                }
            }
            else if (c <= 'b')
            {
                if (c != 'a' && c != 'b')
                {
                }
            }
            else if (c != 'f' && c != 'n')
            {
            }
            details.Error = "Invalid escape sequence: \\" + segment;
            return segment;
        }
    }
}
