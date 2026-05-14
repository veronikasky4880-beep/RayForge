using System;
using System.Drawing;
using System.Windows.Forms;
using RayForge.Core;

namespace RayForge.UI
{
    public sealed class GameForm : Form
    {
        private readonly GameEngine _engine;

        public GameForm()
        {
            Text = "RayForge - Retro Edition";

           
            ClientSize = new Size(320, 240);

           
            MinimumSize = new Size(320, 240);

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            DoubleBuffered = true;
            KeyPreview = true;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.Black;

            _engine = new GameEngine(this);

            Paint += OnPaintFrame;
            FormClosing += OnFormClosing;
        }

        private void OnPaintFrame(object? sender, PaintEventArgs e)
        {
            _engine.Render(e.Graphics);
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            _engine.Dispose();
        }
    }
}