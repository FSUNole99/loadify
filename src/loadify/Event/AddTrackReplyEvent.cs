﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loadify.Spotify;
using loadify.ViewModel;

namespace loadify.Event
{
    public class AddTrackReplyEvent : SessionEvent
    {
        public PlaylistViewModel Playlist { get; set; }
        public string Content { get; set; }

        public AddTrackReplyEvent(string content, PlaylistViewModel playlist, LoadifySession session) :
            base(session)
        {
            Content = content;
            Playlist = playlist;
        }
    }
}
