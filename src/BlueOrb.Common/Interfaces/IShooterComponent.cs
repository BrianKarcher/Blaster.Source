using BlueOrb.Base.Item;

namespace BlueOrb.Base.Interfaces
{
    public interface IShooterComponent
    {
        ProjectileConfig CurrentMainProjectileConfig { get; }
        IProjectileItem CurrentSecondaryProjectile { get; }
        void AddAmmo(int ammo);
    }
}