using System;
using System.Threading;
using System.Windows.Forms;

namespace Project1
{
    internal static class Program
    {
        private static Mutex mutex = new Mutex(true, "Project1_UniqueInstance");

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                if (!mutex.WaitOne(TimeSpan.Zero, true))
                {
                    MessageBox.Show("Приложение уже запущено!", "Предупреждение",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // Добавляем обработчик необработанных исключений
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                
                // Добавляем обработчик закрытия приложения
                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"Произошла ошибка: {e.Exception.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Критическая ошибка: {((Exception)e.ExceptionObject).Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            try
            {
                // Закрываем все открытые формы
                foreach (Form form in Application.OpenForms)
                {
                    form.Close();
                }

                // Освобождаем ресурсы
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при закрытии приложения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
