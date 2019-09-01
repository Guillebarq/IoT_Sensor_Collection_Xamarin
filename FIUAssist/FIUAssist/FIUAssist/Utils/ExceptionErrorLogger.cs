using FIUAssist.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace FIUAssist.Utils
{

    public class ExceptionErrorLogger
    {

        public static void ErrorLogging(Exception ex)
        {

            var er = "Error" + ex.ToString();

            var message = new ErrorMessage
            {
                Message = er
            };

            Device.BeginInvokeOnMainThread(

             () => MessagingCenter.Send(message, "ErrorMessage")

             );

            string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(strPath))
            {
                sw.WriteLine("=============Error Logging ===========");
                sw.WriteLine("===========Start============= " + DateTime.Now);
                sw.WriteLine("Error Message: " + ex.Message);
                sw.WriteLine("Stack Trace: " + ex.StackTrace);
                sw.WriteLine("===========End============= " + DateTime.Now);

            }
        }


        public static void writeFileOnInternalStorage(string error)
        {

            try
            {
                Java.IO.File path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                Java.IO.File newFile = new Java.IO.File(path, "exceptions.txt");

                if (!newFile.Exists())
                {
                    newFile.CreateNewFile();
                }

                string now = DateTime.Now.ToString();

                Java.IO.FileWriter writer = new Java.IO.FileWriter(newFile, true);
                writer.Append("\n" + now + " :EXCEPTION\n" + error + "\n");
                writer.Flush();
                writer.Close();

                //using (var writer = new StreamWriter(newFile))
                //{
                //    writer.WriteLine(error + "THIS IS THE ERROR");
                //}

                //using (var reader = new StreamReader(newFile))
                //{
                //    System.Diagnostics.Debug.WriteLine(reader.ReadLine());
                //}
            }
            catch (Exception ex)
            {
                ExceptionErrorLogger.ErrorLogging(ex);
            }

        }

        public static void CopyDatabase()
        {
            try
            {
                string fileCopyName = string.Format("/sdcard/Database_{0:dd-MM-yyyy_HH-mm-ss-tt}.db", System.DateTime.Now);
                System.IO.File.Copy(StaticObjects.path, fileCopyName);
            }
            catch(Exception ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }
            

        }

        //public static void UnhandledExceptionLogging(RaiseThrowableEventArgs ex)
        //{
        //    string strPath = @"C:\Users\dodtech\Desktop";
        //    if (!File.Exists(strPath))
        //    {
        //        File.Create(strPath).Dispose();
        //    }
        //    using (StreamWriter sw = File.AppendText(strPath))
        //    {
        //        sw.WriteLine("=============Error Logging ===========");
        //        sw.WriteLine("===========Start============= " + DateTime.Now);
        //        sw.WriteLine("Error Message: " + ex.ToString());
        //        sw.WriteLine("===========End============= " + DateTime.Now);

        //    }
        //}

    }
}
