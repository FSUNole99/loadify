using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Filter;

namespace loadify.Spotify
{
    public class SpotifyException : Exception
    {
        public SpotifyException(string msg = ""):
            base(msg)
        { }
    }
}
