using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace RayForge.Graphics
{
    // Клас TextureManager відповідає за зберігання, завантаження та створення текстур для гри
    public sealed class TextureManager
    {
        // Словник для зберігання текстур у пам'яті, де ключ — це ID текстури, а значення — об'єкт Bitmap
        private readonly Dictionary<int, Bitmap> _textures = new();

        // Конструктор класу, який автоматично запускає генерацію стандартних текстур при створенні об'єкта
        public TextureManager()
        {
            GenerateDefaultTextures();
        }

        // Метод для динамічного завантаження текстури з файлу за вказаним шляхом
        public void LoadTexture(int id, string path)
        {
            try
            {
                // Перевірка, чи існує файл за вказаним шляхом
                if (File.Exists(path))
                {
                    // Якщо текстура з таким ID вже існує, видаляємо її з пам'яті (Dispose) для запобігання витоку пам'яті
                    if (_textures.ContainsKey(id)) _textures[id].Dispose();

                    // Створюємо новий об'єкт Bitmap з файлу та зберігаємо у словник
                    _textures[id] = new Bitmap(path);
                }
                else
                {
                    // Якщо файл не знайдено, виводимо повідомлення у консоль відладки
                    System.Diagnostics.Debug.WriteLine($"Файл не знайдено: {path}. Створюємо заглушку.");
                    // Створюємо однотонну пурпурову текстуру як індикатор помилки
                    _textures[id] = CreateSolidTexture(Color.Magenta);
                }
            }
            catch (Exception ex)
            {
                // Обробка помилок при читанні файлу (наприклад, пошкоджений файл або відсутність доступу)
                System.Diagnostics.Debug.WriteLine($"Помилка завантаження {path}: {ex.Message}");
                _textures[id] = CreateSolidTexture(Color.Magenta);
            }
        }

        // Метод для отримання текстури за її ідентифікатором
        public Bitmap GetTexture(int id)
        {
            // Спроба знайти текстуру у словнику
            if (_textures.TryGetValue(id, out Bitmap? texture))
                return texture;

            // Якщо текстуру не знайдено, повертаємо текстуру стіни (ID 1) або білу заглушку, якщо навіть стіни немає
            return _textures.ContainsKey(1) ? _textures[1] : CreateSolidTexture(Color.White);
        }

        // Метод для ініціалізації базового набору текстур при старті
        private void GenerateDefaultTextures()
        {
            try
            {
                // Стіна (ID 1): намагаємось завантажити файл, інакше створюємо темно-зелену текстуру
                if (File.Exists("Assets/wall.png"))
                    _textures[1] = new Bitmap("Assets/wall.png");
                else
                    _textures[1] = CreateSolidTexture(Color.FromArgb(40, 140, 70));

                // Двері (ID 2): намагаємось завантажити файл, інакше створюємо синю текстуру
                if (File.Exists("Assets/door.png"))
                    _textures[2] = new Bitmap("Assets/door.png");
                else
                    _textures[2] = CreateSolidTexture(Color.SteelBlue);

                // Ворог (ID 10): намагаємось завантажити файл, інакше створюємо червону текстуру
                if (File.Exists("Assets/enemy.png"))
                    _textures[10] = new Bitmap("Assets/enemy.png");
                else
                    _textures[10] = CreateSolidTexture(Color.Red);

                // Монетка/Золото (ID 7): намагаємось завантажити файл, інакше створюємо золотисту текстуру
                if (File.Exists("Assets/item.png"))
                    _textures[7] = new Bitmap("Assets/item.png");
                else
                    _textures[7] = CreateSolidTexture(Color.Gold);
            }
            catch (Exception ex)
            {
                // У разі загальної помилки виводимо її опис та створюємо базову яскраву заглушку
                System.Diagnostics.Debug.WriteLine("Помилка: " + ex.Message);
                _textures[1] = CreateSolidTexture(Color.LimeGreen);
            }
        }

        // Допоміжний статичний метод для створення порожньої текстури заданого кольору (розмір 64x64)
        private static Bitmap CreateSolidTexture(Color color)
        {
            Bitmap bmp = new Bitmap(64, 64);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.Clear(color); // Заливка всього полотна вказаним кольором
            }
            return bmp;
        }

        // Допоміжний статичний метод для програмної генерації текстури цегляної кладки
        private static Bitmap CreateBrickTexture(Color primary, Color secondary)
        {
            const int size = 64; // Розмір сторони текстури
            Bitmap bitmap = new(size, size);
            using System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.Clear(primary); // Основний колір цегли

            using Pen mortar = new(secondary, 2f); // Колір "цементних" швів між цеглинами

            // Цикл малювання горизонтальних та вертикальних ліній для створення візерунку цегли
            for (int y = 0; y < size; y += 16)
            {
                g.DrawLine(mortar, 0, y, size, y); // Горизонтальний шов

                // Зсув вертикальних швів для ефекту перев'язки цегли через один ряд
                int offset = (y / 16 % 2 == 0) ? 0 : 8;
                for (int x = offset; x < size; x += 16)
                {
                    g.DrawLine(mortar, x, y, x, y + 16); // Вертикальний шов
                }
            }
            return bitmap;
        }
    }
}