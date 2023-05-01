using System.Threading;
using System;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using flanne;
using flanne.Core;
using flanne.UI;

namespace GeneralUtilityMod.Objects
{

    class AchievementPanel : MonoBehaviour
    {
        private GameController owner = null;
        private Panel panel = null;
        private const string RECKLESS = "Reckless";
        private const string ON_THE_EDGE = "On the edge";
        private const string CATCH_THEM_ALL = "Gotta catch 'em ALL";
        private const string PACIFIST = "Pacifist";
        private const string HITLESS = "Nimble";
        private System.Threading.Timer watcher;
        private const int watcherInterval = 2000;

        public void Init(GameController owner)
        {
            this.owner = owner;
        }

        void Awake()
        {
            Transform controlsDisplay = owner
                .hud
                .transform
                .parent
                .Find("ControlsDisplay");

            // copy the controls display so we don't have to manually add
            // a bunch of tweening stuff
            Transform achivementPanelTextObject = GameObject.Instantiate(
                controlsDisplay,
                transform);

            achivementPanelTextObject.position = controlsDisplay.position;

            DestroyImmediate(achivementPanelTextObject.GetComponent<AutoShowPanel>());

            Transform child = null;

            while (achivementPanelTextObject.childCount > 1)
            {
                child = achivementPanelTextObject.GetChild(1);
                DestroyImmediate(child.gameObject);
            }

            child = achivementPanelTextObject.GetChild(0);
            child.name = "Text";

            achivementPanelTextObject.name = "AchievementText";

            SetupPanel(achivementPanelTextObject.gameObject);

            panel = achivementPanelTextObject.GetComponent<Panel>();

            TextMeshProUGUI textMesh = panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            textMesh.alignment = TextAlignmentOptions.BottomLeft;

            SetupWatcher();
        }

        private void SetupPanel(GameObject panelObj)
        {
            var fitter = panelObj.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform rect = panelObj.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = panelObj.GetComponent<CanvasGroup>();

            // Reset positions
            rect.anchorMin = rect.anchorMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            Panel panel = panelObj.GetComponent<Panel>();

            // some minor changes to make tweening work properly on first go
            Traverse.Create(panel).Field("canvasGroup").SetValue(canvasGroup);
            canvasGroup.alpha = 1f;
        }

        private void UpdateText()
        {
            bool recklessCondition = !owner.shootDetector.usedManualShooting && Loadout.CharacterSelection.name == "Abby" && Loadout.GunSelection.name == "GrenadeLauncherData";

            string text = GetAchievementText(HITLESS, owner.hitlessDetector.hitless);
            text += GetAchievementText(PACIFIST, !owner.shootDetector.usedShooting);
            text += GetAchievementText(RECKLESS, recklessCondition);
            text += GetAchievementText(ON_THE_EDGE, owner.playerHealth.maxHP == 1);
            text += GetAchievementText(CATCH_THEM_ALL, UnityEngine.Object.FindObjectsOfType<Summon>().Length >= 8);

            TextMeshProUGUI textMesh = panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            textMesh.text = text;
        }

        private void SetupWatcher()
        {
            watcher = new System.Threading.Timer(
                e => (e as AchievementPanel).Show(),
                this,
                Timeout.Infinite,
                Timeout.Infinite);
        }

        public void StartWatcher()
        {
            watcher.Change(watcherInterval, watcherInterval);
        }

        public void StopWatcher()
        {
            watcher.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private string GetAchievementText(string value, bool active)
        {
            if (!active && !GUMPlugin.achievementWatcherShowInactive.Value) return "";
            return $"<align=left><color={(active ? "white" : "#555")}>{(active ? ">" : "x")} {value}\n";
        }

        public void Show()
        {
            UpdateText();
            panel.Show();
        }

        public void Hide()
        {
            panel.Hide();
        }
    }
}