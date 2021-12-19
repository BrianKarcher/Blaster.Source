namespace BlueOrb.Base.Interfaces
{
    public interface ILevelStateController
    {
        IShooterComponent ShooterComponent { get; }

        void AddPoints(int points);
        void PrepareStartStageData();
        //void SetMaxHp(float maxHp);
        //void AddHp(float hp);
        //void SetCurrentHp(float hp);
    }
}