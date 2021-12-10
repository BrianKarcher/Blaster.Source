using BlueOrb.Base.Item;

namespace BlueOrb.Base.Interfaces
{
    public interface IShooterComponent
    {
        ProjectileConfig CurrentMainProjectileConfig { get; }
        ProjectileConfig CurrentSecondaryProjectileConfig { get; }
        void AddAmmo(int ammo);
    }
}