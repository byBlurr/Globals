using Discord;
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

        public static string GetImageUrl(string file)
        {
            if (File.Exists(file))
            {
                var message = (CommandHandler.GetBot().GetUser(376841246955667459) as IUser).SendFileAsync(file);
                foreach (var attachment in message.Result.Attachments)
                {
                    return attachment.Url;
                }
            }

            // If it makes it out, we have an error... oopsie
            Console.WriteLine("The image failed...");
            return null;
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
