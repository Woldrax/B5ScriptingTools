using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace B5ScriptingTools
    {
    public class HostIOSubscriberBase : IHostIOSubscriber
        {
        #region Methods

        public virtual void ChoicePrompt(ChoiceDescription choice)
            {
            }

        public virtual void CredentialPrompt(PSCredential credential)
            {
            }

        public virtual void Prompt(Dictionary<string, PSObject> returnValue)
            {
            }

        public virtual void ReadFromHost(string inputText)
            {
            }

        public virtual void WriteDebug(string message)
            {
            }

        public virtual void WriteError(string message)
            {
            }

        public virtual void WriteOutput(string message)
            {
            }

        public virtual void WriteProgress(long sourceId, ProgressRecord record)
            {
            }

        public virtual void WriteVerbose(string message)
            {
            }

        public virtual void WriteWarning(string message)
            {
            }

        public virtual void WriteChange(string message)
            {
            }

        public virtual void WriteLog(string message)
            {
            }

        #endregion Methods
        }
    }