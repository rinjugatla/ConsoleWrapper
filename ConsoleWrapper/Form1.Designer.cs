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
            this.ExePath_TextBox = new System.Windows.Forms.TextBox();
            this.Command_ComboBox = new System.Windows.Forms.ComboBox();
            this.ProcessControl_Button = new System.Windows.Forms.Button();
            this.CommandHistory_ListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // Output_RichTextBox
            // 
            this.Output_RichTextBox.Location = new System.Drawing.Point(12, 41);
            this.Output_RichTextBox.Name = "Output_RichTextBox";
            this.Output_RichTextBox.Size = new System.Drawing.Size(623, 367);
            this.Output_RichTextBox.TabIndex = 0;
            this.Output_RichTextBox.Text = "";
            // 
            // ExePath_TextBox
            // 
            this.ExePath_TextBox.Location = new System.Drawing.Point(12, 13);
            this.ExePath_TextBox.Name = "ExePath_TextBox";
            this.ExePath_TextBox.Size = new System.Drawing.Size(695, 23);
            this.ExePath_TextBox.TabIndex = 2;
            this.ExePath_TextBox.Text = "..\\..\\..\\..\\Child\\bin\\Debug\\net6.0\\Child.exe";
            // 
            // Command_ComboBox
            // 
            this.Command_ComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Command_ComboBox.Enabled = false;
            this.Command_ComboBox.FormattingEnabled = true;
            this.Command_ComboBox.Items.AddRange(new object[] {
            "a",
            "b",
            "c",
            "d"});
            this.Command_ComboBox.Location = new System.Drawing.Point(12, 415);
            this.Command_ComboBox.Name = "Command_ComboBox";
            this.Command_ComboBox.Size = new System.Drawing.Size(776, 24);
            this.Command_ComboBox.TabIndex = 3;
            this.Command_ComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.Command_ComboBox_DrawItem);
            this.Command_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Command_ComboBox_SelectedIndexChanged);
            this.Command_ComboBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Command_ComboBox_KeyUp);
            // 
            // ProcessControl_Button
            // 
            this.ProcessControl_Button.BackColor = System.Drawing.Color.Chartreuse;
            this.ProcessControl_Button.Location = new System.Drawing.Point(713, 12);
            this.ProcessControl_Button.Name = "ProcessControl_Button";
            this.ProcessControl_Button.Size = new System.Drawing.Size(75, 23);
            this.ProcessControl_Button.TabIndex = 4;
            this.ProcessControl_Button.Text = "Run";
            this.ProcessControl_Button.UseVisualStyleBackColor = false;
            this.ProcessControl_Button.Click += new System.EventHandler(this.ProcessControl_Button_Click);
            // 
            // CommandHistory_ListBox
            // 
            this.CommandHistory_ListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.CommandHistory_ListBox.FormattingEnabled = true;
            this.CommandHistory_ListBox.Location = new System.Drawing.Point(641, 42);
            this.CommandHistory_ListBox.Name = "CommandHistory_ListBox";
            this.CommandHistory_ListBox.Size = new System.Drawing.Size(147, 364);
            this.CommandHistory_ListBox.TabIndex = 5;
            this.CommandHistory_ListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.CommandHistory_ListBox_DrawItem);
            this.CommandHistory_ListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.CommandHistory_ListBox_MeasureItem);
            this.CommandHistory_ListBox.DoubleClick += new System.EventHandler(this.CommandHistory_ListBox_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.CommandHistory_ListBox);
            this.Controls.Add(this.ProcessControl_Button);
            this.Controls.Add(this.Command_ComboBox);
            this.Controls.Add(this.ExePath_TextBox);
            this.Controls.Add(this.Output_RichTextBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RichTextBox Output_RichTextBox;
        private TextBox ExePath_TextBox;
        private ComboBox Command_ComboBox;
        private Button ProcessControl_Button;
        private ListBox CommandHistory_ListBox;
    }
}