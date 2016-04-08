using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Reflection;
using System.Security;
using System.Text;

namespace B5ScriptingTools
    {
    public class HostIOInterceptor : PSHostUserInterface
        {
        #region Fields

        private PSHostUserInterface externalUI;
        private PSHost host;
        public static readonly HostIOInterceptor Instance = new HostIOInterceptor();
        private bool paused;
        private readonly List<WeakReference> subscribers;
        private readonly StringBuilder writeCache;
        //   private readonly ConsoleOutput consoleOutput;

        #endregion Fields

        #region Constructors and Destructors

        private HostIOInterceptor()
            {
            externalUI = null;
            subscribers = new List<WeakReference>();
            writeCache = new StringBuilder();
            paused = false;
            host = null;
            }

        #endregion Constructors and Destructors

        #region Properties

        public bool Paused
            {
            get { return paused; }
            set { paused = value; }
            }

        public override PSHostRawUserInterface RawUI
            {
            get
                {
                return externalUI == null ? null : externalUI.RawUI;
                }
            }

        public IEnumerable<IHostIOSubscriber> Subscribers
            {
            get
                {
                foreach (WeakReference reference in subscribers)
                    {
                    var subscriber = (IHostIOSubscriber)reference.Target;
                    if (subscriber != null)
                        {
                        yield return subscriber;
                        }
                    }
                }
            }

        public ConsoleOutput ConsoleOutput;

        #endregion Properties

        #region Public Methods and Operators

        public void AddSubscriber(IHostIOSubscriber subscriber)
            {
            foreach (WeakReference reference in subscribers)
                {
                if (reference.Target == subscriber)
                    {
                    return;
                    }
                }

            subscribers.Add(new WeakReference(subscriber));
            }

        public void AttachToHost(PSHost host)
            {
            if (this.host != null) { return; }
            if (host == null) { return; }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

            object uiRef = host.GetType().GetField("internalUIRef", flags).GetValue(host);
            object ui = uiRef.GetType().GetProperty("Value", flags).GetValue(uiRef, null);

            FieldInfo externalUIField = ui.GetType().GetField("externalUI", flags);

            externalUI = (PSHostUserInterface)externalUIField.GetValue(ui);
            externalUIField.SetValue(ui, this);
            this.host = host;
            }

        public void DetachFromHost()
            {
            if (host == null) { return; }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

            object uiRef = host.GetType().GetField("internalUIRef", flags).GetValue(host);
            object ui = uiRef.GetType().GetProperty("Value", flags).GetValue(uiRef, null);

            FieldInfo externalUIField = ui.GetType().GetField("externalUI", flags);

            if (externalUIField.GetValue(ui) == this)
                {
                externalUIField.SetValue(ui, externalUI);
                }

            externalUI = null;
            host = null;
            }

        public override Dictionary<string, PSObject> Prompt(string caption,
                                                            string message,
                                                            Collection<FieldDescription> descriptions)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            Dictionary<string, PSObject> result = externalUI.Prompt(caption, message, descriptions);

            SendToSubscribers(s => s.Prompt(result));

            return result;
            }

        public override int PromptForChoice(string caption,
                                            string message,
                                            Collection<ChoiceDescription> choices,
                                            int defaultChoice)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            int result = externalUI.PromptForChoice(caption, message, choices, defaultChoice);

            SendToSubscribers(s => s.ChoicePrompt(choices[result]));

            return result;
            }

        public override PSCredential PromptForCredential(string caption,
                                                         string message,
                                                         string userName,
                                                         string targetName)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            PSCredential result = externalUI.PromptForCredential(caption, message, userName, targetName);

            SendToSubscribers(s => s.CredentialPrompt(result));

            return result;
            }

        public override PSCredential PromptForCredential(string caption,
                                                         string message,
                                                         string userName,
                                                         string targetName,
                                                         PSCredentialTypes allowedCredentialTypes,
                                                         PSCredentialUIOptions options)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            PSCredential result = externalUI.PromptForCredential(caption,
                                                                       message,
                                                                       userName,
                                                                       targetName,
                                                                       allowedCredentialTypes,
                                                                       options);

            SendToSubscribers(s => s.CredentialPrompt(result));

            return result;
            }

        public override string ReadLine()
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            string result = externalUI.ReadLine();

            SendToSubscribers(s => s.ReadFromHost(result));

            return result;
            }

        public override SecureString ReadLineAsSecureString()
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            return externalUI.ReadLineAsSecureString();
            }

        public void RemoveAllSubscribers()
            {
            subscribers.Clear();
            }

        public void RemoveSubscriber(IHostIOSubscriber subscriber)
            {
            var matches = new List<WeakReference>();

            foreach (WeakReference reference in subscribers)
                {
                if (reference.Target == subscriber)
                    {
                    matches.Add(reference);
                    }
                }

            foreach (WeakReference reference in matches)
                {
                subscribers.Remove(reference);
                }
            }

        public override void Write(string value)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }
            if (value.Trim().Length < 1)
                {
                externalUI.WriteLine();
                return;
                }
            try
                {
                if (!(string.IsNullOrEmpty(ConsoleOutput.HostOutputStream)))
                    {
                    externalUI.Write(ConsoleOutput.HostOutputStream);
                    //value = (ConsoleOutput.HostOutpu();tStream + value).TrimEnd();
                    }
                }
            catch { }
            externalUI.Write(value.TrimEnd());

            if (!paused)
                {
                writeCache.Append(value);
                }
            }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }
            if (value.Trim().Length < 1)
                {
                externalUI.WriteLine();
                return;
                }
            try
                {
                if (!(string.IsNullOrEmpty(ConsoleOutput.HostOutputStream)))
                    {
                    externalUI.Write(ConsoleOutput.HostOutputStream);
                    //value = (ConsoleOutput.HostOutputStream + value).TrimEnd();
                    }
                }
            catch { }
            externalUI.Write(foregroundColor, backgroundColor, value.TrimEnd());

            if (!paused)
                {
                writeCache.Append(value);
                }
            }

        public override void WriteDebugLine(string message)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            string[] lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
                {
                string temp = line;
                SendToSubscribers(s => s.WriteDebug(temp.TrimEnd() + "\r\n"));
                }
            if (message.Trim().Length < 1)
                {
                externalUI.WriteLine();
                return;
                }
            try
                {
                if (!(string.IsNullOrEmpty(ConsoleOutput.HostDebugStream)))
                    {
                    externalUI.Write(ConsoleOutput.HostDebugStream);
                    //message = (ConsoleOutput.HostDebugStream + message).TrimEnd();
                    }
                }
            catch { }
            externalUI.WriteDebugLine(message.TrimEnd());
            }

        public override void WriteErrorLine(string message)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            string[] lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
                {
                string temp = line;
                SendToSubscribers(s => s.WriteError(temp.TrimEnd() + "\r\n"));
                }
            if (message.Trim().Length < 1)
                {
                externalUI.WriteLine();
                return;
                }

            try
                {
                if (!(string.IsNullOrEmpty(ConsoleOutput.HostErrorStream)))
                    {
                    externalUI.Write(ConsoleOutput.HostErrorStream);
                    // message = (ConsoleOutput.HostErrorStream + message).TrimEnd();
                    }
                }
            catch { }
            externalUI.WriteErrorLine(message.TrimEnd());
            }

        public override void WriteLine()
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            string[] lines = writeCache.ToString().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
                {
                string temp = line;
                SendToSubscribers(s => s.WriteOutput(temp.TrimEnd() + "\r\n"));
                }

            writeCache.Length = 0;
            externalUI.WriteLine();
            }

        public override void WriteLine(string value)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            if (value.StartsWith("(%[CHANGE]%)"))
                {
                value = value.Replace("(%[CHANGE]%)", "");

                string[] liness = (writeCache + value).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (string line in liness)
                    {
                    string temp = line;
                    SendToSubscribers(s => s.WriteChange(temp.TrimEnd() + "\r\n"));
                    }
                if (value.Trim().Length < 1)
                    {
                    externalUI.WriteLine();
                    return;
                    }
                try
                    {
                    if (!(string.IsNullOrEmpty(ConsoleOutput.HostOutputStream)))
                        {
                        externalUI.Write(ConsoleOutput.HostOutputStream);
                        //value = (ConsoleOutput.HostOutputStream + value).TrimEnd();
                        }
                    }
                catch { }
                writeCache.Length = 0;
                externalUI.WriteLine(ConsoleColor.Green, RawUI.BackgroundColor, "CHANGE: " + value.TrimEnd());
                }
            else if (value.StartsWith("(%[LOG]%)"))
                {
                value = value.Replace("(%[LOG]%)", "");

                string[] liness = (writeCache + value).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (string line in liness)
                    {
                    string temp = line;
                    SendToSubscribers(s => s.WriteLog(temp.TrimEnd() + "\r\n"));
                    }
                if (value.Trim().Length < 1)
                    {
                    externalUI.WriteLine();
                    return;
                    }
                try
                    {
                    if (!(string.IsNullOrEmpty(ConsoleOutput.HostOutputStream)))
                        {
                        externalUI.Write(ConsoleOutput.HostOutputStream);
                        //value = (ConsoleOutput.HostOutputStream + value).TrimEnd();
                        }
                    }
                catch { }
                writeCache.Length = 0;
                externalUI.WriteLine(ConsoleColor.Magenta, RawUI.BackgroundColor, "LOG: " + value.TrimEnd());
                }
            else
                {
                string[] lines = (writeCache + value).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (string line in lines)
                    {
                    string temp = line;
                    SendToSubscribers(s => s.WriteOutput(temp.TrimEnd() + "\r\n"));
                    }
                if (value.Trim().Length < 1)
                    {
                    externalUI.WriteLine();
                    return;
                    }
                try
                    {
                    if (!(string.IsNullOrEmpty(ConsoleOutput.HostOutputStream)))
                        {
                        externalUI.Write(ConsoleOutput.HostOutputStream);
                        //value = (ConsoleOutput.HostOutputStream + value).TrimEnd();
                        }
                    }
                catch { }
                writeCache.Length = 0;
                externalUI.WriteLine(value.TrimEnd());
                }
            }

        public void WriteLine(string message, string tags)
            {
            }

        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            string[] lines = (writeCache + value).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
                {
                string temp = line;
                SendToSubscribers(s => s.WriteOutput(temp.TrimEnd() + "\r\n"));
                }
            try
                {
                if (!(string.IsNullOrEmpty(ConsoleOutput.HostOutputStream)))
                    {
                    externalUI.Write(ConsoleOutput.HostOutputStream);
                    //value = (ConsoleOutput.HostOutputStream + value).TrimEnd();
                    }
                }
            catch { }
            writeCache.Length = 0;
            externalUI.WriteLine(foregroundColor, backgroundColor, value.TrimEnd());
            }

        public override void WriteProgress(long sourceId, ProgressRecord record)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            SendToSubscribers(s => s.WriteProgress(sourceId, record));

            externalUI.WriteProgress(sourceId, record);
            }

        public override void WriteVerboseLine(string message)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            string[] lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
                {
                string temp = line;
                SendToSubscribers(s => s.WriteVerbose(temp.TrimEnd() + "\r\n"));
                }
            if (message.Trim().Length < 1)
                {
                externalUI.WriteLine();
                return;
                }
            try
                {
                if (!(string.IsNullOrEmpty(ConsoleOutput.HostVerboseStream)))
                    {
                    externalUI.Write(ConsoleOutput.HostVerboseStream);
                    //message = (ConsoleOutput.HostVerboseStream + message).TrimEnd();
                    }
                }
            catch { }
            externalUI.WriteVerboseLine(message.TrimEnd());
            }

        public override void WriteWarningLine(string message)
            {
            if (externalUI == null)
                {
                throw new InvalidOperationException();
                }

            string[] lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
                {
                string temp = line;
                SendToSubscribers(s => s.WriteWarning(temp.TrimEnd() + "\r\n"));
                }
            if (message.Trim().Length < 1)
                {
                externalUI.WriteLine();
                return;
                }
            try
                {
                if (!(string.IsNullOrEmpty(ConsoleOutput.HostWarningStream)))
                    {
                    externalUI.Write(ConsoleOutput.HostWarningStream);
                    // message = (ConsoleOutput.HostWarningStream + message).TrimEnd();
                    }
                }
            catch { }

            externalUI.WriteWarningLine(message.TrimEnd());
            }

        #endregion Public Methods and Operators

        #region Private Methods

        public void SendToSubscribers(Action<IHostIOSubscriber> action)
            {
            if (paused) { return; }

            var deadReferences = new List<WeakReference>();

            foreach (WeakReference reference in subscribers)
                {
                var subscriber = (IHostIOSubscriber)reference.Target;
                if (subscriber == null)
                    {
                    deadReferences.Add(reference);
                    }
                else
                    {
                    action(subscriber);
                    }
                }

            foreach (WeakReference reference in deadReferences)
                {
                subscribers.Remove(reference);
                }
            }

        #endregion Private Methods
        }
    }