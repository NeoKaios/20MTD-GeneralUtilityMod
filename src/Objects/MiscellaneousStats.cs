using System;
using flanne;
using HarmonyLib;
using UtillI;

namespace GeneralUtilityMod.Objects
{
    class MiscellaneousStats : Registration
    {
        private Traverse enemiesKilled;
        public MiscellaneousStats(PanelPosition pos, DisplayRule rule) : base(pos, rule)
        {
            GUMPlugin.hasMiscStats.SettingChanged += ChangeWatcherMode;
        }
        public override void Init()
        {
            enemiesKilled = Traverse.Create(ScoreCalculator.SharedInstance).Field("_enemiesKilled");
        }

        void ChangeWatcherMode(object sender, EventArgs e)
        {
            rule = GUMPlugin.hasMiscStats.Value;
        }

        override public string GetUpdatedText()
        {
            return "<color=#aaa>Enemy killed: </color>" + enemiesKilled.GetValue();
        }
    }
}
