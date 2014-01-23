using System;

namespace TplWorkshop.Util.Visualizer
{
    internal static class TaskVisualizeExtension
    {
        public static string FormatTaskError(this Exception exception)
        {
            if (exception == null)
            {
                return "(No error)";
            }

            string type = exception.GetType().Name;
            string message = string.IsNullOrEmpty(exception.Message) 
                ? "(No message)" 
                : exception.Message;

            return string.Format("{0}: {1}", type, message);
        }

        public static string FormatTaskResult(this object result)
        {
            if (result == null)
            {
                return "(No result)";
            }

            string type = result.GetType().Name;
            string message = result.ToString()
                .Replace("\r\n", " ")
                .Replace("\r", " ")
                .Replace("\n", " ");
            if (message.Length > 128)
            {
                message = message.Substring(0, 125) + "...";
            }

            return string.Format("{0}: {1}", type, message);
        }
    }
}