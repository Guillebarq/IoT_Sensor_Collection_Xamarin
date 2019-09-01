using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FIUAssist.Utils
{
    public static class StaticObjects
    {
        public static bool IsCollecting;

        public static string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FIUAssistDB.db ");

        public static bool jobCancelled;

        public static bool Connected;

    }
}
