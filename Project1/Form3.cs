using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;

namespace Project1
{
    public partial class Form3 : Form
    {
        private readonly string connectionString;

        public Form3()
        {
            try
            {
                InitializeComponent();
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка инициализации формы: " + ex.Message, "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length >= 2 && name.Length <= 50 
                && Regex.IsMatch(name, @"^[А-ЯЁа-яё\s-]+$");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool IsEmailExists(SqlConnection connection, string email)
        {
            string query = "SELECT COUNT(1) FROM Polzovatel WHERE Email = @Email";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private void regButton_Click(object sender, EventArgs e)
        {
            string name = nameBox.Text.Trim();
            string family = familyBox.Text.Trim();
            string otchestvo = otchestvoBox.Text.Trim();
            string email = emailBox.Text.Trim();
            string password = passwordBox.Text;

            // Валидация полей
            if (!IsValidName(name))
            {
                MessageBox.Show("Пожалуйста, введите корректное имя (только русские буквы, дефис и пробел).", 
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidName(family))
            {
                MessageBox.Show("Пожалуйста, введите корректную фамилию (только русские буквы, дефис и пробел).", 
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidName(otchestvo))
            {
                MessageBox.Show("Пожалуйста, введите корректное отчество (только русские буквы, дефис и пробел).", 
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Пожалуйста, введите корректный email адрес.", 
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов.", 
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Проверка существования email
                    if (IsEmailExists(connection, email))
                    {
                        MessageBox.Show("Пользователь с таким email уже существует.", 
                            "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string hashedPassword = HashPassword(password);
                    
                    string query = @"INSERT INTO Polzovatel (Name, Family, Otchestvo, Email, Password, id_role) 
                                   VALUES (@Name, @Family, @Otchestvo, @Email, @Password, @IdRole)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Family", family);
                        command.Parameters.AddWithValue("@Otchestvo", otchestvo);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", hashedPassword);
                        command.Parameters.AddWithValue("@IdRole", 2); // Роль обычного пользователя

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Регистрация успешна! Теперь вы можете войти в систему.", 
                                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Регистрация не удалась. Пожалуйста, попробуйте позже.", 
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
