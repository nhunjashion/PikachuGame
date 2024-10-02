using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PikachuGame
{
    public class LevelBtn : MonoBehaviour
    {
        public TextMeshProUGUI levelTxt;
        public int level;

        public void SetLevelText(int value)
        {
            levelTxt.text = value.ToString();
            level = value;
        }

        public void OnClick()
        {
            GridManager.Instance.currentLevel = level;

            GridManager.Instance.ClearData();
            GridManager.Instance.LoadLevelData();

            GameSceneManager.Instance.popupSelectLevel.SetActive(false);
            Time.timeScale = 1;
        }
    }
}

