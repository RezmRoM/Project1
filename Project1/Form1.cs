using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Project1
{
    public partial class Form1 : Form
    {
        private readonly string connectionString;

        public Form1()
        {
            try
            {
                InitializeComponent();
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка инициализации приложения: " + ex.Message, "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void authButton_Click(object sender, EventArgs e)
        {
            try
            {
                Form2 form2 = new Form2();
                form2.FormClosed += (s, args) => this.Show();
                form2.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при открытии формы авторизации: " + ex.Message, "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void regButton_Click(object sender, EventArgs e)
        {
            try
            {
                Form3 form3 = new Form3();
                form3.FormClosed += (s, args) => this.Show();
                form3.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при открытии формы регистрации: " + ex.Message, "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
