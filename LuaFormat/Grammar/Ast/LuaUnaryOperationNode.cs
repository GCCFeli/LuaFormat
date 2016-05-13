using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaUnaryOperationNode : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public string Op;

        public AstNode Argument;

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaUnaryOperation;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            Op = treeNode.ChildNodes[0].FindTokenAndGetText();
            Argument = AddChild("Arg", treeNode.ChildNodes[1]);
            AsString = Op + "(unary op)";
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            // no indent needed
            ;
        }
    }
}
