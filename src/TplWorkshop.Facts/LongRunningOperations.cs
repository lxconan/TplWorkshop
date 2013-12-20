using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace TplWorkshop.Facts
{
    public static class LongRunningOperations
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            return sf.GetMethod().Name;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void WriteDebugInfo()
        {
            int taskId = Task.CurrentId.GetValueOrDefault();
            int threadId = Thread.CurrentThread.ManagedThreadId;
            string methodName = GetCurrentMethod();
            Trace.WriteLine(string.Format("\"{0}\" is running on task {1} on thread {2}", methodName, taskId, threadId));
        }

        private static void WriteCompleteInfo()
        {
            string methodName = GetCurrentMethod();
            Trace.WriteLine(string.Format("\"{0}\" running complete.", methodName));
        }

        private static string WaitAndReturnHello(int sec)
        {
            WaitFor(sec);
            return "hello";
        }

        private static void WaitFor(int sec)
        {
            Thread.Sleep(sec * 1000);
        }

        public static string WaitFor1SecAndReturnHello()
        {
            WriteDebugInfo();
            string result = WaitAndReturnHello(1);
            WriteCompleteInfo();
            return result;
        }

        public static string WaitFor2SecAndReturnHello()
        {
            WriteDebugInfo();
            string result = WaitAndReturnHello(2);
            WriteCompleteInfo();
            return result;
        }

        public static string WaitFor3SecAndReturnHello()
        {
            WriteDebugInfo();
            string result = WaitAndReturnHello(3);
            WriteCompleteInfo();
            return result;
        }

        public static object WaitFor1SecAndEcho(object state)
        {
            WriteDebugInfo();
            Thread.Sleep(1000);
            WriteCompleteInfo();
            return state;
        }

        public static void WaitFor1Sec()
        {
            WriteDebugInfo();
            WaitFor(1);
            WriteCompleteInfo();
        }

        public static void WaitFor3Sec()
        {
            WriteDebugInfo();
            WaitFor(3);
            WriteCompleteInfo();
        }
    }
}