using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OOP24
{
    public partial class Form1 : Form
    {
        private Thread thread1;
        private Thread thread2;
        private Thread thread3;


        public Form1()
        {
            InitializeComponent();
        }

        private void Timeout(Label label, int seconds)
        {
            label.BackColor = Color.Red;
            Thread.Sleep(seconds * 1000);
            label.BackColor = Color.Orange;
            Thread.Sleep(seconds * 1000);
            label.BackColor = Color.Lime;
            Thread.Sleep(seconds * 1000);
        }


        #region Завдання1(KHUFU)
        private void KHUFU()
        {
            try
            {
                string plaintext = textBox1.Text;
                byte[] plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
                byte[] key = System.Text.Encoding.UTF8.GetBytes("secretkey");

                Khufu khufu = new Khufu(key);
                byte[] encrypted = khufu.Encrypt(plaintextBytes);

                Timeout(label1, 1);

                label1.Text = (BitConverter.ToString(encrypted).Replace("-", ""));
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}"); }
        }
        #endregion

        #region Завдання 2 (HAVAL)
        private void HAVAL()
        {
            try
            {
                string input = textBox2.Text;
                string hashResult = CalculateHaval256(input);
                Timeout(label2, 1);
                label2.Text= hashResult;



            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}"); }
        }

        public static string CalculateHaval256(string input)
        {
            using (SHA256Managed hashAlgorithm = new SHA256Managed())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = hashAlgorithm.ComputeHash(inputBytes);

                // Перетворення байтів хеша у рядок у шістнадцятковому форматі
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }
        #endregion

        #region Завдання 3 (DSA)

        private void DSA()
        {
            try
            {
                // Створення ключів
                using (DSACryptoServiceProvider dsa = new DSACryptoServiceProvider())
                {
                    // Генеруємо ключі
                    var privateKey = dsa.ExportParameters(true);
                    var publicKey = dsa.ExportParameters(false);

                    // Повідомлення для підпису
                    string message = textBox3.Text;
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                    // Підпис повідомлення
                    byte[] signature = Sign(messageBytes, privateKey);

                    Timeout(label3, 1);
                    label3.Text = BitConverter.ToString(signature).Replace("-", "");
                }
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}"); }
        }

        // Метод для підпису повідомлення
        public static byte[] Sign(byte[] message, DSAParameters privateKey)
        {
            using (DSACryptoServiceProvider dsa = new DSACryptoServiceProvider())
            {
                dsa.ImportParameters(privateKey);
                return dsa.SignData(message, HashAlgorithmName.SHA1);
            }
        }
        #endregion


        private void button4_Click(object sender, EventArgs e)
        {
            thread1?.Interrupt();
            thread2?.Interrupt();
            thread3?.Interrupt();

            label1.Text = string.Empty;
            label2.Text = string.Empty;
            label3.Text = string.Empty;

            thread1 = new Thread(new ThreadStart(KHUFU));
            thread2 = new Thread(new ThreadStart(HAVAL));
            thread3 = new Thread(new ThreadStart(DSA));

            thread1.Start();
            thread2.Start();
            thread3.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = string.Empty;

            thread1?.Interrupt();
            thread1 = new Thread(new ThreadStart(KHUFU));
            thread1.Start();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            label2.Text = string.Empty;

            thread2?.Interrupt();
            thread2 = new Thread(new ThreadStart(HAVAL));
            thread2.Start();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            label3.Text = string.Empty;

            thread3?.Interrupt();
            thread3 = new Thread(new ThreadStart(DSA));
            thread3.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            thread1?.Interrupt();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            thread2?.Interrupt();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            thread3?.Interrupt();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            thread1?.Interrupt();
            thread2?.Interrupt();
            thread3?.Interrupt();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread1?.Abort();
            thread2?.Abort();
            thread3?.Abort();
        }
    }
}
