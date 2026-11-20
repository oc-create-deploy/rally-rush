using System;
using DG.Tweening;
using Lofelt.NiceVibrations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.Core
{
    public class UIManager : Singleton<UIManager>
    {
        public TextMeshProUGUI playerCountText;
        public GameObject startButton;
        public GameObject gameOverPanel;
        public GameObject gameWinPanel;
        public GameObject upperPanel;
        public GameObject startButtonBack;
        public GameObject allPanelsBack;

        public Button soundButton;
        public Button musicButton;
        public Button vibrationButton;
        
        public TextMeshProUGUI levelText;
    
        public Sprite toggleOff;
        public Sprite toggleOn;
        public Image progressBar;
        public GameObject levelCountUIObject;
        public GameObject progressBarUIObject;
        
        private void Start()
        {
            GameFlowManager.onGameStateChange += GameFlowManagerOnGameStateChange;
            levelText.text = "LEVEL " + (FindObjectOfType<LevelController>().currentLevel + 1);
            SetPlayerCountText(GameFlowManager.Instance.GetPlayerCount());
            LabelButton(startButton.GetComponent<Button>(), "START / SWIPE TO STEER");
            // Missing legacy decorative sprites must not become opaque white overlays.
            foreach (var image in startButtonBack.GetComponentsInChildren<Image>(true))
            { image.color = Color.clear; image.raycastTarget = false; }
            foreach (var image in allPanelsBack.GetComponentsInChildren<Image>(true))
                image.color = new Color(.06f, .1f, .18f, .65f);
            LabelPopup(gameOverPanel, "RUN ENDED");
            LabelPopup(gameWinPanel, "RALLY COMPLETE");
            foreach (Button button in FindObjectsOfType<Button>(true))
                {
                    if (button.name == "SettingsButton") LabelButton(button, "MENU");
                    if (button.name == "Restart") LabelButton(button, "RETRY");
                    if (button.onClick.GetPersistentEventCount() > 0 && button.onClick.GetPersistentMethodName(0) == "NextLevelButton") LabelButton(button, "RUN AGAIN");
                    if (button.onClick.GetPersistentEventCount() > 0 && button.onClick.GetPersistentMethodName(0) == "Decrease") LabelButton(button, "TAP TO PUSH");
                }
            GameFlowManagerOnGameStateChange(GameFlowManager.Instance.state);
        }

        private static void LabelPopup(GameObject popup, string title)
        {
            // Reuse the existing popup layout and actions, supplying a readable title.
            var image = popup.GetComponent<Image>();
            if (image != null) image.color = new Color(.1f, .17f, .27f, .95f);
            var go = new GameObject("Outcome title", typeof(RectTransform));
            go.transform.SetParent(popup.transform, false);
            var text = go.AddComponent<TextMeshProUGUI>();
            var rect = text.rectTransform;
            rect.anchorMin = new Vector2(.1f, .65f); rect.anchorMax = new Vector2(.9f, .9f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            text.font = Resources.Load<TMP_FontAsset>("RallyFont");
            text.text = title; text.fontSize = 48; text.color = new Color(.12f, .22f, .3f);
            text.alignment = TextAlignmentOptions.Center; text.raycastTarget = false;
        }

        private static void LabelButton(Button button, string label)
        {
            if (button == null) return;
            var text = button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (text == null)
            {
                var go = new GameObject("Readable label", typeof(RectTransform));
                go.transform.SetParent(button.transform, false);
                text = go.AddComponent<TextMeshProUGUI>();
                var rect = text.rectTransform;
                rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
            }
            text.font = Resources.Load<TMP_FontAsset>("RallyFont");
            text.text = label; text.fontSize = 36; text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(.12f, .22f, .3f); text.raycastTarget = false;
        }

        private void OnDestroy()
        {
            GameFlowManager.onGameStateChange -= GameFlowManagerOnGameStateChange;
        }
        
        private void GameFlowManagerOnGameStateChange(GameState obj)
        {
            startButton.SetActive(obj == GameState.Start);

            startButtonBack.SetActive(obj == GameState.Start);
            
            // RectTransform coordinates are canvas-local, never device pixels.
            var startRect = startButton.GetComponent<RectTransform>();
            startRect.DOKill();
            startRect.anchorMin = startRect.anchorMax = new Vector2(.5f, .18f);
            startRect.anchoredPosition = Vector2.zero;
            startRect.sizeDelta = new Vector2(650, 150);
            
            if (obj == GameState.Lose)
            {
                ActivatePopup(gameOverPanel);
            }

            ChangeUpperPanelElements(obj);
        }

        public void UpdateProcess(float proc)
        {
            progressBar.fillAmount = proc;
        }
        
        public string SetPlayerCountText(int amount)
        {
            return playerCountText.text = amount.ToString();
        }
        
        public void ActivatePopup(GameObject popupType)
        {
            upperPanel.SetActive(false);
            allPanelsBack.SetActive(true);
            startButton.SetActive(false);
            if (GameFlowManager.Instance.state != GameState.Start && GameFlowManager.Instance.state != GameState.Battle )
            {
                GameFlowManager.Instance.UpdateGameState(GameState.Pause);
            }
            popupType.SetActive(true);
            popupType.transform.GetComponent<Transform>().DOScale(new Vector3(1, 1, 1), 0.4F).SetEase(Ease.InExpo);
            
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.clickSound);
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
        }

        public void ClosePopup(GameObject popupType)
        {
            upperPanel.SetActive(true);
            allPanelsBack.SetActive(false);
            
            startButton.SetActive(GameFlowManager.Instance.state == GameState.Start);

            if (GameFlowManager.Instance.state != GameState.Start && GameFlowManager.Instance.state != GameState.Battle)
            {
                GameFlowManager.Instance.UpdateGameState(GameState.Game);
            }
            popupType.transform.GetComponent<Transform>().DOScale(new Vector3(0, 0, 0), 0.3F).SetEase(Ease.OutExpo).OnComplete(() => popupType.SetActive(false));
            
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.clickSound);
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
        }
        
        public void CloseStartButton()
        {
            startButton.transform.GetComponent<Transform>().DOMoveY(-100, 0.3F)
                .OnComplete(() =>
                {
                    startButton.SetActive(false);
                    startButtonBack.SetActive(false);
                    StartButton();
                });
        }

        private void StartButton()
        {
            GameFlowManager.Instance.UpdateGameState(GameState.Game);
        }

        public void RestartButton()
        {
            GameFlowManager.Instance.RestartGame();
        }

        public void NextLevelButton()
        {
            GameFlowManager.Instance.LoadNewLevel();
        }
        
        bool currentMusicState;
        bool currentSoundState;
        bool currentVibrationState;
        
        public void SettingsToggle(Button button)
        {
            string target = button.transform.name;
            if (target == "VibrationButton")
            {
                currentVibrationState = AudioManager.Instance.ToggleVibration();
                ChangeToggleStatus(button, currentVibrationState);
                PlayerPrefs.SetInt("vibration", Convert.ToInt32(currentVibrationState));
            }
            if (target == "MusicButton")
            {
                print("****  " + currentMusicState);

                currentMusicState = AudioManager.Instance.ToggleTheme();
                print("**** 2 " + currentMusicState);
                ChangeToggleStatus(button, currentMusicState);

                PlayerPrefs.SetInt("music", Convert.ToInt32(currentMusicState));
            }
            if (target == "SoundButton")
            {
                currentSoundState = AudioManager.Instance.ToggleSound();
                ChangeToggleStatus(button, currentSoundState);
                PlayerPrefs.SetInt("sound", Convert.ToInt32(currentSoundState));
            }
        }
        
        public void ChangeToggleStatus(Button button, bool currentState)
        {
            button.GetComponent<Image>().sprite = currentState ? toggleOn  : toggleOff ;
        }

        private void ChangeUpperPanelElements(GameState state)
        {
            if (state == GameState.Start)
            {
                levelCountUIObject.SetActive(true);
            }

            if (state == GameState.Game)
            {
                progressBarUIObject.SetActive(true);
                levelCountUIObject.SetActive(false);
            }
        }


    }
}
