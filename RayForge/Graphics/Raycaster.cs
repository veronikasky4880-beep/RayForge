using System;
using System.Collections.Generic;
using System.Drawing;
using RayForge.Math;
using RayForge.World;
namespace RayForge.Graphics
{
    public sealed class Raycaster
    {
        public TextureManager Textures { get; } = new();
        public float[] ZBuffer { get; private set; } = Array.Empty<float>();
        private const float FieldOfView = MathF.PI / 3f;
        private const float MaxDepth = 32f;
        public void Render(System.Drawing.Graphics graphics, GameWorld world, Size viewport)
        {
            if (ZBuffer.Length != viewport.Width)
            {
                ZBuffer = new float[viewport.Width];
            }
            graphics.Clear(Color.Black);
            DrawSky(graphics, viewport);
            DrawFloor(graphics, viewport);
            RenderWalls(graphics, world, viewport);
            RenderSprites(graphics, world, viewport);
            DrawMinimap(graphics, world);
            using var crosshair = new Pen(Color.White, 2f);
            int cx = viewport.Width / 2;
            int cy = viewport.Height / 2;
            graphics.DrawLine(crosshair, cx - 8, cy, cx + 8, cy);
            graphics.DrawLine(crosshair, cx, cy - 8, cx, cy + 8);
        }
        private void RenderWalls(System.Drawing.Graphics graphics, GameWorld world, Size viewport)
        {
            Player player = world.Player;
            for (int column = 0; column < viewport.Width; column += 2)
            {
                float cameraX = (float)column / viewport.Width;
                float rayAngle = player.Angle - FieldOfView / 2f + cameraX * FieldOfView;
                RaycastHit hit = CastRay(world.Map, player.Position, rayAngle);
                float correctedDistance = hit.Distance * MathF.Cos(rayAngle - player.Angle);
                correctedDistance = MathF.Max(correctedDistance, 0.0001f);
                ZBuffer[column] = correctedDistance;
                if (column + 1 < ZBuffer.Length) ZBuffer[column + 1] = correctedDistance;
                int wallHeight = (int)(viewport.Height / correctedDistance);
                int top = (viewport.Height - wallHeight) / 2;
                Bitmap texture = Textures.GetTexture(hit.TileId);
                float wallX = hit.HitVerticalWall ? hit.Position.Y : hit.Position.X;
                wallX -= MathF.Floor(wallX); 
                int texX = (int)(wallX * texture.Width);
                graphics.DrawImage(texture,
                    new Rectangle(column, top, 2, wallHeight),
                    new Rectangle(texX, 0, 1, texture.Height),
                    GraphicsUnit.Pixel);
            }
        }
        private void RenderSprites(System.Drawing.Graphics graphics, GameWorld world, Size viewport)
        {
            foreach (var entity in world.Entities)
            {
                if (!entity.IsActive) continue;
                Vector2D delta = entity.Position - world.Player.Position;
                float distance = delta.Length;
                if (distance < 0.2f) continue;
                float angleToEntity = MathF.Atan2(delta.Y, delta.X);
                float relativeAngle = AngleHelper.Delta(world.Player.Angle, angleToEntity);
                if (MathF.Abs(relativeAngle) > MathF.PI / 2f) continue;
                float correctedDistance = distance * MathF.Cos(relativeAngle);
                float spriteHeight = (viewport.Height / correctedDistance) * 0.7f;
                float spriteWidth = spriteHeight;
                float screenX = (relativeAngle / FieldOfView + 0.5f) * viewport.Width;
                float top = (viewport.Height - spriteHeight) * 0.5f;
                Bitmap texture = Textures.GetTexture(entity.TextureId);
                int startX = (int)(screenX - spriteWidth / 2);
                int endX = (int)(screenX + spriteWidth / 2);
                for (int x = startX; x < endX; x++)
                {
                    if (x < 0 || x >= viewport.Width) continue;
                    if (ZBuffer[x] < correctedDistance - 0.1f) continue;
                    int texX = (int)((float)(x - startX) / spriteWidth * texture.Width);
                    if (texX < 0 || texX >= texture.Width) continue;
                    int finalH = (int)spriteHeight;
                    if (finalH > viewport.Height * 2) finalH = viewport.Height * 2;
                    if (finalH > 0)
                    {
                        graphics.DrawImage(texture,
                            new Rectangle(x, (int)top, 1, finalH),
                            new Rectangle(texX, 0, 1, texture.Height),
                            GraphicsUnit.Pixel);
                    }
                }
            }
        }
        private void DrawSky(System.Drawing.Graphics graphics, Size viewport)
        {
            graphics.FillRectangle(Brushes.SteelBlue, 0, 0, viewport.Width, viewport.Height / 2);
        }
        private void DrawFloor(System.Drawing.Graphics graphics, Size viewport)
        {
            graphics.FillRectangle(Brushes.DimGray, 0, viewport.Height / 2, viewport.Width, viewport.Height / 2);
        }
        private RaycastHit CastRay(GameMap map, Vector2D origin, float angle)
        {
            Vector2D direction = Vector2D.FromAngle(angle); 
            Vector2D position = origin; 
            float distance = 0f; 
            const float step = 0.05f; 
            while (distance < MaxDepth)
            {
                position += direction * step; 
                distance += step; 
                int tileX = (int)position.X; 
                int tileY = (int)position.Y; 
                if (map.IsWall(tileX, tileY))
                {
                    float fx = position.X - tileX; 
                    float fy = position.Y - tileY; 
                    bool vertical = fx < 0.1f || fx > 0.9f;
                    return new RaycastHit(position, distance, vertical, map.GetTile(tileX, tileY));
                }
            }
            return new RaycastHit(position, MaxDepth, false, 0);
        }
        private void DrawMinimap(System.Drawing.Graphics graphics, GameWorld world)
        {
            const int cellSize = 5; 
            const int offset = 10; 
            GameMap map = world.Map; 
            using var wallBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255)); 
            using var floorBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)); 
            using var playerBrush = new SolidBrush(Color.Red); 
            using var enemyBrush = new SolidBrush(Color.Orange); 
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    int tile = map.GetTile(x, y);
                    graphics.FillRectangle(floorBrush, offset + x * cellSize, offset + y * cellSize, cellSize, cellSize);
                    if (tile > 0) graphics.FillRectangle(wallBrush, offset + x * cellSize, offset + y * cellSize, cellSize - 1, cellSize - 1);
                }
            }
            float px = offset + world.Player.Position.X * cellSize;
            float py = offset + world.Player.Position.Y * cellSize;
            graphics.FillEllipse(playerBrush, px - 2, py - 2, 4, 4);
            foreach (var entity in world.Entities)
            {
                if (!entity.IsActive) continue;
                graphics.FillEllipse(enemyBrush, offset + entity.Position.X * cellSize - 2, offset + entity.Position.Y * cellSize - 2, 4, 4);
            }
        }
    }
}