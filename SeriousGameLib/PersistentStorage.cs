using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeriousGameLib
{
    public static class PersistentStorage
    {
        public static int[] Trophies = new int[] { 0, 0, 0 }; // in order: 0:gold, 1:silver, 2:bronze

        public static short LastSchilderDrawingIndex = 0;

        public static void resetTrophies() 
        {
            Trophies = new int[] { 0, 0, 0 };
        }

        static PersistentStorage()
        {

        }
    }
}
