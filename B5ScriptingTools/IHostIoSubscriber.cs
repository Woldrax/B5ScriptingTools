﻿using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace B5ScriptingTools
    {
    /// <summary>
    ///     The Logger interface.
    /// </summary>
    public interface IHostIOSubscriber
        {
        #region Public Methods and Operators

        #region From MWalker Solution (unused by this module)

        // These methods intercept input, which is not really useful for the type of logging I intend this module to perform.
        // The script that called these methods will already have access to the results, and the script author can choose
        // to display it or not (at which point it will be caught by the logging module).
        //
        // WriteProgress is also included in this unused category, because this doesn't seem to make much sense in a log file.

        void ChoicePrompt(ChoiceDescription choice);

        void CredentialPrompt(PSCredential credential);

        void Prompt(Dictionary<string, PSObject> returnValue);

        void ReadFromHost(string inputText);

        void WriteProgress(long sourceId, ProgressRecord record);

        #endregion From MWalker Solution (unused by this module)

        void WriteDebug(string message);

        void WriteError(string message);

        void WriteOutput(string message);

        void WriteVerbose(string message);

        void WriteWarning(string message);

        void WriteChange(string message);

        void WriteLog(string message);

        #endregion Public Methods and Operators
        }
    }