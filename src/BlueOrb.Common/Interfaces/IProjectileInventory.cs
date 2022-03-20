using BlueOrb.Base.Item;

namespace BlueOrb.Base.Interfaces
{
    public interface IProjectileItem
    {
        int CurrentAmmo { get; set; }
        ProjectileConfig ProjectileConfig { get; set; }
    }
}
