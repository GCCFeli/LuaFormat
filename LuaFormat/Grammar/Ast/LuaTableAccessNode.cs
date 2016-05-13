using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaTableAccessNode : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public AstNode Table;

        public AstNode Member;

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaTableAccess;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            if (treeNode.ChildNodes.Count > 0)
            {
                Table = AddChild("Table", treeNode.ChildNodes[0]);
                int count = treeNode.ChildNodes.Count;
                if (count != 2)
                {
                    if (count == 3)
                    {
                        Member = AddChild("Member", treeNode.ChildNodes[2]);
                    }
                }
                else
                {
                    Member = AddChild("Member", treeNode.ChildNodes[1]);
                }
                AsString = Table.AsString + "." + Member.AsString;
            }
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            // no indent needed
        }
    }
}
