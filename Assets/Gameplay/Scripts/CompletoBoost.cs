using System.Collections.Generic;
using UnityEngine;

namespace GameMiners.Gameplay
{
    public sealed partial class GameFlow
    {
        private const float CompletoDuration = 13f, CompletoMove = 1.35f, CompletoAttack = 1.4f;
        private readonly List<Snack> snacks = new();
        private float completoExpires;
        private float AttackRate => combatTime < completoExpires ? CompletoAttack : 1f;
        private float RunningRate => combatTime < completoExpires ? CompletoMove : 1f;

        private void TickCompleto(float now)
        {
            for (int i = snacks.Count - 1; i >= 0; i--)
            {
                var snack = snacks[i];
                float age = now - snack.Born;
                float t = Mathf.Clamp01(age / 0.48f);
                snack.Visual.position = Vector3.Lerp(snack.From, snack.At, t) + Vector3.up *
                    (Mathf.Sin(t * Mathf.PI) * 0.72f + Mathf.Sin(age * 5f) * 0.07f);
                snack.Visual.Rotate(0f, 105f * Time.deltaTime, 0f);
                if (age < 0.22f || Distance(snack.Visual.position, player.transform.position) > 1.32f) continue;
                completoExpires = Mathf.Max(completoExpires, now + CompletoDuration);
                nextAttack = Mathf.Min(nextAttack, now + 0.08f);
                world.CreamSpray(player.transform.position + Vector3.up, Vector3.up, CandyGold, 22, 4.5f, false);
                audio.Play(GameAudio.Cue.Clear, 1.14f);
                Destroy(snack.Visual.gameObject);
                snacks.RemoveAt(i);
            }
            ui.Boost(Mathf.Max(0, completoExpires - now));
        }

        private void SpawnCompleto(Transform chest)
        {
            var from = chest.position + Vector3.up * 0.65f;
            var target = FloorPoint(chest.position + Vector3.forward * 0.82f, 0.37f);
            snacks.Add(new Snack(world.CompletoInWorld(from), from, target, combatTime));
        }

        private sealed class Snack
        {
            public readonly Transform Visual;
            public readonly Vector3 From, At;
            public readonly float Born;
            public Snack(Transform visual, Vector3 from, Vector3 at, float born) =>
                (Visual, From, At, Born) = (visual, from, at, born);
        }
    }

    internal sealed partial class BlockoutWorld
    {
        public Transform CompletoInWorld(Vector3 position)
        {
            var snack = Group("Completo italiano de cofre", root, position);
            Ellipsoid("Pan dorado", snack, new Vector3(0, -0.032f, 0), new Vector3(0.71f, 0.23f, 0.33f), Vanilla);
            Ellipsoid("Vienesa", snack, new Vector3(0, 0.062f, 0), new Vector3(0.74f, 0.126f, 0.17f), Cocoa);
            for (int i = 0; i < 5; i++)
            {
                float x = (i - 2) * 0.108f;
                Ellipsoid("Tomate", snack, new Vector3(x, 0.122f, 0.044f), new Vector3(0.15f, 0.082f, 0.13f), Jam);
                Ellipsoid("Palta", snack, new Vector3(x, 0.131f, -0.035f), new Vector3(0.15f, 0.10f, 0.13f), Pistachio);
                Ellipsoid("Mayonesa", snack, new Vector3(x + 0.05f, 0.177f, -0.013f), new Vector3(0.11f, 0.048f, 0.083f), Frosting);
            }
            return snack;
        }
    }
}