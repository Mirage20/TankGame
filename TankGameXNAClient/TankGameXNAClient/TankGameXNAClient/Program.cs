using System;

namespace TankGameXNAClient
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TankGame game = new TankGame())
            {
                game.Run();
            }
        }
    }
#endif
}

