using System;
using System.IO;

namespace D3.Commission
{
    internal class LogUtility : IDisposable
    {
        private string logFile;

        public LogUtility()
        {
            GetNewLogFile();
        }

        private void GetNewLogFile()
        {
            string directory = FileUtility.GetAssemblyDirectory() + "\\Logs";

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            logFile = directory + "\\" + DateTime.Now.ToFileTimeUtc() + ".log";
        }

        StreamWriter log;
        public void Log(string logInfo)
        {
            if (!File.Exists(logFile))
                log = new StreamWriter(logFile);
            else
                log = File.AppendText(logFile);

            log.WriteLine(DateTime.Now);
            log.WriteLine(logInfo);
            log.WriteLine();

            log.Close();
        }

        public void Dispose()
        {
            log.Dispose();
        }
    }
}
