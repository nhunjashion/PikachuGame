using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PikachuGame
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance;

        public GameObject pausePopup; 
        public GameObject popupSelectLevel;

        private void OnEnable()
        {
            //Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        private void Start()
        {
            Instance = this;
        }
        public void OnClickResetMAp()
        {
            GridManager.Instance.ClearData();
            GridManager.Instance.LoadLevelData();

            GridManager.Instance.popupLose.SetActive(false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            pausePopup.SetActive(true);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            pausePopup.SetActive(false);
        }

        public void NextLevel()
        {
            GridManager.Instance.currentLevel++;
        
            GridManager.Instance.ClearData();
            GridManager.Instance.LoadLevelData();

            GridManager.Instance.popupWin.SetActive(false);
        }

        public void SelectLevel()
        {
            Time.timeScale = 0;
            popupSelectLevel.SetActive(true);
        }

    }
}

