using UnityEngine;

namespace LinhGioi.World
{
    internal static class WorldProceduralVisuals
    {
        private static Sprite _softGroundShadowSprite;
        private static Sprite _worldPlatformGlowSprite;
        private static Sprite _worldPathGlowSprite;
        private static Sprite _worldMistVeilSprite;

        internal static Vector2 GroundUv(Vector3 worldPosition)
        {
            const float courtyardSize = 18f;
            return new Vector2(worldPosition.x / courtyardSize + 0.5f, worldPosition.z / courtyardSize + 0.5f);
        }

        internal static Texture2D CreateTrainingGroundTexture(Vector3 gatePosition, Vector3 keeperPosition, Vector3 stonePosition)
        {
            const int size = 256;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "LGO Procedural Cultivation Platform Texture v1",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            var deepA = new Color(0.085f, 0.145f, 0.220f, 1f);
            var deepB = new Color(0.245f, 0.350f, 0.480f, 1f);
            var stone = new Color(0.31f, 0.39f, 0.49f, 1f);
            var mist = new Color(0.39f, 0.70f, 0.84f, 1f);
            var line = new Color(0.14f, 0.80f, 1.00f, 1f);
            var gold = new Color(0.92f, 0.68f, 0.30f, 1f);
            var center = GroundUv(Vector3.zero);
            var gateUv = GroundUv(gatePosition);
            var keeperUv = GroundUv(keeperPosition);
            var stoneUv = GroundUv(stonePosition);
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var uv = new Vector2((x + 0.5f) / size, (y + 0.5f) / size);
                    var noise = HashNoise(x, y);
                    var slowNoise = HashNoise(x / 12, y / 12);
                    var horizon = Mathf.Clamp01(uv.y);
                    var color = Color.Lerp(deepA, deepB, horizon * 0.48f + slowNoise * 0.08f);
                    color = Color.Lerp(color, stone, noise * 0.045f);
                    var tileU = Mathf.Abs(Mathf.Repeat(uv.x * 8.0f + slowNoise * 0.025f, 1f) - 0.5f);
                    var tileV = Mathf.Abs(Mathf.Repeat(uv.y * 6.0f + slowNoise * 0.020f, 1f) - 0.5f);
                    var tileSeam = Mathf.Clamp01((0.018f - Mathf.Min(tileU, tileV)) / 0.018f);
                    color = Color.Lerp(color, new Color(0.045f, 0.10f, 0.17f, 1f), tileSeam * 0.30f);

                    var toCenter = uv - center;
                    var dist = toCenter.magnitude;
                    var innerRing = SmoothBand(dist, 0.145f, 0.0075f);
                    var midRing = SmoothBand(dist, 0.245f, 0.0065f);
                    var outerRing = SmoothBand(dist, 0.355f, 0.009f);
                    color = Color.Lerp(color, line, innerRing * 0.30f);
                    color = Color.Lerp(color, line, midRing * 0.24f);
                    color = Color.Lerp(color, gold, outerRing * 0.21f);
                    var diagonalA = Mathf.Abs(toCenter.x - toCenter.y);
                    var diagonalB = Mathf.Abs(toCenter.x + toCenter.y);
                    if (dist < 0.34f)
                    {
                        color = Color.Lerp(color, line, SmoothBand(diagonalA, 0f, 0.006f) * 0.050f);
                        color = Color.Lerp(color, line, SmoothBand(diagonalB, 0f, 0.006f) * 0.050f);
                    }
                    if (dist < 0.36f)
                    {
                        color = Color.Lerp(color, line, SmoothBand(Mathf.Abs(toCenter.x), 0f, 0.005f) * 0.064f);
                        color = Color.Lerp(color, line, SmoothBand(Mathf.Abs(toCenter.y), 0f, 0.005f) * 0.064f);
                    }

                    var pathToGate = DistanceToSegment(uv, center, gateUv);
                    var pathToStone = DistanceToSegment(uv, keeperUv, stoneUv);
                    var pathToKeeper = DistanceToSegment(uv, center, keeperUv);
                    var guide = Mathf.Min(pathToGate, Mathf.Min(pathToStone, pathToKeeper));
                    color = Color.Lerp(color, line, SmoothBand(guide, 0f, 0.038f) * 0.30f);

                    var platformGlow = Mathf.Clamp01(1f - dist / 0.44f);
                    color = Color.Lerp(color, mist, platformGlow * 0.16f);
                    var cloudBand = Mathf.Sin((uv.x * 1.9f + uv.y * 1.15f + slowNoise * 0.6f) * Mathf.PI);
                    color = Color.Lerp(color, mist, Mathf.Clamp01(cloudBand) * 0.10f);
                    var depthBand = Mathf.Clamp01(Mathf.Sin((uv.y * 5.5f + slowNoise * 0.18f) * Mathf.PI) * 0.5f + 0.5f);
                    color = Color.Lerp(color, mist, depthBand * horizon * 0.040f);
                    var vignette = Mathf.Clamp01((dist - 0.18f) / 0.58f);
                    color = Color.Lerp(color, Color.black, vignette * 0.10f);
                    var edgeFade = Mathf.Clamp01((Mathf.Abs(uv.x - 0.5f) - 0.36f) / 0.18f);
                    color = Color.Lerp(color, new Color(0.055f, 0.095f, 0.145f, 1f), edgeFade * 0.12f);
                    // Lay stone after atmospheric shading so the route remains readable under the actors.
                    var pavingMask = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.025f, 0.032f, guide));
                    var pavingRow = Mathf.FloorToInt(uv.y * 36f);
                    var pavingU = Mathf.Repeat(uv.x * 24f + (pavingRow % 2) * 0.5f, 1f);
                    var pavingV = Mathf.Repeat(uv.y * 36f, 1f);
                    var seamDistance = Mathf.Min(
                        Mathf.Min(pavingU, 1f - pavingU) * size / 24f,
                        Mathf.Min(pavingV, 1f - pavingV) * size / 36f);
                    var mortarCoverage = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.2f, 0.9f, seamDistance));
                    var paving = Color.Lerp(new Color(0.32f, 0.34f, 0.35f, 1f), new Color(0.46f, 0.47f, 0.46f, 1f), noise * 0.45f);
                    paving = Color.Lerp(paving, new Color(0.20f, 0.22f, 0.23f, 1f), mortarCoverage);
                    color = Color.Lerp(color, paving, pavingMask);
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
                    var alpha = Mathf.Clamp01(1f - dist) * Mathf.Lerp(0.12f, 0.46f, wave);
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
