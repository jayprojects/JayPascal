using System;
using System.Text;
namespace JayPascal
{
	public class TreePrinter
	{
        
		private const String indent = "          ";

        public static void print(Node root, StringBuilder out_Renamed)
		{
			print(root, out_Renamed, "");
		}
		
		private static void  print(Node root, StringBuilder out_Renamed, String offset)
		{
			
			if (root != null)
			{
                out_Renamed.Append(offset + root.Symbol.ToString()+Environment.NewLine);
				int numChildren = root.numChildren();
				for (int childNum = 0; childNum < numChildren; ++childNum)
				{
					print(root.getChild(childNum), out_Renamed, offset + indent);
				}
			}
            //else out_Renamed.WriteLine(offset + "/NULL/")
		}
	}
}