namespace ConsoleWrapper
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Output_RichTextBox = new System.Windows.Forms.RichTextBox();
            this.Run_Button = new System.Windows.Forms.Button();
            this.ExePath_TextBox = new System.Windows.Forms.TextBox();
            this.Kill_Button = new System.Windows.Forms.Button();
            this.Command_ComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // Output_RichTextBox
            // 
            this.Output_RichTextBox.Location = new System.Drawing.Point(12, 41);
            this.Output_RichTextBox.Name = "Output_RichTextBox";
            this.Output_RichTextBox.Size = new System.Drawing.Size(776, 367);
            this.Output_RichTextBox.TabIndex = 0;
            this.Output_RichTextBox.Text = "";
            // 
            // Run_Button
            // 
            this.Run_Button.BackColor = System.Drawing.Color.Chartreuse;
            this.Run_Button.Location = new System.Drawing.Point(632, 12);
            this.Run_Button.Name = "Run_Button";
            this.Run_Button.Size = new System.Drawing.Size(75, 23);
            this.Run_Button.TabIndex = 1;
            this.Run_Button.Text = "Run";
            this.Run_Button.UseVisualStyleBackColor = false;
            this.Run_Button.Click += new System.EventHandler(this.Run_Button_Click);
            // 
            // ExePath_TextBox
            // 
            this.ExePath_TextBox.Location = new System.Drawing.Point(12, 13);
            this.ExePath_TextBox.Name = "ExePath_TextBox";
            this.ExePath_TextBox.Size = new System.Drawing.Size(614, 23);
            this.ExePath_TextBox.TabIndex = 2;
            this.ExePath_TextBox.Text = "..\\..\\..\\..\\Child\\bin\\Debug\\net6.0\\Child.exe";
            // 
            // Kill_Button
            // 
            this.Kill_Button.BackColor = System.Drawing.Color.Salmon;
            this.Kill_Button.Enabled = false;
            this.Kill_Button.Location = new System.Drawing.Point(713, 12);
            this.Kill_Button.Name = "Kill_Button";
            this.Kill_Button.Size = new System.Drawing.Size(75, 23);
            this.Kill_Button.TabIndex = 1;
            this.Kill_Button.Text = "Kill";
            this.Kill_Button.UseVisualStyleBackColor = false;
            this.Kill_Button.Click += new System.EventHandler(this.Kill_Button_Click);
            // 
            // Command_ComboBox
            // 
            this.Command_ComboBox.FormattingEnabled = true;
            this.Command_ComboBox.Items.AddRange(new object[] {
            "a",
            "b",
            "c",
            "d"});
            this.Command_ComboBox.Location = new System.Drawing.Point(12, 415);
            this.Command_ComboBox.Name = "Command_ComboBox";
            this.Command_ComboBox.Size = new System.Drawing.Size(776, 23);
            this.Command_ComboBox.TabIndex = 3;
            this.Command_ComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Command_ComboBox_KeyDown);
            this.Command_ComboBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Command_ComboBox_KeyUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Command_ComboBox);
            this.Controls.Add(this.ExePath_TextBox);
            this.Controls.Add(this.Kill_Button);
            this.Controls.Add(this.Run_Button);
            this.Controls.Add(this.Output_RichTextBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RichTextBox Output_RichTextBox;
        private Button Run_Button;
        private TextBox ExePath_TextBox;
        private Button Kill_Button;
        private ComboBox Command_ComboBox;
    }
}