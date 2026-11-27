using System.Collections;
using UnityEngine;

namespace GameMiners.Gameplay
{
    public sealed partial class GameFlow
    {
        private float impactPauseUntil;

        private IEnumerator AttackArc(Vector3 from, Vector3 forward, float radius, bool heavy)
        {
            var effect = new GameObject("Barrido de cucharón", typeof(LineRenderer));
            effect.transform.SetParent(transform, false);
            var line = effect.GetComponent<LineRenderer>();
            line.sharedMaterial = world.EffectMaterial;
            line.positionCount = 22;
            line.numCapVertices = 4;
            Color bright = heavy ? new Color32(255, 167, 203, 255) : new Color32(190, 247, 255, 255);
            var color = new MaterialPropertyBlock();
            color.SetColor("_Color", bright);
            line.SetPropertyBlock(color);
            for (int i = 0; i < line.positionCount; i++)
            {
                float angle = Mathf.Lerp(-84, 84, i / (line.positionCount - 1f));
                var point = from + Quaternion.AngleAxis(angle, Vector3.up) * forward * radius;
                point.y += Mathf.Lerp(1.2f, 0.66f, i / (line.positionCount - 1f));
                line.SetPosition(i, point);
            }
            for (float time = 0; time < 0.15f; time += Time.deltaTime)
            {
                float left = 1 - time / 0.15f;
                line.widthMultiplier = Mathf.Lerp(0.013f, heavy ? 0.23f : 0.14f, left);
                line.startColor = Color.Lerp(bright, Color.white, left);
                line.endColor = bright;
                yield return null;
            }
            Destroy(effect);
        }

        private IEnumerator DamageNumber(Vector3 at, string value, bool heavy)
        {
            var number = new GameObject("Daño", typeof(TextMesh));
            number.transform.SetParent(transform, false);
            var text = number.GetComponent<TextMesh>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.GetComponent<Renderer>().sharedMaterial = text.font.material;
            text.text = value;
            text.anchor = TextAnchor.MiddleCenter;
            text.fontSize = 72;
            text.fontStyle = FontStyle.Bold;
            float size = value.Length > 4 ? 0.053f : heavy ? 0.119f : 0.095f;
            var outline = new GameObject("Contorno", typeof(TextMesh)).GetComponent<TextMesh>();
            outline.transform.SetParent(number.transform, false);
            outline.transform.localPosition = new Vector3(0.018f, -0.021f, 0.013f);
            outline.font = text.font;
            outline.GetComponent<Renderer>().sharedMaterial = text.font.material;
            outline.text = value;
            outline.fontSize = text.fontSize;
            outline.fontStyle = text.fontStyle;
            outline.anchor = text.anchor;
            var color = heavy ? new Color32(255, 187, 86, 255) : new Color32(255, 251, 224, 255);
            for (float time = 0; time < 0.64f; time += Time.deltaTime)
            {
                float t = time / 0.64f;
                number.transform.SetPositionAndRotation(at + Vector3.up * (0.72f * t), view.transform.rotation);
                float pop = 0.86f + Mathf.Sin(Mathf.Clamp01(t / 0.30f) * Mathf.PI) * 0.43f;
                text.characterSize = outline.characterSize = size * pop;
                outline.color = new Color(0.18f, 0.075f, 0.16f, 1f - t * t);
                text.color = new Color(color.r, color.g, color.b, 1f - t * t);
                yield return null;
            }
            Destroy(number);
        }
    }
}