using System;
namespace JayPascal
{
    //---- Class Identifier
	public class Identifier:Keyword
	{
		public Identifier(String idval):base(idval, Token.IDENTIFIER)
		{
		}
		public override String ToString()
		{
			return "ID[" + base.ToString() + "]";
		}
	}

    //---- Class CharToken
    public class CharToken : Keyword
    {
        public CharToken(char ch)
            : base(ch.ToString(), Token.CHAR)
        {
        }

        public override String ToString()
        {
            return "CHAR[" + base.ToString() + "]";
        }
    }
    //---- Class CharToken
    public class BooleanToken : Keyword
    {
        public BooleanToken(bool ch)
            : base(ch.ToString(), Token.BOOLEAN)
        {
        }

        public override String ToString()
        {
            return "BOOLEAN[" + base.ToString() + "]";
        }
    }
    
    //---- Class StringToken
    public class StringToken : Keyword
    {
        public StringToken(String idval)
            : base(idval, Token.STRING)
        {
        }
        public override String ToString()
        {
            return "STRING[" + base.ToString() + "]";
        }
    }

    //---- Class Operator
    public class Operator : Keyword
    {

        public Operator(String name, int toktype)
            : base(name, toktype)
        {
        }

        public override String ToString()
        {
            return "OPERATOR[" + this.name + "]";
        }
    }

    //---- Class IntegerToken
    public class IntegerToken : Token
    {
        virtual public int Value
        {
            get
            {
                return this.value_Renamed;
            }

        }
        internal int value_Renamed;

        public IntegerToken(int value_Renamed)
            : base(Token.INTEGER)
        {
            this.value_Renamed = value_Renamed;
        }

        public IntegerToken(String original)
            : base(Token.INTEGER)
        {
            this.value_Renamed = Int32.Parse(original);
        }


        public override String ToString()
        {
            return "INT[" + this.value_Renamed + "]";
        }


        public virtual bool equals(IntegerToken other)
        {
            return this.value_Renamed == other.value_Renamed;
        }
    }

    //-- class RealToken
    public class RealToken : Token
    {
        virtual public float Value
        {
            get
            {
                return this.value_Renamed;
            }

        }

        internal float value_Renamed;
        internal String original;

        public RealToken(float value_Renamed)
            : base(Token.REAL)
        {
            this.value_Renamed = value_Renamed;
            this.original = value_Renamed.ToString();
        }

        public RealToken(String original)
            : base(Token.REAL)
        {
            this.value_Renamed = Single.Parse(original);
            this.original = original;
        }

        public override String ToString()
        {
            return "REAL[" + this.original + "]";
        }

        public virtual bool equals(RealToken other)
        {
            return this.value_Renamed == other.value_Renamed;
        }
    }
}