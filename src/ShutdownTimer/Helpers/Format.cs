using System;

namespace ShutdownTimer.Helpers
{
    class Format
    {
        public static String BytesToString(long byteCount)
        {
            // Credits: https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net

            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            string result = (Math.Sign(byteCount) * num).ToString() + suf[place] + " (" + byteCount.ToString() + "byte)";
            return result;
        }
    }
}
