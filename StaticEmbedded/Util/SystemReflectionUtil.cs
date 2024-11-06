using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ITSEmbedded.Util
{
    public class SystemReflectionUtil
    {
        public static string ITSHtmlTemplateNameSpace = "ITSEmbedded";
        public static Stream GetAssemblyResourceStream(string name)
        {
            Stream stream = null;
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                stream = assembly.GetManifestResourceStream(name);
            }
            catch { }
            return stream;
        }
    }
}
