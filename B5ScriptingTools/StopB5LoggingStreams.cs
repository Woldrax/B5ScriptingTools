using System;
using System.IO;
using System.Management.Automation;

namespace B5ScriptingTools
    {
    [Cmdlet("Stop", "B5LoggingStreams")]
    public class StopB5LoggingStreams : PSCmdlet
        {
        #region Fields

        private const string DateTimeFormat = "dd/MM/yy - HH:mm:ss";
        private int id;
        private LogFile inputObject;
        private string message;
        private string name;

        #endregion Fields

        #region Properties

        /// <summary>
        ///
        /// </summary>
        [Parameter(Mandatory = false,
            ValueFromPipeline = true,
            Position = 1)]
        public int Id
            {
            get { return id; }
            set { id = value; }
            }

        [Parameter(Mandatory = false,
                    ValueFromPipeline = true,
                    Position = 2)]
        public LogFile InputObject
            {
            get { return inputObject; }
            set { inputObject = value; }
            }

        /// <summary>
        ///
        /// </summary>
        [Parameter(Mandatory = false,
            ValueFromPipeline = true,
            Position = 0)]
        public string Name
            {
            get { return name; }
            set { name = value; }
            }

        #endregion Properties

        #region Methods

        /// <summary>
        ///
        /// </summary>
        protected override void EndProcessing()
            {
            if (inputObject == null)
                {
                foreach (IHostIOSubscriber subscriber in HostIOInterceptor.Instance.Subscribers)
                    {
                    var logFile = subscriber as LogFile;
                    if (logFile != null)
                        {
                        if (!(string.IsNullOrEmpty(name)))
                            {
                            if (logFile.Name.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                InputObject = logFile;
                                }
                            }
                        else if (id != 0)
                            {
                            if (logFile.Id == id)
                                {
                                InputObject = logFile;
                                }
                            }
                        else
                            {
                            WriteObject(string.Format("Stream cannot be found!"));
                            }
                        }
                    }
                }

            WriteObject(inputObject);
            HostIOInterceptor.Instance.RemoveSubscriber(InputObject);

            message = string.Format(@"

****************************************
Windows PowerShell logging stopped
Stop time: {0,-10}
Script Execution time: {0}
****************************************
http://Woldrax.be/WoldraxScriptModule ", DateTime.Now.ToString("G"));
            message = message.Replace("@", Environment.NewLine);
            File.AppendAllText(Path.GetFullPath(InputObject.Path), message);
            }

        #endregion Methods
        }
    }