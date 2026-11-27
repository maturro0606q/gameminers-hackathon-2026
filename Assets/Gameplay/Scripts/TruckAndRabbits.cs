using UnityEngine;

namespace GameMiners.Gameplay
{
    internal sealed partial class BlockoutWorld
    {
        public void Truck(GameObject jety, out Transform[] customers, out Transform fridge, out Transform apparition)
        {
            DressPark();
            Box("Piso del carrito", root, 0, -0.16f, 0, 4.8f, 0.25f, 8.4f, Cocoa, true);
            Box("Techo crema", root, 0, 2.95f, 0, 4.85f, 0.16f, 8.4f, Frosting);
            Box("Mamparo trasero", root, 0, 1.43f, 4.1f, 4.8f, 2.9f, 0.17f, Blush, true);
            Box("Costado de cocina", root, -2.4f, 1.45f, 0, 0.16f, 2.9f, 8.4f, Lagoon, true);
            Box("Costado bajo", root, 2.4f, 0.56f, 0, 0.17f, 1.12f, 8.4f, Lagoon, true);
            Box("Alerón de la ventana", root, 2.4f, 2.6f, 0, 0.17f, 0.58f, 8.4f, Blush);
            var side = Box("Límite de la ventana", root, 2.4f, 1.6f, 0, 0.06f, 1.15f, 8.3f, Frosting, true);
            side.enabled = false;
            side.gameObject.layer = 2;
            Box("Panel en cabina", root, 0, 0.6f, -4.13f, 4.8f, 1.2f, 0.2f, Lagoon, true);
            var front = Box("Parabrisas", root, 0, 1.95f, -4.13f, 4.8f, 1.5f, 0.12f, Ice, true);
            front.enabled = false;
            for (int x = -2; x <= 2; x++)
            for (int z = -4; z <= 4; z++)
                Box("Mosaico", root, x * 0.91f, -0.031f, z * 0.9f, 0.90f, 0.022f, 0.89f,
                    (x + z) % 2 == 0 ? Blush : Frosting);
            for (int i = 0; i < 5; i++)
            {
                float z = i * 1.43f - 2.9f;
                Box("Refuerzo de ventana", root, 2.37f, 1.57f, z, 0.15f, 2.2f, 0.10f, Frosting);
                Ellipsoid("Lámpara", root, new Vector3(0, 2.8f, z), new Vector3(0.7f, 0.18f, 0.4f), Vanilla);
            }
            Rounded("Tablero del conductor", root, new Vector3(0, 1.03f, -3.88f), new Vector3(4.6f, 0.24f, 0.48f), Lagoon);
            Rounded("Asiento del conductor", root, new Vector3(-1.20f, 0.62f, -3.25f), new Vector3(0.85f, 0.45f, 0.69f), Cocoa);
            Rounded("Respaldo", root, new Vector3(-1.20f, 0.96f, -2.96f), new Vector3(0.85f, 0.77f, 0.22f), Jam);
            for (int i = 0; i < 16; i++)
            {
                float a = i * Mathf.PI / 8f;
                Ellipsoid("Aro del volante", root,
                    new Vector3(-1.20f + 0.24f * Mathf.Cos(a), 1.36f + 0.24f * Mathf.Sin(a), -3.75f),
                    Vector3.one * 0.124f, Blackberry);
            }
            var counter = Group("Mostrador lateral", root, new Vector3(1.9f, 0, -0.38f));
            Box("Mueble", counter, 0, 0.54f, 0, 0.91f, 1.12f, 3.15f, Lagoon, true);
            Rounded("Cubierta", counter, new Vector3(0, 1.11f, 0), new Vector3(1.0f, 0.14f, 3.26f), Vanilla);
            for (int i = 0; i < 3; i++)
            {
                var cup = Group("Cubeta de " + (i == 0 ? "fresa" : i == 1 ? "vainilla" : "menta"),
                    counter, new Vector3(-0.11f, 1.23f, (i - 1) * 0.61f));
                Rounded("Bandeja", cup, Vector3.zero, new Vector3(0.55f, 0.12f, 0.53f), Cocoa);
                flavorTrays[i] = Ellipsoid("Bola", cup, new Vector3(0, 0.09f, 0),
                    new Vector3(0.38f, 0.22f, 0.38f), i == 0 ? Jam : i == 1 ? Vanilla : Pistachio).transform;
            }
            Sign(root, "LA CEREZA • HELADOS", new Vector3(1.36f, 2.60f, 3.87f),
                new Vector2(1.52f, 0.31f), Jam, Frosting, 0.031f);

            OutfitTruck(counter);

            customers = new Transform[4];
            for (int i = 0; i < customers.Length; i++)
            {
                var customer = Group("Cliente " + (i + 1), root, new Vector3(6.74f + i * 0.94f, 0, -0.67f - i * 0.95f));
                customer.rotation = Quaternion.Euler(0, -90, 0);
                var visitor = Person(customer, i == 0 ? Plum : i == 1 ? Mint : i == 2 ? Rose : Vanilla);
                Ellipsoid("Nariz", visitor, new Vector3(0, 1.605f, 0.244f), new Vector3(0.078f, 0.070f, 0.085f), Cream);
                Ellipsoid("Boca", visitor, new Vector3(0, 1.548f, 0.239f),
                    new Vector3(i == 3 ? 0.11f : 0.089f, i == 3 ? 0.033f : 0.024f, 0.027f), Cocoa);
                Rounded("Visera", visitor, new Vector3(0, 1.818f, 0.231f), new Vector3(0.42f, 0.059f, 0.25f),
                    i == 0 ? Lavender : i == 1 ? Lagoon : i == 2 ? Jam : Cocoa);
                customers[i] = customer;
            }

            fridge = BuildFridge(Vector3.forward * 3.49f);
            apparition = Enemy("Yeti del refrigerador", fridge.position + Vector3.back * 0.05f,
                1.13f, Ice, true, out _, jety);
            apparition.rotation = Quaternion.Euler(0, 180, 0);
            apparition.gameObject.SetActive(false);

            // El modelo original aparece en el plano exterior del menú.
            storageRoute = Group("Ruta al congelador", root, Vector3.zero);
            for (int i = 0; i < 3; i++) Chevron(storageRoute, new Vector3(0, 0.005f, 0.8f + i * 0.53f), 0.53f, Lagoon);
            storageRoute.gameObject.SetActive(false);
        }

        public void TruckRoute(bool active) => storageRoute.gameObject.SetActive(active);

        private void LandscapeRoad()
        {
            Box("Costanera", root, 0, -0.24f, 0, 54, 0.2f, 70, new Color32(233, 202, 152, 255));
            Box("Camino", root, 0, -0.13f, 0, 8.1f, 0.08f, 64, Lavender);
            Box("Mar", root, 30, -0.31f, 8, 39, 0.12f, 78, Lagoon);
            for (int i = 0; i < 15; i++)
            {
                float z = i * 3.1f - 21;
                Box("Línea de carretera", root, -3.1f, -0.08f, z, 0.08f, 0.025f, 1.45f, Frosting);
                Ellipsoid("Duna", root, new Vector3(-9 - i % 4 * 2, -0.32f, z),
                    new Vector3(5, 1.8f, 4), Vanilla);
                if (i % 2 == 0) CandyPillar(root, new Vector3(7 + i % 3, -0.12f, z), 2.7f, i % 3 == 0 ? Jam : Lagoon);
            }
        }

        public void TruckBackdrop(Camera camera, GameObject truck)
        {
            LandscapeRoad();
            if (truck != null) MountModel(truck, root);
            else PortableModel.Instantiate("Truck", root, surface);
            camera.transform.position = new Vector3(6.9f, 4f, -9.3f);
            camera.transform.LookAt(new Vector3(0, 1.45f, 0));
        }

        public Transform RabbitGuide(GameObject model, Vector3 at)
        {
            var holder = Group("Conejo guía", root, at);
            var rabbit = model != null ? MountModel(model, holder) : PortableModel.Instantiate("Rabbit", holder, surface);
            if (rabbit == null) return null;
            if (model == null) rabbit.localRotation = Quaternion.Euler(0, 180, 0);
            rabbit.GetComponent<PortableRig>()?.Play("Idle", 1);
            NormalizeModel(rabbit, 1.35f);
            holder.rotation = Quaternion.LookRotation(new Vector3(-0.68f, 0, -1));
            return rabbit;
        }

        public void RabbitAudience()
        {
            for (int i = 0; i < 5; i++)
            {
                var holder = Group("Conejo del público", root, IceTrail.At(2.6f + i * 2.20f, IceTrail.HalfWidth(2.6f + i * 2.20f) + 0.18f, 0.07f));
                var rabbit = PortableModel.Instantiate("Audience", holder, surface);
                if (rabbit == null) return;
                rabbit.localRotation = Quaternion.Euler(0, 180, 0);
                rabbit.GetComponent<PortableRig>()?.Play("Cheer", 0.78f + i * 0.11f);
                NormalizeModel(rabbit, 1.14f + 0.13f * (i % 2));
                holder.rotation = Quaternion.Euler(0, 197 - i * 4, 0);
                if (i == 1 || i == 3)
                {
                    var board = Group("Pancarta", holder, new Vector3(0, 1.15f, 0.4f));
                    board.localRotation = Quaternion.Euler(0, 180, 0);
                    Sign(board, i == 1 ? "NO MÁS CONOS" : "NO MÁS HUMANOS", Vector3.zero,
                        new Vector2(1.74f, 0.34f), Cocoa, Frosting, 0.045f);
                }
                if (i == 2)
                {
                    var snack = Group("Completo italiano", holder, new Vector3(0.23f, 0.73f, 0.45f));
                    snack.localRotation = Quaternion.Euler(0, 4, 14);
                    Ellipsoid("Pan", snack, Vector3.zero, new Vector3(0.53f, 0.16f, 0.21f), Vanilla);
                    Ellipsoid("Vienesa", snack, Vector3.up * 0.07f, new Vector3(0.50f, 0.08f, 0.09f), Jam);
                    for (int j = 0; j < 5; j++)
                        Ellipsoid("Palta y mayonesa", snack, new Vector3((j - 2) * 0.078f, 0.114f, 0), Vector3.one * 0.08f,
                            j % 2 == 0 ? Pistachio : Frosting);
                }
            }
        }

        internal static void NormalizeModel(Transform model, float height)
        {
            var renderers = model.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;
            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
            if (bounds.size.y <= 0.001f) return;
            model.localScale *= height / bounds.size.y;
        }
    }
}