using BlueOrb.Base.Item;

namespace BlueOrb.Base.Interfaces
{
    public interface IShooterComponent
    {
        ProjectileConfig CurrentMainProjectileConfig { get; }
        IProjectileItem GetSecondaryProjectile();
        void SetSecondaryProjectile(string uniqueId);
        void AddAmmoToSelected(int ammo);
        void Clear();
    }
}