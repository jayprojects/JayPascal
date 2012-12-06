using System;
namespace JayPascal
{
	[Serializable]
    public class myException : Exception
    {
        public override string Message
        {
            get
            {
                return base.Message.ToString() + "\nError: on Line: " + cursorPosition.rowNumber +
                " and Column: " + cursorPosition.colNumber + "\n";
            }
        }
        public myException(string Message)
            : base(Message)
        {
            
            //return;
        }
        public myException()
            : base()
        {
            //return;
        }
    }

    public class ScannerException : Exception
    {
        public override string Message
        {
            get
            {
                return base.Message.ToString() + "\nError: on Line: " + cursorPosition.rowNumber +
                " and Column: " + cursorPosition.colNumber + "\n";
            }
        }
        public ScannerException(string Message)
            : base(Message)
        {

            //return;
        }
        public ScannerException()
            : base()
        {
            //return;
        }
    }
    public class ParserException : Exception
    {
        public override string Message
        {
            get
            {
                return base.Message.ToString() + "\nError: on Line: " + cursorPosition.rowNumber +
                " and Column: " + cursorPosition.colNumber + "\n";
            }
        }
        public ParserException(string Message)
            : base(Message)
        {

            //return;
        }
        public ParserException()
            : base()
        {
            //return;
        }
    }

    public class CompilerException : Exception
    {
        Node Nd;
        public override string Message
        {
            get
            {
                return base.Message.ToString() + "\nError: on " +Nd.Position + "\n";
            }
        }
        public CompilerException(string Message, Node n)
            : base(Message)
        {
            Nd = n;
            //return;
        }
        public CompilerException()
            : base()
        {
            //return;
        }
    }

	public class PascalException:Exception
	{

        


		public PascalException(String reason):base(reason)
		{
            //Environment.Exit(1);
            dieError.die(reason);
		}
		public PascalException():base()
		{
            //Environment.Exit(1);
            dieError.die("");
		}
	}

    public class cursorPosition
    {
        public static int rowNumber=1;
        public static int colNumber=1;
    }
    public class dieError
    {
        public static void die(string message)
        {
            //cursorPosition.rowNumber++;
            //cursorPosition.colNumber--;
            /*
            Console.Write(message +  
                "\nError: on Line: " + cursorPosition.rowNumber +
                " and Column: "+ cursorPosition.colNumber+"\n");
             */
            var form = System.Windows.Forms.Form.ActiveForm as Compiler; 
            if (form != null) 
            { 
                form.richTextBoxOutput.Text = message +  
                "\nError: on Line: " + cursorPosition.rowNumber +
                " and Column: "+ cursorPosition.colNumber+"\n";
            }
            //Environment.Exit(0);
        }
    }
    public class newExc : Exception
    {
        
    }
}