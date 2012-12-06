using System;
using System.Collections;
namespace JayPascal
{
	
	
	public class Parser
	{
		
		public virtual Node parse(TokenStream tokens)
		{
			if (!(tokens is Scanner))
				parserError("Can only parse with Pascal tokens.");
			try
			{
				return parseProgram((Scanner) tokens);
			}
			
			catch (Exception e)
			{
                throw new ParserException(e.Message);
			}
			
		}
		
		
		
		private void  parserError(String message)
		{
			//throw new PascalException(message);
            throw new ParserException(message);
		}
		
		
		private void  consume(Token desired, Scanner tokens)
		{
			if (tokens.peek() == desired)
				tokens.next();
			else
				wrongToken(desired, tokens.peek());
		}
		
		
		private Node getID(Scanner tokens)
		{
			if (tokens.peek() is Identifier)
				return new Node(tokens.next());
			else
			{
				throw new PascalException("Expected IDENTIFIER, found " + tokens.peek().ToString());
			}
		}
		
		private ArrayList getIDs(Scanner tokens)
		{
			ArrayList identifiers = ArrayList.Synchronized(new ArrayList(10));
			identifiers.Add(getID(tokens));
			while (tokens.peek() == PascalTokens.TCOMMA)
			{
				tokens.next();
				identifiers.Add(getID(tokens));
			}
			return identifiers;
		}
		
		private Node getInteger(Scanner tokens)
		{
			if (tokens.peek() is IntegerToken)
				return new Node(tokens.next());
			else
			{
				throw new PascalException("Expected INTEGER, found " + tokens.peek().ToString());
			}
		}
		
		
		private bool isAddOp(Token tok)
		{
			return ((tok == PascalTokens.TPLUS) || (tok == PascalTokens.TMINUS) || (tok == PascalTokens.TOR));
		}
		
		
		private bool isMulOp(Token tok)
		{
			return ((tok == PascalTokens.TTIMES) || (tok == PascalTokens.TDIVIDE) || (tok == PascalTokens.TMOD) || (tok == PascalTokens.TAND) || (tok == PascalTokens.TINTDIV));
		}
		
		
		private bool isRelOp(Token tok)
		{
			return ((tok == PascalTokens.TEQUALS) || (tok == PascalTokens.TNOTEQUALS) || (tok == PascalTokens.TLESSTHAN) || (tok == PascalTokens.TLESSEQ) || (tok == PascalTokens.TGREATERTHAN) || (tok == PascalTokens.TGREATEREQ));
		}
		
		
		private Node wrongToken(Symbol expected, Token found)
		{
			throw new PascalException("Expected: " + expected.ToString() + "; Found: " + found.ToString());
		}
		
		
		private Node parseAddingOperator(Scanner tokens)
		{
			Token tok = tokens.peek();
			if (isAddOp(tokens.peek()))
			{
				return new Node(tokens.next());
			}
			else
			{
				return wrongToken(PascalNonterminals.ADDING_OPERATOR, tok);
			}
		}
		
		private Node parseArrayType(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			consume(PascalTokens.TARRAY, tokens);
			consume(PascalTokens.TOPENBRACKET, tokens);
			children.Add(parseSimpleType(tokens));
			while (tokens.peek() == PascalTokens.TCOMMA)
			{
				tokens.next();
				children.Add(parseSimpleType(tokens));
			}
			consume(PascalTokens.TCLOSEBRACKET, tokens);
			consume(PascalTokens.TOF, tokens);
			children.Add(parseType(tokens));
			return new Node(PascalNonterminals.ARRAY_TYPE, children);
		}
		
		private Node parseAssignment(Identifier id, Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			children.Add(parseVariable(id, tokens));
			if (tokens.peek() != PascalTokens.TBECOMES)
				return wrongToken(PascalNonterminals.ASS_STAT, tokens.peek());
			tokens.next();
			children.Add(parseExpression(tokens));
			return new Node(PascalNonterminals.ASS_STAT, children);
		}
		
		private Node parseBlock(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			children.Add(parseLabelDeclarations(tokens));
			children.Add(parseConstantDefinitions(tokens));
			children.Add(parseTypeDefinitions(tokens));
			children.Add(parseVariableDeclarations(tokens));
			children.Add(parseProcFuncDeclarations(tokens));
			children.Add(parseCompoundStatement(tokens));
			return new Node(PascalNonterminals.PROGRAM_BLOCK, children);
		}
		
		
		
		
		private Node parseCompoundStatement(Scanner tokens)
		{
            Node tempNode;
			consume(PascalTokens.TBEGIN, tokens);
			ArrayList statements = ArrayList.Synchronized(new ArrayList(10));
			if (tokens.peek() != PascalTokens.TEND)
			{
				statements.Add(parseStatement(tokens));
				while (tokens.peek() == PascalTokens.TSEMICOLON)
				{
					tokens.next();
                    tempNode = parseStatement(tokens);
                    if(tempNode!=null)
                        statements.Add(tempNode);
				}
			}
			
			consume(PascalTokens.TEND, tokens);
			
			return new Node(PascalNonterminals.COMPOUND_STAT, statements);
		}
		
		private Node parseConstant(Scanner tokens)
		{
			Token tok = tokens.peek();
			if (tok is StringToken)
			{
				tokens.next();
				return new Node(tok);
			}
			else
			{
				ArrayList children = ArrayList.Synchronized(new ArrayList(10));
				if ((tok == PascalTokens.TPLUS) || (tok == PascalTokens.TMINUS))
				{
					tokens.next();
					children.Add(new Node(tok));
				}
				else
				{
					children.Add(null);
				}
				children.Add(parseUnsignedNumber(tokens));
				return new Node(PascalNonterminals.CONSTANT, children);
			}
		}
		
		private Node parseConstantDefinition(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			children.Add(getID(tokens));
			consume(PascalTokens.TEQUALS, tokens);
			children.Add(parseConstant(tokens));
			return new Node(PascalNonterminals.CONSTANT_DEFINITION, children);
		}
		
		private Node parseConstantDefinitions(Scanner tokens)
		{
			if (tokens.peek() == PascalTokens.TCONST)
			{
				tokens.next();
				ArrayList children = ArrayList.Synchronized(new ArrayList(10));
				children.Add(parseConstantDefinition(tokens));
				consume(PascalTokens.TSEMICOLON, tokens);
				while (tokens.peek() is Identifier)
				{
					children.Add(parseConstantDefinition(tokens));
					consume(PascalTokens.TSEMICOLON, tokens);
				}
				return new Node(PascalNonterminals.CONSTANT_DEFINITIONS, children);
			}
			else
			{
				return null;
			}
		}
		
		private Node parseDirection(Scanner tokens)
		{
			Token tok = tokens.peek();
			if ((tok == PascalTokens.TTO) || (tok == PascalTokens.TDOWNTO))
			{
				tokens.next();
				return new Node(tok);
			}
			else
			{
				throw new PascalException("Expection to/downto in for statement, found: " + tok.ToString());
			}
		}
		
		private Node parseElement(Scanner tokens)
		{
			Node exp = parseExpression(tokens);
			if (tokens.peek() == PascalTokens.TDOTDOT)
			{
				tokens.next();
				ArrayList children = ArrayList.Synchronized(new ArrayList(10));
				children.Add(exp);
				children.Add(parseExpression(tokens));
				return new Node(PascalNonterminals.RANGE, children);
			}
			else
				return exp;
		} 
		
		private Node parseExpression(Scanner tokens)
		{
			if (tokens.peek() is StringToken)
				return new Node(tokens.next());
			Node primary = parseSimpleExpression(tokens);
			if (isRelOp(tokens.peek()))
			{
				ArrayList children = ArrayList.Synchronized(new ArrayList(10));
				children.Add(primary);
				children.Add(parseRelationalOperator(tokens));
				children.Add(parseSimpleExpression(tokens));
				return new Node(PascalNonterminals.EXPRESSION, children);
			}
			else
			{
				return primary;
			}
		}
		
		private Node parseFactor(Scanner tokens)
		{
			Token tok = tokens.peek();

            if ((tok is IntegerToken) || (tok is RealToken) || (tok is CharToken) || (tok is BooleanToken))
			{
				return new Node(tokens.next());
			}
           
            else if (tok is Identifier)
            {
                tokens.next();
                
                    return parseVariable((Identifier)tok, tokens);
                
            }
            else if (tok == PascalTokens.TOPENPAREN)
            {
                tokens.next();
                Node result = parseExpression(tokens);
                consume(PascalTokens.TCLOSEPAREN, tokens);
                return result;
            }

            else if (tok == PascalTokens.TNOT)
            {
                tokens.next();
                return new Node(tok, parseFactor(tokens));
            }

            else if (tok == PascalTokens.TOPENBRACKET)
            {
                return parseSetValue(tokens);
            }

            else
            {
                return wrongToken(PascalNonterminals.FACTOR, tokens.peek());
            }
		}
		
		
		
		
		private Node parseForStatement(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			consume(PascalTokens.TFOR, tokens);
			children.Add(getID(tokens));
			consume(PascalTokens.TBECOMES, tokens);
			children.Add(parseExpression(tokens));
			children.Add(parseDirection(tokens));
			children.Add(parseExpression(tokens));
			consume(PascalTokens.TDO, tokens);
			children.Add(parseStatement(tokens));
			return new Node(PascalNonterminals.FOR_STAT, children);
		} 
		
		
		
		private Node parseGoto(Scanner tokens)
		{
			consume(PascalTokens.TGOTO, tokens);
			return new Node(PascalNonterminals.GOTO_STAT, getID(tokens));
		} 
		
		private Node parseIdentifierList(Scanner tokens)
		{
			return new Node(PascalNonterminals.IDENTIFIER_LIST, getIDs(tokens));
		}
		
		private Node parseIf(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			consume(PascalTokens.TIF, tokens);
			children.Add(parseExpression(tokens));
			consume(PascalTokens.TTHEN, tokens);
			children.Add(parseStatement(tokens));
			if (tokens.peek() == PascalTokens.TELSE)
			{
				tokens.next();
				children.Add(parseStatement(tokens));
			}
			return new Node(PascalNonterminals.IF_STAT, children);
		} 
		
		private Node parseLabelDeclarations(Scanner tokens)
		{
			if (tokens.peek() == PascalTokens.TLABEL)
			{
				tokens.next();
				ArrayList children = ArrayList.Synchronized(new ArrayList(10));
				//children.Add(getInteger(tokens));
                children.Add(getID(tokens));
				while (tokens.peek() == PascalTokens.TCOMMA)
				{
					tokens.next();
					//children.Add(getInteger(tokens));
                    children.Add(getID(tokens));
				}
				consume(PascalTokens.TSEMICOLON, tokens);
				return new Node(PascalNonterminals.LABEL_DECLARATIONS, children);
			}
			else
			{
				return null;
			}
		} 
		
		private Node parseMultiplyingOperator(Scanner tokens)
		{
			if (isMulOp(tokens.peek()))
				return new Node(tokens.next());
			else
				return wrongToken(PascalNonterminals.MULTIPLYING_OPERATOR, tokens.peek());
		} 
		
		
		
		public virtual Node parseParameterGroup(Scanner tokens)
		{
			ArrayList children = getIDs(tokens);
			consume(PascalTokens.TCOLON, tokens);
			children.Add(getID(tokens));
			return new Node(PascalNonterminals.PARAMETER_GROUP, children);
		}
		
		
		private Node parseProcedureCall(Identifier id, Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			children.Add(new Node(id));
			if (tokens.peek() == PascalTokens.TOPENPAREN)
			{
				tokens.next();
				children.Add(parseExpression(tokens));
				while (tokens.peek() == PascalTokens.TCOMMA)
				{
					tokens.next();
					children.Add(parseExpression(tokens));
				} 
				consume(PascalTokens.TCLOSEPAREN, tokens);
			} 
			return new Node(PascalNonterminals.PROCEDURE_CALL, children);
		} 
		
		
		private Node parseProcedureDeclaration(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			consume(PascalTokens.TPROCEDURE, tokens);
			children.Add(getID(tokens));
			consume(PascalTokens.TSEMICOLON, tokens);
			children.Add(parseBlock(tokens));
			return new Node(PascalNonterminals.PROCEDURE_DECLARATION, children);
		}
		
		private Node parseProcFuncDeclarations(Scanner tokens)
		{
			Token tok;
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			while ((tok = tokens.peek()) != PascalTokens.TBEGIN)
			{
				if (tok == PascalTokens.TPROCEDURE)
					children.Add(parseProcedureDeclaration(tokens));
				else
				{
					throw new PascalException("Expected procedure/function declaration; " + "found: " + tok.ToString());
				}
				consume(PascalTokens.TSEMICOLON, tokens);
			} 
			return new Node(PascalNonterminals.PROCFUNC_DECLARATIONS, children);
		}
		
		private Node parseProgram(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			children.Add(parseProgramHeading(tokens));
			children.Add(parseBlock(tokens));
			consume(PascalTokens.TDOT, tokens);
			return new Node(PascalNonterminals.PROGRAM, children);
		}
		
		private Node parseProgramHeading(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			consume(PascalTokens.TPROGRAM, tokens);
			children.Add(getID(tokens));
			consume(PascalTokens.TOPENPAREN, tokens);
			children.Add(parseIdentifierList(tokens));
			consume(PascalTokens.TCLOSEPAREN, tokens);
			consume(PascalTokens.TSEMICOLON, tokens);
			return new Node(PascalNonterminals.PROGRAM_HEADING, children);
		}
		
		
		
		private Node parseRelationalOperator(Scanner tokens)
		{
			if (isRelOp(tokens.peek()))
				return new Node(tokens.next());
			else
				return wrongToken(PascalNonterminals.RELATIONAL_OPERATOR, tokens.peek());
		}
		
		private Node parseRepeatStatement(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			consume(PascalTokens.TREPEAT, tokens);
			children.Add(parseStatement(tokens));
			while (tokens.peek() == PascalTokens.TSEMICOLON)
			{
				tokens.next();
				children.Add(parseStatement(tokens));
			}
			consume(PascalTokens.TUNTIL, tokens);
			children.Add(parseExpression(tokens));
			return new Node(PascalNonterminals.REPEAT_STAT, children);
		} 
		
		private Node parseSetValue(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			consume(PascalTokens.TOPENBRACKET, tokens);
			if (tokens.peek() != PascalTokens.TCLOSEBRACKET)
			{
				children.Add(parseElement(tokens));
				while (tokens.peek() == PascalTokens.TCOMMA)
				{
					tokens.next();
					children.Add(parseElement(tokens));
				}
			}
			consume(PascalTokens.TCLOSEBRACKET, tokens);
			return new Node(PascalNonterminals.SET, children);
		} 
		
		private Node parseSetType(Scanner tokens)
		{
			consume(PascalTokens.TSET, tokens);
			consume(PascalTokens.TOF, tokens);
			return new Node(PascalNonterminals.SET_TYPE, parseSimpleType(tokens));
		}

        private Node parseSimpleExpression(Scanner tokens)
        {
            Token tok = tokens.peek();
            Node result;

            if ((tok == PascalTokens.TPLUS) || (tok == PascalTokens.TMINUS))
            {


                result = new Node(new IntegerToken(0));
            }
            else
            {
                result = parseTerm(tokens);
            }

            while (isAddOp(tokens.peek()))
            {
                ArrayList children = ArrayList.Synchronized(new ArrayList(10));
                children.Add(result);
                children.Add(parseAddingOperator(tokens));
                children.Add(parseTerm(tokens));

                result = new Node(PascalNonterminals.EXPRESSION, children);
            }
            return result;

        } 
		
		private Node parseSimpleType(Scanner tokens)
		{
			Token tok = tokens.peek();
			if (tok == PascalTokens.TOPENPAREN)
			{
				tokens.next();
				ArrayList contents = getIDs(tokens);
				consume(PascalTokens.TCLOSEPAREN, tokens);
				return new Node(PascalNonterminals.SCALAR_TYPE, contents);
			}
			else if (tok is Identifier)
			{
				Node id = getID(tokens);
				if (tokens.peek() == PascalTokens.TDOTDOT)
				{
					tokens.next();
					ArrayList children = ArrayList.Synchronized(new ArrayList(10));
					ArrayList grandchildren = ArrayList.Synchronized(new ArrayList(10));
					grandchildren.Add(null);
					grandchildren.Add(id);
					children.Add(new Node(PascalNonterminals.CONSTANT, grandchildren));
					children.Add(parseConstant(tokens));
					return new Node(PascalNonterminals.SUBRANGE_TYPE, children);
				}
				else
					return new Node(PascalNonterminals.TYPE_IDENTIFIER, id);
			}
			else
			{
				ArrayList children = ArrayList.Synchronized(new ArrayList(10));
				children.Add(parseConstant(tokens));
				consume(PascalTokens.TDOTDOT, tokens);
				children.Add(parseConstant(tokens));
				return new Node(PascalNonterminals.SUBRANGE_TYPE, children);
			} 
		} 
		
		private Node parseStatement(Scanner tokens)
		{
			Token tok = tokens.peek();
            //let comment handled by preprocessors
            /*
            if (tok == PascalTokens.TCOMMENT)
            {
                do
                {
                    tokens.next();
                    tok = tokens.peek();
                } while (tok == PascalTokens.TCOMMENT);
            }
            */
			
			if (tok is IntegerToken)
			{
				ArrayList children = ArrayList.Synchronized(new ArrayList(10));
				children.Add(getInteger(tokens));
				consume(PascalTokens.TCOLON, tokens);
				children.Add(parseStatement(tokens));
				return new Node(PascalNonterminals.LABELED_STATEMENT, children);
			}
		
            else if (tok is Identifier)
			{

				Token CurrentToken = tokens.next();
				Token tmp = tokens.peek();
				if ((tmp == PascalTokens.TOPENPAREN) || (tmp == PascalTokens.TSEMICOLON) || (tmp == PascalTokens.TELSE) || (tmp == PascalTokens.TEND) || (tmp == PascalTokens.TUNTIL))
				{
					return parseProcedureCall((Identifier) tok, tokens);
				}
                else if (tmp == PascalTokens.TCOLON)
                {
                    ArrayList children = ArrayList.Synchronized(new ArrayList(10));
                    children.Add(new Node(CurrentToken));
                    consume(PascalTokens.TCOLON, tokens);
                    children.Add(parseStatement(tokens));
                    return new Node(PascalNonterminals.LABELED_STATEMENT, children);
                }
                else
                {

                    return parseAssignment((Identifier)tok, tokens);
                }
			}
			
			else if (tok == PascalTokens.TBEGIN)
			{
				return parseCompoundStatement(tokens);
			}
			
			else if (tok == PascalTokens.TFOR)
			{
				return parseForStatement(tokens);
			}
			
			else if (tok == PascalTokens.TGOTO)
			{
				return parseGoto(tokens);
			}
			
			else if (tok == PascalTokens.TIF)
			{
				return parseIf(tokens);
			}
			
			else if (tok == PascalTokens.TREPEAT)
			{
				return parseRepeatStatement(tokens);
			}
			
			else if (tok == PascalTokens.TWHILE)
			{
				return parseWhileLoop(tokens);
			}
            else if (tok == PascalTokens.TWHILE)
            {
                return parseWhileLoop(tokens);
            }
            else if (tok == PascalTokens.TTRUE)
            {
                return new Node(new BooleanToken(true));
            }
            else if (tok == PascalTokens.TFALSE)
            {
                return new Node(new BooleanToken(false));
            }
			
			
			
			else if ((tok == PascalTokens.TSEMICOLON) || (tok == PascalTokens.TEND) || (tok == PascalTokens.TUNTIL))
			{
				return null;
			}
			
			else
				return wrongToken(PascalNonterminals.STATEMENT, tok);
		} 
		
		private Node parseStructuredType(Scanner tokens)
		{
			Token tok = tokens.peek();
			if (tok == PascalTokens.TARRAY)
			{
				return parseArrayType(tokens);
			}
			else if (tok == PascalTokens.TSET)
			{
				return parseSetType(tokens);
			}
			else
			{
				throw new Exception("Parsing structured type, found " + tok.ToString());
			}
		} 
		
		private Node parseTerm(Scanner tokens)
		{
			Node result = parseFactor(tokens);
			while (isMulOp(tokens.peek()))
			{
				ArrayList children = ArrayList.Synchronized(new ArrayList(10));
				children.Add(result);
				children.Add(parseMultiplyingOperator(tokens));
				children.Add(parseFactor(tokens));
				result = new Node(PascalNonterminals.EXPRESSION, children);
			}
			
			return result;
		}
		
		private Node parseType(Scanner tokens)
		{
			Token tok = tokens.peek();
			if ((tok == PascalTokens.TOPENPAREN) || (tok == PascalTokens.TPLUS) || (tok == PascalTokens.TMINUS) || (tok is IntegerToken) || (tok is Identifier))
			{
				return parseSimpleType(tokens);
			}
			else if ((tok == PascalTokens.TARRAY) || (tok == PascalTokens.TSET))
			{
				return parseStructuredType(tokens);
			}
			else
			{
				throw new PascalException("Tried to match a type, found: " + tok.ToString());
			}
		} 
		
		private Node parseTypeDefinition(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			children.Add(getID(tokens));
			consume(PascalTokens.TEQUALS, tokens);
			children.Add(parseType(tokens));
			return new Node(PascalNonterminals.TYPE_DEFINITION, children);
		} 
		
		private Node parseTypeDefinitions(Scanner tokens)
		{
			if (tokens.peek() == PascalTokens.TTYPE)
			{
				tokens.next();
				ArrayList children = ArrayList.Synchronized(new ArrayList(10));
				while (tokens.peek() is Identifier)
				{
					children.Add(parseTypeDefinition(tokens));
					consume(PascalTokens.TSEMICOLON, tokens);
				} 
				return new Node(PascalNonterminals.TYPE_DEFINITIONS, children);
			}
			else
				return null;
		} 
		
		private Node parseUnsignedNumber(Scanner tokens)
		{
			Token tok = tokens.next();
			if ((tok is IntegerToken) || (tok is RealToken) || (tok is Identifier))
				return new Node(tok);
			else
			{
				throw new PascalException("Expected: unsigned number; Found: " + tok.ToString());
			}
		} 
		
		private Node parseVariable(Scanner tokens)
		{
			Token tok = tokens.peek();
			if (tok is Identifier)
			{
				tokens.next();
				return parseVariable((Identifier) tok, tokens);
			}
			else
			{
				throw new Exception("Attempting to match a variable; found: " + tok.ToString());
			}
		} 
		
		private Node parseVariable(Identifier id, Scanner tokens)
		{
			Node base_Renamed = new Node(PascalNonterminals.VARIABLE_NAME, new Node(id));
			bool done = false;
			while (!done)
			{
				Token tok = tokens.peek();
				if (tok == PascalTokens.TDOT)
				{
                    //CHECKING IF THIS BLOCK GET EVER ACCESSED
                    //OTHER WISE REMOVE FIELD_SELECT FROM PASCALNONTERMINALS 

					tokens.next();
					ArrayList children = ArrayList.Synchronized(new ArrayList(10));
					children.Add(base_Renamed);
					children.Add(getID(tokens));
					base_Renamed = new Node(PascalNonterminals.FIELD_SELECT, children);
				}
				else if (tok == PascalTokens.TOPENBRACKET)
				{
					tokens.next();
					ArrayList children = ArrayList.Synchronized(new ArrayList(10));
					children.Add(base_Renamed);
					children.Add(parseExpression(tokens));
					while (tokens.peek() == PascalTokens.TCOMMA)
					{
						tokens.next();
						children.Add(parseExpression(tokens));
					}
					consume(PascalTokens.TCLOSEBRACKET, tokens);
					base_Renamed = new Node(PascalNonterminals.ARRAY_SELECT, children);
				}
				
				else
				{
					done = true;
				}
			} 
			return base_Renamed;
		} 
		
		private Node parseVariableDeclaration(Scanner tokens)
		{
			ArrayList children = getIDs(tokens);
			consume(PascalTokens.TCOLON, tokens);
			children.Add(parseType(tokens));
			return new Node(PascalNonterminals.VARIABLE_DECLARATION, children);
		}
		
		private Node parseVariableDeclarations(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			if (tokens.peek() != PascalTokens.TVAR)
				return new Node(PascalNonterminals.VARIABLE_DECLARATIONS);
			tokens.next();
			while (tokens.peek() is Identifier)
			{
				children.Add(parseVariableDeclaration(tokens));
				if (tokens.peek() != PascalTokens.TSEMICOLON)
					return wrongToken(PascalTokens.TSEMICOLON, tokens.peek());
				tokens.next();
			}
			return new Node(PascalNonterminals.VARIABLE_DECLARATIONS, children);
		}
		
		
		private Node parseWhileLoop(Scanner tokens)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			consume(PascalTokens.TWHILE, tokens);
			children.Add(parseExpression(tokens));
			consume(PascalTokens.TDO, tokens);
			children.Add(parseStatement(tokens));
			return new Node(PascalNonterminals.WHILE_STAT, children);
		}
	}
}