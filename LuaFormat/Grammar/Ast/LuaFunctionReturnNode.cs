using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaFunctionReturnNode : BlockNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaFuncReturn;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            if (treeNode.ChildNodes.Count == 2 && treeNode.FirstChild.Token.Text == "return")
            {
                AddChild(string.Empty, treeNode.ChildNodes[1]);
            }
            AsString = "LuaFuncReturn";
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            // no indent needed
        }
    }
}
