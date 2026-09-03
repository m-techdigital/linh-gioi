using System;
using System.Globalization;
using LinhGioi.Account;
using LinhGioi.Art;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class PlayableWorldController : MonoBehaviour
    {
        private const float MoveSpeed = 4f;
        private const float RotateSpeed = 120f;
        private CharacterResponse _character;
        private Transform _marker;

        public event Action PositionChanged;

        public Vector3 CurrentPosition => _marker == null ? Vector3.zero : _marker.position;
        public float CurrentYawDegrees => _marker == null ? 0f : _marker.eulerAngles.y;

        public void Enter(CharacterResponse character)
        {
            _character = character ?? throw new ArgumentNullException(nameof(character));
            if (_marker == null) _marker = CreateMarker().transform;
            _marker.position = character.Position;
            _marker.rotation = Quaternion.Euler(0f, character.yawDegrees, 0f);
            PositionChanged?.Invoke();
        }

        public SaveCharacterPositionRequest BuildSaveRequest()
        {
            var position = CurrentPosition;
            return new SaveCharacterPositionRequest(position.x, position.y, position.z, NormalizeYaw(CurrentYawDegrees));
        }

        public string FormatPosition()
        {
            var position = CurrentPosition;
            return string.Format(CultureInfo.InvariantCulture, "x={0:0.00} y={1:0.00} z={2:0.00} yaw={3:0.0}", position.x, position.y, position.z, NormalizeYaw(CurrentYawDegrees));
        }

        public void SetSmokePosition(float x, float y, float z, float yawDegrees)
        {
            if (_marker == null) _marker = CreateMarker().transform;
            _marker.position = new Vector3(x, y, z);
            _marker.rotation = Quaternion.Euler(0f, yawDegrees, 0f);
            PositionChanged?.Invoke();
        }

        private void Update()
        {
            if (_marker == null || _character == null) return;
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            var rotate = 0f;
            if (Input.GetKey(KeyCode.Q)) rotate -= 1f;
            if (Input.GetKey(KeyCode.E)) rotate += 1f;

            if (Mathf.Abs(rotate) > 0.001f)
                _marker.Rotate(Vector3.up, rotate * RotateSpeed * Time.deltaTime, Space.World);

            var input = new Vector3(horizontal, 0f, vertical);
            if (input.sqrMagnitude > 1f) input.Normalize();
            if (input.sqrMagnitude > 0.0001f)
            {
                _marker.position += input * MoveSpeed * Time.deltaTime;
                PositionChanged?.Invoke();
            }
        }

        private static GameObject CreateMarker()
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            marker.name = "LGO Player Marker";
            marker.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
            var renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = RuntimeArtCatalog.CreateMaterial("LGO Hero Placeholder", RuntimeArtCatalog.Spirit);
            }
            EnsureCamera(marker.transform);
            EnsureGround();
            EnsureWorldPlaceholders();
            return marker;
        }

        private static void EnsureCamera(Transform target)
        {
            if (Camera.main != null) return;
            var cameraObject = new GameObject("LGO Playable Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = target.position + new Vector3(0f, 7f, -9f);
            camera.transform.rotation = Quaternion.Euler(38f, 0f, 0f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = RuntimeArtCatalog.Background;
            var lightObject = new GameObject("LGO Playable Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
        }

        private static void EnsureGround()
        {
            if (GameObject.Find("LGO World Ground") != null) return;
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "LGO World Ground";
            ground.transform.localScale = new Vector3(8f, 1f, 8f);
            var renderer = ground.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = RuntimeArtCatalog.CreateMaterial("LGO Training Ground Tile", RuntimeArtCatalog.SurfaceRaised);
            }
        }

        private static void EnsureWorldPlaceholders()
        {
            CreateMarkerCube("LGO NPC Keeper Placeholder", new Vector3(-3f, 0.75f, 3f), RuntimeArtCatalog.Gold, new Vector3(0.9f, 1.5f, 0.9f));
            CreateMarkerCube("LGO Shadow Slime Placeholder", new Vector3(3f, 0.4f, 3f), RuntimeArtCatalog.Shadow, new Vector3(1.4f, 0.8f, 1.4f));
            CreateMarkerCube("LGO Spirit Burst VFX Placeholder", new Vector3(0f, 0.08f, 4.5f), RuntimeArtCatalog.Spirit, new Vector3(1.2f, 0.16f, 1.2f));
        }

        private static void CreateMarkerCube(string name, Vector3 position, Color color, Vector3 scale)
        {
            if (GameObject.Find(name) != null) return;
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = name;
            marker.transform.position = position;
            marker.transform.localScale = scale;
            var renderer = marker.GetComponent<Renderer>();
            if (renderer != null) renderer.material = RuntimeArtCatalog.CreateMaterial(name + " Material", color);
        }

        private static float NormalizeYaw(float yaw)
        {
            yaw %= 360f;
            return yaw < 0f ? yaw + 360f : yaw;
        }
    }
}
