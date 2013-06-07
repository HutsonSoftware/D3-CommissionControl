using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace D3.Commission
{
    public class Settings
    {
        public Settings()
        { 
            GetSettingsFromFile();
        }

        private void GetSettingsFromFile()
        {
            FilePath = string.Format("{0}\\Settings.xml", FileUtility.GetAssemblyDirectory());

            if (!File.Exists(FilePath))
                CreateSettingsFile();

            XmlReader reader = XmlReader.Create(FilePath);
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "ServerName":
                            if (reader.Read())
                                ServerName = reader.Value.Trim();
                            break;
                        case "DatabaseName":
                            if (reader.Read())
                                DatabaseName = reader.Value.Trim();
                            break;
                        case "IntegratedSecurity":
                            if (reader.Read())
                                IntegratedSecurity = (reader.Value.Trim() == "1" ? true : false);
                            break;
                        case "UserID":
                            if (reader.Read())
                                UserID = reader.Value.Trim();
                            break;
                        case "Password":
                            if (reader.Read())
                                Password = reader.Value.Trim();
                            break;
                    }
                }
            }
            reader.Close();
            reader = null;
        }

        private void CreateSettingsFile()
        {
            Assembly assembly = GetType().Assembly;
            BinaryReader reader = new BinaryReader(assembly.GetManifestResourceStream("D3.Commission.Settings.xml"));
            FileStream stream = new FileStream(FilePath, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            try
            {
                byte[] buffer = new byte[64 * 1024];
                int numread = reader.Read(buffer, 0, buffer.Length);

                while (numread > 0)
                {
                    writer.Write(buffer, 0, numread);
                    numread = reader.Read(buffer, 0, buffer.Length);
                }

                writer.Flush();
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        public void Save()
        {
            SaveSettingsToFile();
        }

        private void SaveSettingsToFile()
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
            xmlWriterSettings.OmitXmlDeclaration = true;

            XmlWriter writer = XmlWriter.Create(FilePath, xmlWriterSettings);
            writer.WriteStartElement("Settings");

            writer.WriteElementString("ServerName", ServerName);
            writer.WriteElementString("DatabaseName", DatabaseName);
            writer.WriteElementString("IntegratedSecurity", (IntegratedSecurity == true ? "1" : "0"));
            writer.WriteElementString("UserID", UserID);
            writer.WriteElementString("Password", Password);

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();
            writer = null;
        }

        public string GetConnectionString()
        {
            if (IntegratedSecurity)
            {
                return string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", ServerName, DatabaseName);
            }
            else
            {
                return string.Format("Data Source={0};Initial Catalog={1};Uid={2};Pwd={3};", ServerName, DatabaseName, UserID, Password);
            }
        }
        
        internal string FilePath { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }

        
    }
}

