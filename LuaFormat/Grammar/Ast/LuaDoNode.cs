using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaDoNode : AstNode, IAstNode
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
                return NodeType.LuaDo;
            }
        }

        public AstNode Block;


        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            Block = AddChild("Block", treeNode.ChildNodes[1]);
            AsString = "LuaDo";
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            {
                var startLocation = indentState.GetFirstLine(Block);
                var endLocation = indentState.GetLastLine(Block);
                indentState.Indent(startLocation, endLocation, this);
            }
        }
    }
}
