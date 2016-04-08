using System.Management.Automation;

namespace B5ScriptingTools
    {
    [Cmdlet(VerbsCommon.Get, "B5LoggingStreams")]
    public class GetB5LoggingStreams : PSCmdlet
        {
        private string name;
        private string path;



        #region Properties

        [Parameter(Mandatory = false,
            Position = 2,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public int Id { get; private set; }

        [Parameter(Mandatory = false,
            Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name
            {
            get { return name; }
            set { name = value; }
            }

        [Parameter(Mandatory = false,
                            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Path
            {
            get { return path; }
            set
                {
                path = GetUnresolvedProviderPathFromPSPath(value);
                }
            }

        #endregion Properties

        #region Methods

        protected override void EndProcessing()
            {
            foreach (IHostIOSubscriber subscriber in HostIOInterceptor.Instance.Subscribers)
                {
                var logFile = subscriber as LogFile;
                if (logFile != null)
                    {
                    if (!(string.IsNullOrEmpty(path)))
                        {
                        if (System.IO.Path.GetFullPath(logFile.Path) == System.IO.Path.GetFullPath(path))
                            {
                            WriteObject(logFile);
                            }
                        }
                    else if (!(string.IsNullOrEmpty(name)))
                        {
                        if (logFile.Name.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                            WriteObject(logFile);
                            }
                        }
                    else if (Id != 0)
                        {
                        if (logFile.Id == Id)
                            {
                            WriteObject(logFile);
                            }
                        }
                    else
                        {
                        WriteObject(logFile);
                        }
                    }
                }
            }

        #endregion Methods
        }
    }