using System;

namespace SeriousGameLib
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary> 
        /// The main entry point for the application.
        /// </summary> 
        static void Main(string[] args)
        {
            //try
            //{
                using (SeriousGame game = new SeriousGame())
                {
                    game.Run();
                }
            //}

                // Probably triggered by ending threads:
            //catch (Exception e) { Console.WriteLine(e); }
        }
    }
#endif
}

