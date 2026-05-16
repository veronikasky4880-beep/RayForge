using System; 
using System.Collections.Generic;
using System.Windows.Forms; 
namespace RayForge.Core 
{
    public sealed class InputController : IDisposable 
    {
        private readonly HashSet<Keys> _pressedKeys = new(); 
        private readonly Form _window;
        public InputController(Form window) 
        {
            _window = window; 

            _window.KeyDown += OnKeyDown; 
            _window.KeyUp += OnKeyUp; 
        }
        public bool IsKeyDown(Keys key) 
        {
            return _pressedKeys.Contains(key); 
        }
        private void OnKeyDown(object? sender, KeyEventArgs e) 
        {
            _pressedKeys.Add(e.KeyCode); 
        }
        private void OnKeyUp(object? sender, KeyEventArgs e) 
        {
            _pressedKeys.Remove(e.KeyCode); 
        }
        public void Dispose() 
        {
            _window.KeyDown -= OnKeyDown;
            _window.KeyUp -= OnKeyUp; 
        }
    }
}