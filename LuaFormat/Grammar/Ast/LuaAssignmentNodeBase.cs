using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    abstract class LuaAssignmentNodeBase : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public AstNode Target;

        public string AssignmentOp;

        public AstNode Expression;

        public abstract bool IsLocal
        {
            get;
        }

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaAssignment;
            }
        }

        protected virtual void InitAssignment(ParseTreeNode treeNode)
        {
            AssignmentOp = "=";
            Expression = AddChild("Expr", treeNode.ChildNodes[2]);
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = TreeNode;
            InitAssignment(treeNode);
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            // no indent needed
        }
    }
}
