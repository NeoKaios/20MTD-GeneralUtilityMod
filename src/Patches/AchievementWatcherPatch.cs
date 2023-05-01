using System.Threading.Tasks;
using System.Globalization;
using System.ComponentModel;
using System;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using flanne;
using flanne.Core;
using flanne.UI;
using GeneralUtilityMod.Objects;

namespace GeneralUtilityMod.Patches
{
    public enum AchievementWatcherState
    {
        [DescriptionAttribute("DescriptionAttribute")]
        Never,
        Pause,
        Always,
    };

    [HarmonyPatch]
    class AchievementWatcherPatch
    {
        private static GameController gameController;
        private static PlayerController playerController;
        private static AchievementPanel achievementPanel;

        [HarmonyPatch(typeof(GameController), "Start")]
        static void Prefix(GameController __instance)
        {
            gameController = __instance;

            GameObject panelObj = new("AchievementPanel", typeof(RectTransform));

            panelObj.transform.SetParent(gameController.hud.transform.parent);
            panelObj.transform.localScale = Vector3.one;

            RectTransform transform = panelObj.transform as RectTransform;
            transform.anchorMin = new Vector2(0f, 0f);
            transform.anchorMax = new Vector2(0f, 0f);

            // Position the panel so that it shows up on the bottom left of the screen.
            transform.sizeDelta = new(50, 30);
            transform.anchoredPosition = new(100f, 0f);

            // Avoid calling Awake when we add the component below
            panelObj.SetActive(false);

            var canvasGroup = panelObj.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.ignoreParentGroups = false;

            var layout = panelObj.AddComponent<HorizontalLayoutGroup>();
            layout.useGUILayout = true;
            layout.childAlignment = TextAnchor.LowerLeft;

            achievementPanel = panelObj.AddComponent<AchievementPanel>();
            achievementPanel.Init(gameController);

            // Now call Awake
            panelObj.SetActive(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PauseState), "Enter")]
        private static void PauseStateEnterPostPatch()
        {
            if (GUMPlugin.hasAchievementWatcher.Value == AchievementWatcherState.Pause)
            {
                achievementPanel.Show();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CombatState), "Enter")]
        private static void CombatStateEnterPostPatch()
        {
            switch (GUMPlugin.hasAchievementWatcher.Value)
            {
                case AchievementWatcherState.Always:
                    achievementPanel.StartWatcher();
                    break;
                case AchievementWatcherState.Pause:
                case AchievementWatcherState.Never:
                    achievementPanel.StopWatcher();
                    achievementPanel.Hide();
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerSurvivedState), "Enter")]
        private static void PlayerSurvivedStatePostEnter()
        {
            achievementPanel.StopWatcher();
            achievementPanel.Hide();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerDeadState), "Enter")]
        private static void PlayerDeadStatePostEnter()
        {
            achievementPanel.StopWatcher();
            achievementPanel.Hide();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), "Start")]
        static void PrefixPlayerControllerStart(PlayerController __instance)
        {
            playerController = __instance;
        }
    }
}

