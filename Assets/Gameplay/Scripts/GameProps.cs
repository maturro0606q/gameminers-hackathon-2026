using UnityEngine;

namespace GameMiners.Gameplay
{
    internal sealed partial class BlockoutWorld
    {
        public void FridgeOpening(float progress) => fridgeDoor.localRotation = Quaternion.Euler(0, 112f * progress, 0);

        public Transform CaptureHand(Transform parent)
        {
            var hand = Group("Mano del Yeti", parent, Vector3.zero);
            Ellipsoid("Palma", hand, new Vector3(0, 0, 0.02f), new Vector3(0.38f, 0.29f, 0.48f), Ice);
            for (int i = 0; i < 3; i++)
                Ellipsoid("Dedos", hand, new Vector3(-0.12f + 0.12f * i, -0.065f, -0.23f),
                    new Vector3(0.103f, 0.19f, 0.34f), Frosting);
            Ellipsoid("Pulgar", hand, new Vector3(0.20f, -0.04f, -0.12f), new Vector3(0.13f, 0.24f, 0.21f), Ice);
            return hand;
        }

        public ParticleSystem FrostBurst(Vector3 point, float diameter, Color tint, int count)
        {
            var obj = Group("Partículas", root, point).gameObject;
            var particles = obj.AddComponent<ParticleSystem>();
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = particles.main;
            main.loop = false;
            main.duration = 0.65f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.30f, 0.85f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.8f, 2.7f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.045f, 0.16f);
            main.startColor = tint;
            main.gravityModifier = 0.25f;
            main.stopAction = ParticleSystemStopAction.Destroy;
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = diameter / 2f;
            var emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });
            var renderer = obj.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Mesh;
            if (roundedMesh == null) roundedMesh = CreateRoundedCube();
            renderer.mesh = roundedMesh;
            renderer.sharedMaterial = surface;
            particles.Play();
            return particles;
        }

        public Transform GroundWeapon(string title, Vector3 position, bool spear, GameObject scoopModel)
        {
            position.y = 0.20f;
            var item = Group(title, root, position);
            if (spear) Spear(item, new Vector3(0, 0.08f, -0.28f));
            else
            {
                var visual = Scoop(item, scoopModel);
                visual.localPosition = Vector3.zero;
            }
            Ellipsoid("Brillo de recogida", item, new Vector3(0, -0.15f, 0.3f),
                new Vector3(1.04f, 0.038f, 1.04f), Vanilla);
            return item;
        }

        public Transform Spear(Transform parent, Vector3 position)
        {
            var pivot = Group("Lanza de dulce", parent, position);
            if (PortableModel.Instantiate("CandySpear", pivot, surface) != null)
            {
                Group("Punta", pivot, new Vector3(-0.123f, 0, 1f));
                return pivot;
            }
            var shaft = Rounded("Asta de caramelo", pivot, new Vector3(0, 0, 0.31f),
                new Vector3(0.095f, 0.095f, 1.89f), Jam);
            shaft.transform.localRotation = Quaternion.identity;
            for (int i = 0; i < 5; i++)
                Box("Anillo de azúcar", pivot, 0, 0, -0.42f + 0.31f * i, 0.103f, 0.103f, 0.07f, Frosting);
            var tip = Box("Punta de azúcar", pivot, 0, 0, 1.28f, 0.17f, 0.17f, 0.31f, Ice);
            tip.transform.localRotation = Quaternion.Euler(0, 45, 0);
            Group("Punta", pivot, new Vector3(0, 0, 1.43f));
            return pivot;
        }

        public Transform Bomb(Vector3 position)
        {
            var item = Group("Bomba de chicle", root, position);
            var model = PortableModel.Instantiate("Bomb", item, surface);
            if (model != null)
            {
                model.localScale = Vector3.one * 1.72f;
                model.GetComponent<PortableRig>()?.Play("Idle", 1);
                return item;
            }
            Ball("Chicle", item, 0, 0, 0, 0.33f, Rose);
            Ball("Mecha", item, 0.055f, 0.145f, 0, 0.11f, Vanilla);
            return item;
        }

        public Transform Chest(Vector3 position, Color accent, out Transform lid)
        {
            var chest = Group("Cofre", root, position);
            Rounded("Caja de galleta", chest, new Vector3(0, 0.24f, 0), new Vector3(1.12f, 0.47f, 0.75f), Cocoa);
            lid = Group("Bisagra", chest, new Vector3(0, 0.48f, 0.34f));
            Rounded("Tapa de caramelo", lid, new Vector3(0, 0, -0.34f), new Vector3(1.16f, 0.16f, 0.78f), accent);
            Box("Broche", lid, 0, -0.03f, -0.72f, 0.16f, 0.20f, 0.06f, Vanilla);
            return chest;
        }

        public Transform AlertRing(Vector3 position, float radius)
        {
            var ring = Group("Área de golpe", root, position);
            for (int i = 0; i < 24; i++)
            {
                float a = i * Mathf.PI / 12f;
                Ball("Marca", ring, Mathf.Sin(a) * radius, 0.065f, Mathf.Cos(a) * radius, 0.125f, Rose);
            }
            ring.gameObject.SetActive(false);
            return ring;
        }

        private Renderer ShapeIceCream(Transform enemy, float size, Color color)
        {
            var body = Group("Visual del helado", enemy, Vector3.zero);
            body.localScale = Vector3.one * size;
            Ellipsoid("Barquillo", body, new Vector3(0, 0.37f, 0), new Vector3(0.84f, 0.72f, 0.76f), Vanilla);
            for (int i = 0; i < 4; i++)
            {
                var stripe = Box("Trama de galleta", body, -0.26f + i * 0.17f, 0.36f, 0.322f, 0.06f, 0.49f, 0.045f, Cocoa);
                stripe.transform.rotation = Quaternion.Euler(0, 0, 25f);
            }
            var scoop = Ellipsoid("Bola de helado", body, new Vector3(0, 0.94f, 0), new Vector3(1.25f, 1.04f, 1.09f), color);
            for (int i = 0; i < 5; i++)
                Ellipsoid("Gotas", body, new Vector3(-0.46f + i * 0.23f, 0.69f, 0.36f + (2 - Mathf.Abs(i - 2)) * 0.049f),
                    new Vector3(0.19f, 0.32f, 0.18f), color);
            Ball("Ojo izquierdo", body, -0.20f, 1.01f, 0.486f, 0.20f, Frosting);
            Ball("Ojo derecho", body, 0.20f, 1.01f, 0.486f, 0.20f, Frosting);
            Ball("Pupila izquierda", body, -0.20f, 1.01f, 0.574f, 0.097f, Dark);
            Ball("Pupila derecha", body, 0.20f, 1.01f, 0.574f, 0.097f, Dark);
            Ellipsoid("Mueca", body, new Vector3(0, 0.81f, 0.538f), new Vector3(0.26f, 0.13f, 0.09f), Dark);
            if (size > 1.6f)
            {
                Ellipsoid("Segunda bola", body, new Vector3(0, 1.45f, -0.06f), new Vector3(0.83f, 0.61f, 0.75f), Rose);
                Ball("Guinda", body, 0.12f, 1.77f, 0f, 0.23f, Jam);
            }
            return scoop;
        }

        private Renderer ShapeJety(Transform enemy, float size)
        {
            var visual = Group("Visual del Yeti", enemy, Vector3.zero);
            visual.localScale = Vector3.one * size;
            var body = Ellipsoid("Torso", visual, new Vector3(0, 0.79f, 0), new Vector3(1.32f, 1.32f, 0.90f), Ice);
            Ellipsoid("Pecho blanco", visual, new Vector3(0, 0.75f, 0.35f), new Vector3(0.70f, 0.86f, 0.29f), Frosting);
            Ellipsoid("Cabeza", visual, new Vector3(0, 1.35f, 0.12f), new Vector3(0.91f, 0.76f, 0.75f), Ice);
            Ellipsoid("Hocico", visual, new Vector3(0, 1.24f, 0.461f), new Vector3(0.57f, 0.29f, 0.20f), Frosting);
            Ellipsoid("Boca", visual, new Vector3(0, 1.23f, 0.553f), new Vector3(0.34f, 0.10f, 0.06f), Dark);
            foreach (float side in new[] { -1f, 1f })
            {
                Ellipsoid("Ceja", visual, new Vector3(side * 0.21f, 1.48f, 0.42f), new Vector3(0.28f, 0.095f, 0.13f), Dark);
                Ball("Ojo", visual, side * 0.20f, 1.42f, 0.467f, 0.11f, Frosting);
                Ball("Pupila", visual, side * 0.20f, 1.42f, 0.515f, 0.061f, Dark);
                Ellipsoid("Brazo", visual, new Vector3(side * 0.72f, 0.78f, 0.02f), new Vector3(0.49f, 0.93f, 0.58f), Ice);
                Ellipsoid("Pie", visual, new Vector3(side * 0.32f, 0.14f, 0.14f), new Vector3(0.47f, 0.27f, 0.66f), Ice);
                Ellipsoid("Cuerno", visual, new Vector3(side * 0.37f, 1.65f, 0.01f), new Vector3(0.22f, 0.34f, 0.26f), Frosting);
            }
            return body;
        }
    }
}
