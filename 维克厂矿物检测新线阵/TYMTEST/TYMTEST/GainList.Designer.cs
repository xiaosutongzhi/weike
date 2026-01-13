namespace TYMTest
{
    partial class GainList
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
            this.gain_table = new System.Windows.Forms.DataGridView();
            this.Col_card_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_lowgain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_highgain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gain_table)).BeginInit();
            this.SuspendLayout();
            // 
            // gain_table
            // 
            this.gain_table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gain_table.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_card_no,
            this.Col_lowgain,
            this.Col_highgain});
            this.gain_table.Location = new System.Drawing.Point(23, 28);
            this.gain_table.Name = "gain_table";
            this.gain_table.RowHeadersVisible = false;
            this.gain_table.RowTemplate.Height = 27;
            this.gain_table.Size = new System.Drawing.Size(425, 478);
            this.gain_table.TabIndex = 0;
            // 
            // Col_card_no
            // 
            this.Col_card_no.HeaderText = "Card_No";
            this.Col_card_no.Name = "Col_card_no";
            // 
            // Col_lowgain
            // 
            this.Col_lowgain.HeaderText = "Low Gain";
            this.Col_lowgain.Name = "Col_lowgain";
            // 
            // Col_highgain
            // 
            this.Col_highgain.HeaderText = "High Gain";
            this.Col_highgain.Name = "Col_highgain";
            // 
            // GainList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 540);
            this.Controls.Add(this.gain_table);
            this.MaximizeBox = false;
            this.Name = "GainList";
            this.Text = "GainList";
            ((System.ComponentModel.ISupportInitialize)(this.gain_table)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gain_table;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_card_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_lowgain;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_highgain;
    }
}