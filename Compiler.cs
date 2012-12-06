using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace JayPascal
{
    public partial class Compiler : Form
    {
        public Compiler()
        {
            InitializeComponent();
        }




        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            string path = Application.StartupPath;
            openFileDialog1.InitialDirectory = path + "\\Pascal Program";
            openFileDialog1.Filter = "All files (*.*)|*.*|Pascal Source (*.pas)|*.pas|txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    richTextBoxInput.LoadFile(openFileDialog1.OpenFile(), RichTextBoxStreamType.PlainText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

        }



        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                richTextBoxInput.LoadFile("lastFile.pas", RichTextBoxStreamType.PlainText);
            }
            catch
            {
                richTextBoxInput.Text = "";
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBoxInput.SaveFile("lastFile.pas", RichTextBoxStreamType.PlainText);

            }
            catch
            {
                Console.Write("Save file failed");
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            compile();

        }
        private void compile()
        {
            // Application.Restart();

            buttonCompile.Enabled = false;
            buttonCompile.Text = "Refresh to Compile";
            try
            {
                richTextBoxInput.SaveFile("lastFile.pas", RichTextBoxStreamType.PlainText);

            }
            catch
            {
                Console.WriteLine("Error: File can't be save");
            }


            try
            {
                StreamReader reader = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(preProcess())));
                CharStream acs = new CharStream(reader);
                try
                {
                    Scanner tokenizer = new Scanner(acs);
                    try
                    {
                        Parser parser = new Parser();
                        Node tree = parser.parse(tokenizer);

                        try
                        {
                            StringBuilder outputText = new StringBuilder();
                            CodeGen.generate(tree, outputText);
                            richTextBoxOutput.Text = outputText.ToString();
                        }
                        catch (CompilerException ec) //catch error from generating code
                        {
                            richTextBoxOutput.Text = ec.Message;
                            return;
                        }
                        catch (Exception ec2) //generic Error
                        {
                            richTextBoxOutput.Text = ec2.Message;
                            return;
                        }
                    }
                    catch (ParserException pe) // chatch error from parsing
                    {
                        richTextBoxOutput.Text = pe.Message;
                        return;
                    }
                    catch (Exception pe2) //generic Error
                    {
                        richTextBoxOutput.Text = pe2.Message;
                        return;
                    }
                }
                catch (ScannerException se) // catch error from scanning
                {
                    richTextBoxOutput.Text = se.Message;
                    return;
                }
                catch (Exception se2) //generic Error
                {
                    richTextBoxOutput.Text = se2.Message;
                    return;
                }
            }
            catch (Exception eio) //io error
            {
                richTextBoxOutput.Text = "Unable to read or process the file" + eio.Message;
                return;
            }

        }
        private string preProcess()
        {

            StringBuilder inputText = new StringBuilder();

            foreach (string str in richTextBoxInput.Lines)
            {
                if ((str.Contains("//")) || str.Equals(""))
                {
                    if (str.IndexOf("//") > 0)
                    {
                        inputText.AppendLine(str.Substring(0, str.IndexOf("//")));
                    }
                }
                else
                {

                    inputText.AppendLine(str);
                }

            }

            return inputText.ToString();

            //return richTextBoxInput.Text;
        }
        private void buttonTreeGen_Click(object sender, EventArgs e)
        {
            StreamReader reader = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(preProcess())));
            CharStream acs = new CharStream(reader);
            Scanner tokenizer = new Scanner(acs);
            Parser parser = new Parser();
            Node tree = parser.parse(tokenizer);

            StringBuilder outputText = new StringBuilder();

            TreePrinter.print(tree, outputText);

            richTextBoxOutput.Text = outputText.ToString();

        }

        private void buttonRun_Click(object sender, EventArgs e)
        {

            if (richTextBoxOutput.Text == "")
                compile();
            string path = Application.StartupPath;

            //save Asm file
            string filename = "code.asm";
            richTextBoxOutput.SaveFile(path + "\\" + filename, RichTextBoxStreamType.PlainText);
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            //Create bat files
            string text = "SET PATH=c:\\masm32\\bin" + Environment.NewLine +
                    "SET INCLUDE=c:\\masm32\\INCLUDE" + Environment.NewLine +
                    "SET LIB=c:\\masm32\\LIB" + Environment.NewLine +

                    "make32 code" + Environment.NewLine +
                    "tlink /t code" + Environment.NewLine +
                    "cls";
            System.IO.File.WriteAllText(path + "\\make.bat", text);

            text = "cls" + Environment.NewLine +
                "code" + Environment.NewLine +
                //  "pause" + Environment.NewLine +
                "cls";
            System.IO.File.WriteAllText(path + "\\run.bat", text);



            //Run the bat files
            System.Diagnostics.Process.Start(path + "\\make.bat");
            System.Threading.Thread.Sleep(1000);

            System.Diagnostics.Process.Start(path + "\\run.bat");

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                CodeGen.allowRealToInt = true;
            else
                CodeGen.allowRealToInt = false;

        }

        private void buttonExit2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                richTextBoxInput.SaveFile("lastFile.pas", RichTextBoxStreamType.PlainText);

            }
            catch
            {
                Console.WriteLine("Error: File can't be save");
            }
            Application.Restart();
        }

        private void richTextBoxInput_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBoxOutput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
