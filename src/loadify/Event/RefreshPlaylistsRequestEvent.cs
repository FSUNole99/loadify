using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loadify.Event
{
    public class RefreshPlaylistsRequestEvent
    {
        public string Text { get; set; }

        public RefreshPlaylistsRequestEvent(string text)
        {
            Text = text;
        }
    }
}
