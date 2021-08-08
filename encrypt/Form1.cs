using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace encrypt
{
    public partial class Form1 : Form
    {
        private string path = "";
        private string publicKey = "";
        private string privateKey = "";
        private byte[] a = new byte[1280];
        private string b = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetKey();
        }

        // Encrypt a file.
        public static void AddEncryption(string FileName)
        {

            File.Encrypt(FileName);
        }

        // Decrypt a file.
        public static void RemoveEncryption(string FileName)
        {
            File.Decrypt(FileName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string str = textBox5.Text;
            a = Encryption(str);
            textBox3.Text = Convert.ToBase64String(a);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("123(Encrypted).txt", false))
            {
                file.WriteLine(Convert.ToBase64String(a));
                //file.WriteLine("\t");
                //file.WriteLine(Encrypfilecontent);
            }
        }
        

        private void button4_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 獲取加密所使用的key，RSA算法是一種非對稱密碼算法，所謂非對稱，就是指該算法需要一對密鑰，使用其中一個加密，則需要用另一個才能解密。
        /// </summary>
        public void GetKey()
        {
            string PublicKey = string.Empty;
            string PrivateKey = string.Empty;
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            PublicKey = rSACryptoServiceProvider.ToXmlString(false); // 獲取公匙，用於加密
            PrivateKey = rSACryptoServiceProvider.ToXmlString(true); // 獲取公匙和私匙，用於解密

            textBox1.Text = PublicKey;        // 輸出公匙
            //Console.WriteLine("PrivateKey is {0}", PrivateKey);
            textBox2.Text = PrivateKey;   // 輸出密匙
            // 密匙中含有公匙，公匙是根據密匙進行計算得來的。

            using (StreamWriter streamWriter = new StreamWriter("PublicKey.xml"))
            {
                streamWriter.Write(rSACryptoServiceProvider.ToXmlString(false));// 將公匙保存到運行目錄下的PublicKey
            }
            using (StreamWriter streamWriter = new StreamWriter("PrivateKey.xml"))
            {
                streamWriter.Write(rSACryptoServiceProvider.ToXmlString(true)); // 將公匙&私匙保存到運行目錄下的PrivateKey
            }
        }


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str">需要加密的明文</param>
        /// <returns></returns>
        private static byte[] Encryption(string str)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            using (StreamReader streamReader = new StreamReader("PublicKey.xml")) // 讀取運行目錄下的PublicKey.xml
            {
                rSACryptoServiceProvider.FromXmlString(streamReader.ReadToEnd()); // 將公匙載入進RSA實例中
            }
            byte[] buffer = Encoding.UTF8.GetBytes(str); // 將明文轉換為byte[]

            // 加密後的數據就是一個byte[] 數組,可以以 文件的形式保存 或 別的形式(網上很多教程,使用Base64進行編碼化保存)
            byte[] EncryptBuffer = rSACryptoServiceProvider.Encrypt(buffer, false); // 進行加密

            //string EncryptBase64 = Convert.ToBase64String(EncryptBuffer); // 如果使用base64進行明文化，在解密時 需要再次將base64 轉換為byte[]
            //Console.WriteLine(EncryptBase64);
            return EncryptBuffer;
        }


        private static string Decrypt(byte[] buffer)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            using (StreamReader streamReader = new StreamReader("PrivateKey.xml")) // 讀取運行目錄下的PrivateKey.xml
            {
                rSACryptoServiceProvider.FromXmlString(streamReader.ReadToEnd()); // 將私匙載入進RSA實例中
            }
            // 解密後得到一個byte[] 數組
            try
            {
                byte[] DecryptBuffer = rSACryptoServiceProvider.Decrypt(buffer, false); // 進行解密
                string str = Encoding.UTF8.GetString(DecryptBuffer); // 將byte[]轉換為明文
                return str;
            }
            catch {
                MessageBox.Show("解密失敗");
                return null;
            }          
        }



        private void button4_Click_1(object sender, EventArgs e)
        {
            b = Decrypt(a);
            textBox4.Text = b;
        }

        private void button5_Click(object sender, EventArgs e)
        {   string news = ""; 
            string c = File.ReadAllText(path);
            try
            {
                byte[] bytes = Convert.FromBase64String(c);
                news = Decrypt(bytes);
            }
            catch
            {
                MessageBox.Show("此金鑰已被使用，或無效金鑰");
            }
            textBox4.Text = news;
           
        }
    }
}
