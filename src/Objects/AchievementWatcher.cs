using System;
using flanne;
using flanne.Core;
using UtillI;

namespace GeneralUtilityMod.Objects
{
    class AchievementWatcher : Registration
    {
        private GameController gameController = null;
        private const string RECKLESS = "Reckless";
        private const string ON_THE_EDGE = "On the edge";
        private const string CATCH_THEM_ALL = "Gotta catch 'em ALL";
        private const string PACIFIST = "Pacifist";
        private const string HITLESS = "Nimble";

        public AchievementWatcher(PanelPosition pos, DisplayRule rule) : base(pos, rule)
        {
            GUMPlugin.hasAchievementWatcher.SettingChanged += ChangeWatcherMode;
        }

        void ChangeWatcherMode(object sender, EventArgs e)
        {
            rule = GUMPlugin.hasAchievementWatcher.Value;
        }

        public override void Init()
        {
            gameController = UnityEngine.Object.FindObjectOfType<GameController>();
        }

        private string GetAchievementText(string value, bool active)
        {
            if (!active && !GUMPlugin.achievementWatcherShowInactive.Value) return "";
            return $"<color={(active ? "white>>" : "#555>x")} {value}\n";
        }

        override public string GetUpdatedText()
        {
            bool recklessCondition = !gameController.shootDetector.usedManualShooting && Loadout.CharacterSelection.name == "Abby" && Loadout.GunSelection.name == "GrenadeLauncherData";

            string text = GetAchievementText(HITLESS, gameController.hitlessDetector.hitless);
            text += GetAchievementText(PACIFIST, !gameController.shootDetector.usedShooting);
            text += GetAchievementText(RECKLESS, recklessCondition);
            text += GetAchievementText(ON_THE_EDGE, gameController.playerHealth.maxHP == 1);
            text += GetAchievementText(CATCH_THEM_ALL, UnityEngine.Object.FindObjectsOfType<Summon>().Length >= 8);

            return text;
        }
    }
}