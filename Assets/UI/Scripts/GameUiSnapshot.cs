namespace GameMiners.UI
{
    public enum GameUiScreen
    {
        Menu,
        Shop,
        Combat,
        Portal,
        Pause,
        Defeat,
        FinalCutscene,
        Ending
    }

    public sealed class GameUiSnapshot
    {
        public GameUiScreen Screen { get; set; }
        public string Objective { get; set; }
        public int? StockRemaining { get; set; }
        public string InteractionBinding { get; set; }
        public string InteractionAction { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int DefeatedCount { get; set; }
        public int DefeatedTarget { get; set; }
        public string EquippedWeapon { get; set; }
        public int? BombCount { get; set; }
        public string BossName { get; set; }
        public int BossHealth { get; set; }
        public int BossMaxHealth { get; set; }
        public string PortalMessage { get; set; }
        public float PortalOpacity { get; set; } = 1f;
    }
}
