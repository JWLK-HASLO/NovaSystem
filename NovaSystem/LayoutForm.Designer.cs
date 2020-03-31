namespace NovaSystem
{
    partial class LayoutForm
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
            this.dataGridView_dataList = new System.Windows.Forms.DataGridView();
            this.button_dataImport = new System.Windows.Forms.Button();
            this.textBox_dataPath = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView_dataView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_dataList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_dataView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_dataList
            // 
            this.dataGridView_dataList.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dataGridView_dataList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_dataList.Location = new System.Drawing.Point(1062, 80);
            this.dataGridView_dataList.Name = "dataGridView_dataList";
            this.dataGridView_dataList.RowHeadersWidth = 82;
            this.dataGridView_dataList.RowTemplate.Height = 37;
            this.dataGridView_dataList.Size = new System.Drawing.Size(574, 1000);
            this.dataGridView_dataList.TabIndex = 1;
            // 
            // button_dataImport
            // 
            this.button_dataImport.Location = new System.Drawing.Point(1275, 6);
            this.button_dataImport.Name = "button_dataImport";
            this.button_dataImport.Size = new System.Drawing.Size(133, 43);
            this.button_dataImport.TabIndex = 2;
            this.button_dataImport.Text = "불러오기 ";
            this.button_dataImport.UseVisualStyleBackColor = true;
            this.button_dataImport.Click += new System.EventHandler(this.button_dataImport_Click);
            // 
            // textBox_dataPath
            // 
            this.textBox_dataPath.Location = new System.Drawing.Point(12, 12);
            this.textBox_dataPath.Name = "textBox_dataPath";
            this.textBox_dataPath.Size = new System.Drawing.Size(1244, 35);
            this.textBox_dataPath.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1414, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 43);
            this.button1.TabIndex = 4;
            this.button1.Text = "저장";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // dataGridView_dataView
            // 
            this.dataGridView_dataView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dataGridView_dataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_dataView.Location = new System.Drawing.Point(0, 80);
            this.dataGridView_dataView.Name = "dataGridView_dataView";
            this.dataGridView_dataView.RowHeadersWidth = 82;
            this.dataGridView_dataView.RowTemplate.Height = 37;
            this.dataGridView_dataView.Size = new System.Drawing.Size(1044, 1022);
            this.dataGridView_dataView.TabIndex = 0;
            // 
            // LayoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1648, 1102);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_dataPath);
            this.Controls.Add(this.button_dataImport);
            this.Controls.Add(this.dataGridView_dataList);
            this.Controls.Add(this.dataGridView_dataView);
            this.Name = "LayoutForm";
            this.Text = "New Layout";
            this.Load += new System.EventHandler(this.LayoutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_dataList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_dataView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView_dataList;
        private System.Windows.Forms.Button button_dataImport;
        private System.Windows.Forms.TextBox textBox_dataPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView_dataView;
    }
}