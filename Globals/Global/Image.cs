using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Global
{
    class Image
    {
        public static readonly string appdir = AppContext.BaseDirectory;

        public static string SaveImage(string filename, string file)
        {
            string dir = appdir + "attachments/";

            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(file, dir + filename);
            }

            return dir + filename;
        }

        public static void DeleteImage(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}
