using LinhGioi.Art;
using Unity.Cinemachine;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class OnboardingBlockoutWorld : MonoBehaviour
    {
        public static readonly Vector3 KeeperPoint = new Vector3(-3f, 0f, 1f);
        public static readonly Vector3 StonePoint = new Vector3(3.5f, 0f, 4f);
        public Vector2 Movement { get; set; }
        public Vector2 ScreenMovement { get; set; }
        public Vector3 Position => _player.transform.position;
        private CharacterController _player;
        private Camera _camera;
        private CinemachineBrain _cameraBrain;
        private BoxCollider _cameraVolume;
        private Renderer _keeper;
        private Renderer _stone;
        private TextMesh _keeperLabel;
        private TextMesh _stoneLabel;
        private SpriteRenderer _stoneFocus;
        private bool _stoneReady;
        private float _stoneCompletedAt = -1f;
        private bool _screenInputHeld;
        private Vector3 _inputForward;

        private void Awake()
        {
            RenderSettings.ambientLight = new Color(0.48f, 0.49f, 0.51f);
            var light = new GameObject("Blockout daylight").AddComponent<Light>();
            light.transform.SetParent(transform);
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
            light.shadows = LightShadows.Soft;
            var paving = Material(new Color(0.39f, 0.40f, 0.39f));
            var walls = Material(new Color(0.52f, 0.52f, 0.46f));
            var roof = Material(new Color(0.19f, 0.23f, 0.24f));
            var trim = Material(new Color(0.68f, 0.55f, 0.32f));
            Box("Courtyard", new Vector3(0f, -0.1f, 1.5f), new Vector3(14f, 0.2f, 27f), paving);
            for (var z = -4; z < 15; z++)
                Box("Paving joint", new Vector3(0f, 0.002f, z), new Vector3(8f, 0.004f, 0.025f), roof, false);
            for (var side = -1; side <= 1; side += 2)
            {
                for (var z = 0; z <= 12; z += 6)
                {
                    Box("Street module", new Vector3(side * 5.5f, 1.6f, z), new Vector3(3f, 3.2f, 4.8f), walls);
                    Box("Roof eave", new Vector3(side * 5.5f, 3.3f, z), new Vector3(3.6f, 0.25f, 5.4f), roof);
                    for (var slope = -1; slope <= 1; slope += 2)
                    {
                        var panel = Box("Roof slope", new Vector3(side * 5.5f + slope * 0.8f, 3.8f, z), new Vector3(1.9f, 0.16f, 5.2f), roof);
                        panel.transform.rotation = Quaternion.Euler(0f, 0f, -slope * 25f);
                    }
                    Box("Door", new Vector3(side * 3.985f, 1f, z), new Vector3(0.04f, 2f, 1.1f), trim, false);
                }
                Box("Gate post", new Vector3(side * 5.2f, 2.3f, -4f), new Vector3(0.8f, 4.6f, 0.8f), walls);
                Box("Boundary", new Vector3(side * 7f, 0.6f, 1.5f), new Vector3(0.2f, 1.2f, 27f), walls);
            }
            Box("Far boundary", new Vector3(0f, 0.4f, 15f), new Vector3(14f, 0.8f, 0.2f), walls);
            Box("Arrival boundary", new Vector3(0f, 0.4f, -12f), new Vector3(14f, 0.8f, 0.2f), walls);
            _keeper = Actor("Blockout Keeper", KeeperPoint, LgoVisualAssetRegistryV3B.GateKeeperNpc, 1.8f, trim);
            _stone = Actor("Blockout Stone", StonePoint, LgoVisualAssetRegistryV3B.TrainingStone, 1.5f, trim);
            _keeperLabel = WorldLabelPresenter.Create("Blockout Keeper Label", "Người Giữ Cổng", KeeperPoint, RuntimeArtCatalog.Gold);
            _stoneLabel = WorldLabelPresenter.Create("Blockout Stone Label", "Đá Luyện", StonePoint, RuntimeArtCatalog.Gold);
            _stoneFocus = WorldProceduralVisuals.CreateGroundGlowSprite("Blockout Stone Focus",
                WorldProceduralVisuals.GetWorldPlatformGlowSprite(), StonePoint + Vector3.up * 0.025f,
                Vector3.one * 1.6f, RuntimeArtCatalog.Gold, 4);
            _stoneFocus.transform.SetParent(transform);
            _stoneFocus.gameObject.SetActive(false);

            var player = new GameObject("Blockout player proxy");
            player.tag = "Player";
            player.transform.SetParent(transform);
            player.transform.position = new Vector3(0f, 0.04f, -3f);
            _player = player.AddComponent<CharacterController>();
            _player.height = 1.8f;
            _player.radius = 0.3f;
            _player.center = Vector3.up * 0.9f;
            _player.stepOffset = 0.2f;
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(player.transform, false);
            body.transform.localPosition = Vector3.up * 0.9f;
            body.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);
            Destroy(body.GetComponent<Collider>());
            body.GetComponent<Renderer>().sharedMaterial = Material(new Color(0.82f, 0.81f, 0.76f));
            foreach (var camera in Camera.allCameras) camera.enabled = false;
            _camera = new GameObject("Blockout perspective camera").AddComponent<Camera>();
            _camera.transform.SetParent(transform);
            _camera.tag = "MainCamera";
            _camera.fieldOfView = 55f;
            _camera.nearClipPlane = 0.1f;
            _camera.farClipPlane = 80f;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = new Color(0.46f, 0.54f, 0.57f);
            _cameraBrain = _camera.gameObject.AddComponent<CinemachineBrain>();
            _cameraBrain.UpdateMethod = CinemachineBrain.UpdateMethods.ManualUpdate;
            var tracking = new GameObject("Blockout camera tracking point").transform;
            tracking.SetParent(player.transform, false);
            tracking.localPosition = Vector3.up * 1.1f;
            var shots = new GameObject("Blockout camera shots");
            shots.transform.SetParent(transform);
            var volume = new GameObject("Blockout camera volume");
            volume.transform.SetParent(transform);
            _cameraVolume = volume.AddComponent<BoxCollider>();
            _cameraVolume.center = new Vector3(0f, 4.1f, 1.5f);
            _cameraVolume.size = new Vector3(13.4f, 7.8f, 26.4f);
            _cameraVolume.isTrigger = true;
            var clearShot = shots.AddComponent<CinemachineClearShot>();
            clearShot.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.Cut, 0f);
            CreateCameraShot(shots.transform, tracking, new Vector3(0f, 2f, -6.8f), 20);
            CreateCameraShot(shots.transform, tracking, new Vector3(-6.8f, 2f, 0f), 10);
            CreateCameraShot(shots.transform, tracking, new Vector3(6.8f, 2f, 0f), 10);
        }

        private void CreateCameraShot(Transform parent, Transform tracking, Vector3 offset, int priority)
        {
            var rig = new GameObject("Blockout camera shot " + offset);
            rig.transform.SetParent(parent);
            var virtualCamera = rig.AddComponent<CinemachineCamera>();
            virtualCamera.Priority.Value = priority;
            virtualCamera.Follow = tracking;
            virtualCamera.LookAt = tracking;
            virtualCamera.Lens.FieldOfView = _camera.fieldOfView;
            virtualCamera.Lens.NearClipPlane = _camera.nearClipPlane;
            virtualCamera.Lens.FarClipPlane = _camera.farClipPlane;
            var follow = rig.AddComponent<CinemachineFollow>();
            follow.FollowOffset = offset;
            follow.TrackerSettings.PositionDamping = Vector3.zero;
            rig.AddComponent<CinemachineHardLookAt>();
            var deoccluder = rig.AddComponent<CinemachineDeoccluder>();
            deoccluder.IgnoreTag = "Player";
            // Keep the initial sweep outside the walls of the 1.2 m passage.
            deoccluder.MinimumDistanceFromTarget = 0.05f;
            deoccluder.AvoidObstacles.Enabled = true;
            deoccluder.AvoidObstacles.UseFollowTarget.Enabled = true;
            deoccluder.AvoidObstacles.CameraRadius = 0.2f;
            deoccluder.AvoidObstacles.Strategy = CinemachineDeoccluder.ObstacleAvoidance.ResolutionStrategy.PullCameraForward;
            deoccluder.AvoidObstacles.MaximumEffort = 4;
            deoccluder.AvoidObstacles.Damping = 0.3f;
            deoccluder.AvoidObstacles.DampingWhenOccluded = 0f;
            deoccluder.ShotQualityEvaluation.Enabled = true;
            rig.AddComponent<CinemachineConfiner3D>().BoundingVolume = _cameraVolume;
        }

        private void Update()
        {
            var direction = Vector3.zero;
            if (Application.isFocused && !DialogueVisible)
            {
                var input = ScreenMovement + new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                if (input.sqrMagnitude < 0.0001f) _screenInputHeld = false;
                else if (!_screenInputHeld)
                {
                    _inputForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
                    _screenInputHeld = true;
                }
                // A camera cut must not steer input that the player is still holding.
                var right = Vector3.Cross(Vector3.up, _inputForward);
                direction = _inputForward * input.y + right * input.x + new Vector3(Movement.x, 0f, Movement.y);
            }
            else _screenInputHeld = false;
            _player.SimpleMove(Vector3.ClampMagnitude(direction, 1f) * 3.6f);
        }

        public void ResetMovementInput()
        {
            Movement = Vector2.zero;
            ScreenMovement = Vector2.zero;
            _screenInputHeld = false;
        }

        public bool DialogueVisible { get; set; }

        public void SetStoneFeedback(bool ready, bool completed)
        {
            _stoneReady = ready;
            if (completed && _stoneCompletedAt < 0f)
            {
                _stoneCompletedAt = Time.time;
                WorldLabelPresenter.Set(_stoneLabel, "Đá Luyện\nĐã ổn định", RuntimeArtCatalog.Gold);
            }
        }

        private void LateUpdate()
        {
            _cameraBrain.ManualUpdate();
            _keeperLabel.gameObject.SetActive(!DialogueVisible);
            _stoneLabel.gameObject.SetActive(!DialogueVisible);
            WorldLabelPresenter.PlaceAbove(_keeperLabel, _keeper);
            WorldLabelPresenter.PlaceAbove(_stoneLabel, _stone);
            var completion = _stoneCompletedAt >= 0f;
            var progress = completion ? Mathf.Clamp01((Time.time - _stoneCompletedAt) / 1.2f) : 0f;
            _stoneFocus.gameObject.SetActive(!DialogueVisible && (completion ? progress < 1f : _stoneReady));
            _stoneFocus.transform.localScale = Vector3.one * (1.6f + progress * 0.8f);
            var color = completion ? RuntimeArtCatalog.Spirit : RuntimeArtCatalog.Gold;
            color.a = completion ? 1f - progress : 1f;
            _stoneFocus.color = color;
        }

        private Renderer Actor(string name, Vector3 point, Sprite sprite, float height, Material fallback)
        {
            var collider = Box(name + " collider", point + Vector3.up * height * 0.5f, new Vector3(0.65f, height, 0.65f), fallback);
            if (sprite == null) return collider.GetComponent<Renderer>();
            collider.GetComponent<Renderer>().enabled = false;
            var renderer = new GameObject(name).AddComponent<SpriteRenderer>();
            renderer.transform.SetParent(transform);
            renderer.sprite = sprite;
            renderer.transform.localScale = Vector3.one * (height / sprite.bounds.size.y);
            WorldProceduralVisuals.PlaceStandingSprite(renderer, point);
            return renderer;
        }

        private GameObject Box(string name, Vector3 position, Vector3 size, Material material, bool collision = true)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(transform);
            box.transform.position = position;
            box.transform.localScale = size;
            box.GetComponent<Renderer>().sharedMaterial = material;
            if (!collision) Destroy(box.GetComponent<Collider>());
            return box;
        }

        private static Material Material(Color color)
        {
            var material = RuntimeArtCatalog.CreateMaterial("Blockout surface", color);
            Debug.Log("LGO_BLOCKOUT_MATERIAL shader=" + material.shader.name);
            if (material.shader.name != "Universal Render Pipeline/Lit")
                throw new System.InvalidOperationException("Blockout needs the shared lit shader in the Player build.");
            return material;
        }
    }
}
