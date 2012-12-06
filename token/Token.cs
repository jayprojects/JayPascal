using System;
namespace JayPascal
{
    public interface Symbol
    {
        String ToString();
    }

	
	public class Token : Symbol
	{
		virtual public int Type
		{
			get
			{
				return this.tokType;
			}
			
		}
		public const int IDENTIFIER = 1;
		public const int INTEGER = 2;
		public const int REAL = 3;
		public const int STRING = 4;
		public const int CHAR = 5;
        public const int BOOLEAN = 6;
		public const int FIRSTID = 101;
		
		internal int tokType;
		
		public Token(int type)
		{
			this.tokType = type;
		}

        public virtual bool equals(Token other)
		{
			return this == other;
		}
		
		public override String ToString()
		{
			return "Token[" + this.tokType + "]";
		}
	}
}