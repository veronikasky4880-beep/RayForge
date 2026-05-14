using System;
using System.Windows.Forms;

namespace RayForge
{
    // Статичний клас Program містить основну логіку запуску додатка
    internal static class Program
    {
        /// <summary>
        /// Головна точка входу для додатка.
        /// </summary>
        [STAThread] // Вказує, що моделлю потоків для додатка є однопотокова квартира (необхідно для Windows Forms)
        static void Main()
        {
            // Ініціалізація конфігурації додатка (налаштування шрифтів, стилів тощо)
            ApplicationConfiguration.Initialize();

            // Запуск циклу обробки повідомлень Windows та відкриття головної форми гри.
            // Тут створюється екземпляр GameForm, який знаходиться в просторі імен UI.
            Application.Run(new UI.GameForm());
        }
    }
}