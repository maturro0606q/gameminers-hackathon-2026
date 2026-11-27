using UnityEngine;

namespace GameMiners.Gameplay
{
    public sealed partial class GameFlow
    {
        private enum Service { Arriving, Ordering, Serving, Thanks, Complaint, Reply, Finished }
        private const string LastScoopLine = "Queda una pura bola de helado, voy a ir a buscar al refrigerador,";
        private Service service;
        private float serviceSince, dialogueLength;
        private string serviceLine;
        private ShopGuest[] guests;
        private static readonly string[] GuestNames = { "Nico", "Vale", "Fran", "Tomás" };

        private void BeginService()
        {
            guests = new ShopGuest[customers.Length];
            for (int i = 0; i < customers.Length; i++)
                guests[i] = new ShopGuest(customers[i], i);
            SetService(Service.Arriving, null);
        }

        private void SetService(Service state, string line, string voice = null)
        {
            service = state;
            serviceSince = runSeconds;
            serviceLine = line;
            ui.Dialogue(line);
            dialogueLength = voice != null ? audio.Speak(voice) : 0f;
            UpdateShopObjective();
        }

        private void TickShop()
        {
            foreach (var guest in guests) guest.Tick(Time.deltaTime, runSeconds);
            float elapsed = runSeconds - serviceSince;

            switch (service)
            {
                case Service.Arriving:
                    if (elapsed >= 2.65f && served < guests.Length && guests[served].AtWindow)
                    {
                        audio.Play(GameAudio.Cue.Loot, 1.04f);
                        if (served == Orders.Length)
                            SetService(Service.Complaint, "Tomás · Hey, quiero un completo italiano.", "completo");
                        else
                            SetService(Service.Ordering, $"{GuestNames[served]} · Hey, quiero de {Flavors[Orders[served]]}.",
                                Flavors[Orders[served]]);
                    }
                    return;
                case Service.Ordering:
                case Service.Serving:
                    TickOrder();
                    return;
                case Service.Thanks:
                    if (elapsed >= Mathf.Max(0.85f, dialogueLength - 0.15f))
                    {
                        guests[served - 1].LeaveWithCone();
                        guests[served].Approach();
                        SetService(Service.Arriving, null);
                    }
                    return;
                case Service.Complaint:
                    ui.Hint(elapsed >= 0.8f ? "Responder" : null);
                    if (elapsed >= 0.8f && interact.WasPressedThisFrame())
                    {
                        ui.Hint(null);
                        SetService(Service.Reply, "Tú · Emm, largo de aquí. No vendemos completos, es claramente una heladería, amigo.", "respuesta");
                    }
                    return;
                case Service.Reply:
                    if (elapsed >= Mathf.Max(4.9f, dialogueLength - 0.17f))
                    {
                        guests[served].LeavePark();
                        SetService(Service.Finished, LastScoopLine, "ultima-bola");
                    }
                    return;
            }

            if (serviceLine != null && elapsed >= Mathf.Max(3.6f, dialogueLength))
            {
                serviceLine = null;
                ui.Dialogue(null);
            }
            if (FacingShopTarget(fridge.position + Vector3.up * 1.4f + Vector3.back * 0.6f, fridge, 2.7f, 0.76f))
            {
                ui.Hint("Abrir la heladera");
                if (interact.WasPressedThisFrame()) StartCoroutine(Capture());
            }
            else ui.Hint(null);
        }

        private void TickOrder()
        {
            var customer = customers[served];
            int flavor = Orders[served];
            var tray = world.FlavorTray(flavor);
            if (service == Service.Ordering && FacingShopTarget(tray.position, tray.parent, 2.65f, 0.85f))
            {
                ui.Hint($"Preparar {Flavors[flavor]}");
                if (interact.WasPressedThisFrame())
                {
                    tray.gameObject.SetActive(false);
                    world.ServeCone(flavor);
                    audio.Play(GameAudio.Cue.Serve);
                    world.FrostBurst(tray.position, 0.17f, Rose, 12);
                    SetService(Service.Serving, null);
                }
                return;
            }

            if (service == Service.Serving &&
                FacingShopTarget(customer.position + Vector3.up * 1.56f, customer, 4.1f, 0.71f))
            {
                ui.Hint("Entregar pedido");
                if (interact.WasPressedThisFrame())
                {
                    world.HandOffCone(customer, flavor, guests[served]);
                    served++;
                    audio.Play(GameAudio.Cue.Serve, 1.18f);
                    ui.Hint(null);
                    SetService(Service.Thanks, $"{GuestNames[served - 1]} · ¡Gracias!", "gracias" + served);
                }
                return;
            }
            ui.Hint(null);
        }

        private void UpdateShopObjective()
        {
            world.TruckRoute(service == Service.Finished);
            switch (service)
            {
                case Service.Arriving: ui.ShowShop("Ventanilla · Ya viene el siguiente cliente"); break;
                case Service.Thanks: ui.ShowShop("Pedido entregado"); break;
                case Service.Serving: ui.ShowShop($"Pedido {served + 1}/3 · Entrégalo por la ventanilla"); break;
                case Service.Complaint: ui.ShowShop("Cliente 4 · Escucha el pedido"); break;
                case Service.Reply: ui.ShowShop("Esta es una heladería"); break;
                case Service.Finished: ui.ShowShop("Sigue las flechas al refrigerador de reserva"); break;
                default:
                    string order = $"Prepara {Flavors[Orders[served]]} en el mostrador";
                    ui.ShowShop(served == Orders.Length - 1 ? "¡Queda una pura bola de helado! · " + order : $"Pedido {served + 1}/3 · {order}");
                    break;
            }
        }

        private void ResumeShop()
        {
            UpdateShopObjective();
            ui.Dialogue(serviceLine);
        }
    }

    internal sealed class ShopGuest
    {
        private enum Mode { Waiting, Arriving, AtWindow, ToPark, Eating, Leaving }
        private readonly Transform body;
        private readonly Vector3 counter, picnic;
        private readonly Transform[] arms, legs;
        private Mode mode;
        private Transform cone;
        private float step;

        public ShopGuest(Transform body, int index)
        {
            this.body = body;
            counter = new Vector3(2.98f, 0, -0.40f);
            picnic = index switch
            {
                0 => new Vector3(5.65f, 0, -1.97f),
                1 => new Vector3(6.52f, 0, 0.19f),
                _ => new Vector3(5.28f, 0, 1.23f)
            };
            var visual = body.GetChild(0);
            arms = new[] { visual.Find("Brazo"), GetNth(visual, "Brazo", 1) };
            legs = new[] { visual.Find("Pierna"), GetNth(visual, "Pierna", 1) };
            if (index == 0) Approach();
        }

        public bool AtWindow => mode == Mode.AtWindow;
        public void Approach() => mode = Mode.Arriving;
        public void Receive(Transform item) => cone = item;
        public void LeaveWithCone() => mode = Mode.ToPark;
        public void LeavePark() => mode = Mode.Leaving;

        public void Tick(float dt, float time)
        {
            switch (mode)
            {
                case Mode.Arriving:
                    if (Walk(counter, dt, 2.38f)) { mode = Mode.AtWindow; body.rotation = Quaternion.Euler(0, -90, 0); }
                    break;
                case Mode.ToPark:
                    if (Walk(picnic, dt, 1.78f)) { mode = Mode.Eating; body.rotation = Quaternion.Euler(0, -83f, 0); }
                    break;
                case Mode.Leaving:
                    Walk(new Vector3(14.6f, 0, -10.5f), dt, 2.64f);
                    break;
                case Mode.Eating:
                    float bite = Mathf.Pow(0.5f + 0.5f * Mathf.Sin(time * 2.4f + picnic.z), 5f);
                    if (cone != null)
                    {
                        cone.localPosition = new Vector3(0.30f - bite * 0.19f, 1.13f + 0.31f * bite, 0.37f);
                        cone.localRotation = Quaternion.Euler(15f * bite, 0, -16f * bite);
                    }
                    if (arms[1] != null) arms[1].localRotation = Quaternion.Euler(-38f - 45f * bite, 0, -10f);
                    break;
                case Mode.AtWindow:
                    if (arms[1] != null)
                        arms[1].localRotation = Quaternion.Euler(-25f - 17f * Mathf.Sin(time * 4.1f), 0, -8f);
                    break;
            }
        }

        private bool Walk(Vector3 target, float dt, float speed)
        {
            Vector3 offset = target - body.position;
            if (offset.sqrMagnitude <= 0.011f)
            {
                body.position = target;
                body.GetChild(0).localPosition = Vector3.zero;
                foreach (var leg in legs) if (leg != null) leg.localRotation = Quaternion.identity;
                foreach (var arm in arms) if (arm != null) arm.localRotation = Quaternion.identity;
                return true;
            }
            body.position = Vector3.MoveTowards(body.position, target, speed * dt);
            body.rotation = Quaternion.Slerp(body.rotation, Quaternion.LookRotation(offset), 1 - Mathf.Exp(-12 * dt));
            step += dt * 13f;
            body.GetChild(0).localPosition = Vector3.up * Mathf.Abs(Mathf.Sin(step)) * 0.064f;
            for (int i = 0; i < 2; i++)
            {
                float stride = Mathf.Sin(step) * (i == 0 ? 1 : -1);
                if (legs[i] != null) legs[i].localRotation = Quaternion.Euler(26f * stride, 0, 0);
                if (arms[i] != null && (i == 0 || cone == null))
                    arms[i].localRotation = Quaternion.Euler(-23f * stride, 0, 0);
            }
            return false;
        }

        private static Transform GetNth(Transform parent, string name, int index)
        {
            foreach (Transform child in parent)
            {
                if (child.name != name) continue;
                if (index-- == 0) return child;
            }
            return null;
        }
    }
}