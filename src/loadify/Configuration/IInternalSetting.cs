using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loadify.Configuration
{
    public interface IInternalSetting
    {
        bool FirstUsage { get; set; }
    }
}
