using System;
using System.Collections.Generic;
using System.Drawing;
using RayForge.Math;
using RayForge.World;

namespace RayForge.Graphics
{
    public sealed class SpriteRenderer
    {
        private const float FieldOfView = MathF.PI / 3f;
        private readonly TextureManager _textures;
        public SpriteRenderer(TextureManager textures)
        {
            _textures = textures;
        }
        public void Render(
            System.Drawing.Graphics graphics,
            GameWorld world,
            IReadOnlyList<Entity> entities,
            Size viewport,
            float[] zBuffer)
        {
            var sortedEntities = new List<Entity>(entities);
            sortedEntities.Sort((a, b) =>
            {
                float distA = (a.Position - world.Player.Position).LengthSquared;
                float distB = (b.Position - world.Player.Position).LengthSquared;
                return distB.CompareTo(distA);
            });
            foreach (Entity entity in sortedEntities)
            {
                if (!entity.IsActive) continue;
                DrawEntity(graphics, world, entity, viewport, zBuffer);
            }
        }
        private void DrawEntity(System.Drawing.Graphics graphics, GameWorld world, Entity entity, Size viewport, float[] zBuffer)
        {
            Vector2D delta = entity.Position - world.Player.Position;
            float distance = delta.Length;
            if (distance < 0.1f) return;
            float angleToEntity = MathF.Atan2(delta.Y, delta.X);
            float relativeAngle = AngleHelper.Delta(world.Player.Angle, angleToEntity);
            if (MathF.Abs(relativeAngle) > FieldOfView * 1.5f) return;
            float correctedDistance = distance * MathF.Cos(relativeAngle);
            if (correctedDistance < 0.1f) correctedDistance = 0.1f;
            float spriteHeight = (viewport.Height / correctedDistance) * 0.7f;
            float spriteWidth = spriteHeight;
            float screenX = (relativeAngle / FieldOfView + 0.5f) * viewport.Width;
            float top = (viewport.Height - spriteHeight) * 0.5f;
            Bitmap texture = _textures.GetTexture(entity.TextureId);
            int startX = (int)(screenX - spriteWidth / 2);
            int endX = (int)(screenX + spriteWidth / 2);
            int drawStartX = startX;
            if (drawStartX < 0) drawStartX = 0;
            int drawEndX = endX;
            if (drawEndX > viewport.Width) drawEndX = viewport.Width;
            for (int x = drawStartX; x < drawEndX; x++)
            {
                if (x >= 0 && x < zBuffer.Length)
                {
                    if (zBuffer[x] < correctedDistance - 0.1f) continue;
                }
                float texXPercent = (float)(x - startX) / spriteWidth;
                int texX = (int)(texXPercent * texture.Width);
                if (texX < 0 || texX >= texture.Width) continue;
                int h = (int)spriteHeight;
                int limit = viewport.Height * 2;
                int finalHeight = h > limit ? limit : h;
                int renderTop = (int)top;
                if (finalHeight > 0)
                {
                    graphics.DrawImage(texture,
                        new Rectangle(x, renderTop, 1, finalHeight), 
                        new Rectangle(texX, 0, 1, texture.Height),  
                        GraphicsUnit.Pixel);
                }
            }
        }
    }
}