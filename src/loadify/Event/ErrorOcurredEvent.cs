﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loadify.Event
{
    public class ErrorOcurredEvent : DialogRequestEvent
    {
        public ErrorOcurredEvent(string title, string content)
            : base(title, content)
        { }
    }
}