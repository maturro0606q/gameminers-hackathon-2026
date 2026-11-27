using System;
using UnityEngine;

namespace GameMiners.Gameplay
{
    internal sealed class ShopFirstPersonController
    {
        private readonly Transform player;
        private readonly CharacterController motor;
        private readonly Camera camera;
        private float pitch;

        public ShopFirstPersonController(CharacterController motor, Camera camera)
        {
            this.motor = motor;
            this.camera = camera;
            player = motor.transform;
            camera.transform.SetParent(player, false);
            camera.transform.localPosition = new Vector3(0f, 1.65f, 0f);
        }

        public void Tick(Vector2 input, Vector2 look, float dt)
        {
            player.Rotate(0f, look.x * 0.13f, 0f);
            pitch = Mathf.Clamp(pitch - look.y * 0.13f, -75f, 75f);
            camera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            Vector3 direction = player.right * input.x + player.forward * input.y;
            motor.Move((Vector3.ClampMagnitude(direction, 1f) * 4.4f + Vector3.down * 4f) * dt);
        }
    }

    internal sealed class WorldThirdPersonController
    {
        private static readonly Vector3 Offset = new Vector3(-4.25f, 7.06f, -13.2f);
        private readonly Transform player;
        private readonly CharacterController motor;
        private readonly Camera camera;
        private readonly Vector2 limits;
        private readonly Func<Vector3, Vector3> constrain;
        private Vector3 dashDirection, cameraPosition;
        private float shake;
        private float dashLeft, dashCooldown;
        public bool IsDashing => dashLeft > 0;
        public bool JustDashed { get; private set; }

        public WorldThirdPersonController(CharacterController motor, Camera camera, Vector2 limits, Func<Vector3, Vector3> constrain = null)
        {
            this.motor = motor;
            this.camera = camera;
            this.limits = limits;
            this.constrain = constrain;
            player = motor.transform;
            var focus = player.position + Vector3.up * 0.95f;
            camera.fieldOfView = 41f;
            camera.transform.position = cameraPosition = focus + Offset;
            camera.transform.rotation = Quaternion.LookRotation(-Offset);
        }

        public void Tick(Vector2 input, bool dash, float dt, float speedScale = 1f)
        {
            var right = Vector3.ProjectOnPlane(camera.transform.right, Vector3.up).normalized;
            var forward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
            var direction = Vector3.ClampMagnitude(right * input.x + forward * input.y, 1f);
            if (direction.sqrMagnitude > 0.001f)
                player.rotation = Quaternion.LookRotation(direction);

            JustDashed = false;
            dashCooldown = Mathf.Max(0, dashCooldown - dt);
            if (dash && dashCooldown <= 0)
            {
                dashDirection = direction.sqrMagnitude > 0.001f ? direction : player.forward;
                JustDashed = true;
                dashLeft = 0.32f;
                dashCooldown = 0.75f;
            }
            var velocity = IsDashing ? dashDirection * Mathf.Lerp(7.2f, 12.6f, dashLeft / 0.32f) : direction * (5.05f * speedScale);
            dashLeft = Mathf.Max(0, dashLeft - dt);
            var destination = player.position + velocity * dt;
            if (constrain != null) destination = constrain(destination);
            else
            {
                destination.x = Mathf.Clamp(destination.x, -limits.x, limits.x);
                destination.z = Mathf.Clamp(destination.z, -limits.y, limits.y);
            }
            motor.Move(destination - player.position + Vector3.down * (4f * dt));
        }

        public void Impact(float strength) => shake = Mathf.Max(shake, strength);

        public void Follow(Transform opponent, float dt)
        {
            var focus = player.position + (constrain != null ? Vector3.right * 1.55f : Vector3.zero);
            if (opponent != null)
                focus += Vector3.ClampMagnitude(opponent.position - focus, 7f) * 0.38f;

            var desired = focus + Vector3.up * 0.95f + Offset;
            cameraPosition = Vector3.Lerp(cameraPosition, desired, 1f - Mathf.Exp(-7f * dt));
            shake = Mathf.MoveTowards(shake, 0, dt * 1.45f);
            float time = Time.time;
            camera.transform.position = cameraPosition + shake *
                new Vector3(Mathf.Sin(time * 68), Mathf.Sin(time * 79) * 0.48f, Mathf.Cos(time * 75));
        }
    }
}
