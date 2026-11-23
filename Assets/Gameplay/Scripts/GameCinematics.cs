using System.Collections;
using UnityEngine;

namespace GameMiners.Gameplay
{
    public sealed partial class GameFlow
    {
        private IEnumerator Capture()
        {
            inputLocked = true;
            player.enabled = false;
            ui.HideGameplay();
            ui.Dialogue(null);
            var cam = view.transform;
            cam.SetParent(null, true);
            Vector3 start = cam.position;
            Quaternion startRotation = cam.rotation;
            Vector3 threshold = fridge.position + new Vector3(0, 1.28f, -0.48f);
            Vector3 reveal = fridge.position + new Vector3(0.14f, 1.60f, -2.25f);
            Vector3 jetyInside = apparition.position;
            Vector3 jetyReach = jetyInside + new Vector3(0, 0, -1.07f);
            apparition.gameObject.SetActive(true);
            var hand = world.CaptureHand(apparition);
            hand.gameObject.SetActive(false);

            audio.Play(GameAudio.Cue.Door);
            audio.Fade(0.42f);
            for (float time = 0; time < 0.78f; time += Time.deltaTime)
            {
                float t = Ease(time / 0.78f);
                world.FridgeOpening(t);
                cam.position = Vector3.Lerp(start, reveal, t);
                cam.rotation = Quaternion.Slerp(startRotation, Aim(cam.position, threshold), t);
                ui.Letterbox(Mathf.Min(t * 2f, 1f));
                yield return null;
            }

            world.FridgeOpening(1f);
            var imported = apparition.GetComponentInChildren<PortableRig>();
            imported?.Play("PushAttack", 1.65f);
            hand.gameObject.SetActive(imported == null);
            audio.Play(GameAudio.Cue.Capture);
            world.FrostBurst(threshold, 0.7f, Ice, 36);
            for (float time = 0; time < 0.38f; time += Time.deltaTime)
            {
                float t = Ease(time / 0.38f);
                apparition.position = Vector3.Lerp(jetyInside, jetyReach, t);
                cam.position = reveal + new Vector3(-0.10f, 0.03f, -0.21f) * t;
                cam.rotation = Aim(cam.position, apparition.position + Vector3.up * 1.45f);
                Vector3 palm = Vector3.Lerp(apparition.position + new Vector3(0.57f, 1.14f, -0.25f),
                    cam.position + new Vector3(0.17f, -0.14f, 0.44f), t);
                hand.SetPositionAndRotation(palm, Quaternion.identity);
                yield return null;
            }

            Vector3 grab = cam.position;
            var through = fridge.position + new Vector3(0, 1.19f, 0.69f);
            for (float time = 0; time < 0.83f; time += Time.deltaTime)
            {
                float t = Mathf.Clamp01(time / 0.83f);
                float pull = t * t * (2.2f - 1.2f * t);
                apparition.position = Vector3.Lerp(jetyReach, jetyInside, t);
                cam.position = Vector3.Lerp(grab, through, pull) + Vector3.right * Mathf.Sin(t * Mathf.PI) * 0.19f;
                cam.rotation = Aim(cam.position, through + Vector3.forward * 0.3f) *
                    Quaternion.Euler(0, 0, Mathf.Sin(t * Mathf.PI) * -14f);
                view.fieldOfView = Mathf.Lerp(54f, 73f, t);
                hand.position = cam.position + new Vector3(0.18f, -0.15f, 0.45f);
                ui.Fade(Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0.53f, 0.93f, t)), true);
                audio.Fade(0.42f * (1f - t));
                yield return null;
            }

            ui.Fade(1f, true);
            arrivingFromShop = true;
            Load(Chapter.IceWorld);
        }

        private IEnumerator Arrival()
        {
            inputLocked = true;
            ui.HideGameplay();
            ui.Fade(1f, true);
            ui.Letterbox(1f);
            var destination = player.transform.position;
            var targetCamera = view.transform.position;
            var targetRotation = view.transform.rotation;
            var startCamera = destination + new Vector3(0.35f, 1.38f, -3.15f);
            var from = destination + new Vector3(-0.78f, 2.85f, -1.32f);
            player.enabled = false;
            player.transform.position = from;
            view.transform.SetPositionAndRotation(startCamera, Aim(startCamera, from));
            for (float time = 0; time < 1.72f; time += Time.deltaTime)
            {
                float t = Mathf.Clamp01(time / 1.72f);
                float fall = Mathf.Clamp01(t / 0.55f);
                player.transform.position = Vector3.Lerp(from, destination, fall * fall);
                var position = Vector3.Lerp(startCamera, targetCamera, Ease(t));
                Quaternion tracking = Aim(position, player.transform.position + Vector3.up * 1.02f);
                view.transform.SetPositionAndRotation(position, Quaternion.Slerp(tracking, targetRotation, Ease(t)));
                view.fieldOfView = Mathf.Lerp(68, 41, Ease(t));
                ui.Fade(1f - Ease(Mathf.Clamp01(t / 0.30f)), true);
                ui.Letterbox(1f - Ease(Mathf.InverseLerp(0.63f, 1, t)));
                yield return null;
            }

            player.transform.position = destination;
            player.enabled = true;
            view.transform.SetPositionAndRotation(targetCamera, targetRotation);
            view.fieldOfView = 41f;
            ui.Letterbox(0);
            ui.Fade(0);
            audio.Play(GameAudio.Cue.Door);
            world.FrostBurst(destination + Vector3.up * 0.22f, 0.85f, Ice, 29);
            yield return GuideArrival();
        }

        private IEnumerator GuideArrival()
        {
            inputLocked = true;
            ui.HideGameplay();
            var guideAnimator = guide != null ? guide.GetComponentInChildren<Animator>() : null;
            var guideRig = guide != null ? guide.GetComponent<PortableRig>() : null;
            guideRig?.Play("Greet", 1.24f);
            if (guideAnimator != null)
            {
                guideAnimator.SetTrigger("Greet");
                guideAnimator.SetBool("Talking", true);
            }
            var original = view.transform.position;
            var originalLook = view.transform.rotation;
            var originalFov = view.fieldOfView;
            var speaker = guide != null ? guide.position + Vector3.up * 0.89f : player.transform.position + Vector3.up;
            var closeUp = speaker + new Vector3(-1.72f, 0.66f, -3.34f);
            ui.Letterbox(0.46f);
            ui.Dialogue("Conejo guía\nOh, eres el heladero número 23 en venir aquí. Ten cuidado con los helados.", true);
            float voice = audio.Speak("guia");
            for (float elapsed = 0; elapsed < Mathf.Max(4.9f, voice); elapsed += Time.deltaTime)
            {
                if (elapsed > 0.94f && interact.WasPressedThisFrame()) break;
                if (elapsed >= 1.15f && elapsed - Time.deltaTime < 1.15f) guideRig?.Play("Talk", 1);
                float zoom = Ease(Mathf.Clamp01(elapsed / 0.82f));
                var position = Vector3.Lerp(original, closeUp, zoom);
                view.transform.SetPositionAndRotation(position, Quaternion.Slerp(originalLook, Aim(position, speaker), zoom));
                view.fieldOfView = Mathf.Lerp(originalFov, 37f, zoom);
                yield return null;
            }
            var returning = view.transform.position;
            var returningLook = view.transform.rotation;
            for (float t = 0; t < 0.48f; t += Time.deltaTime)
            {
                float value = Ease(t / 0.48f);
                view.transform.SetPositionAndRotation(Vector3.Lerp(returning, original, value),
                    Quaternion.Slerp(returningLook, originalLook, value));
                view.fieldOfView = Mathf.Lerp(37f, originalFov, value);
                yield return null;
            }
            view.transform.SetPositionAndRotation(original, originalLook);
            view.fieldOfView = originalFov;
            ui.Letterbox(0);
            if (guideAnimator != null) guideAnimator.SetBool("Talking", false);
            guideRig?.Play("Idle", 1);
            ui.Dialogue(null);
            inputLocked = false;
            RefreshCombatHud();
        }

        private IEnumerator PortalCrossing()
        {
            inputLocked = true;
            ui.HideGameplay();
            audio.Play(GameAudio.Cue.Door);
            for (float time = 0; time < 0.43f; time += Time.deltaTime)
            {
                float t = Ease(time / 0.43f);
                ui.Fade(t);
                audio.Fade(1f - t);
                yield return null;
            }
            ui.Fade(1);
            Load(Chapter.JetyRoom);
        }

        private IEnumerator Ending()
        {
            inputLocked = true;
            ui.HideGameplay();
            ui.Letterbox(1f);
            spear.gameObject.SetActive(false);
            scoop.gameObject.SetActive(false);
            player.enabled = false;

            Vector3 from = player.transform.position;
            Vector3 steps = new Vector3(0, 0.04f, 4.65f);
            Quaternion heading = Aim(from, new Vector3(steps.x, from.y, steps.z));
            Vector3 fromCam = view.transform.position;
            audio.Play(GameAudio.Cue.Clear);
            hero?.Play("Walk");
            for (float time = 0; time < 2.62f; time += Time.deltaTime)
            {
                float t = Ease(time / 2.62f);
                player.transform.position = Vector3.Lerp(from, steps, t);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, heading, 0.22f);
                if (hero == null) playerVisual.localPosition = Vector3.up * Mathf.Abs(Mathf.Sin(time * 13f)) * 0.12f;
                var position = Vector3.Lerp(fromCam, new Vector3(4.4f, 2.2f, 1.15f), t);
                view.transform.SetPositionAndRotation(position, Aim(position, player.transform.position + Vector3.up * 1.02f));
                view.fieldOfView = Mathf.Lerp(54, 45, t);
                ui.Fade(Ease(Mathf.InverseLerp(0.79f, 1f, t)));
                audio.Fade(1f - t);
                yield return null;
            }

            if (hero == null)
            {
                playerVisual.gameObject.SetActive(false);
                world.Finale(player.transform, null);
            }
            else
            {
                player.transform.SetPositionAndRotation(new Vector3(0, 0.25f, 5.95f), Quaternion.Euler(0, 180, 0));
                hero.Play("ThroneSit");
                world.ThroneEffigy();
            }
            Vector3 throne = player.transform.position + Vector3.up * 1.05f + Vector3.left * 0.28f;
            const string finalLine = "Y así fue el final del Yeti. Su corona cambió de dueña. La heladería, en cambio, quedó cerrada por hoy.";
            Transform crown = world.TrophyCrown;
            Transform head = CharacterMotion.Find(playerVisual, "Head") ?? player.transform;
            Transform hand = CharacterMotion.Find(playerVisual, "Hand.R");
            // The imported Head bone points up in +Z, with the face toward its +Y.
            bool rigged = head != player.transform;
            Vector3 crownSeat = rigged ? new Vector3(0f, -0.012f, 0.562f) : new Vector3(0f, 1.63f, 0f);
            Quaternion crownFacing = rigged ? Quaternion.LookRotation(Vector3.down, Vector3.forward) : Quaternion.Euler(0f, 180f, 0f);
            Vector3 crownFit = new Vector3(1.17f, 1.0f, 1.23f);
            Vector3 fromCrown = crown != null ? crown.position : Vector3.zero;
            Quaternion crownStart = crown != null ? crown.rotation : Quaternion.identity;
            bool crowned = false, narrating = false;
            float waitForVoice = 7.0f;
            audio.PlayThroneMusic();
            for (float time = 0; time < 4.55f + waitForVoice; time += Time.deltaTime)
            {
                audio.Fade(0.48f * Ease(Mathf.Clamp01((time - 0.5f) / 1.15f)));
                if (hero != null && time >= 1.83f && time - Time.deltaTime < 1.83f) hero.Play("ThroneIdle");
                if (crown != null && !crowned && time > 2.22f)
                {
                    float lift = Ease(Mathf.InverseLerp(2.22f, 3.12f, time));
                    float place = Ease(Mathf.InverseLerp(3.12f, 4.20f, time));
                    var target = head.TransformPoint(crownSeat);
                    var high = Vector3.Lerp(fromCrown, target, 0.56f) + Vector3.up * 0.44f;
                    Vector3 positionCrown = lift < 1 ? Vector3.Lerp(fromCrown, high, lift) : Vector3.Lerp(high, target, place);
                    crown.SetPositionAndRotation(positionCrown, Quaternion.Slerp(crownStart,
                        head.rotation * crownFacing, lift));
                    crown.localScale = Vector3.Lerp(Vector3.one, Vector3.Scale(crownFit, head.lossyScale), lift);
                    if (hand != null && time < 4.15f) ReachForCrown(playerVisual, hand,
                        positionCrown + Vector3.up * 0.11f, Mathf.Sin(Mathf.InverseLerp(2.22f, 4.20f, time) * Mathf.PI));
                    if (place >= 1f)
                    {
                        crown.SetParent(head, false);
                        crown.SetLocalPositionAndRotation(crownSeat, crownFacing);
                        crown.localScale = crownFit;
                        crown.name = "Corona de la nueva reina";
                        crowned = true;
                        audio.Play(GameAudio.Cue.Loot, 1.14f);
                    }
                }
                if (!narrating && time >= 4.55f)
                {
                    ui.Dialogue(finalLine, true);
                    waitForVoice = Mathf.Max(6.1f, audio.Speak("epilogo"));
                    narrating = true;
                }
                float t = Ease(Mathf.Clamp01(time / 4.7f));
                var position = throne + Vector3.Lerp(new Vector3(3.4f, 1.74f, -6.1f), new Vector3(0.34f, 0.85f, -4.75f), t);
                view.transform.SetPositionAndRotation(position, Aim(position, throne));
                view.fieldOfView = Mathf.Lerp(46f, 40f, t);
                ui.Fade(1f - Ease(Mathf.Clamp01(time / 0.58f)));
                yield return null;
            }
            ui.Dialogue(null);

            finished = true;
            if (aimMark != null) aimMark.gameObject.SetActive(false);
            SetCursor(false);
            ui.Fade(0f);
            ui.Letterbox(0f);
            ui.ShowEnding(runSeconds);
            Debug.Log($"GAMEMINERS_RUN_COMPLETE seconds={runSeconds:F2}");
        }

        private void ReachForCrown(Transform actor, Transform hand, Vector3 target, float influence)
        {
            var reach = actor.GetComponent<CrownGesture>() ?? actor.gameObject.AddComponent<CrownGesture>();
            reach.Aim(hand, target, influence);
        }

        [DefaultExecutionOrder(850)]
        private sealed class CrownGesture : MonoBehaviour
        {
            private Transform hand;
            private Vector3 target;
            private float influence;
            private int frame;
            public void Aim(Transform hand, Vector3 target, float influence)
            {
                this.hand = hand; this.target = target; this.influence = influence; frame = Time.frameCount;
            }

            private void LateUpdate()
            {
                if (frame != Time.frameCount || hand == null) return;
                PoseArm(transform, hand, target, influence);
            }
        }

        private static void PoseArm(Transform actor, Transform hand, Vector3 target, float influence)
        {
            var upper = CharacterMotion.Find(actor, "UpperArm.R");
            var forearm = hand.parent;
            if (upper == null || forearm == null || forearm == upper) return;
            var extended = target - upper.position;
            if (extended.sqrMagnitude < 0.001f) return;
            Vector3 direction = Vector3.Lerp(forearm.position - upper.position, extended, 0.66f);
            upper.rotation = Quaternion.Slerp(upper.rotation,
                Quaternion.FromToRotation(forearm.position - upper.position, direction) * upper.rotation, influence);
            forearm.rotation = Quaternion.Slerp(forearm.rotation,
                Quaternion.FromToRotation(hand.position - forearm.position, target - forearm.position) * forearm.rotation, influence);
        }

        private static Quaternion Aim(Vector3 position, Vector3 target) => Quaternion.LookRotation(target - position);
        private static float Ease(float t) => Mathf.SmoothStep(0f, 1f, t);
    }
}
