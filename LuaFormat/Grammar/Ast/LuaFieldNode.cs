using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaFieldNode : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public AstNode Target;

        public AstNode Expression;

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaField;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            if (treeNode.ChildNodes.Count > 1)
            {
                Target = AddChild("key", treeNode.ChildNodes[0]);
                Expression = AddChild("value", treeNode.ChildNodes[2]);
                AsString = "key/value field";
                return;
            }
            Expression = AddChild("entry", treeNode.ChildNodes[0]);
            AsString = "list field";
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            // no indent needed
        }
    }
}
