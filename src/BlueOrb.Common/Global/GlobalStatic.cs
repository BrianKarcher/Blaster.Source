﻿using BlueOrb.Controller.Scene;

namespace BlueOrb.Base.Global
{
    public static class GlobalStatic
    {
        public static string NextScene { get; set; }
        public static SceneConfig NextSceneConfig { get; set; }
        public static string Difficulty { get; set; }
        public static bool NewHighScore { get; set; }
        public static bool StageComplete { get; set; }
    }
}