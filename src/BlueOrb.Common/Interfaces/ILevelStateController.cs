namespace BlueOrb.Base.Interfaces
{
    public interface ILevelStateController
    {
        IShooterComponent ShooterComponent { get; }

        void AddPoints(int points);
        void PrepareStartStageData();
    }
}