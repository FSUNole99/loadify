using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loadify.Spotify
{
    public class InvalidCredentialsException : SpotifyException
    {
        public string Username { get; set; }

        public InvalidCredentialsException(string msg = "", string username = "") :
            base(msg)
        {
            Username = username;
        }
    }
}
