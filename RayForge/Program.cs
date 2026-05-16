using System;
using System.Windows.Forms;
namespace RayForge
{
    internal static class Program
    {
        /// <summary>
        /// Головна точка входу для додатка.
        /// </summary>
        [STAThread] 
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new UI.GameForm());
        }
    }
}