using System.IO;

namespace TeamspeakWebAdmin
{
    public static class Logger
    {
        private static string path = "C:\\temp\\log.txt";
        private static StreamWriter stream;
        public static void Reset()
        {
            if (stream != null)
                stream.Dispose();
            File.Delete(path);
            stream = File.AppendText(path);
        }

        public static void Log(string txt)
        {
            stream.WriteLine(txt);
        }
    }
}
