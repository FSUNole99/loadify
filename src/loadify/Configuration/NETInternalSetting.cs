using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loadify.Properties;

namespace loadify.Configuration
{
    public class NETInternalSetting : IInternalSetting
    {
        public bool FirstUsage
        {
            get { return Settings.Default.FirstUsage; }
            set
            {
                Settings.Default.FirstUsage = value;
                Settings.Default.Save();
            }
        }
    }
}
