using BlueOrb.Base.Item;

namespace BlueOrb.Base.Interfaces
{
    public interface IShooterComponent
    {
        ProjectileConfig CurrentMainProjectileConfig { get; }
        IProjectileInventory CurrentSecondaryProjectile { get; }
        void AddAmmo(int ammo);
    }
}