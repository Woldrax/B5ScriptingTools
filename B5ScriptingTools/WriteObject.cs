using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace B5ScriptingTools
    {
    public class WriteObjectToHost
        {
        #region Fields
        public string message;
        public string tags;
        #endregion

        #region Constructors and Destructors

           public string Message
            {
            get { return message; }
            set { Message = value; }
            }

             public string Tags
            {
            get { return tags; }
            set { Tags = value; }
            }


        #endregion

           public void send(string Message,string Tags)
            {
                          HostIOIntercep

            }



        }
    }
