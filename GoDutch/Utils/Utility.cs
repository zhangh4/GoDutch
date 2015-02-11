using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoDutch.Utils
{
    public static class Utility
    {
        private static int seed = 1;

        public static int GetNextId()
        {
            return seed++;
        }
    }
}