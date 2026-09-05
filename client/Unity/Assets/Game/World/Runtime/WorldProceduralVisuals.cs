using UnityEngine;

namespace LinhGioi.World
{
    internal static class WorldProceduralVisuals
    {
        private static Sprite _softGroundShadowSprite;
        private static Sprite _worldPlatformGlowSprite;
        private static Sprite _worldPathGlowSprite;
        private static Sprite _worldMistVeilSprite;

        internal static Texture2D CreateTrainingGroundTexture()
        {
            const int size = 384;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "LGO Procedural Cultivation Platform Texture v1",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            var deepA = new Color(0.075f, 0.125f, 0.19f, 1f);
            var deepB = new Color(0.135f, 0.205f, 0.30f, 1f);
            var stone = new Color(0.18f, 0.255f, 0.35f, 1f);
            var mist = new Color(0.28f, 0.52f, 0.66f, 1f);
            var line = new Color(0.14f, 0.80f, 1.00f, 1f);
            var gold = new Color(0.92f, 0.68f, 0.30f, 1f);
            var center = new Vector2(0.5f, 0.46f);
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var uv = new Vector2((x + 0.5f) / size, (y + 0.5f) / size);
                    var noise = HashNoise(x, y);
                    var slowNoise = HashNoise(x / 12, y / 12);
                    var horizon = Mathf.Clamp01(uv.y);
                    var color = Color.Lerp(deepA, deepB, horizon * 0.48f + slowNoise * 0.08f);
                    color = Color.Lerp(color, stone, noise * 0.018f);

                    var toCenter = uv - center;
                    var dist = toCenter.magnitude;
                    var innerRing = SmoothBand(dist, 0.145f, 0.0075f);
                    var midRing = SmoothBand(dist, 0.245f, 0.0065f);
                    var outerRing = SmoothBand(dist, 0.355f, 0.009f);
                    color = Color.Lerp(color, line, innerRing * 0.045f);
                    color = Color.Lerp(color, line, midRing * 0.035f);
                    color = Color.Lerp(color, gold, outerRing * 0.035f);
                    var diagonalA = Mathf.Abs(toCenter.x - toCenter.y);
                    var diagonalB = Mathf.Abs(toCenter.x + toCenter.y);
                    if (dist < 0.34f)
                    {
                        color = Color.Lerp(color, line, SmoothBand(diagonalA, 0f, 0.006f) * 0.014f);
                        color = Color.Lerp(color, line, SmoothBand(diagonalB, 0f, 0.006f) * 0.014f);
                    }
                    if (dist < 0.36f)
                    {
                        color = Color.Lerp(color, line, SmoothBand(Mathf.Abs(toCenter.x), 0f, 0.005f) * 0.018f);
                        color = Color.Lerp(color, line, SmoothBand(Mathf.Abs(toCenter.y), 0f, 0.005f) * 0.018f);
                    }

                    var pathToGate = DistanceToSegment(uv, new Vector2(0.50f, 0.28f), new Vector2(0.50f, 0.08f));
                    var pathToStone = DistanceToSegment(uv, new Vector2(0.50f, 0.46f), new Vector2(0.50f, 0.78f));
                    var pathToKeeper = DistanceToSegment(uv, new Vector2(0.50f, 0.46f), new Vector2(0.31f, 0.70f));
                    var guide = Mathf.Min(pathToGate, Mathf.Min(pathToStone, pathToKeeper));
                    color = Color.Lerp(color, line, SmoothBand(guide, 0f, 0.018f) * 0.038f);

                    var platformGlow = Mathf.Clamp01(1f - dist / 0.44f);
                    color = Color.Lerp(color, mist, platformGlow * 0.055f);
                    var cloudBand = Mathf.Sin((uv.x * 1.9f + uv.y * 1.15f + slowNoise * 0.6f) * Mathf.PI);
                    color = Color.Lerp(color, mist, Mathf.Clamp01(cloudBand) * 0.025f);
                    var vignette = Mathf.Clamp01((dist - 0.18f) / 0.58f);
                    color = Color.Lerp(color, Color.black, vignette * 0.14f);
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply(false, true);
            return texture;
        }

        internal static SpriteRenderer CreateGroundShadowSprite(string name, Vector3 position, Vector3 scale, int sortingOrder)
        {
            var sprite = GetSoftGroundShadowSprite();
            if (sprite == null) return null;
            var existing = GameObject.Find(name);
            var holder = existing != null ? existing : new GameObject(name);
            holder.transform.position = position;
            holder.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            holder.transform.localScale = scale;
            var renderer = holder.GetComponent<SpriteRenderer>() ?? holder.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.0f, 0.012f, 0.028f, 0.42f);
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        internal static SpriteRenderer CreateGroundGlowSprite(string name, Sprite sprite, Vector3 position, Vector3 scale, Color color, int sortingOrder)
        {
            if (sprite == null) return null;
            var existing = GameObject.Find(name);
            var holder = existing != null ? existing : new GameObject(name);
            holder.transform.position = position;
            holder.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            holder.transform.localScale = scale;
            var renderer = holder.GetComponent<SpriteRenderer>() ?? holder.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        internal static SpriteRenderer CreatePathGlowSprite(string name, Vector3 position, Vector3 scale, Color color, int sortingOrder)
        {
            return CreateGroundGlowSprite(name, GetWorldPathGlowSprite(), position, scale, color, sortingOrder);
        }

        internal static SpriteRenderer CreateMistVeilSprite(string name, Vector3 position, Vector3 scale, Color color, int sortingOrder)
        {
            return CreateGroundGlowSprite(name, GetWorldMistVeilSprite(), position, scale, color, sortingOrder);
        }

        internal static Sprite GetWorldPlatformGlowSprite()
        {
            if (_worldPlatformGlowSprite != null) return _worldPlatformGlowSprite;
            const int size = 192;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "LGO Procedural World Platform Glow Texture v1",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var u = ((x + 0.5f) / size - 0.5f) * 2f;
                    var v = ((y + 0.5f) / size - 0.5f) * 2f;
                    var dist = Mathf.Sqrt(u * u + v * v);
                    var ringA = SmoothBand(dist, 0.58f, 0.035f);
                    var ringB = SmoothBand(dist, 0.82f, 0.024f);
                    var radial = Mathf.Clamp01(1f - dist);
                    var spokes = Mathf.Abs(Mathf.Sin(Mathf.Atan2(v, u) * 4f));
                    var alpha = radial * radial * 0.12f + ringA * 0.38f + ringB * 0.22f;
                    if (dist < 0.78f) alpha += Mathf.Pow(spokes, 18f) * 0.055f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(alpha)));
                }
            }
            texture.Apply(false, true);
            _worldPlatformGlowSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            _worldPlatformGlowSprite.name = "LGO Procedural World Platform Glow Sprite v1";
            return _worldPlatformGlowSprite;
        }

        private static Sprite GetSoftGroundShadowSprite()
        {
            if (_softGroundShadowSprite != null) return _softGroundShadowSprite;
            const int size = 128;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "LGO Procedural Soft Ground Shadow Texture v1",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var u = ((x + 0.5f) / size - 0.5f) * 2f;
                    var v = ((y + 0.5f) / size - 0.5f) * 2f;
                    var dist = Mathf.Sqrt(u * u + v * v);
                    var alpha = Mathf.Clamp01(1f - dist);
                    alpha = alpha * alpha * 0.82f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            texture.Apply(false, true);
            _softGroundShadowSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            _softGroundShadowSprite.name = "LGO Procedural Soft Ground Shadow Sprite v1";
            return _softGroundShadowSprite;
        }

        private static Sprite GetWorldPathGlowSprite()
        {
            if (_worldPathGlowSprite != null) return _worldPathGlowSprite;
            const int size = 128;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "LGO Procedural World Path Glow Texture v1",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var u = Mathf.Abs((x + 0.5f) / size - 0.5f) * 2f;
                    var v = Mathf.Abs((y + 0.5f) / size - 0.5f) * 2f;
                    var widthFade = Mathf.Clamp01(1f - u);
                    var endFade = Mathf.Clamp01(1f - Mathf.Pow(v, 3f));
                    var alpha = widthFade * widthFade * endFade * 0.76f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            texture.Apply(false, true);
            _worldPathGlowSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            _worldPathGlowSprite.name = "LGO Procedural World Path Glow Sprite v1";
            return _worldPathGlowSprite;
        }

        private static Sprite GetWorldMistVeilSprite()
        {
            if (_worldMistVeilSprite != null) return _worldMistVeilSprite;
            const int size = 128;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "LGO Procedural World Mist Veil Texture v1",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var u = ((x + 0.5f) / size - 0.5f) * 2f;
                    var v = ((y + 0.5f) / size - 0.5f) * 2f;
                    var dist = Mathf.Sqrt(u * u + v * v);
                    var noise = HashNoise(x / 4, y / 4);
                    var wave = Mathf.Abs(Mathf.Sin((u * 2.1f + v * 1.35f + noise * 0.8f) * Mathf.PI));
                    var alpha = Mathf.Clamp01(1f - dist) * Mathf.Lerp(0.08f, 0.34f, wave);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            texture.Apply(false, true);
            _worldMistVeilSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            _worldMistVeilSprite.name = "LGO Procedural World Mist Veil Sprite v1";
            return _worldMistVeilSprite;
        }

        private static float DistanceToSegment(Vector2 point, Vector2 start, Vector2 end)
        {
            var segment = end - start;
            var lengthSq = Vector2.Dot(segment, segment);
            if (lengthSq <= 0.0001f) return Vector2.Distance(point, start);
            var t = Mathf.Clamp01(Vector2.Dot(point - start, segment) / lengthSq);
            return Vector2.Distance(point, start + segment * t);
        }

        private static float SmoothBand(float value, float target, float halfWidth)
        {
            var delta = Mathf.Abs(value - target);
            return Mathf.Clamp01(1f - delta / Mathf.Max(halfWidth, 0.0001f));
        }

        private static float HashNoise(int x, int y)
        {
            unchecked
            {
                var n = x * 374761393 + y * 668265263;
                n = (n ^ (n >> 13)) * 1274126177;
                return ((n ^ (n >> 16)) & 0x7fffffff) / (float)0x7fffffff;
            }
        }
    }
}
