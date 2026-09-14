using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed class RuntimeTouchMovementPad : VisualElement
    {
        internal const string PadClass = "lgo-touch-movement-pad";
        internal const string NubClass = "lgo-touch-movement-nub";
        private int _pointerId = -1;
        public Vector2 Value { get; private set; }

        public RuntimeTouchMovementPad()
        {
            RegisterCallback<PointerDownEvent>(evt =>
            {
                if (_pointerId != -1 || evt.button != 0) return;
                _pointerId = evt.pointerId;
                this.CapturePointer(_pointerId);
                UpdatePointer(evt.position);
                evt.StopPropagation();
            });
            RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (evt.pointerId != _pointerId) return;
                UpdatePointer(evt.position);
                evt.StopPropagation();
            });
            RegisterCallback<PointerUpEvent>(evt =>
            {
                if (evt.pointerId != _pointerId) return;
                ResetInput();
                evt.StopPropagation();
            });
            RegisterCallback<PointerCancelEvent>(evt => { if (evt.pointerId == _pointerId) ResetInput(); });
            RegisterCallback<PointerCaptureOutEvent>(evt => { if (evt.pointerId == _pointerId) ResetInput(); });
            RegisterCallback<DetachFromPanelEvent>(_ => ResetInput());
            RegisterCallback<GeometryChangedEvent>(_ => ResetInput());
        }

        public static Vector2 NormalizeDisplacement(Vector2 displacement, float radius)
        {
            if (radius <= 0f || float.IsNaN(radius) || float.IsInfinity(radius)) return Vector2.zero;
            var input = Vector2.ClampMagnitude(displacement / radius, 1f);
            var magnitude = input.magnitude;
            const float deadZone = 0.12f;
            return magnitude <= deadZone ? Vector2.zero : input.normalized * ((magnitude - deadZone) / (1f - deadZone));
        }

        internal static void ApplyCircularPresentation(
            RuntimeTouchMovementPad pad,
            VisualElement nub,
            Color padBackground,
            Color nubBackground,
            Color borderColor)
        {
            pad.AddToClassList(PadClass);
            pad.style.alignItems = Align.Center;
            pad.style.justifyContent = Justify.Center;
            pad.style.backgroundColor = padBackground;
            pad.style.borderTopLeftRadius = pad.style.borderTopRightRadius = 999;
            pad.style.borderBottomLeftRadius = pad.style.borderBottomRightRadius = 999;
            pad.style.borderTopWidth = pad.style.borderBottomWidth = 2;
            pad.style.borderLeftWidth = pad.style.borderRightWidth = 2;
            pad.style.borderTopColor = pad.style.borderBottomColor = borderColor;
            pad.style.borderLeftColor = pad.style.borderRightColor = borderColor;

            nub.AddToClassList(NubClass);
            nub.style.position = Position.Relative;
            nub.style.width = nub.style.height = 42;
            nub.style.marginLeft = nub.style.marginRight = 0;
            nub.style.marginTop = nub.style.marginBottom = 0;
            nub.style.backgroundColor = nubBackground;
            nub.style.borderTopLeftRadius = nub.style.borderTopRightRadius = 999;
            nub.style.borderBottomLeftRadius = nub.style.borderBottomRightRadius = 999;
            nub.style.borderTopWidth = nub.style.borderBottomWidth = 2;
            nub.style.borderLeftWidth = nub.style.borderRightWidth = 2;
            nub.style.borderTopColor = nub.style.borderBottomColor = borderColor;
            nub.style.borderLeftColor = nub.style.borderRightColor = borderColor;
        }

        public void ResetInput()
        {
            var pointer = _pointerId;
            _pointerId = -1;
            Value = Vector2.zero;
            var nub = this.Q<VisualElement>("LGO World Touch Movement Nub");
            if (nub != null) nub.style.translate = new Translate(new Length(0), new Length(0));
            if (pointer != -1 && this.HasPointerCapture(pointer)) this.ReleasePointer(pointer);
        }

        private void UpdatePointer(Vector2 panelPosition)
        {
            var nub = this.Q<VisualElement>("LGO World Touch Movement Nub");
            var radius = Mathf.Max(0f, (Mathf.Min(contentRect.width, contentRect.height) - (nub?.resolvedStyle.width ?? 0f)) * 0.5f);
            var displacement = this.WorldToLocal(panelPosition) - contentRect.center;
            var normalized = NormalizeDisplacement(displacement, radius);
            Value = new Vector2(normalized.x, -normalized.y);
            if (nub != null)
                nub.style.translate = new Translate(new Length(normalized.x * radius), new Length(normalized.y * radius));
        }
    }
}
