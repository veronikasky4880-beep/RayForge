using System; 
using System.Diagnostics; 
using System.Drawing;
using System.Windows.Forms; 
using RayForge.Graphics; 
using RayForge.World; 
using RayForge.Math; 
namespace RayForge.Core 
{
    public sealed class GameEngine : IDisposable 
    {
        private readonly Form _window; 
        private readonly System.Windows.Forms.Timer _gameTimer; 
        private readonly Stopwatch _stopwatch; 
        private readonly InputController _input; 
        private readonly GameWorld _world; 
        private readonly Raycaster _raycaster; 
        private readonly SpriteRenderer _spriteRenderer; 
        private long _lastTicks; 
        private bool _isGameOver = false; 
        public GameEngine(Form window) 
        {
            _window = window; 
            _input = new InputController(window); 
            _world = new GameWorld(); 
            _raycaster = new Raycaster(); 
            _spriteRenderer = new SpriteRenderer(_raycaster.Textures); 
            _raycaster.Textures.LoadTexture(7, "assets/item.png"); 
            _raycaster.Textures.LoadTexture(10, "assets/enemy.png"); 
            _world.Entities.Add(new Enemy(5.5f, 5.5f) { TextureId = 10 }); 
            _world.Entities.Add(new Item(3.5f, 1.5f) { TextureId = 7 }); 
            _world.Entities.Add(new Item(10.5f, 1.5f) { TextureId = 7 }); 
            _world.Entities.Add(new Item(6.5f, 5.5f) { TextureId = 7 }); 
            _stopwatch = Stopwatch.StartNew(); 
            _lastTicks = _stopwatch.ElapsedTicks; 
            _gameTimer = new System.Windows.Forms.Timer 
            {
                Interval = 16 
            };
            _gameTimer.Tick += UpdateFrame; 
            _gameTimer.Start(); 
        }
        private void UpdateFrame(object? sender, EventArgs e) 
        {
            if (_isGameOver) return; 
            long currentTicks = _stopwatch.ElapsedTicks; 
            float deltaTime = (currentTicks - _lastTicks) / (float)Stopwatch.Frequency; 
            if (deltaTime > 0.05f) deltaTime = 0.05f; 
            _world.Update(_input, deltaTime); 
            int itemsRemaining = 0; 
            foreach (var entity in _world.Entities) 
            {
                if (entity is Item && entity.IsActive)
                {
                    itemsRemaining++; 
                }
            }
            _window.Text = $"RayForge - ЗІБРАТИ МОНЕТКИ: {itemsRemaining}";
            if (itemsRemaining == 0)
            {
                _isGameOver = true; 
                _gameTimer.Stop(); 
                _window.Text = "ПЕРЕМОГА! Золото ЗІБРАНЕ!"; 
                _window.Invalidate(); 
                MessageBox.Show("ВИ ЗІБРАЛИ ВСІ 3 МОНЕТКИ І ВИГРАЛИ!", "УСПІХ!"); 
                return;
            }
            _window.Invalidate(); 
        }
        public void Render(System.Drawing.Graphics graphics) 
        {
            _raycaster.Render(graphics, _world, _window.ClientSize); 
            _spriteRenderer.Render( 
                graphics,
                _world,
                _world.Entities,
                _window.ClientSize,
                _raycaster.ZBuffer); 
        }
        public void Dispose() 
        {
            _gameTimer.Stop(); 
            _gameTimer.Dispose(); 
            _input.Dispose(); 
        }
    }
}