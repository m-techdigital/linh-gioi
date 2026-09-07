using LinhGioi.Art;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class OnboardingBlockoutWorld : MonoBehaviour
    {
        public static readonly Vector3 KeeperPoint = new Vector3(-3f, 0f, 1f);
        public static readonly Vector3 StonePoint = new Vector3(3.5f, 0f, 4f);
        public static readonly Vector3 ForecourtPoint = new Vector3(2f, 0f, 22f);
        private Vector3 _guideDestination = StonePoint;
        public Vector2 Movement { get; set; }
        public Vector2 ScreenMovement { get; set; }
        public Vector3 Position => _player.transform.position;
        private CharacterController _player;
        private Animator _characterAnimator;
        private Transform _characterVisual;
        private static readonly int SpeedParameter = Animator.StringToHash("Speed");
        private static readonly int InteractState = Animator.StringToHash("Base Layer.Interact");
        private static readonly int LocomotionState = Animator.StringToHash("Base Layer.Locomotion");
        private Camera _camera;
        private CinemachineBrain _cameraBrain;
        private BoxCollider _cameraVolume;
        private Renderer _keeper;
        private Animator _keeperAnimator;
        private NpcGuideGesture _keeperGuide;
        private bool _keeperGuiding;
        private bool _keeperTalking;
        private bool _keeperWasReady;
        private float _keeperGreetingUntil;
        private int _keeperConversationLayer;
        private static readonly int TalkingState = Animator.StringToHash("Conversation.Talking");
        private static readonly int GuidingState = Animator.StringToHash("Conversation.Guiding");
        private static readonly Quaternion KeeperRestRotation = Quaternion.LookRotation(new Vector3(3f, 0f, -4f));
        private Renderer _stone;
        private TextMesh _keeperLabel;
        private TextMesh _stoneLabel;
        private TextMesh _playerLabel;
        private TextMesh[] _reservedLabels;
        private SpriteRenderer _stoneFocus;
        private Material _stoneSealMaterial;
        private SpriteRenderer _keeperFocus;
        private bool _stoneReady;
        private float _stoneCompletedAt = -1f;
        private bool _stoneFacingActive;
        private const float StoneFeedbackDuration = 1.2f;
        private bool _screenInputHeld;
        private Vector3 _inputForward;
        private Mesh _stoneMesh;
        private Mesh _stoneSealMesh;
        private Mesh _pavingMesh;
        private Texture2D _pavingTexture;
        private Mesh _houseFacadeMesh;
        private Mesh _lanternFrameMesh;
        private Mesh _pavilionBenchMesh;
        private Mesh _pavilionFrameMesh;
        private readonly List<Material> _materials = new List<Material>();
        private Camera[] _previousCameras;
        private Color _previousAmbient;
        private UnityEngine.Rendering.AmbientMode _previousAmbientMode;
        private bool _ownsPresentation;

        private void Awake()
        {
            _previousCameras = Camera.allCameras;
            _previousAmbient = RenderSettings.ambientLight;
            _previousAmbientMode = RenderSettings.ambientMode;
            _ownsPresentation = true;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.75f, 0.76f, 0.78f);
            var light = new GameObject("Blockout daylight").AddComponent<Light>();
            light.transform.SetParent(transform);
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
            light.shadows = LightShadows.Soft;
            var paving = Material(new Color(0.39f, 0.40f, 0.39f));
            var walls = Material(new Color(0.58f, 0.59f, 0.56f));
            var roof = Material(new Color(0.19f, 0.23f, 0.24f));
            var trim = Material(new Color(0.18f, 0.19f, 0.18f));
            Box("Courtyard", new Vector3(0f, -0.1f, 1.5f), new Vector3(14f, 0.2f, 27f), paving).GetComponent<Renderer>().enabled = false;
            for (var side = -1; side <= 1; side += 2)
            {
                for (var z = 0; z <= 12; z += 6)
                    CreateStreetHouse(new Vector3(side * 5.5f, 0f, z), 3.2f, walls, roof, trim);
                Box("Gate post", new Vector3(side * 5.2f, 2.3f, -4f), new Vector3(0.8f, 4.6f, 0.8f), walls);
                Box("Boundary", new Vector3(side * 7f, 0.6f, 1.5f), new Vector3(0.2f, 1.2f, 27f), walls);
            }
            CreateForecourt(paving, walls, roof, trim);
            CreateStreetHouse(new Vector3(-10f, 0f, 42f), 3.2f, walls, roof, trim, scenery: true);
            CreateStreetHouse(new Vector3(-5f, 0f, 40f), 3.8f, walls, roof, trim, scenery: true);
            CreateStreetHouse(new Vector3(1f, 0f, 46f), 4f, walls, roof, trim, scenery: true);
            CreateStreetHouse(new Vector3(7f, 0f, 42f), 3.2f, walls, roof, trim, scenery: true);
            CreateStreetHouse(new Vector3(12f, 0f, 48f), 3.6f, walls, roof, trim, scenery: true);
            CreateStreetLanterns(trim);
            CreateStreetPaving(paving);
            Box("Arrival boundary", new Vector3(0f, 0.4f, -12f), new Vector3(14f, 0.8f, 0.2f), walls);
            Box("Blockout Keeper collider", KeeperPoint + Vector3.up * 0.9f, new Vector3(0.65f, 1.8f, 0.65f), trim)
                .GetComponent<Renderer>().enabled = false;
            _keeperAnimator = CreateHumanoid("LGOGateKeeperCandidate", transform, KeeperPoint + Vector3.up * 0.04f);
            _keeperAnimator.name = "Blockout Keeper";
            _keeperConversationLayer = _keeperAnimator.GetLayerIndex("Conversation");
            if (_keeperConversationLayer < 1) throw new System.InvalidOperationException("Guide requires the masked conversation layer.");
            _keeperGuide = _keeperAnimator.gameObject.AddComponent<NpcGuideGesture>();
            _keeperGuide.Initialize(_keeperConversationLayer);
            _keeperAnimator.transform.rotation = KeeperRestRotation;
            _keeper = _keeperAnimator.GetComponentInChildren<SkinnedMeshRenderer>();
            _stone = CreateTrainingStone();
            _keeperLabel = WorldLabelPresenter.Create("Blockout Keeper Label", "Người Giữ Cổng", KeeperPoint, RuntimeArtCatalog.Gold);
            _stoneLabel = WorldLabelPresenter.Create("Blockout Stone Label", "Đá Luyện", StonePoint, RuntimeArtCatalog.Gold);
            _keeperLabel.transform.SetParent(transform, true);
            _stoneLabel.transform.SetParent(transform, true);
            _reservedLabels = new[] { _keeperLabel, _stoneLabel };
            _stoneFocus = CreateInteractionFocus("Blockout Stone Focus", StonePoint);
            _keeperFocus = CreateInteractionFocus("Blockout Keeper Focus", KeeperPoint);

            var player = new GameObject("Blockout player proxy");
            player.tag = "Player";
            player.transform.SetParent(transform);
            player.transform.position = new Vector3(0f, 0.04f, -3f);
            _player = player.AddComponent<CharacterController>();
            _player.height = 1.8f;
            _player.radius = 0.3f;
            _player.center = Vector3.up * 0.9f;
            _player.stepOffset = 0.2f;
            _characterAnimator = CreateHumanoid("LGOArrivalOutfitCandidate", player.transform, Vector3.up * 0.025f);
            _characterVisual = _characterAnimator.transform;
            _playerLabel = WorldLabelPresenter.Create("Blockout Player Label", string.Empty, player.transform.position, RuntimeArtCatalog.Text);
            _playerLabel.transform.SetParent(transform, true);
            WorldLabelPresenter.SetActive(_playerLabel, false);
            foreach (var camera in _previousCameras) camera.enabled = false;
            _camera = new GameObject("Blockout perspective camera").AddComponent<Camera>();
            _camera.transform.SetParent(transform);
            _camera.tag = "MainCamera";
            _camera.fieldOfView = 55f;
            _camera.nearClipPlane = 0.1f;
            _camera.farClipPlane = 80f;
            _camera.clearFlags = CameraClearFlags.Skybox;
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
            _cameraVolume.center = new Vector3(0f, 4.1f, 7.5f);
            _cameraVolume.size = new Vector3(13.4f, 7.8f, 38.4f);
            _cameraVolume.isTrigger = true;
            var clearShot = shots.AddComponent<CinemachineClearShot>();
            clearShot.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.Cut, 0f);
            CreateCameraShot(shots.transform, tracking, new Vector3(0f, 2f, -6.8f), 20);
            CreateCameraShot(shots.transform, tracking, new Vector3(-6.8f, 2f, 0f), 10);
            CreateCameraShot(shots.transform, tracking, new Vector3(6.8f, 2f, 0f), 10);
        }

        private void CreateStreetPaving(Material paving)
        {
            _pavingTexture = WorldProceduralVisuals.CreateStreetPavingTexture();
            paving.color = Color.white;
            paving.mainTexture = _pavingTexture;
            var corners = new[]
            {
                new Vector3(-7f, 0.002f, -12f), new Vector3(7f, 0.002f, -12f),
                new Vector3(-7f, 0.002f, 27f), new Vector3(7f, 0.002f, 27f)
            };
            var uv = new Vector2[corners.Length];
            for (var i = 0; i < corners.Length; i++)
                uv[i] = new Vector2(corners[i].x / 3f, corners[i].z / 2f);
            _pavingMesh = new Mesh { name = "Continuous street paving", vertices = corners, uv = uv,
                triangles = new[] { 0, 2, 1, 1, 2, 3 } };
            _pavingMesh.RecalculateNormals();
            _pavingMesh.RecalculateBounds();
            var surface = new GameObject("Street paving surface");
            surface.transform.SetParent(transform);
            surface.AddComponent<MeshFilter>().sharedMesh = _pavingMesh;
            surface.AddComponent<MeshRenderer>().sharedMaterial = paving;
        }

        private void CreateStreetLanterns(Material timber)
        {
            var paper = Material(new Color(0.86f, 0.68f, 0.35f));
            for (var side = -1; side <= 1; side += 2)
            for (var row = 0; row < 3; row++)
            {
                var position = new Vector3(side * 3.65f, 2.25f, -1.3f + row * 6f);
                var shade = Box("Street lantern paper", position, new Vector3(0.32f, 0.5f, 0.32f), paper, false);
                if (_lanternFrameMesh == null)
                {
                    var cube = shade.GetComponent<MeshFilter>().sharedMesh;
                    var parts = new List<CombineInstance>();
                    void Beam(Vector3 centre, Vector3 size)
                    {
                        parts.Add(new CombineInstance { mesh = cube, transform = Matrix4x4.TRS(centre, Quaternion.identity, size) });
                    }
                    for (var x = -1; x <= 1; x += 2)
                    for (var z = -1; z <= 1; z += 2)
                        Beam(new Vector3(x * 0.16f, 0f, z * 0.16f), new Vector3(0.035f, 0.54f, 0.035f));
                    for (var end = -1; end <= 1; end += 2)
                        Beam(Vector3.up * (end * 0.27f), new Vector3(0.37f, 0.04f, 0.37f));
                    Beam(new Vector3(-0.18f, 0.58f, 0f), new Vector3(0.4f, 0.05f, 0.05f));
                    Beam(new Vector3(0f, 0.41f, 0f), new Vector3(0.025f, 0.3f, 0.025f));
                    Beam(new Vector3(-0.36f, 0.52f, 0f), new Vector3(0.04f, 0.22f, 0.11f));
                    _lanternFrameMesh = new Mesh { name = "Shared street lantern frame" };
                    _lanternFrameMesh.CombineMeshes(parts.ToArray(), true, true);
                }
                var frame = new GameObject("Street lantern frame");
                frame.transform.SetParent(transform);
                frame.transform.position = position;
                frame.transform.rotation = Quaternion.Euler(0f, side < 0 ? 0f : 180f, 0f);
                frame.AddComponent<MeshFilter>().sharedMesh = _lanternFrameMesh;
                frame.AddComponent<MeshRenderer>().sharedMaterial = timber;
            }
        }

        private void CreateStreetHouse(Vector3 centre, float height, Material walls, Material roof, Material trim, bool scenery = false)
        {
            var house = new GameObject(scenery ? "Street scenery house" : "Street house").transform;
            house.SetParent(transform);
            house.position = centre;
            var body = Box("Street module", centre + Vector3.up * height * 0.5f,
                new Vector3(3f, height, 4.8f), walls, !scenery, house);
            CreateRoof(centre + Vector3.up * (height + 0.1f), roof, !scenery, house);
            Box("Stone plinth", centre + Vector3.up * 0.15f, new Vector3(3.04f, 0.3f, 4.84f), roof, false, house);
            Box("Timber eave band", centre + Vector3.up * (height - 0.08f), new Vector3(3.05f, 0.16f, 4.85f), trim, false, house);
            if (_houseFacadeMesh == null)
            {
                var cube = body.GetComponent<MeshFilter>().sharedMesh;
                var parts = new System.Collections.Generic.List<CombineInstance>();
                void Beam(Vector3 position, Vector3 size)
                {
                    parts.Add(new CombineInstance { mesh = cube, transform = Matrix4x4.TRS(position, Quaternion.identity, size) });
                }
                Beam(new Vector3(0.02f, 1f, 0f), new Vector3(0.04f, 2f, 1.1f));
                for (var side = -1; side <= 1; side += 2)
                    Beam(new Vector3(0.055f, 1.05f, side * 0.6f), new Vector3(0.11f, 2.1f, 0.1f));
                Beam(new Vector3(0.055f, 2.05f, 0f), new Vector3(0.11f, 0.1f, 1.3f));
                Beam(new Vector3(0.065f, 1f, 0f), new Vector3(0.09f, 1.95f, 0.04f));
                for (var rail = 0; rail < 2; rail++)
                    Beam(new Vector3(0.065f, 0.65f + rail * 0.7f, 0f), new Vector3(0.09f, 0.04f, 1.1f));
                for (var side = -1; side <= 1; side += 2)
                {
                    var windowZ = side * 1.65f;
                    for (var edge = -1; edge <= 1; edge += 2)
                    {
                        Beam(new Vector3(0.045f, 1.5f + edge * 0.4f, windowZ), new Vector3(0.09f, 0.07f, 0.8f));
                        Beam(new Vector3(0.045f, 1.5f, windowZ + edge * 0.365f), new Vector3(0.09f, 0.8f, 0.07f));
                        Beam(new Vector3(0.035f, 1.5f, windowZ + edge * 0.12f), new Vector3(0.07f, 0.8f, 0.025f));
                    }
                    Beam(new Vector3(0.035f, 1.5f, windowZ), new Vector3(0.07f, 0.025f, 0.8f));
                }
                _houseFacadeMesh = new Mesh { name = "Shared street house facade" };
                _houseFacadeMesh.CombineMeshes(parts.ToArray(), true, true);
            }
            var facade = new GameObject("House facade");
            facade.transform.SetParent(house);
            facade.transform.position = centre + Vector3.right * (centre.x < 0f ? 1.505f : -1.505f);
            facade.transform.rotation = Quaternion.Euler(0f, centre.x < 0f ? 0f : 180f, 0f);
            facade.AddComponent<MeshFilter>().sharedMesh = _houseFacadeMesh;
            facade.AddComponent<MeshRenderer>().sharedMaterial = trim;
            if (scenery) house.rotation = Quaternion.Euler(0f, centre.x < 0f ? 90f : -90f, 0f);
        }

        private void CreateRoof(Vector3 eave, Material roof, bool collision = true, Transform parent = null)
        {
            Box("Roof eave", eave, new Vector3(3.6f, 0.25f, 5.4f), roof, collision, parent);
            for (var slope = -1; slope <= 1; slope += 2)
            {
                var panel = Box("Roof slope", eave + new Vector3(slope * 0.8f, 0.5f, 0f),
                    new Vector3(1.9f, 0.16f, 5.2f), roof, collision, parent);
                panel.transform.rotation = Quaternion.Euler(0f, 0f, -slope * 25f);
            }
        }

        private void CreateForecourt(Material paving, Material walls, Material roof, Material trim)
        {
            // Same-height local extension: no map transition or invisible gate across the street.
            Box("Forecourt paving", new Vector3(0f, -0.1f, 21f), new Vector3(14f, 0.2f, 12f), paving).GetComponent<Renderer>().enabled = false;
            for (var side = -1; side <= 1; side += 2)
                Box("Forecourt garden wall", new Vector3(side * 7f, 0.6f, 21f), new Vector3(0.2f, 1.2f, 12f), walls);
            Box("Garden rear wall", new Vector3(0f, 0.9f, 27f), new Vector3(14f, 1.8f, 0.2f), walls);
            CreateStreetHouse(new Vector3(-5f, 0f, 20f), 2.6f, walls, roof, trim);
            CreateRoof(new Vector3(5f, 2.7f, 23f), roof);
            var timber = Material(new Color(0.38f, 0.25f, 0.15f));
            CreatePavilionFrame(timber);
            CreatePavilionBench(timber);
            Box("Garden bed", new Vector3(-1f, 0.15f, 25.5f), new Vector3(5f, 0.3f, 2.6f), walls);
            Box("Garden soil", new Vector3(-1f, 0.305f, 25.5f), new Vector3(4.7f, 0.01f, 2.3f),
                Material(new Color(0.19f, 0.22f, 0.14f)), false);
            Actor("Forecourt pine", new Vector3(-1f, 0.3f, 25.5f), LgoVisualAssetRegistryV3B.TreePine, 4.6f, trim);
            var treeShadow = WorldProceduralVisuals.CreateGroundShadowSprite("Forecourt pine grounding",
                new Vector3(-1f, 0.315f, 25.5f), new Vector3(0.8f, 0.5f, 1f), 2);
            if (treeShadow != null) treeShadow.transform.SetParent(transform);
        }

        private void CreatePavilionFrame(Material timber)
        {
            // Open timber pavilion from the street/forecourt draft. Keep the four
            // post colliders; crossbeams and knee braces sit above walking clearance.
            var parts = new List<CombineInstance>();
            Mesh cube = null;
            void Beam(Vector3 centre, Vector3 size, Quaternion rotation)
            {
                parts.Add(new CombineInstance { mesh = cube, transform = Matrix4x4.TRS(centre, rotation, size) });
            }
            for (var x = -1; x <= 1; x += 2)
            for (var z = -1; z <= 1; z += 2)
            {
                var centre = new Vector3(x * 1.3f, 1.35f, z * 2f);
                var size = new Vector3(0.18f, 2.7f, 0.18f);
                var post = Box("Pavilion post", new Vector3(5f, 0f, 23f) + centre, size, timber);
                cube = post.GetComponent<MeshFilter>().sharedMesh;
                post.GetComponent<Renderer>().enabled = false;
                Beam(centre, size, Quaternion.identity);
                Beam(new Vector3(x * 1.1f, 2.25f, z * 2f), new Vector3(0.57f, 0.12f, 0.12f),
                    Quaternion.Euler(0f, 0f, -x * 45f));
                Beam(new Vector3(x * 1.3f, 2.25f, z * 1.8f), new Vector3(0.12f, 0.12f, 0.57f),
                    Quaternion.Euler(z * 45f, 0f, 0f));
            }
            for (var side = -1; side <= 1; side += 2)
            {
                Beam(new Vector3(0f, 2.5f, side * 2f), new Vector3(2.78f, 0.18f, 0.18f), Quaternion.identity);
                Beam(new Vector3(side * 1.3f, 2.5f, 0f), new Vector3(0.18f, 0.18f, 4.18f), Quaternion.identity);
            }
            _pavilionFrameMesh = new Mesh { name = "Forecourt timber frame" };
            _pavilionFrameMesh.CombineMeshes(parts.ToArray(), true, true);
            var frame = new GameObject("Pavilion timber frame");
            frame.transform.SetParent(transform);
            frame.transform.position = new Vector3(5f, 0f, 23f);
            frame.AddComponent<MeshFilter>().sharedMesh = _pavilionFrameMesh;
            frame.AddComponent<MeshRenderer>().sharedMaterial = timber;
        }

        private void CreatePavilionBench(Material timber)
        {
            // Forecourt draft: an open resting pavilion after SCN-002. The bench
            // faces the garden; all timber shares one renderer and the old footprint.
            var bench = Box("Pavilion bench", new Vector3(6f, 0f, 23f), Vector3.one, timber);
            var collider = bench.GetComponent<BoxCollider>();
            collider.center = new Vector3(0f, 0.44f, 0f);
            collider.size = new Vector3(0.45f, 0.88f, 3.6f);
            var cube = bench.GetComponent<MeshFilter>().sharedMesh;
            var parts = new List<CombineInstance>();
            void Timber(Vector3 centre, Vector3 size)
            {
                parts.Add(new CombineInstance { mesh = cube, transform = Matrix4x4.TRS(centre, Quaternion.identity, size) });
            }
            for (var slat = -1; slat <= 1; slat++)
                Timber(new Vector3(slat * 0.1575f, 0.43f, 0f), new Vector3(0.135f, 0.08f, 3.6f));
            for (var end = -1; end <= 1; end += 2)
            {
                for (var side = -1; side <= 1; side += 2)
                    Timber(new Vector3(side * 0.14f, 0.195f, end * 1.5f), new Vector3(0.1f, 0.39f, 0.14f));
                Timber(new Vector3(0f, 0.345f, end * 1.5f), new Vector3(0.45f, 0.09f, 0.14f));
                Timber(new Vector3(0.185f, 0.66f, end * 1.5f), new Vector3(0.08f, 0.44f, 0.1f));
            }
            Timber(new Vector3(0.19f, 0.78f, 0f), new Vector3(0.07f, 0.16f, 3.6f));
            _pavilionBenchMesh = new Mesh { name = "Forecourt timber bench" };
            _pavilionBenchMesh.CombineMeshes(parts.ToArray(), true, true);
            bench.GetComponent<MeshFilter>().sharedMesh = _pavilionBenchMesh;
        }

        private void CreateCameraShot(Transform parent, Transform tracking, Vector3 offset, int priority)
        {
            var rig = new GameObject("Blockout camera shot " + offset);
            rig.transform.SetParent(parent);
            var virtualCamera = rig.AddComponent<CinemachineCamera>();
            // ClearShot must compare current obstacle scores, including during slow startup frames.
            virtualCamera.StandbyUpdate = CinemachineVirtualCameraBase.StandbyUpdateMode.Always;
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
            var velocity = Vector3.ProjectOnPlane(_player.velocity, Vector3.up);
            _characterAnimator.SetFloat(SpeedParameter, velocity.magnitude, 0.1f, Time.deltaTime);
            if (direction.sqrMagnitude > 0.0001f)
            {
                // A requested crossfade may not appear in Animator state info until evaluation.
                if (_stoneFacingActive)
                    _characterAnimator.CrossFadeInFixedTime(LocomotionState, 0.1f, 0);
                _stoneFacingActive = false;
            }
            var facing = DialogueVisible
                ? Vector3.ProjectOnPlane(KeeperPoint - _player.transform.position, Vector3.up)
                : velocity;
            if (!DialogueVisible && _stoneFacingActive && Time.time - _stoneCompletedAt < StoneFeedbackDuration)
                facing = Vector3.ProjectOnPlane(StonePoint - _player.transform.position, Vector3.up);
            if (facing.sqrMagnitude > 0.0025f)
                _characterVisual.rotation = Quaternion.RotateTowards(_characterVisual.rotation,
                    Quaternion.LookRotation(facing, Vector3.up), 720f * Time.deltaTime);
        }

        public void ResetMovementInput()
        {
            Movement = Vector2.zero;
            ScreenMovement = Vector2.zero;
            _screenInputHeld = false;
        }

        public void LogCameraShots()
        {
            Debug.Log("LGO_ARRIVAL_CAMERA frame=" + Time.frameCount + " forward=" + _camera.transform.forward + " focused=" + Application.isFocused);
            foreach (var shot in GetComponentsInChildren<CinemachineCamera>())
                Debug.Log("LGO_ARRIVAL_SHOT priority=" + shot.Priority.Value + " quality=" + shot.State.ShotQuality.ToString("R")
                    + " position=" + shot.transform.position);
        }

        private bool _dialogueVisible;
        public bool DialogueVisible
        {
            get => _dialogueVisible;
            set
            {
                _dialogueVisible = value;
                if (!value) return;
                // UI callbacks can run after LateUpdate (including after capture).
                // Cancel immediately so opening a modal cannot retain a pointing pose.
                _keeperGreetingUntil = 0f;
                _keeperGuide?.Cancel();
            }
        }
        public void GuideToStone() => GuideTo(StonePoint);
        public void GuideToForecourt() => GuideTo(ForecourtPoint);
        private void GuideTo(Vector3 destination)
        {
            _guideDestination = destination;
            _keeperGuide.Begin(destination);
        }
        public bool KeeperGuiding => _keeperGuide != null && _keeperGuide.Active;
        public bool KeeperGreeting => KeeperReady && !DialogueVisible && Time.time < _keeperGreetingUntil;
        public bool KeeperReady { get; set; }

        private SpriteRenderer CreateInteractionFocus(string name, Vector3 point)
        {
            var focus = WorldProceduralVisuals.CreateGroundGlowSprite(name,
                WorldProceduralVisuals.GetWorldPlatformGlowSprite(), point + Vector3.up * 0.025f,
                Vector3.one * 1.6f, RuntimeArtCatalog.Gold, 4);
            focus.transform.SetParent(transform);
            focus.gameObject.SetActive(false);
            return focus;
        }

        public void SetStoneFeedback(bool ready, bool completed)
        {
            _stoneReady = ready;
            if (completed && _stoneCompletedAt < 0f)
            {
                _stoneCompletedAt = Time.time;
                _stoneFacingActive = true;
                _characterAnimator.CrossFadeInFixedTime(InteractState, 0.1f, 0, 0f);
                WorldLabelPresenter.Set(_stoneLabel, "Đá Luyện\nĐã ổn định", RuntimeArtCatalog.Gold);
            }
        }

        public void SetPlayerName(string name) => WorldLabelPresenter.Set(_playerLabel, name, RuntimeArtCatalog.Text);

        private void LateUpdate()
        {
            _cameraBrain.ManualUpdate();
            if (KeeperReady && !_keeperWasReady) _keeperGreetingUntil = Time.time + 2.5f;
            _keeperWasReady = KeeperReady;
            if (DialogueVisible)
            {
                _keeperGreetingUntil = 0f;
                _keeperGuide.Cancel();
            }
            var guiding = _keeperGuide.Active;
            if (_keeperGuiding != guiding)
            {
                _keeperGuiding = guiding;
                _keeperAnimator.Play(guiding ? GuidingState : TalkingState, _keeperConversationLayer, 0f);
            }
            var conversing = DialogueVisible || KeeperGreeting;
            if (_keeperTalking != conversing)
            {
                _keeperTalking = conversing;
                if (_keeperTalking && !guiding) _keeperAnimator.Play(TalkingState, _keeperConversationLayer, 0f);
            }
            _keeperAnimator.SetLayerWeight(_keeperConversationLayer, Mathf.MoveTowards(
                _keeperAnimator.GetLayerWeight(_keeperConversationLayer), _keeperTalking || guiding ? 1f : 0f, Time.deltaTime / 0.15f));
            var keeperFacing = guiding ? Quaternion.LookRotation(Vector3.ProjectOnPlane(_guideDestination - KeeperPoint, Vector3.up)) : _keeperTalking
                ? Quaternion.LookRotation(Vector3.ProjectOnPlane(_player.transform.position - KeeperPoint, Vector3.up))
                : KeeperRestRotation;
            _keeperAnimator.transform.rotation = Quaternion.RotateTowards(_keeperAnimator.transform.rotation, keeperFacing, 180f * Time.deltaTime);
            _keeperFocus.gameObject.SetActive(!DialogueVisible && KeeperReady);
            _keeperLabel.gameObject.SetActive(!DialogueVisible);
            _stoneLabel.gameObject.SetActive(!DialogueVisible);
            WorldLabelPresenter.PlaceAbove(_keeperLabel, _keeper);
            WorldLabelPresenter.PlaceAbove(_stoneLabel, _stone);
            WorldLabelPresenter.SetActive(_playerLabel, !DialogueVisible && !string.IsNullOrEmpty(_playerLabel.text));
            WorldLabelPresenter.PlaceAbove(_playerLabel, _player.bounds, _camera, _reservedLabels);
            var completion = _stoneCompletedAt >= 0f;
            var progress = completion ? Mathf.Clamp01((Time.time - _stoneCompletedAt) / StoneFeedbackDuration) : 0f;
            _stoneFocus.gameObject.SetActive(!DialogueVisible && (completion ? progress < 1f : _stoneReady));
            _stoneFocus.transform.localScale = Vector3.one * (1.6f + progress * 0.8f);
            var flash = completion ? Mathf.Sin(progress * Mathf.PI) : 0f;
            _stoneSealMaterial.color = Color.Lerp(RuntimeArtCatalog.Gold, RuntimeArtCatalog.Text, flash);
            var color = Color.Lerp(RuntimeArtCatalog.Gold, RuntimeArtCatalog.Text, flash * 0.5f);
            color.a = completion ? 1f - progress : 1f;
            _stoneFocus.color = color;
        }

        private Renderer CreateTrainingStone()
        {
            var surface = Material(new Color(0.40f, 0.49f, 0.51f));
            var stone = Box("Blockout Stone", StonePoint + Vector3.up * 0.75f, new Vector3(0.65f, 1.5f, 0.65f), surface);
            stone.transform.position = StonePoint;
            stone.transform.localScale = Vector3.one;
            var collider = stone.GetComponent<BoxCollider>();
            collider.center = Vector3.up * 0.75f;
            collider.size = new Vector3(0.65f, 1.5f, 0.65f);
            // Five octagonal rings give the low stone a broad shoulder and rounded crown.
            var rings = new[]
            {
                new Vector3(0.325f, 0f, 0.30f), new Vector3(0.31f, 0.12f, 0.29f),
                new Vector3(0.325f, 0.45f, 0.30f), new Vector3(0.285f, 0.85f, 0.255f),
                new Vector3(0.20f, 1.04f, 0.19f)
            };
            var points = new Vector3[rings.Length * 8];
            for (var ring = 0; ring < rings.Length; ring++)
            for (var side = 0; side < 8; side++)
            {
                var angle = (side + 0.5f) * Mathf.PI * 0.25f;
                points[ring * 8 + side] = new Vector3(Mathf.Cos(angle) * rings[ring].x,
                    rings[ring].y, Mathf.Sin(angle) * rings[ring].z);
            }
            var vertices = new System.Collections.Generic.List<Vector3>(240);
            void Face(Vector3 a, Vector3 b, Vector3 c)
            {
                vertices.Add(a); vertices.Add(b); vertices.Add(c);
            }
            for (var side = 0; side < 8; side++)
            {
                var next = (side + 1) % 8;
                Face(Vector3.zero, points[side], points[next]);
                for (var ring = 0; ring < rings.Length - 1; ring++)
                {
                    var lower = ring * 8;
                    var upper = lower + 8;
                    Face(points[lower + side], points[upper + side], points[upper + next]);
                    Face(points[lower + side], points[upper + next], points[lower + next]);
                }
                Face(points[32 + side], new Vector3(0.04f, 1.1f, -0.01f), points[32 + next]);
            }
            var indices = new int[vertices.Count];
            for (var i = 0; i < indices.Length; i++) indices[i] = i;
            _stoneMesh = new Mesh { name = "Blockout stone volume" };
            _stoneMesh.SetVertices(vertices);
            _stoneMesh.triangles = indices;
            _stoneMesh.RecalculateNormals();
            _stoneMesh.RecalculateBounds();
            stone.GetComponent<MeshFilter>().sharedMesh = _stoneMesh;
            _stoneSealMaterial = Material(RuntimeArtCatalog.Gold);
            _stoneSealMesh = CreateStoneSealMesh();
            void Seal(string name, bool roadFace)
            {
                const float height = 0.62f;
                var lower = rings[2];
                var upper = rings[3];
                var face = Vector3.Lerp(lower, upper, (height - lower.y) / (upper.y - lower.y));
                var octagonFace = Mathf.Cos(Mathf.PI * 0.125f);
                var radius = (roadFace ? face.x : face.z) * octagonFace;
                var slope = (roadFace ? lower.x - upper.x : lower.z - upper.z) * octagonFace / (upper.y - lower.y);
                var normal = (roadFace ? new Vector3(-1f, slope, 0f) : new Vector3(0f, slope, -1f)).normalized;
                var center = roadFace ? new Vector3(-radius, height, 0f) : new Vector3(0f, height, -radius);
                var seal = Box(name, StonePoint + center + normal * 0.009f,
                    Vector3.one, _stoneSealMaterial, false);
                seal.GetComponent<MeshFilter>().sharedMesh = _stoneSealMesh;
                seal.transform.rotation = Quaternion.LookRotation(normal);
            }
            Seal("Blockout stone seal front", false);
            Seal("Blockout stone seal side", true);
            return stone.GetComponent<Renderer>();
        }

        private static Mesh CreateStoneSealMesh()
        {
            // SCN-002 reference: a small resonance spiral, fitted to the low stone.
            // One ribbon mesh is shared by both faces; the existing material owns its pulse.
            const int segments = 64;
            var vertices = new Vector3[(segments + 1) * 2];
            var indices = new int[segments * 6];
            for (var i = 0; i <= segments; i++)
            {
                var t = i / (float)segments;
                var angle = t * Mathf.PI * 2.8f - Mathf.PI * 0.5f;
                var radius = Mathf.Lerp(0.155f, 0.015f, t);
                var halfWidth = Mathf.Lerp(0.018f, 0.012f, t);
                var radial = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                vertices[i * 2] = radial * (radius - halfWidth);
                vertices[i * 2 + 1] = radial * (radius + halfWidth);
                if (i == segments) continue;
                var vertex = i * 2;
                var index = i * 6;
                indices[index] = vertex;
                indices[index + 1] = vertex + 1;
                indices[index + 2] = vertex + 3;
                indices[index + 3] = vertex;
                indices[index + 4] = vertex + 3;
                indices[index + 5] = vertex + 2;
            }
            // The winding spiral is asymmetric; center its bounds before fitting it
            // inside the narrower octagonal side face (rather than around its origin).
            var bounds = new Bounds(vertices[0], Vector3.zero);
            foreach (var vertex in vertices) bounds.Encapsulate(vertex);
            var fit = 0.20f / bounds.size.x;
            for (var i = 0; i < vertices.Length; i++) vertices[i] = (vertices[i] - bounds.center) * fit;
            var mesh = new Mesh { name = "Training stone resonance inlay" };
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private void OnDisable()
        {
            if (!_ownsPresentation) return;
            _ownsPresentation = false;
            foreach (var camera in _previousCameras)
                if (camera != null) camera.enabled = true;
            RenderSettings.ambientMode = _previousAmbientMode;
            RenderSettings.ambientLight = _previousAmbient;
        }

        private void OnDestroy()
        {
            if (_stoneMesh != null) Destroy(_stoneMesh);
            if (_stoneSealMesh != null) Destroy(_stoneSealMesh);
            if (_pavingMesh != null) Destroy(_pavingMesh);
            if (_pavingTexture != null) Destroy(_pavingTexture);
            foreach (var material in _materials)
                if (material != null) Destroy(material);
            if (_houseFacadeMesh != null) Destroy(_houseFacadeMesh);
            if (_lanternFrameMesh != null) Destroy(_lanternFrameMesh);
            if (_pavilionBenchMesh != null) Destroy(_pavilionBenchMesh);
            if (_pavilionFrameMesh != null) Destroy(_pavilionFrameMesh);
        }

        private Animator CreateHumanoid(string resource, Transform parent, Vector3 localPosition)
        {
            var prefab = Resources.Load<GameObject>(resource);
            if (prefab == null) throw new System.InvalidOperationException("Humanoid candidate prefab missing: " + resource);
            var instance = Instantiate(prefab, parent, false);
            instance.transform.localPosition = localPosition;
            var animator = instance.GetComponent<Animator>();
            if (animator == null || !animator.isHuman || animator.runtimeAnimatorController == null)
                throw new System.InvalidOperationException("Candidate has no Humanoid locomotion: " + resource);
            animator.applyRootMotion = false;
            return animator;
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
            WorldProceduralVisuals.PlaceStandingSprite(renderer, point, point.y);
            return renderer;
        }

        private GameObject Box(string name, Vector3 position, Vector3 size, Material material, bool collision = true, Transform parent = null)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(parent != null ? parent : transform);
            box.transform.position = position;
            box.transform.localScale = size;
            box.GetComponent<Renderer>().sharedMaterial = material;
            if (!collision) Destroy(box.GetComponent<Collider>());
            return box;
        }

        private Material Material(Color color)
        {
            var material = RuntimeArtCatalog.CreateMaterial("Blockout surface", color);
            _materials.Add(material);
            Debug.Log("LGO_BLOCKOUT_MATERIAL shader=" + material.shader.name);
            if (material.shader.name != "Universal Render Pipeline/Lit")
                throw new System.InvalidOperationException("Blockout needs the shared lit shader in the Player build.");
            return material;
        }
    }
}
