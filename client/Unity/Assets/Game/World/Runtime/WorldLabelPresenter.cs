using LinhGioi.Art;
using UnityEngine;

namespace LinhGioi.World
{
    internal static class WorldLabelPresenter
    {
        internal static TextMesh Create(string name, string text, Vector3 position, Color color)
        {
            var existing = GameObject.Find(name);
            var holder = existing != null ? existing : new GameObject(name);
            holder.transform.position = position;
            holder.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
            var label = holder.GetComponent<TextMesh>() ?? holder.AddComponent<TextMesh>();
            label.text = text;
            label.fontSize = 42;
            label.characterSize = 0.042f;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = color;
            var renderer = holder.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.sortingOrder = 9;
            EnsureShadow(holder.transform, text);
            return label;
        }

        internal static void Set(TextMesh label, string text, Color color)
        {
            if (label == null) return;
            label.text = text;
            label.color = color;
            EnsureShadow(label.transform, text);
        }

        internal static void EnsureShadow(Transform parent, string text)
        {
            var shadowName = parent.name + " Shadow";
            var existing = parent.Find(shadowName);
            var holder = existing != null ? existing.gameObject : new GameObject(shadowName);
            holder.transform.SetParent(parent, false);
            holder.transform.localPosition = new Vector3(0.025f, -0.025f, 0.01f);
            holder.transform.localRotation = Quaternion.identity;
            holder.transform.localScale = Vector3.one;
            var shadow = holder.GetComponent<TextMesh>() ?? holder.AddComponent<TextMesh>();
            shadow.text = text;
            shadow.fontSize = 42;
            shadow.characterSize = 0.042f;
            shadow.anchor = TextAnchor.MiddleCenter;
            shadow.alignment = TextAlignment.Center;
            shadow.color = new Color(0f, 0f, 0f, 0.72f);
            var renderer = holder.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.sortingOrder = 8;
        }

        internal static void SetActive(TextMesh label, bool active)
        {
            if (label == null) return;
            label.gameObject.SetActive(active);
        }
    }
}
