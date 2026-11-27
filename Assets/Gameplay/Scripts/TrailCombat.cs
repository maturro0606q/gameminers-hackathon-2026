using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameMiners.Gameplay
{
    public sealed partial class GameFlow
    {
        private static readonly Color VanillaCream = new Color32(255, 218, 139, 255);
        private static readonly Color StrawberryCream = new Color32(242, 116, 162, 255);
        private static readonly Color MintCream = new Color32(105, 222, 172, 255);
        private static readonly Color BerryCream = new Color32(169, 117, 236, 255);
        private static readonly Color CandyGold = new Color32(255, 200, 82, 255);

        private static int trailCheckpoint, bankedSugar;
        private int trailArea, sugar, streak;
        private float streakUntil, dashPuffAt;
        private Transform[] trailGates;
        private Transform aimMark;
        private readonly List<SugarDrop> sugarDrops = new();

        private void StartTrail()
        {
            trailArea = Mathf.Clamp(trailCheckpoint, 0, IceTrail.Areas.Length - 1);
            sugar = bankedSugar;
            for (int i = 0; i < trailArea; i++) trailGates[i].gameObject.SetActive(false);
            AddTrailChest(1.4f, -2.75f, Rose, Reward.Spear);
            AddTrailChest(16.4f, 2.85f, Mint, Reward.Bombs);
            AddTrailChest(26.2f, -2.48f, CandyGold, Reward.Completo);
            AddTrailChest(43.9f, -2.65f, Rose, Reward.Bombs);
            AddTrailChest(54.5f, 2.39f, CandyGold, Reward.Completo);
            AddTrailChest(69.6f, 2.75f, Mint, Reward.Bombs);
            AddTrailChest(82.6f, -2.42f, Grape, Reward.Bombs);
            AddTrailChest(88.8f, 2.56f, CandyGold, Reward.Completo);
            PopulateTrail();
        }

        private void AddTrailChest(float x, float lane, Color color, Reward item)
        {
            var chest = world.Chest(IceTrail.At(x, lane), color, out var lid);
            chest.rotation = Quaternion.Euler(0, -13f, 0);
            chests.Add(new ChestReward(chest, lid, item));
        }

        private void PopulateTrail()
        {
            var area = IceTrail.Areas[trailArea];
            if (trailArea == IceTrail.Areas.Length - 1)
            {
                SpawnEnemy("Helado gigante", IceTrail.At(area.FirstEnemy, 0.25f), 2.28f, StrawberryCream, 23, 15, 2.32f,
                    true, false, iceCreamBossModel);
                return;
            }

            for (int i = 0; i < area.Regular + area.Guards; i++)
            {
                bool guard = i >= area.Regular;
                float x = area.FirstEnemy + (i % 2 == 0 ? 0 : 1.85f) + (i / 2) * 3.74f;
                float lane = (i % 2 == 0 ? -1 : 1) * (1.05f + (i % 3) * 0.65f);
                Color cream = ((i + trailArea) % 3) switch { 0 => StrawberryCream, 1 => VanillaCream, _ => MintCream };
                string flavor = cream == StrawberryCream ? "fresa" : cream == VanillaCream ? "vainilla" : "menta";
                Vector3 spawn = IceTrail.Clamp(IceTrail.At(x, lane));
                SpawnEnemy(guard ? "Guardia de " + flavor : "Helado de " + flavor, spawn,
                    guard ? 1.19f : 0.88f, cream, guard ? 7 : 4, guard ? 10 : 6,
                    guard ? 2.38f : 2.73f, false, false, guard ? largeIceCreamModel : smallIceCreamModel, guard);
            }
        }

        private Vector3 TrailMovement(Vector3 point) => IceTrail.Clamp(point,
            trailArea < trailGates.Length ? IceTrail.Areas[trailArea].Gate - 0.07f : IceTrail.Max);

        private void CompleteTrailArea()
        {
            if (trailArea >= trailGates.Length)
            {
                ui.HideBoss();
                bossDefeated = true;
                portal.gameObject.SetActive(true);
                world.FrostBurst(portal.position + Vector3.up * 0.9f, 1.05f, MintCream, 36);
            }
            else
            {
                StartCoroutine(LiftGate(trailGates[trailArea]));
                trailArea++;
                trailCheckpoint = trailArea;
                bankedSugar = sugar;
                health = Mathf.Min(MaxHealth, health + 24);
                PopulateTrail();
                ui.TrailNotice("PASO ABIERTO  •  " + IceTrail.Areas[trailArea].Name);
            }
            audio.Play(GameAudio.Cue.Clear);
            RefreshCombatHud();
        }

        private IEnumerator LiftGate(Transform gate)
        {
            Vector3 from = gate.position;
            for (float t = 0; t < 0.65f; t += Time.deltaTime)
            {
                gate.position = from + Vector3.up * Mathf.SmoothStep(0f, 3.4f, t / 0.65f);
                yield return null;
            }
            gate.gameObject.SetActive(false);
        }

        private void BeginDash()
        {
            audio.Play(GameAudio.Cue.Dash, 0.86f);
            dashPuffAt = 0;
        }

        private void DashTrail(float now)
        {
            if (!combatController.IsDashing || now < dashPuffAt) return;
            dashPuffAt = now + 0.048f;
            world.FrostBurst(player.transform.position + Vector3.up * 0.14f - player.transform.forward * 0.28f,
                0.20f, new Color32(216, 248, 246, 255), 5);
        }

        private Vector3 AimDirection()
        {
            var ray = PointerRay();
            if (!ray.HasValue || !new Plane(Vector3.up, Vector3.up * 0.82f).Raycast(ray.Value, out var distance))
                return player.transform.forward;

            Vector3 hit = ray.Value.GetPoint(distance);
            Vector3 planar = hit - player.transform.position;
            planar.y = 0;
            if (planar.sqrMagnitude < 0.12f) return player.transform.forward;
            return planar.normalized;
        }

        private Ray? PointerRay()
        {
            var mouse = Mouse.current;
            if (mouse == null) return null;
            var screen = mouse.position.ReadValue();
            if (screen.x <= 0 || screen.y <= 0 || screen.x >= Screen.width || screen.y >= Screen.height) return null;
            return view.ScreenPointToRay(screen);
        }

        private void UpdateAimMark()
        {
            if (aimMark == null) aimMark = world.CursorPip();
            if (chapter != Chapter.IceWorld && chapter != Chapter.JetyRoom) return;
            Vector3 point = player.transform.position + AimDirection() * 1.33f;
            point.y = 0.003f;
            aimMark.position = point;
        }

        private void RewardSugar(Vector3 at, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (sugarDrops.Count >= 48)
                {
                    Destroy(sugarDrops[0].Root.gameObject);
                    sugarDrops.RemoveAt(0);
                    sugar++;
                }
                Vector2 scatter = Random.insideUnitCircle * 1.6f;
                var from = at + Vector3.up * 0.67f;
                var end = FloorPoint(at + new Vector3(scatter.x, 0, scatter.y), 0.23f);
                sugarDrops.Add(new SugarDrop(world.SugarGem(from, i % 3 == 0 ? MintCream : CandyGold),
                    from, end, combatTime + i * 0.035f));
            }
        }

        private void UpdateSugar(float now)
        {
            int picked = 0;
            for (int i = sugarDrops.Count - 1; i >= 0; i--)
            {
                var drop = sugarDrops[i];
                float t = Mathf.Clamp01((now - drop.Born) / 0.61f);
                if (t < 1f)
                    drop.Root.position = Vector3.Lerp(drop.From, drop.Landing, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * 0.92f;
                else if (now - drop.Born > 2.8f || Distance(drop.Root.position, player.transform.position) < 3.1f)
                {
                    Vector3 target = player.transform.position + Vector3.up * 0.91f;
                    drop.Root.position = Vector3.MoveTowards(drop.Root.position, target, Time.deltaTime * 17f);
                    if ((drop.Root.position - target).sqrMagnitude < 0.065f)
                    {
                        Destroy(drop.Root.gameObject);
                        sugarDrops.RemoveAt(i);
                        sugar++;
                        picked++;
                    }
                }
                drop.Root.Rotate(0, 260f * Time.deltaTime, 0);
            }
            if (picked > 0) audio.Play(GameAudio.Cue.Loot, 1 + picked * 0.06f);
            if (now > streakUntil) streak = 0;
            ui.Score(sugar, streak);
        }

        private void OpenReward(ChestReward chest)
        {
            if (chest.Open) return;
            chest.Open = true;
            StartCoroutine(OpenLid(chest.Lid));
            string label;
            switch (chest.Item)
            {
                case Reward.Spear:
                    DropWeapon(Weapon.Spear, FloorPoint(chest.Root.position + Vector3.forward * 0.93f, 0.22f), Quaternion.identity);
                    label = "¡LANZA!";
                    break;
                case Reward.Completo:
                    SpawnCompleto(chest.Root);
                    label = "¡COMPLETO ITALIANO!";
                    break;
                default:
                    bombs = Mathf.Min(12, bombs + 3);
                    label = "+3 BOMBAS";
                    break;
            }
            RewardSugar(chest.Root.position, 8);
            world.CreamSpray(chest.Root.position + Vector3.up * 0.53f, Vector3.up, CandyGold, 15, 3.3f, false);
            StartCoroutine(DamageNumber(chest.Root.position + Vector3.up * 1.38f, label, true));
            audio.Play(GameAudio.Cue.Chest);
            RefreshCombatHud();
        }

        private IEnumerator OpenLid(Transform lid)
        {
            for (float t = 0; t < 0.28f; t += Time.deltaTime)
            {
                lid.localRotation = Quaternion.Euler(-112 * Mathf.Sin(t / 0.28f * Mathf.PI * 0.5f), 0, 0);
                yield return null;
            }
            lid.localRotation = Quaternion.Euler(-112f, 0, 0);
        }

        private sealed class SugarDrop
        {
            public readonly Transform Root;
            public readonly Vector3 From, Landing;
            public readonly float Born;
            public SugarDrop(Transform root, Vector3 from, Vector3 landing, float born) =>
                (Root, From, Landing, Born) = (root, from, landing, born);
        }
    }
}