using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConstellationIni
{
    /// <summary>
    /// Mostly from: https://stackoverflow.com/a/14906422
    /// </summary>
    internal class IniFile
    {
        string path;
        string currentExe = Assembly.GetExecutingAssembly().GetName().Name;

        public IniFile(string iniPath = null)
        {
            path = new FileInfo(iniPath ?? currentExe + ".ini").FullName;
        }

        public string Read(string key, string section = null)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? currentExe, key, "", retVal, 255, path);
            return retVal.ToString();
        }

        //public int ReadAsInt(string key, string section = null)
        //{
        //    return Convert.ToInt32(Read(key, section));
        //}

        //public double ReadAsDouble(string key, string section = null)
        //{
        //    return Convert.ToDouble(Read(key, section));
        //}

        public void Write(string key, string value, string section = null)
        {
            WritePrivateProfileString(section ?? currentExe, key, value, path);
        }

        public void DeleteKey(string key, string section = null)
        {
            Write(key, null, section ?? currentExe);
        }

        public void DeleteSection(string section = null)
        {
            Write(null, null, section ?? currentExe);
        }

        public bool KeyExists(string key, string section = null)
        {
            return Read(key, section).Length > 0;
        }



        //https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-getprivateprofilestring
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string section, string key, string @default, StringBuilder retVal, int size, string filePath);


        //https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-writeprivateprofilestringa
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

    }
}
