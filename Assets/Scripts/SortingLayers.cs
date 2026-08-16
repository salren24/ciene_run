public static class SortingLayers
{
    public const string Sky = "Sky";
    public const string FarBackground = "FarBackground";
    public const string NearBackground = "NearBackground";
    public const string Terrain = "Terrain";
    public const string Objects = "Objects";
    public const string Player = "Player";
    public const string Foreground = "Foreground";
    public const string VFX = "VFX";
    public const string UI = "UI";

    // sortingOrder dentro de la layer "Objects", de atras hacia adelante: decorativos/
    // estructurales de la meta, luego recolectables, luego amenazas (el jugador siempre
    // debe ver al enemigo aunque se superponga con una moneda). Huecos a proposito para
    // insertar objetos futuros sin renumerar todo.
    public static class Order
    {
        public const int GoalBase = 1;
        public const int GoalPole = 2;
        public const int GoalBanner = 3;
        public const int GoalFinial = 4;
        public const int GoalBannerAccent = 5; // encima del Banner (mismo sitio), sigue siendo parte de la meta
        public const int Elevator = 10;
        public const int Obstacle = 20;
        public const int MovingObstacle = 21;
        public const int SpecialCoinHalo = 29; // detras de su propia SpecialCoin
        public const int Coin = 30;
        public const int SpecialCoin = 31;
        public const int FinalCoin = 32;
        public const int PowerUpHalo = 40;
        public const int PowerUp = 41;
        public const int Enemy = 50;
        public const int ChargeEnemy = 51;
    }
}
