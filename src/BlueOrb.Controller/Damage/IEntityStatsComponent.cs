using BlueOrb.Common.Components;

namespace BlueOrb.Controller.Damage
{
    public interface IEntityStatsComponent : IComponentBase
    {
        void AddHp(float hp);
        EntityStatsData GetEntityStats();
        bool IsDead();
    }
}