namespace SqlLab
{
    partial class SqlLabForm
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
            textBox1 = new TextBox();
            ConnTestBtn = new Button();
            RandomInsert = new Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(46, 65);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(475, 144);
            textBox1.TabIndex = 0;
            // 
            // ConnTestBtn
            // 
            ConnTestBtn.Location = new Point(46, 25);
            ConnTestBtn.Name = "ConnTestBtn";
            ConnTestBtn.Size = new Size(112, 34);
            ConnTestBtn.TabIndex = 1;
            ConnTestBtn.Text = "測試連線";
            ConnTestBtn.UseVisualStyleBackColor = true;
            ConnTestBtn.Click += ConnTestBtnClick;
            // 
            // RandomInsert
            // 
            RandomInsert.Location = new Point(550, 160);
            RandomInsert.Name = "RandomInsert";
            RandomInsert.Size = new Size(112, 34);
            RandomInsert.TabIndex = 2;
            RandomInsert.Text = "亂數新增";
            RandomInsert.UseVisualStyleBackColor = true;
            RandomInsert.Click += RandomInsertClick;
            // 
            // SqlLabForm
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1204, 450);
            Controls.Add(RandomInsert);
            Controls.Add(ConnTestBtn);
            Controls.Add(textBox1);
            Name = "SqlLabForm";
            Text = "SqlLabForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Button ConnTestBtn;
        private Button RandomInsert;
    }
}
