namespace BlueOrb.Base.Interfaces
{
    public interface ILevelStateController
    {
        IShooterComponent ShooterComponent { get; }

        void AddPoints(int points);
        void PrepareStartStageData();
        float GetCurrentHp();
        void SetCurrentHp(float hp);
        float GetMaxHp();

        bool HasLevelBegun { get; }
        bool EnableInput { get; set; }
    }
}