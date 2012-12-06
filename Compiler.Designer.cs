namespace JayPascal
{
    partial class Compiler
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.label2 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBoxInput = new System.Windows.Forms.RichTextBox();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.buttonExit2 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonRun = new System.Windows.Forms.Button();
            this.buttonTreeGen = new System.Windows.Forms.Button();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonCompile = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(105, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 23;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(418, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(2, 450);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(628, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 17);
            this.label2.TabIndex = 22;
            this.label2.Text = "Assembly Output";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(398, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(2, 450);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(110, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 17);
            this.label1.TabIndex = 21;
            this.label1.Text = "Pascal Source";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.splitter2);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.richTextBoxInput);
            this.panel1.Controls.Add(this.richTextBoxOutput);
            this.panel1.Location = new System.Drawing.Point(24, 78);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(765, 450);
            this.panel1.TabIndex = 20;
            // 
            // richTextBoxInput
            // 
            this.richTextBoxInput.Dock = System.Windows.Forms.DockStyle.Left;
            this.richTextBoxInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxInput.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxInput.Name = "richTextBoxInput";
            this.richTextBoxInput.Size = new System.Drawing.Size(398, 450);
            this.richTextBoxInput.TabIndex = 0;
            this.richTextBoxInput.Text = "";
            this.richTextBoxInput.TextChanged += new System.EventHandler(this.richTextBoxInput_TextChanged);
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Dock = System.Windows.Forms.DockStyle.Right;
            this.richTextBoxOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxOutput.Location = new System.Drawing.Point(420, 0);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.Size = new System.Drawing.Size(345, 450);
            this.richTextBoxOutput.TabIndex = 1;
            this.richTextBoxOutput.Text = "";
            this.richTextBoxOutput.TextChanged += new System.EventHandler(this.richTextBoxOutput_TextChanged);
            // 
            // buttonExit2
            // 
            this.buttonExit2.Location = new System.Drawing.Point(620, 17);
            this.buttonExit2.Name = "buttonExit2";
            this.buttonExit2.Size = new System.Drawing.Size(66, 23);
            this.buttonExit2.TabIndex = 19;
            this.buttonExit2.Text = "Exit";
            this.buttonExit2.UseVisualStyleBackColor = true;
            this.buttonExit2.Click += new System.EventHandler(this.buttonExit2_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.Location = new System.Drawing.Point(24, 535);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(362, 21);
            this.checkBox1.TabIndex = 18;
            this.checkBox1.Text = "Allow Real to Integer Assignment (Round conversion)";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(314, 17);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 17;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // buttonTreeGen
            // 
            this.buttonTreeGen.Location = new System.Drawing.Point(411, 17);
            this.buttonTreeGen.Name = "buttonTreeGen";
            this.buttonTreeGen.Size = new System.Drawing.Size(108, 23);
            this.buttonTreeGen.TabIndex = 14;
            this.buttonTreeGen.Text = "GenerateTree";
            this.buttonTreeGen.UseVisualStyleBackColor = true;
            this.buttonTreeGen.Click += new System.EventHandler(this.buttonTreeGen_Click);
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(24, 17);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonOpen.TabIndex = 13;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(525, 17);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(89, 23);
            this.buttonExit.TabIndex = 16;
            this.buttonExit.Text = "Save and Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonCompile
            // 
            this.buttonCompile.Location = new System.Drawing.Point(186, 17);
            this.buttonCompile.Name = "buttonCompile";
            this.buttonCompile.Size = new System.Drawing.Size(122, 23);
            this.buttonCompile.TabIndex = 15;
            this.buttonCompile.Text = "Compile";
            this.buttonCompile.UseVisualStyleBackColor = true;
            // 
            // Compiler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 572);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonExit2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.buttonTreeGen);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonCompile);
            this.Name = "Compiler";
            this.Text = "Compiler";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox richTextBoxInput;
        public System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.Button buttonExit2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonTreeGen;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonCompile;
    }
}