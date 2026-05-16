using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace RayForge.Graphics
{
    public sealed class TextureManager
    {
        private readonly Dictionary<int, Bitmap> _textures = new();
        public TextureManager()
        {
            GenerateDefaultTextures();
        }
        public void LoadTexture(int id, string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    if (_textures.ContainsKey(id)) _textures[id].Dispose();
                    _textures[id] = new Bitmap(path);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Файл не знайдено: {path}. Створюємо заглушку.");
                    _textures[id] = CreateSolidTexture(Color.Magenta);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка завантаження {path}: {ex.Message}");
                _textures[id] = CreateSolidTexture(Color.Magenta);
            }
        }
        public Bitmap GetTexture(int id)
        {
            if (_textures.TryGetValue(id, out Bitmap? texture))
                return texture;
            return _textures.ContainsKey(1) ? _textures[1] : CreateSolidTexture(Color.White);
        }
        private void GenerateDefaultTextures()
        {
            try
            {
                if (File.Exists("Assets/wall.png"))
                    _textures[1] = new Bitmap("Assets/wall.png");
                else
                    _textures[1] = CreateSolidTexture(Color.FromArgb(40, 140, 70));
                if (File.Exists("Assets/door.png"))
                    _textures[2] = new Bitmap("Assets/door.png");
                else
                    _textures[2] = CreateSolidTexture(Color.SteelBlue);
                if (File.Exists("Assets/enemy.png"))
                    _textures[10] = new Bitmap("Assets/enemy.png");
                else
                    _textures[10] = CreateSolidTexture(Color.Red);
                if (File.Exists("Assets/item.png"))
                    _textures[7] = new Bitmap("Assets/item.png");
                else
                    _textures[7] = CreateSolidTexture(Color.Gold);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Помилка: " + ex.Message);
                _textures[1] = CreateSolidTexture(Color.LimeGreen);
            }
        }
        private static Bitmap CreateSolidTexture(Color color)
        {
            Bitmap bmp = new Bitmap(64, 64);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.Clear(color); 
            }
            return bmp;
        }
        private static Bitmap CreateBrickTexture(Color primary, Color secondary)
        {
            const int size = 64; 
            Bitmap bitmap = new(size, size);
            using System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.Clear(primary); 

            using Pen mortar = new(secondary, 2f); 
            for (int y = 0; y < size; y += 16)
            {
                g.DrawLine(mortar, 0, y, size, y); 
                int offset = (y / 16 % 2 == 0) ? 0 : 8;
                for (int x = offset; x < size; x += 16)
                {
                    g.DrawLine(mortar, x, y, x, y + 16); 
                }
            }
            return bitmap;
        }
    }
}