﻿namespace BlueOrb.Base.Item
{
    public class ProjectileConfig : ItemConfig
    {
        public string Message;
        public float MaxSpeed = 200f;
        public int Ammo = 5;
        public bool IsSecondary = true;
        public float Cooldown = 1f;
    }
}