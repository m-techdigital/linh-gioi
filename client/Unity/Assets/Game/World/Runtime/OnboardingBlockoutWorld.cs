using LinhGioi.Art;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class OnboardingBlockoutWorld : MonoBehaviour
    {
        public static readonly Vector3 KeeperPoint = new Vector3(-3f, 0f, 1f);
        public static readonly Vector3 StonePoint = new Vector3(3.5f, 0f, 4f);
        public Vector2 Movement { get; set; }
        public Vector3 Position => _player.transform.position;
        private CharacterController _player;
        private Camera _camera;
        private Renderer _keeper;
        private Renderer _stone;
        private TextMesh _keeperLabel;
        private TextMesh _stoneLabel;

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

            var player = new GameObject("Blockout player proxy");
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
        }

        private void Update()
        {
            var input = Application.isFocused && !DialogueVisible ? Movement + new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) : Vector2.zero;
            input = Vector2.ClampMagnitude(input, 1f);
            _player.SimpleMove(new Vector3(input.x, 0f, input.y) * 3.6f);
        }

        public bool DialogueVisible { get; set; }

        private void LateUpdate()
        {
            _camera.transform.position = Position + new Vector3(0f, 3.1f, -6.8f);
            _camera.transform.LookAt(Position + new Vector3(0f, 1.1f, 1.3f));
            _keeperLabel.gameObject.SetActive(!DialogueVisible);
            _stoneLabel.gameObject.SetActive(!DialogueVisible);
            WorldLabelPresenter.PlaceAbove(_keeperLabel, _keeper);
            WorldLabelPresenter.PlaceAbove(_stoneLabel, _stone);
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
