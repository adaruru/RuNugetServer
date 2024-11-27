
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
            CurrentQueryTextBox = new TextBox();
            ConnTestBtn = new Button();
            RandomInsert = new Button();
            TablesSelect = new ComboBox();
            InsertCount = new TextBox();
            InsertCountLabel = new Label();
            ManualStopConn = new Button();
            ErrorTextLabel = new Label();
            RandomBulkInsert = new Button();
            SuspendLayout();
            // 
            // CurrentQueryTextBox
            // 
            CurrentQueryTextBox.Location = new Point(19, 59);
            CurrentQueryTextBox.Margin = new Padding(2);
            CurrentQueryTextBox.Multiline = true;
            CurrentQueryTextBox.Name = "CurrentQueryTextBox";
            CurrentQueryTextBox.Size = new Size(304, 95);
            CurrentQueryTextBox.TabIndex = 0;
            // 
            // ConnTestBtn
            // 
            ConnTestBtn.Location = new Point(345, 130);
            ConnTestBtn.Margin = new Padding(2);
            ConnTestBtn.Name = "ConnTestBtn";
            ConnTestBtn.Size = new Size(71, 22);
            ConnTestBtn.TabIndex = 1;
            ConnTestBtn.Text = "測試連線";
            ConnTestBtn.UseVisualStyleBackColor = true;
            ConnTestBtn.Click += ConnTestBtnClick;
            // 
            // RandomInsert
            // 
            RandomInsert.Location = new Point(367, 21);
            RandomInsert.Margin = new Padding(2);
            RandomInsert.Name = "RandomInsert";
            RandomInsert.Size = new Size(71, 22);
            RandomInsert.TabIndex = 2;
            RandomInsert.Text = "亂數新增";
            RandomInsert.UseVisualStyleBackColor = true;
            RandomInsert.Click += RandomInsertClick;
            // 
            // TablesSelect
            // 
            TablesSelect.FormattingEnabled = true;
            TablesSelect.Location = new Point(19, 20);
            TablesSelect.Margin = new Padding(2);
            TablesSelect.Name = "TablesSelect";
            TablesSelect.Size = new Size(171, 23);
            TablesSelect.TabIndex = 3;
            TablesSelect.SelectedIndexChanged += TableComboBoxSelectChange;
            // 
            // InsertCount
            // 
            InsertCount.Location = new Point(255, 20);
            InsertCount.Margin = new Padding(2);
            InsertCount.Name = "InsertCount";
            InsertCount.Size = new Size(97, 23);
            InsertCount.TabIndex = 4;
            // 
            // InsertCountLabel
            // 
            InsertCountLabel.AutoSize = true;
            InsertCountLabel.Location = new Point(198, 25);
            InsertCountLabel.Margin = new Padding(2, 0, 2, 0);
            InsertCountLabel.Name = "InsertCountLabel";
            InsertCountLabel.Size = new Size(55, 15);
            InsertCountLabel.TabIndex = 5;
            InsertCountLabel.Text = "新增筆數";
            // 
            // ManualStopConn
            // 
            ManualStopConn.Location = new Point(551, 21);
            ManualStopConn.Margin = new Padding(2);
            ManualStopConn.Name = "ManualStopConn";
            ManualStopConn.Size = new Size(71, 22);
            ManualStopConn.TabIndex = 6;
            ManualStopConn.Text = "手動停止";
            ManualStopConn.UseVisualStyleBackColor = true;
            ManualStopConn.Click += ManualStopConnClick;
            // 
            // ErrorTextLabel
            // 
            ErrorTextLabel.Location = new Point(19, 164);
            ErrorTextLabel.Margin = new Padding(2, 0, 2, 0);
            ErrorTextLabel.Name = "ErrorTextLabel";
            ErrorTextLabel.Size = new Size(566, 87);
            ErrorTextLabel.TabIndex = 7;
            ErrorTextLabel.Text = "錯誤訊息";
            ErrorTextLabel.Click += ErrorTextLabelCopy;
            // 
            // RandomBulkInsert
            // 
            RandomBulkInsert.Location = new Point(442, 21);
            RandomBulkInsert.Margin = new Padding(2);
            RandomBulkInsert.Name = "RandomBulkInsert";
            RandomBulkInsert.Size = new Size(95, 22);
            RandomBulkInsert.TabIndex = 8;
            RandomBulkInsert.Text = "亂數大批新增";
            RandomBulkInsert.UseVisualStyleBackColor = true;
            RandomBulkInsert.Click += RandomBulkInsertClick;
            // 
            // SqlLabForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(766, 293);
            Controls.Add(RandomBulkInsert);
            Controls.Add(ErrorTextLabel);
            Controls.Add(ManualStopConn);
            Controls.Add(InsertCountLabel);
            Controls.Add(InsertCount);
            Controls.Add(TablesSelect);
            Controls.Add(RandomInsert);
            Controls.Add(ConnTestBtn);
            Controls.Add(CurrentQueryTextBox);
            Margin = new Padding(2);
            Name = "SqlLabForm";
            Text = "SqlLabForm";
            Load += SqlLabFormLoad;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox CurrentQueryTextBox;
        private Button ConnTestBtn;
        private Button RandomInsert;
        private ComboBox TablesSelect;
        private TextBox InsertCount;
        private Label InsertCountLabel;
        private Button ManualStopConn;
        private Label ErrorTextLabel;
        private Button RandomBulkInsert;
    }
}
