using UnityEngine;

namespace GameMiners.Gameplay
{
    internal sealed class CharacterMotion
    {
        private readonly Animator animator;
        private readonly PortableRig rig;
        private string state;
        private float heldUntil;

        public CharacterMotion(Transform model, RuntimeAnimatorController controller)
        {
            rig = model.GetComponentInChildren<PortableRig>();
            animator = model.GetComponentInChildren<Animator>();
            if (rig != null) { Play("Idle"); return; }
            if (animator == null || controller == null) return;
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            Play("Idle");
        }

        public void Play(string clip, float speed = 1f)
        {
            if (rig != null)
            {
                if (rig.Play(clip, speed)) state = clip;
                return;
            }
            if (animator == null || animator.runtimeAnimatorController == null) return;
            int hash = Animator.StringToHash("Base Layer." + clip);
            if (!animator.HasState(0, hash)) return;
            animator.speed = speed;
            animator.CrossFadeInFixedTime(hash, 0.055f, 0, 0f);
            state = clip;
        }

        public void Action(string clip, float now, float duration, float speed = 1f)
        {
            heldUntil = now + duration;
            Play(clip, speed);
        }

        public void Locomotion(string clip, float now)
        {
            if (now >= heldUntil && state != clip) Play(clip);
        }

        public static Transform Find(Transform root, string name)
        {
            foreach (var child in root.GetComponentsInChildren<Transform>(true))
                if (child.name == name) return child;
            return null;
        }
    }
}