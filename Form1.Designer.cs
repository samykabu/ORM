namespace DataObjectGen
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.DatabaseName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SavePath = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.NameSpacetxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ClassPrefix = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(237, 194);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Generate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Database Name";
            // 
            // DatabaseName
            // 
            this.DatabaseName.Location = new System.Drawing.Point(151, 42);
            this.DatabaseName.Name = "DatabaseName";
            this.DatabaseName.Size = new System.Drawing.Size(258, 20);
            this.DatabaseName.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Save To Directory";
            // 
            // SavePath
            // 
            this.SavePath.Location = new System.Drawing.Point(151, 68);
            this.SavePath.Name = "SavePath";
            this.SavePath.Size = new System.Drawing.Size(258, 20);
            this.SavePath.TabIndex = 1;
            this.SavePath.Text = "C:\\DalGen";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(334, 194);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // NameSpacetxt
            // 
            this.NameSpacetxt.Location = new System.Drawing.Point(151, 94);
            this.NameSpacetxt.Name = "NameSpacetxt";
            this.NameSpacetxt.Size = new System.Drawing.Size(258, 20);
            this.NameSpacetxt.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Default Namespace";
            // 
            // ClassPrefix
            // 
            this.ClassPrefix.Location = new System.Drawing.Point(151, 120);
            this.ClassPrefix.Name = "ClassPrefix";
            this.ClassPrefix.Size = new System.Drawing.Size(258, 20);
            this.ClassPrefix.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Class Prefix";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 230);
            this.Controls.Add(this.ClassPrefix);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NameSpacetxt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.SavePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DatabaseName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox DatabaseName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SavePath;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox NameSpacetxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ClassPrefix;
        private System.Windows.Forms.Label label4;        
    }
}

