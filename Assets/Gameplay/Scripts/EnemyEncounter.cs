using UnityEngine;

namespace GameMiners.Gameplay
{
    internal sealed class EnemyEncounter
    {
        private static readonly Color Warning = new Color32(247, 79, 123, 255);
        private static readonly Color Vulnerable = new Color32(255, 215, 114, 255);
        private readonly Renderer body;
        private readonly Transform attackRing, shield;
        private readonly bool yeti, trail;
        private readonly bool twoScoops;
        private readonly CharacterMotion motion;
        private readonly CreamSurface skin;
        private readonly YetiScars scars;
        private readonly ImportedYetiDamage importedDamage;
        private bool slamStrike;
        private Vector3 spikePoint;
        private float spikesAt;
        private Vector3 recoil;
        private bool alerted;
        private readonly float speed;
        private float strikeAt, recoveredAt, flashUntil, lastMove, guardUntil;
        private int swingNumber;

        public EnemyEncounter(BlockoutWorld world, string name, Vector3 spawn, float size, Color color,
            int life, int damage, float speed, bool boss, bool yeti, GameObject model, RuntimeAnimatorController controller,
            bool trail, bool guard)
        {
            Name = name;
            Health = MaxHealth = life;
            Damage = damage;
            IsBoss = boss;
            Radius = size * 0.52f;
            this.speed = speed;
            this.yeti = yeti;
            this.trail = trail;
            Cream = color;
            Root = world.Enemy(name, spawn, size, color, yeti, out body, model);
            twoScoops = Root.Find("IceMedium") != null;
            if (!yeti)
            {
                skin = new CreamSurface(Root);
                if (controller != null || Root.GetComponentInChildren<PortableRig>() != null)
                    motion = new CharacterMotion(Root, controller);
                skin.Set(Cream);
            }
            else
            {
                var modelSkin = Root.GetComponentInChildren<SkinnedMeshRenderer>();
                if (modelSkin != null && CharacterMotion.Find(Root, "UpperArm.L") != null)
                {
                    importedDamage = new ImportedYetiDamage(Root, modelSkin, world);
                    motion = new CharacterMotion(Root, null);
                }
                else scars = new YetiScars(Root, world);
            }
            if (guard) shield = world.WaferGuard(Root, size);
            if (boss) attackRing = world.AlertRing(spawn, Reach);
        }

        public string Name { get; }
        public Transform Root { get; }
        public int Health { get; private set; }
        public int MaxHealth { get; }
        public int Damage { get; }
        public bool IsBoss { get; }
        public float Radius { get; }
        public Color Cream { get; }
        public int ArmsRemaining => importedDamage?.ArmsRemaining ?? scars?.ArmsRemaining ?? 0;
        public int OpenHoles => importedDamage?.Holes ?? scars?.OpenHoles ?? 0;

        private bool Stomp => strikeAt > 0f ? slamStrike :
            yeti && (ArmsRemaining == 0 || swingNumber % (ArmsRemaining == 1 ? 2 : 3) == 2 % (ArmsRemaining == 1 ? 2 : 3));
        private float Reach => Radius + (Stomp ? 1.77f : yeti ? 1.28f : IsBoss ? 1.09f : 0.71f);

        public bool CanTakeDamage(float now) => !yeti || now < recoveredAt;

        public bool Blocks(Vector3 attacker, float now)
        {
            if (shield == null || now < guardUntil || strikeAt > 0f || now < recoveredAt) return false;
            var from = Vector3.ProjectOnPlane(attacker - Root.position, Vector3.up);
            return Vector3.Dot(from.normalized, Root.forward) > 0.35f;
        }

        public void BreakGuard(float now)
        {
            guardUntil = now + 2.2f;
            shield.gameObject.SetActive(false);
        }

        public Vector3 Wound(Vector3 origin, Ray? aim, int damage, Vector3 fallback) =>
            importedDamage?.Hit(origin, aim, damage) ?? scars?.Hit(origin, aim, damage) ?? fallback;

        public void Hit(BlockoutWorld world, float now, int damage, Vector3 push)
        {
            Health -= damage;
            alerted = true;
            if (!IsBoss)
            {
                recoil = push * (damage >= 4 ? 5.3f : 2.25f);
                strikeAt = 0;
                recoveredAt = now + (damage >= 4 ? 0.43f : 0.24f);
                motion?.Action("Hit", now, 0.28f, 2.35f);
                Root.localScale = Vector3.one;
            }
            if (yeti && motion != null && Health > 0) motion.Action("Hit", now, 0.32f, 2.4f);
            flashUntil = now + 0.115f;
            world.Tint(body, Color.white);
            skin?.Set(Color.Lerp(Cream, Color.white, 0.84f));
        }

        public void Remove(float now)
        {
            if (motion != null)
            {
                Root.localScale = Vector3.one;
                motion.Action(yeti ? "Defeat" : "Death", now, yeti ? 1.95f : 1.2f, yeti ? 1f : 1.45f);
            }
            Object.Destroy(Root.gameObject, motion != null ? yeti ? 2.1f : 1.18f : 0f);
            if (attackRing != null) Object.Destroy(attackRing.gameObject);
        }

        public bool Tick(BlockoutWorld world, Vector3 target, Vector3 separation, float now, float dt)
        {
            bool spikeHit = spikesAt > 0f && now >= spikesAt && (target - spikePoint).sqrMagnitude < 1.05f;
            if (spikesAt > 0f && now >= spikesAt) spikesAt = 0f;
            if (shield != null && now >= guardUntil && !shield.gameObject.activeSelf)
                shield.gameObject.SetActive(true);

            if (recoil.sqrMagnitude > 0.01f)
            {
                Root.position = Walkable(Root.position + recoil * dt);
                recoil = Vector3.MoveTowards(recoil, Vector3.zero, dt * 17f);
            }
            var offset = Vector3.ProjectOnPlane(target - Root.position, Vector3.up);
            float distance = offset.magnitude;
            Color tint = now < flashUntil ? Color.white : Cream;
            skin?.Set(now < flashUntil ? Color.Lerp(Cream, Color.white, 0.84f) : Cream);
            if (trail && !alerted && distance > (IsBoss ? 10.2f : 6.8f))
            {
                motion?.Locomotion("Idle", now);
                return false;
            }

            alerted = true;
            if (distance > 16f && trail)
            {
                motion?.Locomotion("Idle", now);
                return false;
            }
            if (distance > 0.02f && strikeAt <= 0f)
                Root.rotation = Quaternion.Slerp(Root.rotation, Quaternion.LookRotation(offset), 1 - Mathf.Exp(-12f * dt));

            if (strikeAt > 0f)
            {
                world.Tint(body, Warning);
                if (now < strikeAt)
                {
                    float t = Mathf.InverseLerp(strikeAt - Windup, strikeAt, now);
                    Root.localScale = new Vector3(1f + 0.12f * t, 1f - (Stomp ? 0.25f : 0.13f) * t, 1f + 0.12f * t);
                    return false;
                }
                bool usedSlam = slamStrike;
                strikeAt = 0f;
                recoveredAt = now + (yeti ? 2.03f : IsBoss ? 0.95f : 0.66f);
                Root.localScale = Vector3.one;
                world.Tint(body, Vulnerable);
                if (attackRing != null)
                {
                    attackRing.gameObject.SetActive(false);
                    world.FrostBurst(Root.position + Vector3.up * 0.21f, Reach * 1.3f, Warning, usedSlam ? 39 : 26);
                    if (usedSlam && importedDamage != null)
                    {
                        var marker = CharacterMotion.Find(Root, "GroundImpact");
                        spikePoint = marker != null ? marker.position : Root.position + Root.forward * 1.2f;
                        spikePoint.y = 0f;
                        world.YetiEruption(spikePoint, Root.rotation);
                        spikesAt = now + 0.53f;
                    }
                    if (usedSlam) world.CreamSpray(Root.position + Vector3.up * 0.21f, Vector3.up, Cream, 16, 4f, false);
                }
                bool inArc = usedSlam || IsBoss && !yeti || Vector3.Dot(Root.forward, offset.normalized) > -0.02f;
                float range = yeti ? Radius + (usedSlam ? 1.77f : 1.28f) : Reach;
                return spikeHit || distance <= range + 0.18f && inArc;
            }

            if (now < recoveredAt)
            {
                world.Tint(body, now < flashUntil ? tint : Vulnerable);
            }
            else if (distance <= Reach)
            {
                slamStrike = Stomp;
                float wind = Windup;
                strikeAt = now + wind;
                swingNumber++;
                if (importedDamage != null)
                    motion?.Action(slamStrike ? "GroundSlam" : "PushAttack", now, wind + 0.7f, (slamStrike ? 1.03f : 0.60f) / wind);
                else if (twoScoops) motion?.Action("Bite", now, wind + 0.67f, 0.80f / wind);
                else motion?.Action("Attack", now, wind + 0.52f, 0.63f / wind);
                world.Tint(body, Warning);
                if (attackRing != null)
                {
                    attackRing.position = Root.position;
                    float radius = Radius + (yeti ? 1.28f : 1.09f);
                    attackRing.localScale = new Vector3(Reach / radius, 1, Reach / radius);
                    attackRing.gameObject.SetActive(true);
                }
            }
            else
            {
                motion?.Locomotion("Walk", now);
                var forward = trail ? IceTrail.Steer(Root.position, target, offset.normalized) : offset.normalized;
                var desired = forward * Mathf.Min(speed, (distance - Reach * 0.87f) / Mathf.Max(dt, 0.001f))
                    + separation * 2.3f;
                Root.position = Walkable(Root.position + desired * dt);
                lastMove += dt;
                Root.localScale = new Vector3(1f, 1f + Mathf.Sin(lastMove * (IsBoss ? 10f : 13f)) * 0.06f, 1f);
                world.Tint(body, tint);
            }
            return spikeHit;
        }

        private Vector3 Walkable(Vector3 point)
        {
            point.y = 0;
            return trail ? IceTrail.Clamp(point, margin: Mathf.Min(Radius, 0.64f)) :
                new Vector3(Mathf.Clamp(point.x, -9f, 9f), 0, Mathf.Clamp(point.z, -6.9f, 6.9f));
        }

        private float Windup => Stomp ? 0.95f : yeti ? 0.84f : IsBoss ? 0.72f : 0.49f;
    }

    internal sealed partial class BlockoutWorld
    {
        public Transform WaferGuard(Transform actor, float size)
        {
            var shield = Group("Escudo de barquillo", actor, new Vector3(0, 0.79f, 0.65f) * size);
            Rounded("Cara de la galleta", shield, Vector3.zero, new Vector3(0.86f, 0.94f, 0.15f) * size, Vanilla);
            Rounded("Borde de chocolate", shield, Vector3.back * 0.044f,
                new Vector3(0.96f, 1.04f, 0.13f) * size, Cocoa);
            for (int i = 0; i < 3; i++)
            {
                var stripe = Box("Trama del escudo", shield, 0, (i - 1) * 0.23f, 0.092f, 0.68f, 0.04f, 0.025f, Jam);
                stripe.transform.localRotation = Quaternion.Euler(0, 0, (i % 2 == 0 ? 1 : -1) * 27f);
            }
            return shield;
        }
    }
}