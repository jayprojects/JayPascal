using System;
using System.Collections;
namespace JayPascal
{
	public class Type
	{
		internal String name;
		public Type(String name)
		{
			this.name = name;
		}
		
		public override String ToString()
		{
			return this.name;
		}
	}
	
	class BasicTypes
	{
		public static readonly Type INTEGER = new Type("Integer");
		public static readonly Type REAL = new Type("Real");
		public static readonly Type STRING = new Type("String");
		public static readonly Type CHAR = new Type("Char");
		public static readonly Type BOOLEAN = new Type("Boolean");
	}
	
	
	public class SymbolTable
	{
		
		internal ArrayList contents;
		internal int scope;
		public SymbolTable()
		{
			contents = ArrayList.Synchronized(new ArrayList(10));
			contents.Insert(0, Hashtable.Synchronized(new Hashtable()));
			scope = 0;
		}
		
		
		private static Object makeKey(Object sym, Object att)
		{
			return new Pair(sym, att);
		}
		public virtual Object get_Renamed(Object symbol, Object attribute)
		{
			Object key = makeKey(symbol, attribute);
			for (int curScope = scope; curScope >= 0; curScope--)
			{
				Hashtable currentScope = ((Hashtable) contents[curScope]);
				Object result = currentScope[key];
				if (result != null)
					return result;
			}
			return null;
		}
		
		public virtual bool isSet(Object symbol, Object attribute)
		{
			Object key = makeKey(symbol, attribute);
			Hashtable currentScope = ((Hashtable) contents[scope]);
			return currentScope.ContainsKey(key);
		}
		
		
		public virtual void  set_Renamed(Object symbol, Object attribute, Object value_Renamed)
		{
			Object key = makeKey(symbol, attribute);
			Hashtable currentScope = ((Hashtable) contents[scope]);
			if (currentScope.ContainsKey(key))
			{
				throw new Exception("symbol '" + symbol.ToString() + "'" + " already has the " + "'" + attribute + "' attribute.");
			}
			currentScope[key] = value_Renamed;
		}
		public virtual void  update(Object symbol, Object attribute, Object value_Renamed)
		{
			Object key = makeKey(symbol, attribute);
			Hashtable currentScope = ((Hashtable) contents[scope]);
			if (!currentScope.ContainsKey(key))
			{
				throw new Exception("symbol '" + symbol.ToString() + "'" + " lacks the " + "'" + attribute + "' attribute.");
			}
			currentScope[key] = value_Renamed;
		}
		
		
		public virtual void  beginScope()
		{
			contents.Insert(++scope, Hashtable.Synchronized(new Hashtable()));
		}
		
		
		public virtual void  endScope()
		{
			if (scope == 0)
				throw new Exception("Ending most outter scope");
			contents.RemoveAt(scope--);
		}
		
		public virtual void  setType(Identifier id, Type type)
		{
			this.set_Renamed(id, "type", type);
		}
		
		
		public virtual Type getType(Identifier id)
		{
			Type type = (Type) this.get_Renamed(id, "type");
			if (type == null)
			{
				throw new Exception("No type found for "+id);
			}
			return type;
		}
	}
	class Pair
	{
		
		internal Object car;
		internal Object cdr;
		
		public Pair(Object car, Object cdr)
		{
			this.car = car;
			this.cdr = cdr;
		}
		
		
		public  override bool Equals(Object other)
		{
			try
			{
				return equals((Pair) other);
			}
			catch (InvalidCastException cce)
			{
                Console.Write(cce.Message);
				return false;
			}
		}
		
		
		public bool equals(Pair other)
		{
			return this.car.Equals(other.car) && this.cdr.Equals(other.cdr);
		}
		
		
		public override int GetHashCode()
		{
			return car.GetHashCode() / 2 + cdr.GetHashCode() / 2;
		}
		
		
		public override String ToString()
		{
			return "(" + car.ToString() + " . " + cdr.ToString() + ")";
		}
	}
}