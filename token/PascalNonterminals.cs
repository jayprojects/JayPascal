using System;
namespace JayPascal
{
	
	public class Nonterminal : Symbol
	{
		
		private String name;
		public Nonterminal(String name)
		{
			this.name = name;
		}
		
		public override String ToString()
		{
			return "<" + this.name + ">";
		}
	}
	
	
	public class PascalNonterminals
	{
		public static readonly Nonterminal ADDING_OPERATOR = new Nonterminal("plus_operator");
		public static readonly Nonterminal ARRAY_SELECT = new Nonterminal("array_select");
		public static readonly Nonterminal ARRAY_TYPE = new Nonterminal("array_type");
		public static readonly Nonterminal ASS_STAT = new Nonterminal("ass_stat");
		//public static readonly Nonterminal ASSIGNMENT_LIST = new Nonterminal("assignment_list");
		public static readonly Nonterminal PROGRAM_BLOCK = new Nonterminal("program_block");
		//public static readonly Nonterminal BOOLEAN_EXPRESSION = new Nonterminal("boolean_expression");
		public static readonly Nonterminal COMPOUND_STAT = new Nonterminal("compound_stat");
		public static readonly Nonterminal CONSTANT = new Nonterminal("constant");
		public static readonly Nonterminal CONSTANT_DEFINITION = new Nonterminal("constant_definition");
		public static readonly Nonterminal CONSTANT_DEFINITIONS = new Nonterminal("constant_definitions");
		//public static readonly Nonterminal DEREFERENCE = new Nonterminal("dereference");
		public static readonly Nonterminal EXPRESSION = new Nonterminal("expression");
		public static readonly Nonterminal FACTOR = new Nonterminal("factor");
		public static readonly Nonterminal FIELD_SELECT = new Nonterminal("field_select");
		public static readonly Nonterminal FOR_STAT = new Nonterminal("for_stat");
		public static readonly Nonterminal WHILE_STAT = new Nonterminal("while_stat");
		public static readonly Nonterminal GOTO_STAT = new Nonterminal("goto_stat");
        public static readonly Nonterminal IF_STAT = new Nonterminal("if_stat");
		public static readonly Nonterminal LABEL = new Nonterminal("label");
		public static readonly Nonterminal LABEL_DECLARATIONS = new Nonterminal("label_declarations");
		public static readonly Nonterminal LABELED_STATEMENT = new Nonterminal("labeled_statement");
		public static readonly Nonterminal IDENTIFIER_LIST = new Nonterminal("identifier_list");
		public static readonly Nonterminal MULTIPLYING_OPERATOR = new Nonterminal("mult_operator");
		public static readonly Nonterminal PARAMETER_GROUP = new Nonterminal("parameter_group");
		public static readonly Nonterminal PROCEDURE_CALL = new Nonterminal("procedure_call");
		public static readonly Nonterminal PROCEDURE_DECLARATION = new Nonterminal("procedure_declaration");
		public static readonly Nonterminal PROCFUNC_DECLARATIONS = new Nonterminal("procfunc_declarations");
		public static readonly Nonterminal PROGRAM = new Nonterminal("program");
		public static readonly Nonterminal PROGRAM_HEADING = new Nonterminal("program_heading");
		public static readonly Nonterminal RANGE = new Nonterminal("RANGE");
		public static readonly Nonterminal RELATIONAL_OPERATOR = new Nonterminal("relational_opeator");
		public static readonly Nonterminal REPEAT_STAT = new Nonterminal("repeat_stat");
		public static readonly Nonterminal SCALAR_TYPE = new Nonterminal("scalar_type");
		public static readonly Nonterminal SET = new Nonterminal("set");
		public static readonly Nonterminal SET_TYPE = new Nonterminal("set_type");
		public static readonly Nonterminal SUBRANGE_TYPE = new Nonterminal("subrange_type");
		public static readonly Nonterminal STATEMENT = new Nonterminal("statement");
		public static readonly Nonterminal TERM = new Nonterminal("term");
		public static readonly Nonterminal TYPE_DEFINITION = new Nonterminal("type_definition");
		public static readonly Nonterminal TYPE_DEFINITIONS = new Nonterminal("type_definitions");
		public static readonly Nonterminal TYPE_IDENTIFIER = new Nonterminal("type_identifier");
		public static readonly Nonterminal UNSIGNED_NUMBER = new Nonterminal("unsigned_number");
		public static readonly Nonterminal VARIABLE = new Nonterminal("variable");
		public static readonly Nonterminal VARIABLE_DECLARATION = new Nonterminal("variable_declaration");
		public static readonly Nonterminal VARIABLE_DECLARATIONS = new Nonterminal("variable_declarations");
		public static readonly Nonterminal VARIABLE_NAME = new Nonterminal("variable_name");
	}
}