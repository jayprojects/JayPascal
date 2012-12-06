JayPascal
=========
This is a Pascal compiler. Written in C# under .Net environment.
It can compile Pascal source code and produce corresponding assembly source code. The assembly source code then can be compile/assemble use Masm assembler. 

Installation:
=========
1.	Copy the “masm32” folder to c: drive. JayPascal folder has the JayPascal.exe file. This the main executable file of my project.
2.	Copy JayPascal to local drive. Run JayPascal.exe


Indluded Folders:
=========
1.	Masm32 : This folder has “masm32rt.inc” file. This file is required for printf and scanf procedure. 
Also has a modified bat file to make the bridge between masm and JayPascal.

2.	Pascal Test Programs: This folder contains some pascal source program. You may use these file to test the compiler. This file also copied to JayPascal folder for quick access from the application.
