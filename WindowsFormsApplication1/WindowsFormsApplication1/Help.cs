using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class Help
    {
        public static bool WinIs64()
        {
            return Environment.Is64BitOperatingSystem;
        }
    }
}
