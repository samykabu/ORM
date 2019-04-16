using System.Drawing;
using System.Windows.Forms;
using SchemaObjects;

namespace Sql2005Server.SchemaProvider.Forms
{
    public partial class ConnectionBuilder : Form, IConnectionStringBuilder
    {
        private string _Constr = string.Empty;
        private Button button1;
        private CheckBox checkBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Panel panel1;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;

        public ConnectionBuilder()
        {
        }

        #region IConnectionStringBuilder Members

        public string ConnectionString()
        {
            return _Constr;
        }

        public void ShowForm()
        {
            Show();
        }

        #endregion

        private void InitializeComponent()
        {
            label1 = new Label();
            textBox1 = new TextBox();
            checkBox1 = new CheckBox();
            panel1 = new Panel();
            label2 = new Label();
            textBox2 = new TextBox();
            label3 = new Label();
            textBox3 = new TextBox();
            button1 = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 43);
            label1.Name = "label1";
            label1.Size = new Size(69, 13);
            label1.TabIndex = 0;
            label1.Text = "Server Name";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(88, 40);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(171, 20);
            textBox1.TabIndex = 1;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(88, 66);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(141, 17);
            checkBox1.TabIndex = 4;
            checkBox1.Text = "Windows Authuntication";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(textBox3);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(label2);
            panel1.Location = new Point(12, 103);
            panel1.Name = "panel1";
            panel1.Size = new Size(260, 64);
            panel1.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(5, 7);
            label2.Name = "label2";
            label2.Size = new Size(58, 13);
            label2.TabIndex = 0;
            label2.Text = "User name";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(76, 4);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(171, 20);
            textBox2.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 34);
            label3.Name = "label3";
            label3.Size = new Size(53, 13);
            label3.TabIndex = 2;
            label3.Text = "Password";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(76, 34);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(171, 20);
            textBox3.TabIndex = 3;
            // 
            // button1
            // 
            button1.Location = new Point(184, 173);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 6;
            button1.Text = "Connect";
            button1.UseVisualStyleBackColor = true;
            // 
            // ConnectionBuilder
            // 
            ClientSize = new Size(284, 203);
            Controls.Add(button1);
            Controls.Add(panel1);
            Controls.Add(checkBox1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Name = "ConnectionBuilder";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}