using Irony.Ast;
using Irony.Parsing;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaLocalAssignmentNode : LuaAssignmentNodeBase
    {
        public AstNode Local;

        public override bool IsLocal
        {
            get
            {
                return true;
            }
        }

        protected override void InitAssignment(ParseTreeNode treeNode)
        {
            ParseTreeNode parseTreeNode = treeNode.ChildNodes[0];
            Local = AddChild("Local", parseTreeNode.ChildNodes[0]);
            Target = AddChild("To", parseTreeNode.ChildNodes[1]);
            base.InitAssignment(treeNode);
            AsString = AssignmentOp + " (local assignment)";
        }
    }
}
