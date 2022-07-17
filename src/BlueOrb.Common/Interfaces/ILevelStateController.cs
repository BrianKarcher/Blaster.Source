namespace BlueOrb.Base.Interfaces
{
    public interface ILevelStateController
    {
        IShooterComponent ShooterComponent { get; }

        void AddPoints(int points);
        int GetCurrentScore();
        void SetCurrentScore(int score);
        float GetCurrentHp();
        void SetCurrentHp(float hp);
        float GetMaxHp();
        void SetMaxHp(float hp);
        void ProcessEndStage();
        void UpdateUIScore(bool immediate);

        bool HasLevelBegun { get; }
        bool EnableInput { get; set; }
    }
}