using System;
using System.Collections;
namespace JayPascal
{
	
	
	public class Scanner : TokenStream
	{
		internal const int MAX_DIGITS = 64;
		internal const int MAX_IDENTIFIER = 128;
		internal const int MAX_STRING = 1024;
		
		internal CharStream cs;
		internal Hashtable idsAndKeywords;
		internal Token buffer;
		internal bool dotdot_next = false;
		
		public Scanner(CharStream cs)
		{
			this.cs = cs;
			try
			{
				this.skipSpaces();
			}
			catch (Exception e)
			{
                throw new ScannerException(e.Message);
			}
			this.idsAndKeywords = Hashtable.Synchronized(new Hashtable());
			this.idsAndKeywords["and"] = PascalTokens.TAND;
			this.idsAndKeywords["array"] = PascalTokens.TARRAY;
			this.idsAndKeywords["begin"] = PascalTokens.TBEGIN;
			
			this.idsAndKeywords["const"] = PascalTokens.TCONST;
			
            this.idsAndKeywords["div"] = PascalTokens.TINTDIV;
            
			this.idsAndKeywords["do"] = PascalTokens.TDO;
			this.idsAndKeywords["downto"] = PascalTokens.TDOWNTO;
			this.idsAndKeywords["else"] = PascalTokens.TELSE;
			this.idsAndKeywords["end"] = PascalTokens.TEND;
			
			this.idsAndKeywords["for"] = PascalTokens.TFOR;
			this.idsAndKeywords["goto"] = PascalTokens.TGOTO;
			this.idsAndKeywords["if"] = PascalTokens.TIF;
			this.idsAndKeywords["in"] = PascalTokens.TIN;
			this.idsAndKeywords["label"] = PascalTokens.TLABEL;
			this.idsAndKeywords["not"] = PascalTokens.TNOT;
			this.idsAndKeywords["of"] = PascalTokens.TOF;
			this.idsAndKeywords["or"] = PascalTokens.TOR;
			
			this.idsAndKeywords["procedure"] = PascalTokens.TPROCEDURE;
			this.idsAndKeywords["program"] = PascalTokens.TPROGRAM;
			
			this.idsAndKeywords["repeat"] = PascalTokens.TREPEAT;
			this.idsAndKeywords["then"] = PascalTokens.TTHEN;
			this.idsAndKeywords["to"] = PascalTokens.TTO;
			this.idsAndKeywords["type"] = PascalTokens.TTYPE;
			this.idsAndKeywords["until"] = PascalTokens.TUNTIL;
			this.idsAndKeywords["var"] = PascalTokens.TVAR;
			this.idsAndKeywords["while"] = PascalTokens.TWHILE;
			
			this.idsAndKeywords["boolean"] = PascalTokens.TBOOLEAN;
			this.idsAndKeywords["char"] = PascalTokens.TCHAR;
			this.idsAndKeywords["integer"] = PascalTokens.TINTEGER;
			this.idsAndKeywords["real"] = PascalTokens.TREAL;
			
			this.idsAndKeywords["false"] = PascalTokens.TFALSE;
			this.idsAndKeywords["true"] = PascalTokens.TTRUE;
		}
		
		internal virtual void  scannerError(String message)
		{
			this.skipSpaces();
            throw new ScannerException(message);
		}
		
		internal virtual void  fillBuffer()
		{
			if (buffer != null)
				return ;
			if (this.dotdot_next)
			{
				buffer = PascalTokens.TDOTDOT;
				this.dotdot_next = false;
				return ;
			}
			if (!cs.hasMore())
				return ;
			char ch = cs.next();
			if (isAlpha(ch))
			{
				char[] chars = new char[MAX_IDENTIFIER];
				int len = 1;
				chars[0] = ch;
				while ((len < MAX_IDENTIFIER) && (cs.hasMore()) && (isIdChar(cs.peek())))
				{
					chars[len++] = cs.next();
				}
				
				if (len == MAX_IDENTIFIER)
					scannerError("Identifiers are limited to " + MAX_IDENTIFIER + " chars.");
				
				String str = (new String(chars, 0, len)).ToLower();
				Token tok = (Token) idsAndKeywords[str];
				
				if (tok != null)
				{
					buffer = tok;
				}
				
				else
				{
					tok = new Identifier(str);
					idsAndKeywords[str] = tok;
					buffer = tok;
				}
			}
			
			else if (isDigit(ch))
			{
				char[] digits = new char[MAX_DIGITS];
				digits[0] = ch;
				int len = 1;
				bool isReal = false;
				while ((len < MAX_DIGITS) && (isDigit(cs.peek())))
					digits[len++] = cs.next();
				if (len == MAX_DIGITS)
					scannerError("Numbers are limited to " + MAX_DIGITS + " characters.");
				if (cs.peek() == '.')
				{
					cs.next();
					if (cs.peek() == '.')
					{
						cs.next();
						this.dotdot_next = true;
						buffer = new IntegerToken(new String(digits, 0, len));
					}
					else if (isDigit(cs.peek()))
					{
						if ((len + 2) >= MAX_DIGITS)
							scannerError("Numbers are limited to " + MAX_DIGITS + " characters.");
						isReal = true;
						digits[len++] = '.';
						digits[len++] = cs.next();
						while ((len < MAX_DIGITS) && (isDigit(cs.peek())))
							digits[len++] = cs.next();
					}
					else
					{
						scannerError("Sorry, Pascal doesn't allow number dot " + cs.peek());
					}
				} 
				
				if (len == MAX_DIGITS)
					scannerError("Numbers are limited to " + MAX_DIGITS + " characters.");
				
				if ((buffer == null) && ((cs.peek() == 'E') || (cs.peek() == 'e')))
				{
					if (len + 2 >= MAX_DIGITS)
						scannerError("Numbers are limited to " + MAX_DIGITS + " characters.");
					isReal = true;
					digits[len++] = cs.next(); // 'E'
					
					if ((cs.peek() == '+') || (cs.peek() == '-'))
						digits[len++] = cs.next(); // '+' or '-'
					
					if (len + 1 >= MAX_DIGITS)
						scannerError("Numbers are limited to " + MAX_DIGITS + " characters.");
					
					if (!isDigit(cs.peek()))
						scannerError("Sorry, Pascal doesn't allow number dot " + cs.peek());
					
					digits[len++] = cs.next();
					while ((len < MAX_DIGITS) && (isDigit(cs.peek())))
						digits[len++] = cs.next();
					
					if (len == MAX_DIGITS)
						scannerError("Numbers are limited to " + MAX_DIGITS + " characters.");
				}
				
				
				String number = new String(digits, 0, len);
				if (isReal)
				{
					try
					{
						buffer = new RealToken(number);
					}
					catch (FormatException e)
					{
                        Console.Write(e.Message);
						scannerError("Invalid integer: " + number);
					}
				}
				else
				{
					try
					{
						buffer = new IntegerToken(number);
					}
					catch (FormatException e)
					{
                        Console.Write(e.Message);
						scannerError("Invalid real: " + number);
					}
				}
			}
			
			else if (ch == '\'')
			{
				char[] chars = new char[MAX_STRING];
				int len = 0;
				while ((len < MAX_STRING) && ((ch = cs.next()) != '\''))
				{
					if (ch == '\\')
						chars[len++] = cs.next();
					else
						chars[len++] = ch;
				}
				
				if (len == MAX_STRING)
					scannerError("Strings are limited to " + MAX_STRING + " chars.");
				
				if (len == 1)
				{
					buffer = new CharToken(chars[0]);
				}
				
				else
				{
					buffer = new StringToken(new String(chars, 0, len));
				}
			}
			
			else
			{
				switch (ch)
				{
					
					case '+':  buffer = PascalTokens.TPLUS; break;
					case '-':  buffer = PascalTokens.TMINUS; break;
					case '*':  buffer = PascalTokens.TTIMES; break;
					case '/':  buffer = PascalTokens.TDIVIDE; break;
                    /*
                    case '/':
                        if (cs.hasMore() && (cs.peek() == '/'))
                        {
                            // comment found
                            do
                            {
                                ch =cs.next();
                            } while (cs.hasMore() && (ch != '\n'));

                            buffer = PascalTokens.TCOMMENT;
                        }
                        else
                            buffer = PascalTokens.TDIVIDE;
                        break;
                     * */
					case '(':  buffer = PascalTokens.TOPENPAREN; break;
					case ')':  buffer = PascalTokens.TCLOSEPAREN; break;
					case ',':  buffer = PascalTokens.TCOMMA; break;
					case ';':  buffer = PascalTokens.TSEMICOLON; break;
					case '=':  buffer = PascalTokens.TEQUALS; break;
					case '[':  buffer = PascalTokens.TOPENBRACKET; break;
					case ']':  buffer = PascalTokens.TCLOSEBRACKET; break;
                    
					case ':': 
						if (cs.hasMore() && (cs.peek() == '='))
						{
							cs.next();
							buffer = PascalTokens.TBECOMES;
						}
						else
							buffer = PascalTokens.TCOLON;
						break;
					
					case '.': 
						if (cs.hasMore() && (cs.peek() == '.'))
						{
							cs.next();
							buffer = PascalTokens.TDOTDOT;
						}
						else
							buffer = PascalTokens.TDOT;
						break;
					
					case '<': 
						if (cs.hasMore() && (cs.peek() == '='))
						{
							cs.next();
							buffer = PascalTokens.TLESSEQ;
						}
						else if (cs.hasMore() && (cs.peek() == '>'))
						{
							cs.next();
							buffer = PascalTokens.TNOTEQUALS;
						}
						else
							buffer = PascalTokens.TLESSTHAN;
						break;
					
					case '>': 
						if (cs.hasMore() && (cs.peek() == '='))
						{
							cs.next();
							buffer = PascalTokens.TGREATEREQ;
						}
						else
							buffer = PascalTokens.TGREATERTHAN;
						break;
					
					default: 
						scannerError("Unknown char '" + ch + "'\n");
						break;
					
				}
			}
			
			
			this.skipSpaces();
		} 
		
		internal virtual bool isAlpha(char ch)
		{
			return Char.IsLetter(ch);
		}
		
		
		internal virtual bool isDigit(char ch)
		{
			return Char.IsDigit(ch);
		}
		
		
		internal virtual bool isIdChar(char ch)
		{
			return isAlpha(ch) || (ch == '_') || isDigit(ch);
		}
		
		
		internal virtual bool isSpace(char ch)
		{
            if (ch == '\n')
            {
                cursorPosition.rowNumber++;
                cursorPosition.colNumber = 1;
            }
			return (ch == ' ') || (ch == '\n') || (ch == '\t') || (ch == 10) || (ch == 13);
            
		}
		
		
		internal virtual void  skipSpaces()
		{
			while (cs.hasMore() && isSpace(cs.peek()))
				cs.next();
			if (!cs.hasMore())
				return ;
			if (cs.peek() == '{')
			{
                
				while (cs.hasMore() && (cs.peek() != '}'))
					cs.next();
				if (!cs.hasMore())
					return ;
				cs.next();
				skipSpaces();
			}
		}
		
		
		public virtual bool hasMore()
		{
			return (this.buffer != null) || (this.cs.hasMore());
		}
		
		
		public virtual Token peek()
		{
			this.fillBuffer();
			return this.buffer;
		}
		
		
		public virtual void  addToken(String name, Token tok)
		{
			this.idsAndKeywords[name] = tok;
		}

		public virtual Token next()
		{
			this.fillBuffer();
			Token tok = this.buffer;
			this.buffer = null;
			return tok;
		}
	}
}