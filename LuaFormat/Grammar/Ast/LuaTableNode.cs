using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaTableNode : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public AstNode FieldList;

        private Token BeginRegionToken;

        private Token EndRegionToken;

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaTable;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            if (treeNode.ChildNodes.Count > 2)
            {
                if (treeNode.ChildNodes[0].Token != null)
                {
                    BeginRegionToken = treeNode.ChildNodes[0].Token;
                }
                FieldList = AddChild("field list", treeNode.ChildNodes[1]);
                if (treeNode.ChildNodes[2].Token != null)
                {
                    EndRegionToken = treeNode.ChildNodes[2].Token;
                }
            }
            AsString = "LuaTable";
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            // no indent needed
        }
    }
}
