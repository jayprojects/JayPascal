using System;
namespace JayPascal
{
	public class Keyword:Token
	{
		virtual public String Name
		{
			get
			{
				return this.name;
			}
			
		}
		public String name;
		public Keyword(String name, int tokType):base(tokType)
		{
			this.name = name;
		}
		
		public override int GetHashCode()
		{
			return this.name.GetHashCode();
		}
		
		public override String ToString()
		{
			return this.name;
		}
		
		public override bool equals(Token other)
		{
			try
			{
				return equals((Keyword) other);
			}
			catch (InvalidCastException cce)
			{
                Console.Write(cce.Message);
				return false;
			}
		}
		
		public virtual bool equals(Keyword other)
		{
			return this == other || ((this.Type == other.Type) && other.name.Equals(this.name) && other.GetType().Equals(this.GetType()));
		}
	}




    
}