using System;
using System.Collections;
using System.IO;
using System.Management.Automation;

namespace B5ScriptingTools
    {
    [Cmdlet(VerbsLifecycle.Start, "B5Logging")]
    public class StartB5Logging : PSCmdlet
        {
        #region Fields

        //public Hashtable Hash = new Hashtable();

        private int id;
        private string message;
        private string name;
        private string filepath;
        private StreamType streams = StreamType.All;
        public static ArrayList StreamList = new ArrayList();

        private ConsoleOutput consoleOutput;
        private string DateTimeFormat;
        // var appConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

        // private string consoleOutputModifier;

        #endregion Fields

        #region Parameters

        [Parameter(Mandatory = false,
        Position = 2,
        ParameterSetName = "New")]
        public string Name
            {
            get { return name; }
            set { name = value; }
            }

        [Parameter(Mandatory = false,
                    Position = 0,
            ParameterSetName = "New")]
        public string FilePath
            {
            get { return filepath; }
            set
                {
                filepath = GetUnresolvedProviderPathFromPSPath(value);
                }
            }

        [Parameter(ParameterSetName = "New")]
        public StreamType StreamType
            {
            get { return streams; }
            set { streams = value; }
            }

        [Parameter(ParameterSetName = "New")]
        public ConsoleOutput ConsoleOutput
            {
            get { return consoleOutput; }
            set { consoleOutput = value; }
            }

        public int Id
            { get { return id; } }

        #endregion Parameters

        #region Methods

        protected override void EndProcessing()
            {
            LogFile logFile;

            int id = 1;
            foreach (IHostIOSubscriber subscriber in HostIOInterceptor.Instance.Subscribers)
                {
                var logF = subscriber as LogFile;
                id++;
                }
            logFile = new LogFile(filepath, name, id, streams);
            StreamList.Add(logFile);

            string ScriptPath = GetUnresolvedProviderPathFromPSPath(MyInvocation.ScriptName);
            string ScriptName = Path.GetFileName(ScriptPath);
            string ScriptRoot = Path.GetDirectoryName(ScriptPath);

            message = string.Format(@"
****************************************
Windows PowerShell logging started
Start time: {0,-10}
Username: {1}
ComputerName: {2}
PSVersion: {3}
ScriptName: {4}
ScriptPath: {5}
****************************************

", DateTime.Now.ToString("G"), (System.Security.Principal.WindowsIdentity.GetCurrent().Name), Environment.MachineName, Host.Version, ScriptName, ScriptRoot);

            File.WriteAllText(filepath, message);

            HostIOInterceptor.Instance.ConsoleOutput = ConsoleOutput;
            HostIOInterceptor.Instance.AttachToHost(Host);
            HostIOInterceptor.Instance.AddSubscriber(logFile);

            WriteObject(logFile);
            }

        #endregion Methods
        } // End AddLogFileCommand class
    }