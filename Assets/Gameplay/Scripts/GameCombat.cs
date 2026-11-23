using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

namespace GameMiners.Gameplay
{
    public sealed partial class GameFlow
    {
        private enum Weapon { None, Scoop, Spear }
        private enum Reward { Spear, Bombs, Completo }
        private const float AttackPeriod = 0.36f;
        private float bufferUntil = -1f;
        private bool bufferedHeavy, bufferedPointer;
        private Vector3 attackHeading;
        private Ray? attackRay;
        private readonly List<EnemyEncounter> enemies = new();
        private readonly List<Pickup> pickups = new();
        private readonly List<ChestReward> chests = new();

        private Weapon weapon = Weapon.Scoop;
        private int bombs;
        private float combatTime, nextAttack, swingUntil, hurtUntil;
        private bool bossDefeated, heavyStrike;
        private float strikeAt = -1;
        private float spearReleaseAt = -1;
        private int combo;
        private float comboUntil;
        private Projectile spearInFlight, bombInFlight;

        private void StartCombat(bool finalRoom)
        {
            if (finalRoom)
            {
                health = Mathf.Max(65, carriedHealth);
                weapon = carriedWeapon == Weapon.None ? Weapon.Scoop : carriedWeapon;
                bombs = carriedBombs;
                sugar = bankedSugar;
                SpawnEnemy("Yeti", new Vector3(1.8f, 0, 2.6f), 1.98f, Ice, 38, 17, 2.35f, true, true, jetyModel);
            }
            else StartTrail();

            SetWeapon(weapon);
        }

        private void TickCombat(bool bombPressed)
        {
            var keys = Keyboard.current;
            bool heavyPressed = Mouse.current?.rightButton.wasPressedThisFrame == true || keys?.kKey.wasPressedThisFrame == true;
            if (attack.WasPressedThisFrame() || heavyPressed || keys?.jKey.wasPressedThisFrame == true)
            {
                bufferedHeavy = heavyPressed;
                var mouse = Mouse.current;
                bufferedPointer = mouse != null && (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame);
                bufferUntil = combatTime + 0.19f;
            }
            if (Time.time < impactPauseUntil) return;
            float now = combatTime += Time.deltaTime;
            world.AdvanceCream(Time.deltaTime);
            UpdateSugar(now);
            TickCompleto(now);
            DashTrail(now);

            if (bufferUntil >= now && now >= nextAttack && strikeAt < 0 && spearReleaseAt < 0 && !combatController.IsDashing)
            {
                bufferUntil = -1;
                attackHeading = bufferedPointer ? AimDirection() : player.transform.forward;
                attackRay = bufferedPointer ? PointerRay() : null;
                player.transform.rotation = Quaternion.LookRotation(attackHeading);
                if (weapon == Weapon.Spear && spearInFlight == null)
                {
                    nextAttack = now + 0.52f / AttackRate;
                    hero?.Action("CandySpearThrow", now, 0.52f / AttackRate, 2.3f * AttackRate);
                    spearReleaseAt = now + 0.50f / (2.3f * AttackRate);
                }
                else if (weapon != Weapon.Spear)
                {
                    combo = now <= comboUntil && !bufferedHeavy ? combo % 3 + 1 : 1;
                    comboUntil = now + 1.05f;
                    heavyStrike = bufferedHeavy || combo == 3;
                    nextAttack = now + (bufferedHeavy ? 0.68f : heavyStrike ? 0.46f : AttackPeriod) / AttackRate;
                    swingUntil = now + AttackPeriod / AttackRate;
                    strikeAt = now + (bufferedHeavy ? 0.21f : heavyStrike ? 0.15f : 0.105f) / AttackRate;
                    hero?.Action("LadleSwing", now, (heavyStrike ? 0.42f : AttackPeriod) / AttackRate, (heavyStrike ? 2.65f : 3.3f) * AttackRate);
                    audio.Play(GameAudio.Cue.Swing, heavyStrike ? 0.82f : 1f + combo * 0.07f);
                }
            }
            if (spearReleaseAt >= 0f && now >= spearReleaseAt)
            {
                spearReleaseAt = -1f;
                ThrowSpear(now);
            }
            if (strikeAt >= 0 && now >= strikeAt)
            {
                strikeAt = -1;
                player.transform.rotation = Quaternion.LookRotation(attackHeading);
                StartCoroutine(AttackArc(player.transform.position, attackHeading, heavyStrike ? 2.05f : 1.67f, heavyStrike));
                Strike(now);
            }

            if (inputLocked) return;
            if (bombPressed && bombs > 0 && bombInFlight == null) ThrowBomb(now);
            UpdateProjectiles(now);
            if (inputLocked) return;

            float swing = Mathf.Clamp01((swingUntil - now) / AttackPeriod);
            if (scoop.parent == player.transform) scoop.localRotation = Quaternion.Euler(0, Mathf.Sin(swing * Mathf.PI) * -99f, 0);
            AnimatePlayer();
            TickEnemies(now);
            if (!inputLocked && spearReleaseAt < 0f) UpdateCombatInteraction();
        }

        private void AnimatePlayer()
        {
            float moving = move.ReadValue<Vector2>().sqrMagnitude;
            if (hero != null)
            {
                hero.Locomotion(moving > 0.02f || combatController.IsDashing ? "Run" : "Idle", combatTime);
                return;
            }
            float bob = moving > 0.02f ? Mathf.Abs(Mathf.Sin(combatTime * 15f)) * 0.09f : 0f;
            var visualPosition = playerVisual.localPosition;
            visualPosition.y = bob;
            playerVisual.localPosition = visualPosition;
        }

        private void TickEnemies(float now)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                var separation = Vector3.zero;
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (i == j) continue;
                    Vector3 away = enemy.Root.position - enemies[j].Root.position;
                    float range = enemy.Radius + enemies[j].Radius + 0.32f;
                    if (away.sqrMagnitude < range * range)
                        separation += away.normalized * (range - away.magnitude);
                }

                bool hit = enemy.Tick(world, player.transform.position, separation, now, Time.deltaTime);
                if (!hit || now < hurtUntil || combatController.IsDashing) continue;
                health = Mathf.Max(0, health - enemy.Damage);
                streak = 0;
                hurtUntil = now + 0.80f;
                audio.Play(GameAudio.Cue.Hurt);
                combatController.Impact(0.2f);
                hero?.Action("Hit", now, 0.36f, 2f);
                world.FrostBurst(player.transform.position + Vector3.up * 0.85f, 0.20f, Rose, 15);
                RefreshCombatHud();
                if (health != 0) continue;

                hero?.Play("Defeat");
                finished = inputLocked = true;
                SetCursor(false);
                ui.HideGameplay();
                ui.ShowDefeat();
                return;
            }
        }

        private void Strike(float now)
        {
            Vector3 origin = player.transform.position;
            Vector3 forward = attackHeading;
            foreach (var chest in chests)
            {
                var offset = chest.Root.position - origin;
                offset.y = 0;
                if (!chest.Open && offset.sqrMagnitude < 5.2f && Vector3.Dot(forward, offset.normalized) >= 0.02f)
                    OpenReward(chest);
            }
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var enemy = enemies[i];
                var offset = enemy.Root.position - origin;
                offset.y = 0;
                float reach = (weapon == Weapon.None ? 1.14f : 2.00f) + enemy.Radius;
                if (offset.sqrMagnitude > reach * reach || Vector3.Dot(forward, offset.normalized) < 0.08f)
                    continue;
                HurtEnemy(i, weapon == Weapon.None ? 1 : heavyStrike ? 4 : 2, now, attackRay);
                if (inputLocked) return;
            }
        }

        private void HurtEnemy(int index, int damage, float now, Ray? aim = null)
        {
            var enemy = enemies[index];
            if (!enemy.CanTakeDamage(now)) return;
            var origin = player.transform.position;
            var push = Vector3.ProjectOnPlane(enemy.Root.position - origin, Vector3.up).normalized;
            Vector3 location = enemy.Root.position + Vector3.up * (0.64f + enemy.Radius);
            bool strong = damage >= 4;
            if (enemy.Blocks(origin, now))
            {
                audio.Play(GameAudio.Cue.Guard);
                world.CreamSpray(location, -push, CandyGold, strong ? 20 : 10, 3.8f, false);
                StartCoroutine(DamageNumber(location + Vector3.up * 0.42f, strong ? "¡GUARDIA ROTA!" : "BLOQUEO", strong));
                if (!strong) { combatController.Impact(0.061f); return; }
                enemy.BreakGuard(now);
            }

            int oldArms = enemy.ArmsRemaining;
            location = enemy.Wound(origin, aim, damage, location);
            if (enemy.ArmsRemaining < oldArms)
                StartCoroutine(DamageNumber(location + Vector3.up * 0.94f, "¡BRAZO CORTADO!", true));
            enemy.Hit(world, now, damage, push);
            streak++;
            streakUntil = now + 2.9f;
            ui.Score(sugar, streak);
            bool defeated = enemy.Health <= 0;
            combatController.Impact(defeated ? 0.23f : strong ? 0.155f : 0.079f);
            float pause = defeated ? 0.084f : strong ? 0.066f : 0.038f;
            impactPauseUntil = Mathf.Max(impactPauseUntil, Time.time + pause);
            StartCoroutine(DamageNumber(location + Vector3.up * 0.53f, damage.ToString(), strong));
            audio.Play(defeated ? GameAudio.Cue.Pop : strong ? GameAudio.Cue.Crush : GameAudio.Cue.Hit,
                0.93f + Mathf.Min(streak, 12) * 0.023f);
            world.CreamSpray(location, push, enemy.Cream, defeated ? 35 : strong ? 25 : 14,
                defeated ? 6.2f : strong ? 4.7f : 3.6f, true);
            if (strong || defeated)
                world.CreamSpray(location, push, CandyGold, defeated ? 13 : 8, 4.7f, false);

            if (!defeated)
            {
                if (enemy.IsBoss) RefreshCombatHud();
                return;
            }

            world.CreamStain(enemy.Root.position, enemy.Cream, enemy.Radius * 0.83f + 0.32f);
            if (chapter == Chapter.IceWorld) RewardSugar(enemy.Root.position, enemy.IsBoss ? 20 : strong ? 4 : 3);
            enemy.Remove(now);
            enemies.RemoveAt(index);
            if (chapter == Chapter.JetyRoom)
            {
                StartCoroutine(Ending());
                return;
            }
            if (enemies.Count == 0) CompleteTrailArea();
            else RefreshCombatHud();
        }

        private void SpawnEnemy(string title, Vector3 position, float size, Color color, int life, int damage,
            float speed, bool boss, bool jety, GameObject model, bool guard = false)
        {
            enemies.Add(new EnemyEncounter(world, title, position, size, color, life, damage, speed, boss, jety,
                model, jety ? null : iceMotion, chapter == Chapter.IceWorld, guard));
        }

        private void RefreshCombatHud()
        {
            string objective = chapter == Chapter.JetyRoom ? "Yeti · Esquiva y golpea cuando se recupere" :
                bossDefeated ? "Cruza el portal hacia la sala del Yeti" :
                trailArea == IceTrail.Areas.Length - 1 ? "Palacio de helado · Busca al gigante" :
                IceTrail.Areas[trailArea].Name + " · Sigue el sendero hacia la derecha";
            ui.ShowCombat(objective, health, MaxHealth, 0, 0);
            ui.SetEquipment(weapon == Weapon.Spear ? "Lanza de dulce" : weapon == Weapon.Scoop ? "Cucharón" : "Puños", bombs);
            if (enemies.Count == 1 && enemies[0].IsBoss)
            {
                var boss = enemies[0];
                string name = chapter == Chapter.JetyRoom && boss.ArmsRemaining == 0 ? "Yeti · Pisotón" : boss.Name;
                ui.ShowBoss(name, boss.Health, boss.MaxHealth);
            }
        }

        private void UpdateCombatInteraction()
        {
            Vector3 position = player.transform.position;
            if (bossDefeated && Distance(portal.position, position) < 2.30f)
            {
                ui.Hint("Entrar a la sala del Yeti");
                if (interact.WasPressedThisFrame())
                {
                    carriedHealth = health;
                    carriedWeapon = weapon;
                    carriedBombs = bombs;
                    bankedSugar = sugar;
                    StartCoroutine(PortalCrossing());
                }
                return;
            }

            int nearby = -1;
            float radius = 1.52f;
            for (int i = 0; i < pickups.Count; i++)
            {
                float distance = Distance(pickups[i].Root.position, position);
                if (distance >= radius) continue;
                radius = distance;
                nearby = i;
            }
            if (nearby >= 0)
            {
                var item = pickups[nearby];
                ui.Hint(item.Weapon == Weapon.Spear ? "Recoger lanza" : "Recoger cucharón");
                if (interact.WasPressedThisFrame())
                {
                    if (weapon != Weapon.None)
                        DropWeapon(weapon, position - player.transform.forward * 0.85f, player.transform.rotation);
                    Destroy(item.Root.gameObject);
                    pickups.RemoveAt(nearby);
                    SetWeapon(item.Weapon);
                    audio.Play(GameAudio.Cue.Serve);
                }
                return;
            }

            foreach (var chest in chests)
            {
                if (chest.Open || Distance(position, chest.Root.position) >= 1.85f) continue;
                ui.Hint("Abrir cofre");
                if (interact.WasPressedThisFrame()) OpenReward(chest);
                return;
            }
            ui.Hint(null);
        }

        private void SetWeapon(Weapon next)
        {
            weapon = next;
            scoop.gameObject.SetActive(next == Weapon.Scoop);
            spear.gameObject.SetActive(next == Weapon.Spear);
            RefreshCombatHud();
        }

        private void DropWeapon(Weapon item, Vector3 position, Quaternion rotation)
        {
            bool isSpear = item == Weapon.Spear;
            var model = world.GroundWeapon(isSpear ? "Lanza de dulce recuperable" : "Cucharón recuperable", position, isSpear, scoopModel);
            model.rotation = rotation;
            pickups.Add(new Pickup(item, model));
        }

        private void ThrowSpear(float now)
        {
            var direction = attackHeading;
            player.transform.rotation = Quaternion.LookRotation(direction);
            var start = spear.position;
            var shot = world.Spear(transform, start);
            shot.rotation = Quaternion.LookRotation(direction);
            spearInFlight = new Projectile(shot, start, direction, now);
            SetWeapon(Weapon.None);
            audio.Play(GameAudio.Cue.Swing);
        }

        private void ThrowBomb(float now)
        {
            hero?.Action("BubblegumThrow", now, 0.55f, 2.3f);
            bombs--;
            var direction = AimDirection();
            var start = player.transform.position + Vector3.up * 0.9f + direction * 0.6f;
            bombInFlight = new Projectile(world.Bomb(start), start, direction, now);
            RefreshCombatHud();
        }

        private void UpdateProjectiles(float now)
        {
            if (spearInFlight != null)
            {
                var shot = spearInFlight;
                float t = Mathf.Clamp01((now - shot.Started) / 0.64f);
                float launchHeight = shot.Origin.y - FloorPoint(shot.Origin, 0f).y;
                Vector3 next = FloorPoint(shot.Origin + shot.Direction * (t * 8.2f), Mathf.Lerp(launchHeight, 0.20f, t));
                Vector3 tip = shot.Root.Find("Punta").position;
                Vector3 tipNext = tip + next - shot.Root.position;
                int hitIndex = -1;
                for (int i = 0; i < enemies.Count; i++)
                {
                    var enemy = enemies[i];
                    Vector3 segment = tipNext - tip;
                    float along = Mathf.Clamp01(Vector3.Dot(enemy.Root.position + Vector3.up * tipNext.y - tip, segment)
                        / Mathf.Max(0.001f, segment.sqrMagnitude));
                    if (Distance(tip + segment * along, enemy.Root.position) < enemy.Radius + 0.52f)
                    {
                        hitIndex = i;
                        break;
                    }
                }
                shot.Root.position = next;
                if (t >= 1f || hitIndex >= 0)
                {
                    DropWeapon(Weapon.Spear, next, shot.Root.rotation);
                    Destroy(shot.Root.gameObject);
                    spearInFlight = null;
                    if (hitIndex >= 0) HurtEnemy(hitIndex, 5, now);
                }
            }

            if (inputLocked || bombInFlight == null) return;
            var bomb = bombInFlight;
            float travel = Mathf.Clamp01((now - bomb.Started) / 0.83f);
            Vector3 impact = FloorPoint(bomb.Origin + bomb.Direction * (4.75f * travel),
                Mathf.Lerp(0.90f, 0.1f, travel) + 1.45f * Mathf.Sin(travel * Mathf.PI));
            bomb.Root.position = impact;
            if (travel < 1f) return;

            if (bomb.BurstTime <= 0f)
            {
                var visual = bomb.Root.GetComponentInChildren<PortableRig>();
                bomb.BurstTime = now + (visual != null && visual.Play("Explode", 1f) ? 0.33f : 0.001f);
                return;
            }
            if (now < bomb.BurstTime) return;
            Destroy(bomb.Root.gameObject, 1.18f);
            bombInFlight = null;
            audio.Play(GameAudio.Cue.Bomb);
            world.FrostBurst(impact + Vector3.up * 0.26f, 2.1f, Rose, 52);
            world.CreamSpray(impact + Vector3.up * 0.37f, Vector3.up, Rose, 26, 5.3f, false);
            combatController.Impact(0.23f);
            foreach (var chest in chests)
                if (!chest.Open && Distance(chest.Root.position, impact) < 2.6f) OpenReward(chest);
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (Distance(impact, enemies[i].Root.position) < 2.6f + enemies[i].Radius) HurtEnemy(i, 5, now);
                if (inputLocked || enemies.Count == 0) return;
            }
        }

        private Transform CameraOpponent()
        {
            Transform best = null;
            float distance = 8.4f;
            foreach (var enemy in enemies)
            {
                float next = Distance(player.transform.position, enemy.Root.position);
                if (next >= distance) continue;
                distance = next;
                best = enemy.Root;
            }
            return best;
        }

        private Vector3 FloorPoint(Vector3 point, float height)
        {
            point.y = height;
            return chapter == Chapter.IceWorld ?
                IceTrail.Clamp(point, trailArea < trailGates.Length ? IceTrail.Areas[trailArea].Gate : IceTrail.Max, 0.24f, false) :
                new Vector3(Mathf.Clamp(point.x, -8.65f, 8.65f), height, Mathf.Clamp(point.z, -6.65f, 6.65f));
        }

        private static float Distance(Vector3 a, Vector3 b)
        {
            a.y = b.y = 0;
            return Vector3.Distance(a, b);
        }

        private sealed class ChestReward
        {
            public readonly Transform Root, Lid;
            public readonly Reward Item;
            public bool Open;
            public ChestReward(Transform root, Transform lid, Reward item) => (Root, Lid, Item) = (root, lid, item);
        }

        private sealed class Pickup
        {
            public readonly Weapon Weapon;
            public readonly Transform Root;
            public Pickup(Weapon weapon, Transform root) => (Weapon, Root) = (weapon, root);
        }

        private sealed class Projectile
        {
            public readonly Transform Root;
            public readonly Vector3 Origin, Direction;
            public readonly float Started;
            public float BurstTime;
            public Projectile(Transform root, Vector3 origin, Vector3 direction, float started) =>
                (Root, Origin, Direction, Started) = (root, origin, direction, started);
        }
    }
}
