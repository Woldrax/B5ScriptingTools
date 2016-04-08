namespace B5ScriptingTools
    {
    public enum StreamType
        {
        None = 0,
        Output = 1,
        Verbose = 2,
        Warning = 4,
        Error = 8,
        Debug = 16,
        Change = 32,
        Log = 64,
        Default = Output | Verbose | Warning | Error | Debug,
        All = Output | Verbose | Warning | Error | Debug | Change | Log
        }
    }