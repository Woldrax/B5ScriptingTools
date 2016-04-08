﻿using System;
using System.IO;

namespace B5ScriptingTools
    {
    public class LogFile : HostIOSubscriberBase
        {
        #region Fields

        private const string DateTimeFormat = "T";
        private readonly string fileName;
        private readonly string path;
        private int id;
        private string logName;
        string name;

        #endregion Fields

        #region Constructors and Destructors

        public LogFile(string filename, string name, int id, StreamType streams = StreamType.All)
            {
            Name = name;
            Id = id;
            fileName = System.IO.Path.GetFileName(filename);
            path = System.IO.Path.GetDirectoryName(filename);

            Streams = streams;
            }

        public LogFile(string filename, string name, int id, StreamType streams = StreamType.All, string logName = null) : this(filename, name, id,/* errorCallback, */streams)
            {
            this.logName = logName;
            }

        #endregion Constructors and Destructors

        #region Properties

        public int Id
            {
            get { return id; }
            set { id = value; }
            }

        public string Name { get; set; }

        /*     public ScriptBlock ErrorCallback { get; set; }*/

        public string Path
            {
            get { return System.IO.Path.Combine(path, fileName); }
            }

        public StreamType Streams { get; set; }

        #endregion Properties

        #region Public Methods and Operators

        //
        // IPSOutputSubscriber
        //

        public override void WriteDebug(string message)
            {
            if ((Streams & StreamType.Debug) != StreamType.Debug)
                {
                return;
                }
            if (message == null)
                {
                message = String.Empty;
                }

            try
                {
                CheckDirectory();
                if (message != String.Empty)
                    {
                    message = String.Format("[D]{0,-8} {1}", DateTime.Now.ToString(DateTimeFormat), message);
                    }

                File.AppendAllText(System.IO.Path.Combine(path, fileName), message);
                }
            catch (Exception e)
                {
                ReportError(e);
                }
            }

        public override void WriteError(string message)
            {
            if ((Streams & StreamType.Error) != StreamType.Error)
                {
                return;
                }
            if (message == null)
                {
                message = String.Empty;
                }

            try
                {
                CheckDirectory();
                if (message.Trim() != String.Empty)
                    {
                    message = String.Format("[E]{0,-8} {1}", DateTime.Now.ToString(DateTimeFormat), message);
                    }

                File.AppendAllText(System.IO.Path.Combine(path, fileName), message);
                }
            catch (Exception e)
                {
                ReportError(e);
                }
            }

        public override void WriteOutput(string message)
            {
            if ((Streams & StreamType.Output) != StreamType.Output)
                {
                return;
                }
            if (message == null)
                {
                message = String.Empty;
                }

            try
                {
                CheckDirectory();
                if (message.Trim() != String.Empty)
                    {
                    message = String.Format("[O]{0,-8} {1}", DateTime.Now.ToString(DateTimeFormat), message);
                    }

                File.AppendAllText(System.IO.Path.Combine(path, fileName), message);
                }
            catch (Exception e)
                {
                ReportError(e);
                }
            }

        public override void WriteVerbose(string message)
            {
            if ((Streams & StreamType.Verbose) != StreamType.Verbose)
                {
                return;
                }
            if (message == null)
                {
                message = String.Empty;
                }

            try
                {
                CheckDirectory();
                if (message.Trim() != String.Empty)
                    {
                    message = String.Format("[V]{0,-8} {1}", DateTime.Now.ToString(DateTimeFormat), message);
                    }

                File.AppendAllText(System.IO.Path.Combine(path, fileName), message);
                }
            catch (Exception e)
                {
                ReportError(e);
                }
            }

        public override void WriteWarning(string message)
            {
            if ((Streams & StreamType.Warning) != StreamType.Warning)
                {
                return;
                }
            if (message == null)
                {
                message = String.Empty;
                }

            try
                {
                CheckDirectory();
                if (message.Trim() != String.Empty)
                    {
                    message = String.Format("[W]{0,-8} {1}", DateTime.Now.ToString(DateTimeFormat), message);
                    }

                File.AppendAllText(System.IO.Path.Combine(path, fileName), message);
                }
            catch (Exception e)
                {
                ReportError(e);
                }
            }

        public override void WriteChange(string message)
            {
            if ((Streams & StreamType.Change) != StreamType.Change)
                {
                return;
                }
            if (message == null)
                {
                message = String.Empty;
                }

            try
                {
                CheckDirectory();
                if (message.Trim() != String.Empty)
                    {
                    message = String.Format("[C]{0,-8} {1}", DateTime.Now.ToString(DateTimeFormat), message);
                    }

                File.AppendAllText(System.IO.Path.Combine(path, fileName), message);
                }
            catch (Exception e)
                {
                ReportError(e);
                }
            }

        public override void WriteLog(string message)
            {
            if ((Streams & StreamType.Log) != StreamType.Log)
                {
                return;
                }
            if (message == null)
                {
                message = String.Empty;
                }

            try
                {
                CheckDirectory();
                if (message.Trim() != String.Empty)
                    {
                    message = String.Format("[L]{0,-8} {1}", DateTime.Now.ToString(DateTimeFormat), message);
                    }

                File.AppendAllText(System.IO.Path.Combine(path, fileName), message);
                }
            catch (Exception e)
                {
                ReportError(e);
                }
            }

        #endregion Public Methods and Operators

        #region Private Methods

        private void CheckDirectory()
            {
            if (!String.IsNullOrEmpty(path) && !Directory.Exists(path))
                {
                Directory.CreateDirectory(path);
                }
            }

        private void ReportError(Exception e)
            {
            /*  if (ErrorCallback == null)
              {
                  return;
              }

              // ReSharper disable once EmptyGeneralCatchClause
              try
              {
                  HostIOInterceptor.Instance.Paused = true;
                  ErrorCallback.Invoke(this, e);
              }
              catch { }
              finally
              {
                  HostIOInterceptor.Instance.Paused = false;
              }*/
            }

        #endregion Private Methods
        }
    }