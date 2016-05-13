using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaForNode : AstNode, IAstNode
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
                return NodeType.LuaFor;
            }
        }

        public AstNode Conditions;
        public AstNode Block;

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            Conditions = AddChild("Conditions", treeNode.ChildNodes[1]);
            Block = AddChild("Block", treeNode.ChildNodes[3]);
            AsString = "LuaFor";
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            {
                var startLocation = indentState.GetFirstLine(Conditions);
                var endLocation = indentState.GetLastLine(Conditions);
                indentState.Indent(startLocation, endLocation, this);
            }
            {
                var startLocation = indentState.GetFirstLine(Block);
                var endLocation = indentState.GetLastLine(Block);
                indentState.Indent(startLocation, endLocation, this);
            }
        }
    }
}
