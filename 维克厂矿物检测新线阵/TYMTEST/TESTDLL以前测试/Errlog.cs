using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYMDLL
{

    internal class ErrLog
    {
        static object _root = new object();
        static ErrLog recorder;

        List<string> content;
        DateTime time;
        string dst;

        public static ErrLog Recorder
        {
            get
            {
                lock (_root)
                {
                    if (recorder == null)
                    {
                        recorder = new ErrLog();
                    }
                    return recorder;
                }
            }
        }
        ErrLog()
        {
            time = DateTime.Now;
            content = new List<string>();
            string path = ".\\log\\line_err_log\\";
            CreateDirectoryRecursively(path);
            dst = path + "log" + time.ToString("yyyy_MM_dd__HH_mm_sss") + ".txt";
        }

        private void CreateDirectoryRecursively(string path)
        {
            if (!Directory.Exists(path))
            {
                CreateDirectoryRecursively(Path.GetDirectoryName(path));
                Directory.CreateDirectory(path);
            }

        }

        public static ErrLog GetRecorder()
        {
            return Recorder;
        }

        public void AddErrMsg(string msg)
        {
            content.Add(msg);
        }

        public void WriteMsg()
        {
            FileStream fs = new FileStream(dst, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (string line in content)
            {
                sw.WriteLine(line);
            }
            sw.Close();
            fs.Close();
            Console.ReadKey();
        }

    }
}
