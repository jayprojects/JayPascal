using System;
using System.IO;
namespace JayPascal
{
	
	public class CharStream
	{
		private static int EOF = - 1;
		private static int UNDEF = - 2;

        protected internal StreamReader reader;
		protected internal int buffer;
		private int curline;
		private int curpos;
		
		
		public CharStream(StreamReader reader)
		{
			this.reader = reader;
			this.buffer = UNDEF;
			this.curline = 1;
			this.curpos = 1;
		}
		private void  fillBuffer()
		{
			try
			{
				if (this.buffer == UNDEF)
				{
					this.buffer = reader.Read();
				}
			}
			catch (Exception e)
			{
				this.buffer = EOF;
                Console.Write(e.Message);
			}
		}
		public virtual bool hasMore()
		{
			this.fillBuffer();
			return (this.buffer != EOF);
		}
		
		public virtual char peek()
		{
			this.fillBuffer();
			if (this.buffer == EOF)
			{
				throw new Exception("End of the file");
			}
			return (char) this.buffer;
		}
		
		
		public virtual char next()
		{
			char ch = this.peek();
			this.buffer = UNDEF;
			if (ch == '\n')
			{
				this.curline++;
				this.curpos = 1;

                //cursorPosition.rowNumber++;
                cursorPosition.colNumber = 1;
			}
			else
			{
				this.curpos++;
                cursorPosition.colNumber++;
			}
			return ch;
		}
	}
}