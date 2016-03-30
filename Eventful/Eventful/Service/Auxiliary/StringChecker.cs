using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventful.Service.Auxiliary
{
    public static class StringChecker
    {
        public static bool FilenameStringCheck(string text)
        {
            if (text == "")
                return false;
            // \\ / : * ? \" < > | not allowed by Windows
            if (text.Contains("\\"))
                return false;
            if (text.Contains("/"))
                return false;
            if (text.Contains(":"))
                return false;
            if (text.Contains("*"))
                return false;
            if (text.Contains("?"))
                return false;
            if (text.Contains("\""))
                return false;
            if (text.Contains("<"))
                return false;
            if (text.Contains(">"))
                return false;
            if (text.Contains("|"))
                return false;
            return true;
        }
    }
}
