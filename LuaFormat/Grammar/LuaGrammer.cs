using Lua.LanguageService.Grammar.Ast;
using Lua.LanguageService.Grammar.Parser;
using Irony.Ast;
using Irony.Parsing;
using System;

namespace Lua.LanguageService.Grammar
{
    using Grammar = Irony.Parsing.Grammar;

    [Language("Lua", "5.1", "Lua Script Language")]
    class LuaGrammar : Grammar
    {
        public static LuaGrammar Instance
        {
            get
            {
                if (CurrentGrammar == null)
                {
                    return new LuaGrammar();
                }
                return (LuaGrammar)CurrentGrammar;
            }
        }

        public LuaGrammar() : base(true)
        {
            #region Declare Terminals Here

            StringLiteral STRING = CreateLuaString("string");
            NumberLiteral NUMBER = CreateLuaNumber("number");

            var LONGSTRING = new LuaLongStringTerminal("string block");

            // This includes both single-line and block comments
            var COMMENT = new LuaCommentTerminal("comment");
            NonGrammarTerminals.Add(COMMENT);

            //  Regular Operators

            //  Member Select Operators
            KeyTerm DOT = Operator(".");
            DOT.EditorInfo = new TokenEditorInfo(TokenType.Operator, TokenColor.Text, TokenTriggers.MemberSelect);

            KeyTerm COLON = Operator(":");
            COLON.EditorInfo = new TokenEditorInfo(TokenType.Operator, TokenColor.Text, TokenTriggers.MemberSelect);

            //  Standard Operators
            KeyTerm EQ = Operator("=");
            KeyTerm MINUS = Operator("-");
            KeyTerm PLUS = Operator("+");
            KeyTerm UMINUS = Operator("-");
            KeyTerm CONCAT = Operator("..");
            KeyTerm GETN = Operator("#");
            KeyTerm NOT = Keyword("not");
            KeyTerm AND = Keyword("and");
            KeyTerm OR = Keyword("or");

            KeyTerm keyTermBigBraceLeft = Brace("{");
            KeyTerm keyTermBigBraceRight = Brace("}");

            #region Keywords

            KeyTerm LOCAL = Keyword("local");
            KeyTerm DO = Keyword("do");
            KeyTerm END = Keyword("end");
            KeyTerm WHILE = Keyword("while");
            KeyTerm REPEAT = Keyword("repeat");
            KeyTerm UNTIL = Keyword("until");
            KeyTerm IF = Keyword("if");
            KeyTerm THEN = Keyword("then");
            KeyTerm ELSEIF = Keyword("elseif");
            KeyTerm ELSE = Keyword("else");
            KeyTerm FOR = Keyword("for");
            KeyTerm IN = Keyword("in");
            KeyTerm FUNCTION = Keyword("function");
            KeyTerm RETURN = Keyword("return");
            KeyTerm BREAK = Keyword("break");

            //KeyTerm NIL = Keyword("nil");
            //KeyTerm FALSE = Keyword("false");
            //KeyTerm TRUE = Keyword("true");

            KeyTerm ELLIPSIS = Keyword("...");

            #endregion

            var IDENTIFIER = new IdentifierTerminal("identifier");

            #endregion

            #region Declare Transient NonTerminals Here

            // Organize the transient non-terminals so they are easier to find. None of these
            // will have an AST type by definition

            var Statement = new NonTerminal("statment");
            var LastStatement = new NonTerminal("last statment", typeof(LuaFunctionReturnNode));
            var Statements = new NonTerminal("statement list");
            var StatementsEnd = new NonTerminal("statement list end");
            var SingleStatementWithTermOpt = new NonTerminal("single statement", Statement | Statement + ";");
            var LastStatementWithTermOpt = new NonTerminal("last statement", LastStatement | LastStatement + ";");

            var BinOp = new NonTerminal("bin op");

            var PrefixExpr = new NonTerminal("prefix expression");
            var Var = new NonTerminal("var");
            var Args = new NonTerminal("args");
            var EllipsisOpt = new NonTerminal("optional ellipsis");

            var ParentheicalExpression = new NonTerminal("parentheical expression");
            var ArgsParameters = new NonTerminal("parentheical arguments");

            var ColonCallOpt = new NonTerminal("colon call");
            var FunctionParameters = new NonTerminal("function parameters");
            MarkTransient(Statement, Statements, StatementsEnd, SingleStatementWithTermOpt, /*ColonCallOpt,*/
                          FunctionParameters,
                          LastStatementWithTermOpt, BinOp, PrefixExpr, Var, Args, EllipsisOpt, ArgsParameters,
                          ParentheicalExpression);

            #endregion

            #region Declare NonTerminals Here

            // These non-terminals will all require AST types. Anything that isnt really a language construct should be
            // refactored into a transient

            NonTerminal Expr = new NonTerminal("expression", typeof(LuaExpressionNode));

            NonTerminal Chunk = new NonTerminal("chunk", typeof(LuaChunkNode));
            NonTerminal Block = new NonTerminal("block", typeof(BlockNode)); // probably should be transient

            NonTerminal FunctionName = new NonTerminal("function name", typeof(LuaFunctionIdentifierNode));
            NonTerminal VarList = new NonTerminal("var list", typeof(LuaIdentifierListNode));
            NonTerminal NameList = new NonTerminal("name list", typeof(LuaIdentifierListNode));
            NonTerminal ExprList = new NonTerminal("expression list", typeof(LuaExpressionListNode));

            NonTerminal FunctionCall = new NonTerminal("function call", typeof(LuaFunctionCallNode));
            NonTerminal Function = new NonTerminal("anonymous function definition", typeof(LuaFunctionDefNode));
            NonTerminal FunctionBody = new NonTerminal("function body", typeof(LuaFunctionBodyNode));
            NonTerminal ParamList = new NonTerminal("parlist", typeof(LuaParamListNode));

            NonTerminal LocalVariableDeclaration = new NonTerminal("local variable declaration", typeof(LuaLocalVariableNode));
            NonTerminal LocalVariableDeclarationWithAssignment = new NonTerminal("local variable declaration with assignment", typeof(LuaLocalAssignmentNode));
            NonTerminal TableConstructor = new NonTerminal("table construct", typeof(LuaTableNode));
            NonTerminal FieldList = new NonTerminal("field list", typeof(LuaExpressionListNode));
            NonTerminal Field = new NonTerminal("field", typeof(LuaFieldNode));
            NonTerminal FieldSep = new NonTerminal("field sep");

            NonTerminal IdentifierWithOptNamespace = new NonTerminal("identifier including namespace");

            NonTerminal BinExp = new NonTerminal("binexp", typeof(BinaryOperationNode)) { Rule = Expr + BinOp + Expr };
            NonTerminal UnOp = new NonTerminal("unary op", typeof(LuaUnaryOperatorNode));
            NonTerminal UniExp = new NonTerminal("uniexp", typeof(LuaUnaryOperationNode)) { Rule = UnOp + Expr };

            NonTerminal VariableAssignment = new NonTerminal("variable assign", typeof(LuaAssignmentNode));

            NonTerminal IfBlock = new NonTerminal("if block", typeof(LuaIfNode));
            NonTerminal ElseIfBlock = new NonTerminal("ElseIfClause", typeof(LuaIfNode));
            NonTerminal ElseIfBlockList = new NonTerminal("ElseIfClause*");
            NonTerminal ElseBlockOpt = new NonTerminal("ElseClause");

            NonTerminal DoBlock = new NonTerminal("do block", typeof(LuaDoNode));
            NonTerminal WhileBlock = new NonTerminal("while block", typeof(LuaWhileNode));
            NonTerminal RepeatBlock = new NonTerminal("repeat block", typeof(LuaRepeatNode));

            NonTerminal NumForCondition = new NonTerminal("num for condition");
            NonTerminal NumForBlock = new NonTerminal("num for block", typeof(LuaForNode));
            NonTerminal GenericForCondition = new NonTerminal("generic for condition");
            NonTerminal GenericForBlock = new NonTerminal("generic for block", typeof(LuaForNode));

            NonTerminal LocalFunctionDeclaration = new NonTerminal("local function declaration", typeof(LuaFunctionLocalDefNode));
            NonTerminal FunctionDeclaration = new NonTerminal("function declaration", typeof(LuaFunctionDefNode));

            NonTerminal TableAccess = new NonTerminal("table access", typeof(LuaTableAccessNode));

            #endregion

            #region Place Rules Here

            //Using Lua 5.1 grammar as defined in
            //http://www.lua.org/manual/5.1/manual.html#8
            Root = Chunk;

            //chunk ::= {stat [`;´]} [laststat [`;´]]
            Statements.Rule = SingleStatementWithTermOpt.Star();
            StatementsEnd.Rule = Empty | LastStatementWithTermOpt;
            Chunk.Rule = Statements + StatementsEnd;

            //block ::= chunk
            Block = Chunk;

            VariableAssignment.Rule = VarList + EQ + ExprList;

            FunctionDeclaration.Rule = FUNCTION + FunctionName + FunctionParameters + FunctionBody;

            LocalFunctionDeclaration.Rule = LOCAL + FunctionDeclaration;

            LocalVariableDeclaration.Rule = LOCAL + NameList;
            LocalVariableDeclarationWithAssignment.Rule = LocalVariableDeclaration + EQ + ExprList;

            DoBlock.Rule = DO + Block + END;
            WhileBlock.Rule = WHILE + Expr + DO + Block + END;
            RepeatBlock.Rule = REPEAT + Block + UNTIL + Expr;

            ElseBlockOpt.Rule = Empty | ELSE + Block;
            ElseIfBlock.Rule = ELSEIF + Expr + THEN + Block;
            ElseIfBlockList.Rule = MakeStarRule(ElseIfBlockList, null, ElseIfBlock);
            IfBlock.Rule = IF + Expr + THEN + Block + ElseIfBlockList + ElseBlockOpt + END;

            NumForCondition.Rule = IDENTIFIER + EQ + Expr + "," + Expr + ("," + Expr).Q();
            NumForBlock.Rule = FOR + NumForCondition + DO + Block + END;
            GenericForCondition.Rule = NameList + IN + ExprList;
            GenericForBlock.Rule = FOR + GenericForCondition + DO + Block + END;

            //stat ::=  varlist `=´ explist | 
            //     functioncall | 
            //     do block end | 
            //     while exp do block end | 
            //     repeat block until exp | 
            //     if exp then block {elseif exp then block} [else block] end | 
            //     for Name `=´ exp `,´ exp [`,´ exp] do block end | 
            //     for namelist in explist do block end | 
            //     function funcname funcbody | 
            //     local function Name funcbody | 
            //     local namelist [`=´ explist]
            Statement.Rule = VariableAssignment |
                             FunctionCall |
                             DoBlock |
                             WhileBlock |
                             RepeatBlock |
                             IfBlock |
                             NumForBlock |
                             GenericForBlock |
                             FunctionDeclaration |
                             LocalFunctionDeclaration |
                             LocalVariableDeclaration |
                             LocalVariableDeclarationWithAssignment;

            //laststat ::= return [explist] | break
            LastStatement.Rule = RETURN + ExprList | RETURN | BREAK;

            //funcname ::= Name {`.´ Name} [`:´ Name]
            //FuncName.Rule = Name + (DOT + Name).Star() + (COLON + Name).Q();
            ColonCallOpt.Rule = Empty | COLON + IDENTIFIER;
            IdentifierWithOptNamespace.Rule = MakePlusRule(IdentifierWithOptNamespace, DOT, IDENTIFIER);
            FunctionName.Rule = IdentifierWithOptNamespace + ColonCallOpt;

            //varlist ::= var {`,´ var}
            VarList.Rule = MakePlusRule(VarList, ToTerm(","), Var);

            //namelist ::= Name {`,´ Name}
            NameList.Rule = MakePlusRule(NameList, ToTerm(","), IDENTIFIER);

            //explist ::= {exp `,´} exp
            ExprList.Rule = MakePlusRule(ExprList, ToTerm(","), Expr);

            //exp ::=  nil | false | true | Number | String | `...´ | function | 
            //     prefixexp | tableconstructor | exp binop exp | unop exp 
            Expr.Rule = /*NIL | FALSE | TRUE |*/ NUMBER | STRING | LONGSTRING | ELLIPSIS | Function |
                        PrefixExpr | TableConstructor | BinExp | UniExp;

            //var ::=  Name | prefixexp `[´ exp `]´ | prefixexp `.´ Name
            TableAccess.Rule = PrefixExpr + "[" + Expr + "]" | PrefixExpr + DOT + IDENTIFIER;
            Var.Rule = IDENTIFIER | TableAccess;

            //prefixexp ::= var | functioncall | `(´ exp `)´
            ParentheicalExpression.Rule = ToTerm("(") + Expr + ToTerm(")");
            PrefixExpr.Rule = Var | FunctionCall | ParentheicalExpression;

            //functioncall ::=  prefixexp args | prefixexp `:´ Name args 
            FunctionCall.Rule = PrefixExpr + Args | PrefixExpr + COLON + IDENTIFIER + Args;

            //args ::=  `(´ [explist] `)´ | tableconstructor | String 
            ArgsParameters.Rule = ToTerm("(") + ExprList + ToTerm(")") | ToTerm("(") + ToTerm(")");
            Args.Rule = ArgsParameters | TableConstructor | STRING | LONGSTRING;

            //function ::= function funcbody
            Function.Rule = FUNCTION + FunctionParameters + FunctionBody;

            //funcbody ::= `(´ [parlist] `)´ block end
            FunctionParameters.Rule = ToTerm("(") + ParamList + ToTerm(")");
            FunctionBody.Rule = Block + END;

            //parlist ::= namelist [`,´ `...´] | `...´
            EllipsisOpt.Rule = Empty | ToTerm(",") + ELLIPSIS;
            ParamList.Rule = NameList + EllipsisOpt | ELLIPSIS | Empty;

            //tableconstructor ::= `{´ [fieldlist] `}´
            //TableConstructor.Rule = "{" + FieldList.Q() + "}";
            TableConstructor.Rule = keyTermBigBraceLeft + FieldList + keyTermBigBraceRight;

            //fieldlist ::= field {fieldsep field} [fieldsep]
            //FieldList.Rule = Field + (FieldSep + Field).Star() + FieldSep.Q();
            FieldList.Rule = MakeStarRule(FieldList, FieldSep, Field, TermListOptions.AllowTrailingDelimiter);

            //field ::= `[´ exp `]´ `=´ exp | Name `=´ exp | exp
            Field.Rule = "[" + Expr + "]" + "=" + Expr | IDENTIFIER + "=" + Expr | Expr;

            //fieldsep ::= `,´ | `;´
            FieldSep.Rule = (ToTerm(",") | ";");

            //binop ::= `+´ | `-´ | `*´ | `/´ | `^´ | `%´ | `..´ | 
            //          `<´ | `<=´ | `>´ | `>=´ | `==´ | `~=´ | 
            //          and | or
            BinOp.Rule = PLUS | MINUS | "*" | "/" | "^" | "%" | CONCAT |
                         "<" | "<=" | ">" | ">=" | "==" | "~=" |
                         AND | OR;

            //unop ::= `-´ | not | `#´
            UnOp.Rule = UMINUS | NOT | GETN;

            #endregion

            #region Define Keywords and Register Symbols

            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");
            RegisterBracePair("[", "]");

            MarkPunctuation(",", ";");
            MarkPunctuation("(", ")");
            MarkPunctuation("{", "}");
            MarkPunctuation("[", "]");

            RegisterOperators(1, OR);
            RegisterOperators(2, AND);
            RegisterOperators(3, "<", ">", "<=", ">=", "~=", "==");
            RegisterOperators(4, Associativity.Right, CONCAT);
            RegisterOperators(5, MINUS, PLUS);
            RegisterOperators(6, "*", "/", "%");
            RegisterOperators(7, NOT, UMINUS);
            RegisterOperators(8, Associativity.Right, "^");

            #endregion

            LanguageFlags = LanguageFlags.CreateAst | LanguageFlags.SupportsCommandLine | LanguageFlags.TailRecursive;
        }

        public Irony.Parsing.Parser GetParser(ParseMode mode = ParseMode.VsLineScan)
        {
            return new Irony.Parsing.Parser(Instance)
            {
                Context =
                {
                    Source =
                    {
                        TabWidth = 1
                    },
                    TabWidth = 1,
                    Mode = mode
                }
            };
        }

        public new void RegisterOperators(int precedence, params string[] opSymbols)
        {
            RegisterOperators(precedence, Associativity.Left, opSymbols);
        }

        public new void RegisterOperators(int precedence, Associativity associativity, params string[] opSymbols)
        {
            foreach (string op in opSymbols)
            {
                KeyTerm opSymbol = Operator(op);
                opSymbol.Precedence = precedence;
                opSymbol.Associativity = associativity;
            }
        }

        public KeyTerm Keyword(string keyword)
        {
            KeyTerm term = ToTerm(keyword);
            //term.SetFlag(TermFlags.IsKeyword, true);
            //term.SetFlag(TermFlags.IsReservedWord, true);
            term.EditorInfo = new TokenEditorInfo(TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);

            return term;
        }

        public KeyTerm Operator(string op)
        {
            KeyTerm term = new KeyTerm(CaseSensitive ? op : op.ToLower(), op);
            term.SetFlag(TermFlags.IsOperator, true);
            term.EditorInfo = new TokenEditorInfo(TokenType.Operator, TokenColor.Keyword, TokenTriggers.None);

            return term;
        }

        public KeyTerm Brace(string name, TokenTriggers trigger = TokenTriggers.None)
        {
            return new KeyTerm(name, name)
            {
                EditorInfo = new TokenEditorInfo(TokenType.Delimiter, TokenColor.Text, trigger | TokenTriggers.MatchBraces)
            };
        }

        protected static NumberLiteral CreateLuaNumber(string name)
        {
            var term = new NumberLiteral(name, NumberOptions.AllowStartEndDot);
            //default int types are Integer (32bit) -> LongInteger (BigInt); Try Int64 before BigInt: Better performance?
            term.DefaultIntTypes = new[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
            term.DefaultFloatType = TypeCode.Double; // it is default
            term.AddPrefix("0x", NumberOptions.Hex);

            return term;
        }

        protected static StringLiteral CreateLuaString(string name)
        {
            return new LuaStringLiteral(name);
        }
    }
}
