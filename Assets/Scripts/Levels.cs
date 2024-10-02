using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PikachuGame
{
    public class Levels : MonoBehaviour
    {
        public LevelBtn leveBtnPrefabs;

        [SerializeField] Transform content;

        public List<LevelBtn> listBtn = new();


        private void OnEnable()
        {
            ClearData();
            for (int i = 0; i < GridManager.Instance.listLevels.Count; i++)
            {
                var levelBtn = Instantiate(leveBtnPrefabs, content);
                levelBtn.SetLevelText(GridManager.Instance.listLevels[i].level);

                listBtn.Add(levelBtn);
            }
        }

        public void ClearData()
        {
            listBtn.Clear();
            for (int i = 0; i < content.childCount; i++)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
    }
}

