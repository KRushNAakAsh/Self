using System;
using System.IO;

namespace Self
{
    public class Logger
    {
        public const int SELF_SUCCESS = 0;
        public const int EXCEPTION = -1;
        public const int FILEIO_ERROR = -2;
        public const int MEM_ERROR = -3;
        public const int UNSUPPORTED_CMD = 1;
        public const int INISECTION_NOT_FOUND = 2;
        public const int INIFIELD_NOT_FOUND = 3;
        public const int PROCESS_ERROR = 10;

        public StreamWriter LOG = null;

        public Logger(String LogFilePath)
        {
            if ((LOG = new StreamWriter(LogFilePath)) == null)
            { 
                Console.WriteLine("Could not open log file {0}.", LogFilePath);
                
                Environment.Exit(FILEIO_ERROR);
            }
        }

        public void CloseLog()
        {
            if (LOG != null)
            { LOG.Close(); }
        }
        public void WriteLine(String LogLine, Boolean ConsoleToo)
        {
            LOG.WriteLine(LogLine);
            if (ConsoleToo)
            { Console.WriteLine(LogLine); }
        }
        public int errorout(String Method, String ErrMsg, int errorcode, Boolean Exit)
        {
            LOG.WriteLine("SELF: {0} returned {1} ,{2}", Method, errorcode, ErrMsg, true);
            if (Exit)
            {
                LOG.Close();
                Environment.Exit(errorcode);
            }
            return (errorcode);
        }
    }

}
