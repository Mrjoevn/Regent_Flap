using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FLap_New.Object
{
    public class ConnectionToSql
    {
        private string ConnectionString;

       public ConnectionToSql(string ConnectionString)
       {
            this.ConnectionString = ConnectionString;
       }

        public string[] GetRawConnectionString()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            Console.WriteLine(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            // Lấy tất cả các thẻ <app> trong <connectionString>
            XmlNodeList appNodes = xml.SelectNodes("//connectionString/app");
            if (appNodes == null || appNodes.Count == 0)
                throw new Exception("Không tìm thấy thẻ connectionString/app trong App.config");

            HillCipher4x4 hill_Encryption = new HillCipher4x4();

            string[] strings = new string[5];
            int i = 0;
            foreach (XmlNode node in appNodes)
            {
                string raw = node.Attributes["connectionString"].Value;

                // Chuyển đổi từ chuỗi tự định nghĩa sang connection string chuẩn
                string[] parts = raw.Split('~');
                string dbName = parts[0].Split(':')[1];
                string server = parts[1].Split(':')[1];
                string user = parts[2].Split(':')[1];
                string pass = parts[3].Split(':')[1];
                i++;
                pass = hill_Encryption.Decrypt(pass).ToLower();

                string connString = $"Data Source={server};Initial Catalog={dbName};User ID={user};Password={pass}";
                strings[i] = connString;
                Console.WriteLine(connString); // hoặc lưu vào list, dictionary, ...
            }
            return strings;
        }
    }
}
