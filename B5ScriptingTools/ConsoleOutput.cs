namespace B5ScriptingTools
    {
    public class ConsoleOutput
        {
        #region Fields

        public string HostOutputStream;
        public string HostErrorStream;
        public string HostDebugStream;
        public string HostVerboseStream;
        public string HostWarningStream;
        public string HostChangeStream;
        public string HostLogStream;

        #endregion Fields

        #region Constructors and Destructors

        public ConsoleOutput(string hostOutputStream, string hostErrorStream, string hostDebugStream, string hostVerboseStream, string hostWarningStream, string hostChangeStream, string hostLogStream)
            {
            HostOutputStream = hostOutputStream;
            HostErrorStream = hostErrorStream;
            HostDebugStream = hostDebugStream;
            HostVerboseStream = hostVerboseStream;
            HostWarningStream = hostWarningStream;
            HostChangeStream = hostChangeStream;
            HostLogStream = hostLogStream;
            }

        #endregion Constructors and Destructors
        }
    }