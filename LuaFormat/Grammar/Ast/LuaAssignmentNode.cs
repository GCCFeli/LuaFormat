using Irony.Parsing;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaAssignmentNode : LuaAssignmentNodeBase
    {
        public override bool IsLocal
        {
            get
            {
                return false;
            }
        }

        protected override void InitAssignment(ParseTreeNode treeNode)
        {
            Target = AddChild("To", treeNode.ChildNodes[0]);
            base.InitAssignment(treeNode);
            AsString = AssignmentOp + " (assignment)";
        }
    }
}
