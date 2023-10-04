namespace Band2310
{
    partial class mainForm
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
            this.threadLabel = new System.Windows.Forms.Label();
            this.threadMainButton = new System.Windows.Forms.Button();
            this.threadNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.threadsListView = new System.Windows.Forms.ListView();
            this.threadIDHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dataHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.threadNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // threadLabel
            // 
            this.threadLabel.AutoSize = true;
            this.threadLabel.Location = new System.Drawing.Point(12, 5);
            this.threadLabel.Name = "threadLabel";
            this.threadLabel.Size = new System.Drawing.Size(81, 13);
            this.threadLabel.TabIndex = 0;
            this.threadLabel.Text = "Atšakų skaičius";
            // 
            // threadMainButton
            // 
            this.threadMainButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.threadMainButton.Location = new System.Drawing.Point(12, 47);
            this.threadMainButton.Name = "threadMainButton";
            this.threadMainButton.Size = new System.Drawing.Size(265, 23);
            this.threadMainButton.TabIndex = 1;
            this.threadMainButton.Text = "Start";
            this.threadMainButton.UseVisualStyleBackColor = true;
            this.threadMainButton.Click += new System.EventHandler(this.threadMainButton_Click);
            // 
            // threadNumericUpDown
            // 
            this.threadNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.threadNumericUpDown.Location = new System.Drawing.Point(12, 21);
            this.threadNumericUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.threadNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.threadNumericUpDown.Name = "threadNumericUpDown";
            this.threadNumericUpDown.Size = new System.Drawing.Size(265, 20);
            this.threadNumericUpDown.TabIndex = 3;
            this.threadNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.threadNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // threadsListView
            // 
            this.threadsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.threadsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.threadIDHeader,
            this.dataHeader});
            this.threadsListView.HideSelection = false;
            this.threadsListView.Location = new System.Drawing.Point(12, 76);
            this.threadsListView.Name = "threadsListView";
            this.threadsListView.Size = new System.Drawing.Size(265, 416);
            this.threadsListView.TabIndex = 4;
            this.threadsListView.UseCompatibleStateImageBehavior = false;
            this.threadsListView.View = System.Windows.Forms.View.Details;
            // 
            // threadIDHeader
            // 
            this.threadIDHeader.Text = "Thread ID";
            // 
            // dataHeader
            // 
            this.dataHeader.Text = "Data";
            this.dataHeader.Width = 193;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 504);
            this.Controls.Add(this.threadsListView);
            this.Controls.Add(this.threadNumericUpDown);
            this.Controls.Add(this.threadMainButton);
            this.Controls.Add(this.threadLabel);
            this.Name = "mainForm";
            this.Text = "Band2310";
            ((System.ComponentModel.ISupportInitialize)(this.threadNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label threadLabel;
        private System.Windows.Forms.Button threadMainButton;
        private System.Windows.Forms.NumericUpDown threadNumericUpDown;
        private System.Windows.Forms.ListView threadsListView;
        private System.Windows.Forms.ColumnHeader threadIDHeader;
        private System.Windows.Forms.ColumnHeader dataHeader;
    }
}

