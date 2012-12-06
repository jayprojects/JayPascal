using System;
using System.Collections;
namespace JayPascal
{
	
	public class Node
	{
		virtual public Symbol Symbol
		{
			get
			{
				return this.symbol;
			}
			
		}
        virtual public String Position
        {
            get
            {
                return this.position;
            }

        }
		internal Symbol symbol;
		internal ArrayList children;
		internal Hashtable attributes;
        internal String position;
		
        public Node(Symbol symbol, ArrayList children)
		{
			this.initialize(symbol, children);
		}
		
		public Node(Symbol symbol)
		{
			this.initialize(symbol, null);
		}
		
		public Node(Symbol symbol, Node child)
		{
			ArrayList children = ArrayList.Synchronized(new ArrayList(10));
			children.Add(child);
			this.initialize(symbol, children);
		}
		
		private void  initialize(Symbol symbol, ArrayList children)
		{
            this.position = " Line: " + cursorPosition.rowNumber.ToString() + " col: " + cursorPosition.colNumber.ToString();
			this.symbol = symbol;
			this.children = children;
			this.attributes = Hashtable.Synchronized(new Hashtable());
		}
		
		public virtual int numChildren()
		{
			if (children == null)
				return 0;
			else
				return children.Count;
		}
		
		public virtual Node getChild(int childNum)
		{
            CodeGen.currentChildNumber++;
           
			return (Node) children[childNum];
		}
		
		public virtual void  replaceChild(int childnum, Node newchild)
		{
			this.children[childnum] = newchild;
		}
		
		public virtual Object getAttribute(String attribute)
		{
			return attributes[attribute];
		}
		
		public virtual void  setAttribute(String attribute, Object value_Renamed)
		{
			attributes[attribute] = value_Renamed;
		}
	}
}