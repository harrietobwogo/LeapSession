using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeapSession.Model
{
    public class ConversationData
    {
        //track whether we have asked user for name
        public bool PromptedUserForName { get; set; } = false;
    }
}
