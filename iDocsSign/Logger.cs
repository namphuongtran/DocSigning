using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iDocsSign
{
   public class Logger
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("iDocsSign");
        public static void WriteError(string message, Exception ex)
        {
            string errorMsg = string.Format("{0} \r\n {1}",
                message,
                ex != null ? (ex.Message + "\r\n" + ex.StackTrace) : "");
            System.Diagnostics.Debug.WriteLine(errorMsg);
            logger.Error(errorMsg);
        }

        public static void WriteLogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            logger.Info(message);
        }
    }
}
