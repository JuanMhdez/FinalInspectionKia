using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FinalInspectionKia.INIFILES
{
    class INIFILE
    {
        private string path;

        public INIFILE(string iniPath)
        {
            path = iniPath;
        }

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern int GetPrivateProfileString(
            string section,
            string key,
            string defaultValue,
            StringBuilder returnValue,
            int size,
            string filePath);

        public string Read(string section, string key)
        {
            var buffer = new StringBuilder(255);
            int charsRead = GetPrivateProfileString(section, key,null, buffer, 255, path);

            Console.WriteLine($"Valor de charsread {charsRead}");

            if (charsRead == 0)
            {
                return null;
            }

            return buffer.ToString();
        }
    }
}
