using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loadify.Spotify
{
    public class ResourceException : Exception
    {
        public ResourceException(string msg = ""):
            base(msg)
        { }
    }
}
