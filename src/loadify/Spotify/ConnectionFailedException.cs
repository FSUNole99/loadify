using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loadify.Spotify
{
    public class ConnectionFailedException : SpotifyException
    {
        public ConnectionFailedException(string msg = ""):
            base(msg)
        { }
    }
}
