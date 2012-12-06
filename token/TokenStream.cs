using System;
namespace JayPascal
{
	
	public interface TokenStream
	{
		
		bool hasMore();
		
		Token peek();
		Token next();
	}
}