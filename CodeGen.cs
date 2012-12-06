using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
namespace JayPascal
{
    public class CodeGen
    {
        #region variables 
        public static int currentChildNumber = 0;
        public static int treeDepth=0;
        public static bool allowRealToInt = false;
        static String exprResutType = "UNKNOWN";  

        //static bool realFlag = false;
        static int real_constant_index = 0;
        static int string_constant_index = 0;
        static int label_index = 0;
       // static int array_var_index = 0;

        static String progName = "";
        static StringBuilder verdec = new StringBuilder();
        static StringBuilder mainCompStatement = new StringBuilder();
        private const String indent = "      ";
        static SortedList varTypeList = new SortedList();
        static SortedList typeDefinationList = new SortedList();
        static SortedList labelDefList = new SortedList();
        static SortedList constDefList = new SortedList();
        
        
        static String generatedArrayName = "";
        static String generatedArrayType = "";
        class arryDef
        {
            public string ArrayDefName;
            public string PremitiveType;
            public int LowIndex;
            public int HighIndex;
            public string OriginalType;
            public arryDef(string arryDefName, string premitiveType, string originalType, int lowIndex, int highIndex)
            {
                ArrayDefName = arryDefName;
                PremitiveType = premitiveType;
                LowIndex = lowIndex;
                HighIndex = highIndex;
                OriginalType = originalType;
            }
        }
        class labelDef2
        {
            public string LableName;
            public int LableDepth;
            public bool seenBefore;
            public labelDef2(String lblID, int depth)
            {
                LableName = lblID;
                LableDepth = depth;
                seenBefore = false;
            }
        }
        #endregion

        public static void generate(Node root, StringBuilder out_Renamed)
        {
            generate(root, null, "");

            //StringBuilder asmCode = new StringBuilder();

            generatecode(out_Renamed);

        }
        private static void generate(Node root, StringBuilder out_Renamed, String offset)
        {

            if (root != null)
            {
                //out_Renamed.Append(offset + root.Symbol.ToString() + Environment.NewLine);

                if (root.Symbol.ToString().Equals("<program_heading>"))
                {
                    Node tempN = root.getChild(0);
                    
                    progName = tempN.Symbol.ToString().Substring(3, tempN.Symbol.ToString().Length - 4).ToUpper();
                    generate(tempN, null, offset + indent);
                }
                else if (root.Symbol.ToString().Equals("<type_definition>"))
                {
                    generateTypeDeclarationsCode(root);
                }
                else if (root.Symbol.ToString().Equals("<constant_definition>"))
                {
                    generateConstDeclarationsCode(root);
                }
                else if (root.Symbol.ToString().Equals("<variable_declaration>"))
                {
                    generateVarDeclarationsCode(root); 
                }
                else if (root.Symbol.ToString().Equals("<label_declarations>"))
                {
                    generateLabelDeclarationsCode(root);
                }
                else if (root.Symbol.ToString().Equals("<compound_stat>"))
                {
                    mainCompStatement.Append(generateCompStatementCode(root));

                }
                else
                {
                    int numChildren = root.numChildren();
                    for (int childNum = 0; childNum < numChildren; ++childNum)
                    {
                        generate(root.getChild(childNum), out_Renamed, offset + indent);
                    }
                }
            }
            
        }

        private static void generateLabelDeclarationsCode(Node root)
        {
            for (int childNum = 0; childNum < root.numChildren(); ++childNum)
            {
                String labelStr = root.getChild(childNum).Symbol.ToString();
                labelStr = "defLabel_" + labelStr.Substring((labelStr.IndexOf("[") + 1),
                (labelStr.Length - (labelStr.IndexOf("[") + 2)));
                //labelDefList.Add(labelStr, new labelDef(labelStr,0));
                labelDefList.Add(labelStr, labelStr);
            }
        }



        private static void generateTypeDeclarationsCode(Node root)
        {
            List<String> varlist = new List<String>();

            int numChildren2 = root.numChildren();
            for (int childNum = 0; childNum < (numChildren2 - 1); ++childNum)
            {
                varlist.Add(root.getChild(childNum).Symbol.ToString());
            }
            
            Node type = root.getChild(numChildren2 - 1);//mainly array_type

            if (type.Symbol.ToString().Equals("<array_type>"))
            {
                string rangeFrom = strip(type.getChild(0).getChild(0).getChild(1));
                string rangeTo = strip(type.getChild(0).getChild(1).getChild(1));
                string typeStr = type.getChild(1).getChild(0).Symbol.ToString();
                if (typeStr.Equals("ID[INTEGER]"))
                {
                    for (int i = 0; i < varlist.Count; i++)
                    {
                        arryDef ad = new arryDef(varlist[i], "DB",typeStr, int.Parse(rangeFrom), int.Parse(rangeTo));
                        //need to check for duplicate defination
                        typeDefinationList.Add(varlist[i], ad);
                    }

                }


            }

        }
        
        private static void generateConstDeclarationsCode(Node root)
        {
            
            String conName = strip(root.getChild(0));
            Node conNode = root.getChild(1).getChild(1);
            String type = conNode.Symbol.ToString();
            String value = strip(conNode);
            if (type.Contains("REAL")) // real const
            {
                checkVar(conName, root, true);
                verdec.Append(conName + " REAL4 " + value + "\n");
                varTypeList.Add(conName, "REAL4CONST");
                constDefList.Add(conName, "REAL4CONST");
            }
            else if (type.Contains("INT"))
            {
                checkVar(conName, root, true);
                verdec.Append(conName + " SDWORD " + value + "\n");
                varTypeList.Add(conName, "SDWORDCONST");
                constDefList.Add(conName, "SDWORDCONST");
            }
            
        }

        private static void generateVarDeclarationsCode(Node root)
        {
            List<String> varlist = new List<String>();

            int numChildren2 = root.numChildren();

            for (int childNum = 0; childNum < (numChildren2 - 1); ++childNum)
            {
                
                varlist.Add(strip(root.getChild(childNum)));
            }
            
            String type = root.getChild(numChildren2 - 1).getChild(0).Symbol.ToString();

            if (type.Equals("ID[INTEGER]"))
            {
                for (int i = 0; i < varlist.Count; i++)
                {
                    checkVar(varlist[i], root, true);
                    verdec.Append(varlist[i] + " SDWORD ?\n");
                    varTypeList.Add(varlist[i], "SDWORD");
                }

            }
            else if (type.Equals("ID[REAL]"))
            {
                for (int i = 0; i < varlist.Count; i++)
                {
                    checkVar(varlist[i], root, true);
                    verdec.Append(varlist[i] + " REAL4 ?\n");
                    varTypeList.Add(varlist[i], "REAL4");

                }

            }
            else if (type.Equals("ID[CHAR]"))
            {
                for (int i = 0; i < varlist.Count; i++)
                {
                    checkVar(varlist[i], root, true);
                    verdec.Append(varlist[i] + " BYTE ?\n");
                    varTypeList.Add(varlist[i], "BYTE");

                }

            }
            else
            {
                //user defined type... for now array only
                if (typeDefinationList.ContainsKey(type))
                {
                    arryDef userDefType = (arryDef)typeDefinationList.GetByIndex(typeDefinationList.IndexOfKey(type));
                    //U_P$SUM_ARRAY_NUMBERS	DB	10 DUP(?)
                   
                    for (int i = 0; i < varlist.Count; i++)
                    {
                        string arrayName = varlist[i].Replace("var$_$","arr$_$");
                        checkVar(arrayName, root, true);
                        verdec.Append(arrayName + " " + userDefType.PremitiveType +
                            " "+((userDefType.HighIndex-userDefType.LowIndex+1)*4).ToString() +" DUP (?)\n");
                        varTypeList.Add(arrayName, type);

                    }
                }
                else
                    throw new CompilerException("unknown type found ", root);

            }
        }
        private static string generateCompStatementCode(Node root)
        {
            treeDepth++;
            StringBuilder comStm = new StringBuilder();
            int numChildren = root.numChildren();
            for (int childNum = 0; childNum < numChildren; ++childNum)
            {
                Node cNode = root.getChild(childNum);
                if (cNode.Symbol.ToString().Equals("<compound_stat>"))
                {
                    generateCompStatementCode(cNode);
                }
                else
                    comStm.Append(generateStatementCode(cNode));
            }
            treeDepth--;
            
            return comStm.ToString();
        }

        private static string generateStatementCode(Node root)
        {
            StringBuilder strStmt = new StringBuilder();
            
                if (root.Symbol.ToString().Equals("<ass_stat>"))
                {
                    strStmt.Append(generateAssignStatementCode(root));
                }
                else if (root.Symbol.ToString().Equals("<if_stat>"))
                {
                    strStmt.Append(generateIfStatCode(root));
                }
                else if (root.Symbol.ToString().Equals("<for_stat>"))
                {
                    strStmt.Append(generateForStatCode(root));
                }
                else if (root.Symbol.ToString().Equals("<while_stat>"))
                {
                    strStmt.Append(generateWhileStatCode(root));
                }
                else if (root.Symbol.ToString().Equals("<repeat_stat>"))
                {
                    strStmt.Append(generateRepeatStatCode(root));
                }
                else if (root.Symbol.ToString().Equals("<goto_stat>"))
                {
                    String labelStr = root.getChild(0).Symbol.ToString();
                    labelStr = "defLabel_" + labelStr.Substring((labelStr.IndexOf("[") + 1),
                    (labelStr.Length - (labelStr.IndexOf("[") + 2)));
                    if (labelDefList.ContainsKey(labelStr))
                    {

                        
                        strStmt.Append("\njmp " + labelStr + "\n");
                    }
                    else
                    {
                        throw new CompilerException("Undefined Label!", root);
                    }
                   // strStmt.Append(generateRepeatStatCode(root));
                }
                else if (root.Symbol.ToString().Equals("<labeled_statement>"))
                {
                    String labelStr = root.getChild(0).Symbol.ToString();
                    labelStr = "defLabel_" + labelStr.Substring((labelStr.IndexOf("[") + 1),
                    (labelStr.Length - (labelStr.IndexOf("[") + 2)));
                    if (labelDefList.ContainsKey(labelStr))
                    {
                        
                        strStmt.Append("\n" + labelStr + ":\n");
                    }
                    else
                    {
                        throw new CompilerException("Undefined Label!", root);
                    }
                    strStmt.Append(generateStatementCode(root.getChild(1)));
                }
                    
                else if (root.Symbol.ToString().Equals("<procedure_call>"))
                {
                    strStmt.Append(generateProcedureCallCode(root));
                }

                return strStmt.ToString();
        }
        private static string generateForStatCode(Node root)
        {
            StringBuilder forStr = new StringBuilder();
            if (root.numChildren() == 5)
            {
                Node c1 = root.getChild(0);
                String varName = strip(c1);
                //checkVar(varName, root, false);
                //String VarType = (String)varTypeList.GetByIndex(varTypeList.IndexOfKey(varName));
                String VarType = getVarType(varName, c1);
                if (VarType.Equals("SDWORD"))
                {
                    Node c2 = root.getChild(1);

                    //c2 is inaital value, it can be a vairalbe, constant or expression
                    //assume its a constant for now

                    Node c4 = root.getChild(3);
                    //c4 is end value

                    Node c5 = root.getChild(4);
                    //c5 is the body of the loop, it canbe satement, statement bock, or procedure
                    if ((c2.Symbol.ToString().Contains("INT[")) && (c4.Symbol.ToString().Contains("INT[")))
                    {
                        string constForm = strip(c2);
                        string constTo = strip(c4);
                        string stra = "", strb = "", strc = "";
                        if (int.Parse(constForm) < int.Parse(constTo)) //incremental index
                        {
                            stra = "sub";
                            strb = "add";
                            strc = "jl";
                        }
                        else //decremental index
                        {
                            stra = "add";
                            strb = "sub";
                            strc = "jg";
                        }
                        forStr.Append("mov	dword ptr [" + varName + "], " + strip(c2) + "\n");
                        forStr.Append(stra + "	dword ptr [" + varName + "],1\n");

                        label_index++;
                        String lablefor = "@@syslbl_" + label_index.ToString();
                        forStr.Append("\n" + lablefor + ":\n");
                        forStr.Append(strb + "	dword ptr [" + varName + "],1\n");
                        //things in the loop

                        forStr.Append("\n");
                        
                        if (c5.Symbol.ToString().Equals("<compound_stat>"))
                            forStr.Append(generateCompStatementCode(c5));
                        else
                            forStr.Append(generateStatementCode(c5));

                        forStr.Append("\n");

                        forStr.Append("cmp	word ptr [" + varName + "], " + strip(c4) + "\n");
                        forStr.Append(strc + " " + lablefor + "\n");
                    }
                    else
                    {
                        throw new CompilerException("Invalid " + varName.Replace("var$_$", "") + " Initialization", c1);
                    }
                }
                else
                {
                    throw new CompilerException("Variable " + varName.Replace("var$_$", "") + " Should be Integer Type", c1);
                }

            }
            else
            {
                throw new CompilerException("Incomplite for statement ", root);
            }
            return forStr.ToString();
        }
        private static string generateWhileStatCode(Node root)
        {
            StringBuilder whileStr = new StringBuilder();


            if (root.numChildren() < 2)
            {
                // error
                throw new CompilerException("Incomplite if statement ", root);
            }
            else if ((root.numChildren() == 2) || (root.numChildren() == 3))
            {
                //if - end -> 2 children
                //if - else - end -> 3 children

                Node c1 = root.getChild(0);
                Node c2 = root.getChild(1);


                if (c1.Symbol.ToString().Equals("<expression>"))
                {

                    //solve the condition expression, the expression has three child
                    Node ec1 = c1.getChild(0);
                    String c2_str = c1.getChild(1).Symbol.ToString();
                    Node ec3 = c1.getChild(2);

                    //------- the first operand
                    if (ec1.Symbol.ToString().Equals("<expression>"))
                    {
                        //solve the regular exprsstion first
                        whileStr.Append(generateExpressionCode(ec1));
                        whileStr.Append("pop eax\n");
                    }
                    else if (ec1.Symbol.ToString().Equals("<variable_name>"))
                    {
                        String varType = getVarType(strip(ec1), ec1);
                        if(varType.Equals("SDWORD"))
                            whileStr.Append("mov eax, dword ptr [" + strip(ec1) + "]\n");
                    }
                    else if (ec1.Symbol.ToString().Contains("INT["))
                    {
                        whileStr.Append("mov eax, " + strip(ec1) + "\n");
                    }

                    //---------- the econd operand
                    if (ec3.Symbol.ToString().Equals("<expression>"))
                    {
                        //solve the regular exprsstion first
                        whileStr.Append(generateExpressionCode(ec3));
                        whileStr.Append("pop ebx\n");
                    }
                    else if (ec3.Symbol.ToString().Equals("<variable_name>"))
                    {
                        String varType = getVarType(strip(ec3), ec3);
                        if (varType.Equals("SDWORD"))
                            whileStr.Append("mov ebx, dword ptr [" + strip(ec3) + "]\n");
                    }
                    else if (ec3.Symbol.ToString().Contains("INT["))
                    {
                        whileStr.Append("mov ebx, " + strip(ec3) + "\n");
                    }
                   

                    if ((c2_str.Equals("OPERATOR[>]")) || (c2_str.Equals("OPERATOR[<]")) || (c2_str.Equals("OPERATOR[>=]")) || (c2_str.Equals("OPERATOR[<=]")) || (c2_str.Equals("OPERATOR[=]")) || (c2_str.Equals("OPERATOR[<>]")))
                    {
                        String op_str = "";
                        if (c2_str.Equals("OPERATOR[>]"))
                            op_str = "JG";
                        else if (c2_str.Equals("OPERATOR[>=]"))
                            op_str = "JGE";
                        else if (c2_str.Equals("OPERATOR[<]"))
                            op_str = "JL";
                        else if (c2_str.Equals("OPERATOR[<=]"))
                            op_str = "JLE";
                        else if (c2_str.Equals("OPERATOR[=]"))
                            op_str = "JE";
                        else if (c2_str.Equals("OPERATOR[<>]"))
                            op_str = "JNE";
                        //create two lables 
                        label_index++;
                        String lableWhileStart = "@@syslbl_wstart" + label_index.ToString();
                        label_index++;
                        String lableWhileEnd = "@@syslbl_wend" + label_index.ToString();

                        //check condition first
                        whileStr.Append("cmp eax, ebx\n");
                        whileStr.Append(op_str + " " + lableWhileStart + "\n");
                        whileStr.Append("jmp " + lableWhileEnd + "\n");
                        
                        //put the start lable
                        whileStr.Append("\n" + lableWhileStart + ":\n");
                        //------------- while loop start ----------------


                        if (c2.Symbol.ToString().Equals("<compound_stat>"))
                            whileStr.Append(generateCompStatementCode(c2));
                        else
                            whileStr.Append(generateStatementCode(c2));

                        //-------------- while loop end----------------
                        
                        //check condition again
                        whileStr.Append("cmp eax, ebx\n");
                        whileStr.Append(op_str + " " + lableWhileStart + "\n");
                        whileStr.Append("jmp " + lableWhileEnd + "\n");
                        //put the end lable
                        whileStr.Append("\n" + lableWhileEnd + ":\n");

                    }

                }
                else
                {
                    throw new CompilerException("Expression Expected on the while statement", root);
                }
            }

            return whileStr.ToString();
        }
        private static string generateRepeatStatCode(Node root)
        {
            StringBuilder repeatStr = new StringBuilder();


            if (root.numChildren() < 2)
            {
                // error
                throw new CompilerException("Incomplite if statement ", root);
            }
            else if ((root.numChildren() == 2) || (root.numChildren() == 3))
            {
                //if - end -> 2 children
                //if - else - end -> 3 children

                Node c1 = root.getChild(1); //condition
                Node c2 = root.getChild(0); //body


                if (c1.Symbol.ToString().Equals("<expression>"))
                {

                    //solve the condition expression, the expression has three child
                    Node ec1 = c1.getChild(0);
                    String c2_str = c1.getChild(1).Symbol.ToString();
                    Node ec3 = c1.getChild(2);

                    //------- the first operand
                    if (ec1.Symbol.ToString().Equals("<expression>"))
                    {
                        //solve the regular exprsstion first
                        repeatStr.Append(generateExpressionCode(ec1));
                        repeatStr.Append("pop eax\n");
                    }
                    else if (ec1.Symbol.ToString().Equals("<variable_name>"))
                    {
                        String varType = getVarType(strip(ec1), ec1);
                        if (varType.Equals("SDWORD"))
                            repeatStr.Append("mov eax, dword ptr [" + strip(ec1) + "]\n");
                    }
                    else if (ec1.Symbol.ToString().Contains("INT["))
                    {
                        repeatStr.Append("mov eax, " + strip(ec1) + "\n");
                    }

                    //---------- the econd operand
                    if (ec3.Symbol.ToString().Equals("<expression>"))
                    {
                        //solve the regular exprsstion first
                        repeatStr.Append(generateExpressionCode(ec3));
                        repeatStr.Append("pop ebx\n");
                    }
                    else if (ec3.Symbol.ToString().Equals("<variable_name>"))
                    {
                        String varType = getVarType(strip(ec3), ec3);
                        if (varType.Equals("SDWORD"))
                            repeatStr.Append("mov ebx, dword ptr [" + strip(ec3) + "]\n");
                    }
                    else if (ec3.Symbol.ToString().Contains("INT["))
                    {
                        repeatStr.Append("mov ebx, " + strip(ec3) + "\n");
                    }


                    if ((c2_str.Equals("OPERATOR[>]")) || (c2_str.Equals("OPERATOR[<]")) || (c2_str.Equals("OPERATOR[>=]")) || (c2_str.Equals("OPERATOR[<=]")) || (c2_str.Equals("OPERATOR[=]")) || (c2_str.Equals("OPERATOR[<>]")))
                    {
                        String op_str = "";
                        if (c2_str.Equals("OPERATOR[>]"))
                            op_str = "JG";
                        else if (c2_str.Equals("OPERATOR[>=]"))
                            op_str = "JGE";
                        else if (c2_str.Equals("OPERATOR[<]"))
                            op_str = "JL";
                        else if (c2_str.Equals("OPERATOR[<=]"))
                            op_str = "JLE";
                        else if (c2_str.Equals("OPERATOR[=]"))
                            op_str = "JE";
                        else if (c2_str.Equals("OPERATOR[<>]"))
                            op_str = "JNE";
                        //create two lables 
                        label_index++;
                        String lableWhileStart = "@@syslbl_wstart" + label_index.ToString();
                        label_index++;
                        String lableWhileEnd = "@@syslbl_wend" + label_index.ToString();

                        
                        //put the start lable
                        repeatStr.Append("\n" + lableWhileStart + ":\n");
                        //------------- while loop start ----------------


                        if (c2.Symbol.ToString().Equals("<compound_stat>"))
                            repeatStr.Append(generateCompStatementCode(c2));
                        else
                            repeatStr.Append(generateStatementCode(c2));

                        //-------------- while loop end----------------

                        //check condition again
                        repeatStr.Append("cmp eax, ebx\n");
                        repeatStr.Append(op_str + " " + lableWhileEnd + "\n");
                        repeatStr.Append("jmp " + lableWhileStart + "\n");
                        //put the end lable
                        repeatStr.Append("\n" + lableWhileEnd + ":\n");

                    }

                }
                else
                {
                    throw new CompilerException("Expression Expected on the while statement", root);
                }
            }

            return repeatStr.ToString();
        }
        private static string generateIfStatCode(Node root)
        {
            // a if statement may have 2 or 3 nodes, 
                        //expression, a boolean variable, or constant 0/1
                        //block if ture -> this can be a statement, compound statement or procedure call 
                        //block if not ture (optional) -> this also can be a statement, compound statement or procedure call or another if statiemetn 

            StringBuilder ifcondStr = new StringBuilder();
            


                if (root.numChildren() < 2)
                {
                    // error
                    throw new CompilerException("Incomplite if statement ", root);
                }
                else if ((root.numChildren() == 2) ||(root.numChildren() == 3))
                {
                    //if - end -> 2 children
                    //if - else - end -> 3 children

                    Node c1 = root.getChild(0);
                    Node c2 = root.getChild(1);
                    

                    if (c1.Symbol.ToString().Equals("<expression>"))
                    {
                        ifcondStr.Append(generateExpressionCode(c1));
                        ifcondStr.Append("pop eax\n");
                        ifcondStr.Append("cmp eax, 1\n");

                        label_index++;
                        String lableIfTure = "@@syslbl_" + label_index.ToString();
                        label_index++;
                        String lableIfFalse = "@@syslbl_" + label_index.ToString();
                        label_index++;
                        String lableEndif = "@@syslbl_" + label_index.ToString();



                        ifcondStr.Append("je " + lableIfTure + "\n");
                        ifcondStr.Append("jmp " + lableIfFalse + "\n");


                        ifcondStr.Append("\n" + lableIfTure + ":\n");
                        //------------------- code for if block
                        //  if (c2.Symbol.ToString().Equals("<procedure_call>"))
                        //{
                        if (c2.Symbol.ToString().Equals("<compound_stat>"))
                            ifcondStr.Append(generateCompStatementCode(c2));
                        else
                            ifcondStr.Append(generateStatementCode(c2));
                        //}
                        //------------------- end of if block
                        ifcondStr.Append("jmp " + lableEndif + "\n");
                        ifcondStr.Append("\n" + lableIfFalse + ":\n");
                        //------------------- code for else block
                        if ((root.numChildren() == 3))
                        {
                            Node c3 = root.getChild(2);
                            if (c3.Symbol.ToString().Equals("<compound_stat>"))
                                ifcondStr.Append(generateCompStatementCode(c3));
                            else
                                ifcondStr.Append(generateStatementCode(c3));
                        }
                        //-------------------- end of else block
                        ifcondStr.Append("\n" + lableEndif + ":\n");


                        

                    }
                    else
                    {
                        throw new CompilerException("Expression Expected on the if statement", root);
                    }
                }
                else
                {
                    //multiple if else
                }
                return ifcondStr.ToString();
            
        }
        private static string generateAssignStatementCode(Node root)
        {
            StringBuilder assStm = new StringBuilder();
            //assignment has two childs
            //first child is destination v)ariable
            //and the second child can be an expression, a vairalble, a constant


            Node destVarNode = root.getChild(0);

            
            //normal varialbe
            String tempDestVarName = "";
            String tempDestVarType = "";
            if (destVarNode.Symbol.ToString().Equals("<array_select>"))//array variable
            {
                tempDestVarName = strip(destVarNode.getChild(0).getChild(0)).Replace("var$_$", "arr$_$");
                assStm.Append(generateCodeUpdateArrayName(tempDestVarName,destVarNode.getChild(1)));
                tempDestVarName = generatedArrayName;
                tempDestVarType = generatedArrayType;
            }
            else
            {

                tempDestVarName = strip(destVarNode);
                tempDestVarType = getVarType(tempDestVarName, root);
            }
            if (tempDestVarType.Equals("SDWORD")) // destination variable or "the right side" is integer
            {
                #region Assignmnet to Integer
                Node expression = root.getChild(1); // left side as node
                String expressionStr = expression.Symbol.ToString(); // left side as string

                if (expressionStr.Equals("<expression>")) // if left side is an entire expression
                {
                    string expressionValue = generateExpressionCode(expression);
                    assStm.Append(expressionValue);

                    if (exprResutType.Equals("SDWORD")) // int = int (expression)
                        assStm.Append("POP " + tempDestVarName + "\n");
                    else if (exprResutType.Equals("REAL4")) //int = real
                        if (allowRealToInt)
                        {
                            assStm.Append("pop temp_real4\n");
                            assStm.Append("fld temp_real4\n");
                            assStm.Append("FISTP " + tempDestVarName + "\n");
                        }
                        else
                            throw new CompilerException("Real cannot be assigned to integer", root);

                }

                else if ((expressionStr.Equals("<variable_name>")) || (expressionStr.Equals("<array_select>"))) // if the left side is a vairable
                {
                    String tempSrcVarName = "";
                    String tempSrcVarType = "";
                    //Node srcVarNode = 
                    //*************************** need  to revise this part*************************
                    if (expressionStr.Equals("<array_select>"))//array variable
                    {
                        tempDestVarName = strip(expression.getChild(0).getChild(0)).Replace("var$_$", "arr$_$");
                        assStm.Append(generateCodeUpdateArrayName(tempDestVarName, expression.getChild(1)));
                        tempDestVarName = generatedArrayName;
                        tempDestVarType = generatedArrayType;
                    }
                    else
                    {
                        tempSrcVarName = strip(expression.getChild(0));
                        tempSrcVarType = getVarType(tempSrcVarName, expression);
                    }
                    if (tempSrcVarType.Equals("SDWORD") || tempSrcVarType.Equals("SDWORDCONST")) //int = int var
                        assStm.Append("MOV " + tempDestVarName + ", " + tempSrcVarName + " \n");
                    else if (tempSrcVarType.Equals("REAL4") || tempSrcVarType.Equals("REAL4CONST")) // int = real var
                    {
                        if (allowRealToInt)
                        {
                            assStm.Append("fld " + tempSrcVarName + "\n");
                            assStm.Append("FISTP " + tempDestVarName + "\n");
                        }
                        else
                            throw new CompilerException("Real variable Cannot be assigned to Integer variable", expression);
                    }
                }
                else if (expressionStr.Contains("INT[")) // if the left side is an integer constant
                {
                    //int = int const
                    assStm.Append("MOV " + tempDestVarName + ", " + strip(expression) + "\n");
                }
                else if (expressionStr.Contains("REAL[")) // if the left side is a rean constant
                {
                    //int = real const
                    if (allowRealToInt)
                    {
                        int x = (int)Math.Round(double.Parse(strip(expression)));
                        assStm.Append("MOV " + tempDestVarName + ", " + x.ToString() + "\n");
                    }
                    else
                        throw new CompilerException("Real constant Cannot be assigned to Integer variable", expression);//error
                }
                #endregion
            }
            else if (tempDestVarType.Equals("REAL4")) //destination variable or "the right side" is real
            {
                #region Assignment to real
                Node expression = root.getChild(1);
                String expressionStr = expression.Symbol.ToString();

                if (expressionStr.Equals("<expression>"))// if left side is an entire expression
                {
                    string expressionValue = generateExpressionCode(expression);
                    assStm.Append(expressionValue);
                    if (exprResutType.Equals("SDWORD")) // real = int (expression)
                    {
                        assStm.Append("pop temp_sdword\n");
                        assStm.Append("fild temp_sdword\n");
                        assStm.Append("fstp " + tempDestVarName + "\n");
                    }
                    else if (exprResutType.Equals("REAL4")) //real = real
                    {
                        assStm.Append("pop " + tempDestVarName + "\n");
                    }


                }
                else if (expressionStr.Equals("<variable_name>"))// if left side is a variable
                {

                    String tempSrcVarName = strip(expression.getChild(0));
                    String tempSrcVarType = getVarType(tempSrcVarName, expression);
                    if (tempSrcVarType.Equals("SDWORD")) //real = int var
                    {
                        assStm.Append("fild " + tempSrcVarName + "\n");
                        assStm.Append("fst qword ptr [" + tempDestVarName + "]\n");

                    }
                    else if (tempSrcVarType.Equals("REAL4")) // real = real var
                    {
                        assStm.Append("fld " + tempSrcVarName + "\n");
                        assStm.Append("fst qword ptr [" + tempDestVarName + "]\n");
                    }

                }
                else if ((expressionStr.Contains("INT[")) || (expressionStr.Contains("REAL["))) //if expression is a constant value
                {
                    string decimalString = "";
                    if (expressionStr.Contains("INT[")) decimalString = ".0";

                    // real = int const
                    //add the constat as a variable
                    real_constant_index++;
                    String newConstToVarName = "real$_$const_" + real_constant_index.ToString();
                    verdec.Append(newConstToVarName + " real4 " + strip(expression) + decimalString + "\n");

                    //assign it to destination varialbe
                    assStm.Append("fld " + newConstToVarName + "\n");
                    assStm.Append("fst " + tempDestVarName + "\n");
                }

                else if (expressionStr.Contains("STRING["))
                {
                    Console.WriteLine(expression.GetType().ToString());
                }
                #endregion
            }
            else if (tempDestVarType.Equals("BYTE"))
            {

                #region Assignmnet to Char
                Node expression = root.getChild(1); // left side as node
                String expressionStr = expression.Symbol.ToString(); // left side as string

                if (expressionStr.Equals("<expression>")) // if left side is an entire expression
                {
                    string expressionValue = generateExpressionCode(expression);
                    assStm.Append(expressionValue);

                    if (exprResutType.Equals("BYTE")) // int = int (expression)
                        assStm.Append("POP " + tempDestVarName + "\n");


                }

                else if (expressionStr.Equals("<variable_name>")) // if the left side is a vairable
                {

                    String tempSrcVarName = strip(expression.getChild(0));
                    String tempSrcVarType = getVarType(tempSrcVarName, expression);
                    if (tempSrcVarType.Equals("BYTE")) //int = int var
                        assStm.Append("MOV " + tempDestVarName + ", " + tempSrcVarName + " \n");

                }
                else if (expressionStr.Contains("CHAR[")) // if the left side is an integer constant
                {
                    //int = int const
                    assStm.Append("MOV " + tempDestVarName + ", '" + strip(expression) + "'\n");
                }
                else if (expressionStr.Contains("INT[")) // if the left side is an integer constant
                {
                    //int = int const
                    assStm.Append("MOV " + tempDestVarName + ", " + strip(expression) + "\n");
                }

                #endregion
            }


            //if second cilde is expression
           
            assStm.Append("\n");

            return assStm.ToString();
        }

        private static string generateCodeUpdateArrayName(string varName, Node IndexNode)
        {
            StringBuilder storeIndexStr = new StringBuilder();
            storeIndexStr.Append("\n");

            String defType = getVarType(varName, IndexNode);
            arryDef arrayDefType = (arryDef)typeDefinationList.GetByIndex(typeDefinationList.IndexOfKey(defType));


            
            if (IndexNode.Symbol.ToString().Equals("<variable_name>")) //index is a variable
            {
                String indexVarName = strip(IndexNode.getChild(0));
                if (getVarType(indexVarName, IndexNode).Equals("SDWORD")) //index mustbe an integer
                {
                    //say we are trying to access myarry[i] ; here varName = myarray
                    // and say i=7 in this case ; indexVarname =i
                    // myarray has index [5..9] ; lowIndex = 5;

                    storeIndexStr.Append("push eax\n");
                    storeIndexStr.Append("push ebx\n");
                    storeIndexStr.Append("mov eax, " + indexVarName + "\n"); //put 7 to eax
                    storeIndexStr.Append("mov ebx, " + arrayDefType.LowIndex.ToString() + "\n"); // put 5 to ebx
                    storeIndexStr.Append("sub eax, ebx\n"); // 7-5 will give use relative index
                    storeIndexStr.Append("mov ebx, 4\n");//4 byte each element, multiply by 4 will give
                    storeIndexStr.Append("mul ebx\n"); //  number of byte wee need to look ahed 
                   
                    storeIndexStr.Append("mov ebx, offset " + varName + "\n");
                    storeIndexStr.Append("add eax, ebx\n"); // add total bytes to the starting address of array
                    storeIndexStr.Append("mov ebp, eax\n"); // store the index to ebp as the function required

                    storeIndexStr.Append("pop ebx\n"); //restore eax, ebx
                    storeIndexStr.Append("pop eax\n");

                    generatedArrayName = "dword ptr [ebp]";
                }
                else
                    throw new CompilerException("Index of array must me an Integer", IndexNode);
            }
            else if (IndexNode.Symbol.ToString().Equals("<expression>"))
            {
                storeIndexStr.Append(generateExpressionCode(IndexNode));
                if (!(exprResutType.Equals("SDWORD")))
                    throw new CompilerException("Array Index can be Integer only", IndexNode);

                storeIndexStr.Append("pop ebp\n");
                storeIndexStr.Append("push eax\n");
                storeIndexStr.Append("push ebx\n");
                storeIndexStr.Append("mov eax, ebp\n"); //put 7 to eax
                storeIndexStr.Append("mov ebx, " + arrayDefType.LowIndex.ToString() + "\n"); // put 5 to ebx
                storeIndexStr.Append("sub eax, ebx\n"); // 7-5 will give use relative index
                storeIndexStr.Append("mov ebx, 4\n");//4 byte each element, multiply by 4 will give
                storeIndexStr.Append("mul ebx\n"); //  number of byte wee need to look ahed 

                storeIndexStr.Append("mov ebx, offset " + varName + "\n");
                storeIndexStr.Append("add eax, ebx\n"); // add total bytes to the starting address of array
                storeIndexStr.Append("mov ebp, eax\n"); // store the index to ebp as the function required

                storeIndexStr.Append("pop ebx\n"); //restore eax, ebx
                storeIndexStr.Append("pop eax\n");

                generatedArrayName = "dword ptr [ebp]";

            }
            else if (IndexNode.Symbol.ToString().Contains("INT[")) //index is constant
            {
                int arrayIndex = int.Parse(strip(IndexNode));
                arrayIndex = (arrayIndex - arrayDefType.LowIndex) * 4;
                generatedArrayName = "dword ptr [" + varName + "+" + arrayIndex.ToString() + "]";
            }

            if (arrayDefType.OriginalType.Equals("ID[INTEGER]"))
                generatedArrayType = "SDWORD";
            return storeIndexStr.ToString();
        }
        private static string generateExpressionCode(Node root)
        {
            StringBuilder expStr = new StringBuilder();
            //an expression node has three child

            Node c1 = root.getChild(0); //c1 can be a variable or a constant
            Node c2 = root.getChild(1); // c2 can be operators
            Node c3 = root.getChild(2);// c3 can be constant, variable, expression

            String c1_str = c1.Symbol.ToString();
            String c2_str = c2.Symbol.ToString();
            String c3_str = c3.Symbol.ToString();

            String var1Name = strip(c1);
            String var2Name = strip(c3);

            String var1Type="";
            String var2Type = "";

            if (c1_str.Equals("<expression>")) //if the first operand is expression solve the expression first
            {
                expStr.Append(generateExpressionCode(c1));
            }
            if (c3_str.Equals("<expression>")) //if the second operand is expression solve the expression first
            {
                expStr.Append(generateExpressionCode(c3));
            }

            //at this point nested expresisons (if any!) on the both side of the operator has been resolved

            if (c2_str.Equals("OPERATOR[=]") ||
                c2_str.Equals("OPERATOR[<]") || c2_str.Equals("OPERATOR[<=]") ||
                c2_str.Equals("OPERATOR[>]") || c2_str.Equals("OPERATOR[>=]") ||
                c2_str.Equals("AND") || c2_str.Equals("OR"))
            {
                //LETS ASSUME OPERANDS ARE EITHER VARIABLE OR CONSTANT
                if (c1_str.Equals("<expression>")) //if the first operand was a result of expression, then pop the result
                    expStr.Append("POP EAX\n");
                else if (c1_str.Equals("<array_select>")) //if ifrst operand is an array variable
                {
                    String tempArrayVarName = strip(c1.getChild(0).getChild(0)).Replace("var$_$", "arr$_$");
                    expStr.Append(generateCodeUpdateArrayName(tempArrayVarName, c1.getChild(1)));
                    tempArrayVarName = generatedArrayName;

                    expStr.Append("MOV EAX, " + tempArrayVarName + "\n");

                }
                else //first operand was not an expression, just use the value of variable or constant
                    expStr.Append("MOV EAX, " + strip(c1) + "\n");

                //------------second operand--------------- move value to ebx and do the edition
                //do same for the second operand and add/sub with the first operand
                if (c3_str.Equals("<expression>"))
                {
                    expStr.Append("POP EBX\n");
                }
                else if (c3_str.Equals("<array_select>")) //if ifrst operand is an array variable
                {
                    String tempArrayVarName = strip(c3.getChild(0).getChild(0)).Replace("var$_$", "arr$_$");
                    expStr.Append(generateCodeUpdateArrayName(tempArrayVarName, c3.getChild(1)));
                    tempArrayVarName = generatedArrayName;

                    expStr.Append("MOV EBX, " + tempArrayVarName + "\n");

                }
                else // just a variable or a constant 
                {
                    expStr.Append("MOV EBX, " + strip(c3) + "\n");
                }

                //AT THIS POINTS OPERANDS ARE IN EAX AND EBX

                if (c2_str.Equals("AND") || c2_str.Equals("OR"))
                {
                    expStr.Append(c2_str+" EAX, EBX\n");
                    expStr.Append("push EAX\n");
                }
                else
                {
                    String op_str = "";
                    if (c2_str.Equals("OPERATOR[>]"))
                        op_str = "JG";
                    else if (c2_str.Equals("OPERATOR[>=]"))
                        op_str = "JGE";
                    else if (c2_str.Equals("OPERATOR[<]"))
                        op_str = "JL";
                    else if (c2_str.Equals("OPERATOR[<=]"))
                        op_str = "JLE";
                    else if (c2_str.Equals("OPERATOR[=]"))
                        op_str = "JE";

                    label_index++;
                    String lblCondTrue = "@@lblCondTrue" + label_index.ToString();
                    label_index++;
                    String lblCondFalse = "@@lblCondFalse" + label_index.ToString();



                    expStr.Append("CMP EAX, EBX\n");
                    expStr.Append(op_str + " " + lblCondTrue + "\n");
                    expStr.Append("push 0\n");
                    expStr.Append("jmp " + lblCondFalse + "\n");
                    expStr.Append("\n" + lblCondTrue + ":\n");
                    expStr.Append("push 1\n");
                    expStr.Append("\n" + lblCondFalse + ":\n");
                }
                exprResutType = "SDWORD";



            }
            else
            {

                #region arithmetic operation
                if (((c1_str.Equals("<expression>") || c3_str.Equals("<expression>")) && exprResutType.Equals("REAL4")) ||
                        (c1_str.Contains("REAL[") || c1_str.Contains("REAL[")) ||
                        (c1_str.Equals("<variable_name>") && getVarType(strip(c1), c1).Equals("REAL4")) ||
                        (c1_str.Equals("<variable_name>") && getVarType(strip(c1), c1).Equals("REAL4CONST")) ||
                        (c3_str.Equals("<variable_name>") && getVarType(strip(c3), c3).Equals("REAL4")) ||
                        (c3_str.Equals("<variable_name>") && getVarType(strip(c3), c3).Equals("REAL4CONST")) ||
                        c2_str.Equals("OPERATOR[/]") ||
                        allowRealToInt
                        )
                {
                    String op_str = "";
                    if (c2_str.Equals("OPERATOR[+]"))
                        op_str = "FADDP";
                    else if (c2_str.Equals("OPERATOR[-]"))
                        op_str = "FSUBP";
                    else if (c2_str.Equals("OPERATOR[*]"))
                        op_str = "FMULP";
                    else if (c2_str.Equals("OPERATOR[/]"))
                        op_str = "FDIVP";

                    #region realdivision


                    //================ first operand ========================
                    if (c1_str.Equals("<expression>")) //if the first operand was a result of expression, then pop the result
                    {

                        if (exprResutType.Equals("SDWORD"))
                        {
                            expStr.Append("pop	dword ptr [temp_sdword]\n"); //load from stack to temp variable
                            expStr.Append("fild temp_sdword\n"); //push temp var to st
                        }
                        else if (exprResutType.Equals("REAL4"))
                        {
                            expStr.Append("pop	dword ptr [temp_real4]\n"); //load from stack to temp variable
                            expStr.Append("fld temp_real4\n"); //push temp var to st
                        }

                    }

                    else if (c1_str.Equals("<variable_name>")) //first operand is an vairable
                    {
                        var1Type = getVarType(var1Name, c1);
                        if (var1Type.Equals("SDWORD") || var1Type.Equals("SDWORDCONST")) // first operand is an integer variablie
                        {
                            expStr.Append("fild " + var1Name + "\n");//push integer variable to st
                        }
                        else if (var1Type.Equals("REAL4") || var1Type.Equals("REAL4CONST")) // first operand is an integer variablie
                        {
                            expStr.Append("fld " + var1Name + "\n");
                        }
                    }
                    else if ((c1_str.Contains("INT[")) || (c1_str.Contains("REAL["))) //if expression is a constant value
                    {
                        string decimalString = "";
                        if (c1_str.Contains("INT[")) decimalString = ".0"; //convert integer constant to real

                        // real = int const
                        //add the constat as a variable
                        real_constant_index++;
                        String newConstToVarName = "real$_$const_" + real_constant_index.ToString();
                        verdec.Append(newConstToVarName + " real4 " + var1Name + decimalString + "\n"); //var1Name is a const here

                        //assign it to destination varialbe
                        expStr.Append("fld " + newConstToVarName + "\n");
                    }

                    //================ second operand ========================
                    //do same for the second operand and add/sub with the first operand

                    if (c3_str.Equals("<expression>")) //if the first operand was a result of expression, then pop the result
                    {
                        if (exprResutType.Equals("SDWORD"))
                        {
                            expStr.Append("pop	dword ptr [temp_sdword]\n"); //load from stack to temp variable
                            expStr.Append("fild temp_sdword\n"); //push temp var to st
                        }
                        else if (exprResutType.Equals("REAL4"))
                        {
                            expStr.Append("pop	dword ptr [temp_real4]\n"); //load from stack to temp variable
                            expStr.Append("fld temp_real4\n"); //push temp var to st
                        }
                    }

                    else if (c3_str.Equals("<variable_name>")) //first operand is an vairable
                    {
                        var2Type = getVarType(var2Name, c3);
                        if (var2Type.Equals("SDWORD") || var2Type.Equals("SDWORDCONST")) // first operand is an integer variablie
                        {
                            expStr.Append("fild " + var2Name + "\n");//push integer variable to st
                        }
                        else if (var2Type.Equals("REAL4") || var2Type.Equals("REAL4CONST")) // first operand is an integer variablie
                        {
                            expStr.Append("fld " + var2Name + "\n");
                        }
                    }
                    else if ((c3_str.Contains("INT[")) || (c3_str.Contains("REAL["))) //if expression is a constant value
                    {
                        string decimalString = "";
                        if (c3_str.Contains("INT[")) decimalString = ".0"; //convert integer constant to real

                        // real = int const
                        //add the constat as a variable
                        real_constant_index++;
                        String newConstToVarName = "real$_$const_" + real_constant_index.ToString();
                        verdec.Append(newConstToVarName + " real4 " + var2Name + decimalString + "\n"); //var1Name is a const here

                        //assign it to destination varialbe
                        expStr.Append("fld " + newConstToVarName + "\n");
                    }

                    //at this point both operands (const or variable) has been handaled and puhsed to st1 and st


                    expStr.Append(op_str + "	st(1),st\n");
                    expStr.Append("fstp	dword ptr [temp_real4]\n");

                    //push the result on the stack
                    expStr.Append("PUSH temp_real4\n\n");
                    exprResutType = "REAL4"; // a real value was pushed to stack
                    #endregion
                }
                else  //Integer operation
                {
                    //Integer Addition and substruction
                    if ((c2_str.Equals("OPERATOR[+]")) || (c2_str.Equals("OPERATOR[-]")))
                    {
                        String op_str = "ADD";
                        if (c2_str.Equals("OPERATOR[-]"))
                            op_str = "SUB";
                        //--------------- first operand----------------- move to eax
                        if (c1_str.Equals("<expression>")) //if the first operand was a result of expression, then pop the result
                            expStr.Append("POP EAX\n");
                        else if (c1_str.Equals("<array_select>")) //if ifrst operand is an array variable
                        {
                            String tempArrayVarName = strip(c1.getChild(0).getChild(0)).Replace("var$_$", "arr$_$");
                            expStr.Append(generateCodeUpdateArrayName(tempArrayVarName, c1.getChild(1)));
                            tempArrayVarName = generatedArrayName;

                            expStr.Append("MOV EAX, " + tempArrayVarName + "\n");

                        }
                        else //first operand was not an expression, just use the value of variable or constant
                            expStr.Append("MOV EAX, " + strip(c1) + "\n");

                        //------------second operand--------------- move value to ebx and do the edition
                        //do same for the second operand and add/sub with the first operand
                        if (c3_str.Equals("<expression>"))
                        {
                            expStr.Append("POP EBX\n");
                            expStr.Append(op_str + " EAX, EBX\n");
                        }
                        else if (c3_str.Equals("<array_select>")) //if ifrst operand is an array variable
                        {
                            String tempArrayVarName = strip(c3.getChild(0).getChild(0)).Replace("var$_$", "arr$_$");
                            expStr.Append(generateCodeUpdateArrayName(tempArrayVarName, c3.getChild(1)));
                            tempArrayVarName = generatedArrayName;

                            expStr.Append("MOV EBX, " + tempArrayVarName + "\n");
                            expStr.Append(op_str + " EAX, EBX\n");

                        }
                        else // just a variable or a constant 
                        {
                            expStr.Append(op_str + " EAX, " + strip(c3) + "\n");
                        }

                        //push the result on the stack
                        expStr.Append("PUSH EAX\n\n");
                    }

                    //Integer multiplication and division
                    else if ((c2_str.Equals("OPERATOR[*]")) || (c2_str.Equals("OPERATOR[DIV]")))
                    {
                        String op_str = "MUL";
                        if (c2_str.Equals("OPERATOR[DIV]"))
                        {
                            op_str = "DIV";
                            expStr.Append("mov edx, 0\n");
                        }
                        //------------ first operand -------------------
                        if (c1_str.Equals("<expression>")) //if the first operand was a result of expression, then pop the result
                            expStr.Append("POP EAX\n");
                        else if (c1_str.Equals("<array_select>")) //if ifrst operand is an array variable
                        {
                            String tempArrayVarName = strip(c1.getChild(0).getChild(0)).Replace("var$_$", "arr$_$");
                            expStr.Append(generateCodeUpdateArrayName(tempArrayVarName, c1.getChild(1)));
                            tempArrayVarName = generatedArrayName;

                            expStr.Append("MOV EAX, " + tempArrayVarName + "\n");
                        }
                        else //first operand was not an expression, just use the value of variable or constant
                            expStr.Append("MOV EAX, " + strip(c1) + "\n");

                        //--------- second operand -------------
                        //do same for the second oprand
                        if (c3_str.Equals("<expression>"))
                            expStr.Append("POP EBX\n");
                        else if (c3_str.Equals("<array_select>")) //if ifrst operand is an array variable
                        {
                            String tempArrayVarName = strip(c3.getChild(0).getChild(0)).Replace("var$_$", "arr$_$");
                            expStr.Append(generateCodeUpdateArrayName(tempArrayVarName, c3.getChild(1)));
                            tempArrayVarName = generatedArrayName;
                            expStr.Append("MOV EBX, " + tempArrayVarName + "\n");
                        }
                        else
                            expStr.Append("MOV EBX, " + strip(c3) + "\n");

                        //do the multiplication/division and push the result to stakc
                        expStr.Append(op_str + " EBX\n" +
                                       "PUSH EAX\n\n");
                    }
                    exprResutType = "SDWORD"; // a Integer value was pushed to stack
                }
                #endregion
            }
            return expStr.ToString();

        }
        private static string generateProcedureCallCode(Node root)
        {
            StringBuilder procStr = new StringBuilder();
            
            String firstChildStr = root.getChild(0).Symbol.ToString();


            if (firstChildStr.Equals("ID[write]"))
            {
                int numChildren = root.numChildren();
                for (int childNum = 0; childNum < numChildren; ++childNum)
                {
                    Node cNode = root.getChild(childNum);

                    if (cNode.Symbol.ToString().Contains("STRING["))
                    {
                        string_constant_index++;
                        verdec.Append("pStr$" + string_constant_index.ToString() + " BYTE  \"" + strip(cNode) + "\",0\n");

                        procStr.Append("mov  EDX, OFFSET pStr$" + string_constant_index.ToString() + "\n");
                        procStr.Append("invoke crt_printf, addr format$String, EDX\n");
                    }

                    else if (cNode.Symbol.ToString().Equals("<variable_name>"))
                    {
                        string tempVarName = strip(cNode.getChild(0));
                        string tempVarType = getVarType(tempVarName, cNode);
                        if (tempVarType.Equals("SDWORD"))
                        {
                            procStr.Append("invoke crt_printf, addr format$Int, " + tempVarName + "\n");
                        }
                        else if (tempVarType.Equals("BYTE"))
                        {
                            procStr.Append("invoke crt_printf, addr format$Char, dword ptr[" + tempVarName + "]\n");
                        }
                        else if (tempVarType.Equals("REAL4"))
                        {
                            procStr.Append("fld " + tempVarName + "\n");
                            procStr.Append("fst out_real8\n");
                            procStr.Append("invoke crt_printf, ADDR format$Float, out_real8\n");

                        }

                    }
                    else if (cNode.Symbol.ToString().Equals("<expression>"))
                    {
                        procStr.Append(generateExpressionCode(cNode));
                        if (exprResutType.Equals("SDWORD"))
                        {
                            procStr.Append("pop eax\n");
                            procStr.Append("invoke crt_printf, addr format$Int, eax\n");
                        }
                        else if (exprResutType.Equals("REAL4"))
                        {
                            procStr.Append("pop	dword ptr [temp_real4]\n");
                            procStr.Append("fld temp_real4\n");
                            procStr.Append("fst out_real8\n");
                            procStr.Append("invoke crt_printf, ADDR format$Float, out_real8\n");

                        }

                    }
                    

                }
            }
            else if (firstChildStr.Equals("ID[writeln]"))
            {
                int numChildren = root.numChildren();
                for (int childNum = 0; childNum < numChildren; ++childNum)
                {
                    Node cNode = root.getChild(childNum);

                    if (cNode.Symbol.ToString().Contains("STRING["))
                    {
                        string_constant_index++;
                        verdec.Append("pStr$" + string_constant_index.ToString() + " BYTE  \"" + strip(cNode) + "\",0\n");

                        procStr.Append("mov  EDX, OFFSET pStr$" + string_constant_index.ToString() + "\n");
                        procStr.Append("invoke crt_printf, addr format$String, EDX\n");
                    }

                    else if (cNode.Symbol.ToString().Equals("<variable_name>"))
                    {
                        string tempVarName = strip(cNode.getChild(0));
                        string tempVarType = getVarType(tempVarName, cNode);
                        if (tempVarType.Equals("SDWORD"))
                        {
                            procStr.Append("invoke crt_printf, addr format$Int, " + tempVarName + "\n");
                        }
                        if (tempVarType.Equals("BYTE"))
                        {
                            procStr.Append("invoke crt_printf, addr format$Char, dword ptr[" + tempVarName + "]\n");
                        }
                        else if (tempVarType.Equals("REAL4"))
                        {
                            procStr.Append("fld " + tempVarName + "\n");
                            procStr.Append("fst out_real8\n");
                            procStr.Append("invoke crt_printf, ADDR format$Float, out_real8\n");

                        }

                    }
                    else if (cNode.Symbol.ToString().Equals("<expression>"))
                    {
                        procStr.Append(generateExpressionCode(cNode));
                        if (exprResutType.Equals("SDWORD"))
                        {
                            procStr.Append("pop eax\n");
                            procStr.Append("invoke crt_printf, addr format$Int, eax\n");
                        }
                        else if (exprResutType.Equals("REAL4"))
                        {
                            procStr.Append("pop	dword ptr [temp_real4]\n");
                            procStr.Append("fld temp_real4\n");
                            procStr.Append("fst out_real8\n");
                            procStr.Append("invoke crt_printf, ADDR format$Float, out_real8\n");

                        }

                    }
                    else if (cNode.Symbol.ToString().Equals("<array_select>"))
                    {
                        String tempArrayVarName = strip(cNode.getChild(0).getChild(0)).Replace("var$_$", "arr$_$");
                        procStr.Append(generateCodeUpdateArrayName(tempArrayVarName, cNode.getChild(1)));
                        tempArrayVarName = generatedArrayName;
                        String tempArryVarType = generatedArrayType;
                        if (tempArryVarType.Equals("SDWORD"))
                        {
                            procStr.Append("invoke crt_printf, addr format$Int, " + tempArrayVarName + "\n");
                        }

                        
                    }

                }
                procStr.Append("invoke crt_printf, ADDR format$Char, 10\n");
            }
            else if (firstChildStr.Equals("ID[read]"))
            {
                int numChildren = root.numChildren();
                if (numChildren == 2)
                {
                    Node cNode = root.getChild(1);
                    if (cNode.Symbol.ToString().Equals("<variable_name>"))
                    {
                        string tempVarName = strip(cNode.getChild(0));
                        string tempVarType = getVarType(tempVarName, cNode);
                        if (tempVarType.Equals("SDWORD"))
                        {
                            procStr.Append("invoke crt_scanf, addr format$Int, addr " + tempVarName + "\n");
                        }
                        else if (tempVarType.Equals("REAL4"))
                        {
                            procStr.Append("invoke crt_scanf, addr format$Int, addr " + tempVarName + "\n");
                        }

                    }
                    else
                        throw new CompilerException("Read function accept a variable only!", root);

                }
                else
                    throw new CompilerException("Read function accept one argument only!", root);
                
            }


            return procStr.ToString();
        }
        private static void checkVar(string varname, Node root, bool isDuplicate)
        {
            if (isDuplicate) //check for duplicate vairalbe
            {
                if (varTypeList.ContainsKey(varname))
                    throw new CompilerException("Variable '" + varname.Replace("var$_$","") + "' Already Exist.", root);
                else
                    return;
            }
            else //check for existance 
            {
                if (varTypeList.ContainsKey(varname))
                    return;
                else
                    throw new CompilerException("Variable '" + varname.Replace("var$_$", "") + "' cannot resolve.", root);
            }
        }
        private static string getVarType(string varname, Node root)
        {
            if (varTypeList.ContainsKey(varname))
            {
                return (string)varTypeList.GetByIndex(varTypeList.IndexOfKey(varname));
            }
            else
                throw new CompilerException("Variable '" + varname.Replace("var$_$", "") + "' cannot resolve.", root);
        }
        private static string strip(Node root)
        {
            if (root==null) return "null";

            String nodeStr = root.Symbol.ToString();
            if (nodeStr.Equals("<variable_name>"))
                nodeStr = root.getChild(0).Symbol.ToString();
            else if (nodeStr.Equals("<expression>"))
                return nodeStr;

            if (nodeStr.Contains("ID["))
                return "var$_$" + nodeStr.Substring((nodeStr.IndexOf("[") + 1),
                (nodeStr.Length - (nodeStr.IndexOf("[") + 2)));

            return nodeStr.Substring((nodeStr.IndexOf("[") + 1),
                (nodeStr.Length - (nodeStr.IndexOf("[") + 2)));
        }
        private static void generatecode(StringBuilder asmCode)
        {
            asmCode.Append("TITLE " + progName + " (" + progName + ".asm)\n");
            //asmCode.Append("INCLUDE Irvine32.inc\n");
            asmCode.Append("include masm32rt.inc\n");
            
            asmCode.Append("\n");

            asmCode.Append(".data\n");
            asmCode.Append(";System vairables\n");
            asmCode.Append("format$Char db \"%c\",0\n");
            asmCode.Append("format$String db \"%s\",0\n");
            asmCode.Append("format$Int db \"%d\",0\n");
            asmCode.Append("format$Float db \"%f\",0\n");
            asmCode.Append("out_real8 real8 ?\n");
            asmCode.Append("temp_real4 real4 ?\n");
            asmCode.Append("temp_sdword sdword ?\n");
            asmCode.Append("\n");
            asmCode.Append(";Program vairables\n");
            
	
            

            //------- Declare variable here
            asmCode.Append(verdec.ToString());

            asmCode.Append("\n");
            //--------End of vaiable
            asmCode.Append(".code\n");
            asmCode.Append("start:\n\n");
            //--------Main Program
            asmCode.Append(mainCompStatement.ToString());
            
                    
            //--------End of Main program
            asmCode.Append("\n");
            asmCode.Append("invoke crt_printf, addr format$Char, 10\n");
            asmCode.Append("inkey \"Press any key to exit...\"\n");
            asmCode.Append("exit\n");
            asmCode.Append("end start\n");


        }
    }
}