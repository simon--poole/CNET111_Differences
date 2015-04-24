using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
namespace TCP_Socket
{
    static class ServerClass
    {
        public static String WorkingDirectory;
        public static Regex rInstance;
        public static string getFileLoc(string path)
        {   Uri public_dir = new Uri(WorkingDirectory, UriKind.Absolute);
            String file = public_dir.AbsolutePath + path;
            //Check if file path ends in any .extension.
            rInstance = new Regex(@"\.(\S+)$");
            Match match = rInstance.Match(file);
            //if file ends in an extension, then we can just serve that file.
            if (match.Success)
            {
                return file;
            }
            else
            {
                rInstance = new Regex(@"\/$");
                match = rInstance.Match(file);
                //File path ends in / already, so we can just add index.html
                //eg /public/folder/ -> /public/folder/index.html
                if (match.Success)
                {
                    return file + "index.html";
                } else 
                //No / at end, so add one too.
                //eg /public/folder -> /public/folder/index.html
                {
                    return file + "/index.html";
                }
            }
        }
        public static Boolean fileExists(string path)
        {
            return File.Exists(path);
        }
        public static String getContentType(string path)
        {
            rInstance = new Regex(@"\.(\S+)$");
            Match result = rInstance.Match(path);
            //Isolate file extension
            String extension = result.Captures[0].ToString().Remove(0, 1);
            //Return appropriate content-type for extension.
            //Common website ones included below, but this would need to be extended for images etc.
            switch(extension){
                case "html":
                    return "text/html";
                case "css":
                    return "text/css";
                case "js":
                    return "application/javascript";
                case "jpg":
                case "jpeg":
                    return "image/jpeg";
                default:
                    return "text/plain";
            }
        }
        public static Byte[] loadFile(string path)
        {
            //Load entire file into byte array and return.
            return File.ReadAllBytes(path);
        }
    }
}
