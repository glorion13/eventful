namespace Eventful.Service.Auxiliary
{
    public static class StringChecker
    {
        public static bool IsFilenameValid(string text)
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
