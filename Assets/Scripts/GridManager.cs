using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;
using Random = UnityEngine.Random;

namespace PikachuGame
{
    public class GridManager :  MonoBehaviour
    {
        public static GridManager Instance;

        [Header("Field information")]
        public MapType mapType;
        public int width = 9;
        public int height = 4;
        public int imageCount = 6;
        public GameObject line;
        public float timer=0.4f;
        public BlockObj blockObjPrefabs;
        public BlockObj blockObjPrefabs2;
        public GridLayoutGroup parentGroup;


        [Header("Data")]
        public int currentLevel = 0;
        public TextMeshProUGUI levelTxt;
        public float maxTime = 20;
        public float currentTime;
        public int blockAmount;
        public SpriteSO sprites;
        public SpriteSO blockBgs;
        public Transform Content;
        public Slider timeBar;

        public List<LevelSO> listLevels = new List<LevelSO>();
        public LevelSO[] dataLevel;


        [Header("Popup")]
        public GameObject popupWin;
        public GameObject popupLose;

        [Header("List block")]
        public List<Block> listBlocks;
        public List<BlockObj> listObjBlock;
        public List<Sprite> contentImg;
        public List<Sprite> bgImg;
        public List<Sprite> blockContentImg;



        [Header("Block Select")]
        public BlockObj blockSelect1;
        public BlockObj blockSelect2;
        public GameObject img1;
        public GameObject img2;
        public List<BlockObj> listBlockLine;
        public int stepCount = 0;

        private void OnEnable()
        {
            Instance = this; 
            LoadListLevel();
        }

        private void Start()
        {
            LoadLevelData();
        }

        private void Update()
        {
            currentTime -= Time.deltaTime;
            timeBar.value = currentTime;

            CheckWinLose();
        }


        #region LOAD LEVEL DATA

        public void LoadListLevel()
        {
            dataLevel = Resources.LoadAll<LevelSO>("Levels");

           // listLevels.Sort((x, y) => x.level.CompareTo(y.level));
       

            foreach (LevelSO item in dataLevel)
            {
                listLevels.Add(item);

            }

            listLevels = listLevels.OrderBy(t => t.level).ToList();
        }

        public void LoadLevelData()
        {
            isWin = false;
            isLose = false;
            width = listLevels[currentLevel].height;
            height = listLevels[currentLevel].width;
            mapType = listLevels[currentLevel].mapType;
            maxTime = listLevels[currentLevel].time;
            imageCount = listLevels[currentLevel].pokemonCount;

            levelTxt.text =  "Level: " + listLevels[currentLevel].level.ToString();

            ClearData();

            this.LoadBlockImg();
            this.InitGridSystem();

            switch(mapType)
            {
                case MapType.Normal:
                    SpawnBlockObj();
                    break;

                case MapType.Lion:
                    SpawnBlockLionMap();
                    break;

                case MapType.Dragon:
                    SpawnBlockDragonMap();
                    break;

                case MapType.Baron:
                    SpawnBlockBaronMap();
                    break;

                case MapType.Falcon:
                    SpawnBlockFalonMap();
                    break;
            }

           // blockActive = listObjBlock.Where(t => !t.isBlank).ToList();



            CreateListImg();

            this.SetImgBlockSpawn();
            this.FindBlockObjsNeighbors();

            currentTime = maxTime;
            timeBar.maxValue = maxTime;
            blockAmount = blockActive.Count;
        }


        #endregion


        #region LOAD IMG DATA
        public void LoadBlockImg()
        {
            if (this.sprites == null) this.sprites = Resources.Load<SpriteSO>("ScriptableObjects/Dragon"); 
            else
            {
                foreach (var item in sprites.sprites)
                {
                    contentImg.Add(item);
                }
            }

            if (this.blockBgs == null) this.blockBgs = Resources.Load<SpriteSO>("ScriptableObjects/BlockBg");
            else
            {
                foreach (var item in blockBgs.sprites)
                {
                    bgImg.Add(item);
                }
            }

        }
        public List<BlockObj> blockActive = new();
        public void CreateListImg()
        {
            int colorCount = 0;
            Sprite img = ImgSelected();
            for(int i = 0; i < blockActive.Count; i++)
            {            
                colorCount++;
                blockContentImg.Add(img);
                if(colorCount == imageCount)
                {
                    img = ImgSelected();

                    colorCount = 0;
                }
            }
        }

        public Sprite ImgSelected()
        {
            int index = Random.Range(0, contentImg.Count);
            Sprite img = contentImg[index];
            contentImg.RemoveAt(index);
            return img;
        }

        public void ClearData()
        {
            blockActive.Clear();
            contentImg.Clear();
            blockContentImg.Clear();
            listBlocks.Clear();
            listObjBlock.Clear();
            for (int i = 0; i < Content.childCount; i++)
            {
                Destroy(Content.GetChild(i).gameObject);
            }
        }
        #endregion


        #region SPAWN BLOCK 
        public void InitGridSystem()
        {
            if (this.listBlocks.Count > 0) return;

            int blockId = 0;

            for (int y = 0; y < this.height+2; y++)
            {        
                for (int x = 0; x < this.width+2; x++)
                    {
                    Block block = new Block
                    {
                        x = x,
                        y = y,
                        id = blockId,
                    };
                    this.listBlocks.Add(block);

                    blockId++;
                }
            }
        }

        public void SpawnBlockObj() // flex w & h
        {
            int index = Random.Range(0, contentImg.Count);

            parentGroup.constraintCount = width + 2;

            BlockObj blockPrefab = blockObjPrefabs;

            if (listBlocks.Count <= 60)
            {
                parentGroup.cellSize = new(157, 157);
                blockPrefab = blockObjPrefabs2;
            }
            else if(listBlocks.Count <= 96)
            {
                parentGroup.cellSize = new(140, 140);
                blockPrefab = blockObjPrefabs2;
            }
            else
            {
                parentGroup.cellSize = new(90, 90);
                blockPrefab = blockObjPrefabs;
            }
            foreach (Block block in this.listBlocks)
            {

                BlockObj blockObj;


                blockObj = Instantiate(blockPrefab, Content);
                if (block.x == 0 || block.x == this.width + 1 || block.y == 0 || block.y == this.height + 1)
                {
                    block.isBlank = true;
                    blockObj.isBlank = true;
                    blockObj.blockImg.gameObject.SetActive(false);
                    listObjBlock.Add(blockObj);
                }
                else
                {
                    block.isBlank = false;
                    blockObj.isBlank = false;
                    listObjBlock.Add(blockObj);
                    blockActive.Add(blockObj);
                }

                blockObj.blockID = block.id;
                blockObj.x = block.x;
                blockObj.y = block.y;
                block.blockObj = blockObj;
            }
        }

        public void SpawnBlockDragonMap() //16 x 9
        {
            int index = Random.Range(0, contentImg.Count);


            parentGroup.cellSize = new(90, 90);
            parentGroup.constraintCount = width + 2;

            foreach (Block block in this.listBlocks)
            {
                BlockObj blockObj;
                blockObj = Instantiate(blockObjPrefabs, Content);
                if (block.x == 0 || block.x == this.width + 1 || block.y == 0 || block.y == this.height + 1)
                {
                    block.isBlank = true;
                    blockObj.isBlank = true;
                    blockObj.blockImg.gameObject.SetActive(false);
                    listObjBlock.Add(blockObj);
                }
                else if((block.x == 1 && (block.y == 7 || block.y == 8)) 
                        || (block.x == 2 && (block.y == 1 || block.y == 8 || block.y == 9) )
                        || (block.x == 3 && (block.y == 1 || block.y == 4 || block.y == 5 || block.y == 12 || block.y == 13)) 
                        || (block.x == 4 && (block.y == 1 || block.y == 5 || block.y == 6 || block.y == 11 || block.y == 12 || block.y == 16)) 
                        || (block.x == 5 && (block.y == 1 || block.y == 6 || block.y == 7 || block.y == 10 || block.y == 14 || block.y == 15 || block.y == 16))
                        || (block.x == 6 && (block.y == 1 || block.y == 2 || block.y == 3 || block.y == 6 || block.y == 7 || block.y == 8 || block.y == 9 || block.y == 10 || block.y == 13 || block.y == 14 || block.y == 16)) 
                        || (block.x == 7 && (block.y == 7 || block.y == 8 || block.y == 9 || block.y == 10 || block.y == 13)) 
                        || (block.x == 8 && (block.y == 1 || block.y == 10 || block.y == 13 || block.y == 14 || block.y == 16))
                        || (block.x == 9 && (block.y == 1 || block.y == 2 || block.y == 6 || block.y == 8 || block.y == 10 || block.y == 16))
                            )
                {
                    block.isBlank = true;
                    blockObj.isBlank = true;
                    blockObj.blockImg.gameObject.SetActive(false);
                    listObjBlock.Add(blockObj);
                }
                else
                {
                    block.isBlank = false;
                    blockObj.isBlank = false;
                    listObjBlock.Add(blockObj);
                    blockActive.Add(blockObj);
                }

                blockObj.blockID = block.id;
                blockObj.x = block.x;
                blockObj.y = block.y;
                block.blockObj = blockObj;
            }
        }

        public void SpawnBlockLionMap() // 14 x 9
        {
            int index = Random.Range(0, contentImg.Count);


            parentGroup.cellSize = new(90, 90);
            parentGroup.constraintCount = width + 2;
            foreach (Block block in this.listBlocks)
            {
                BlockObj blockObj;

                blockObj = Instantiate(blockObjPrefabs, Content);
                if (block.x == 0 || block.x == this.width + 1 || block.y == 0 || block.y == this.height + 1)
                {
                    block.isBlank = true;
                    blockObj.isBlank = true;
                    blockObj.blockImg.gameObject.SetActive(false);
                    listObjBlock.Add(blockObj);
                }
                else if ((block.x == 1 && (block.y == 1 || block.y == 5  || block.y == 6 || block.y == 10 || block.y == 11 || block.y == 12 || block.y == 13 || block.y == 14))
                        || (block.x == 2 && (block.y == 1 || block.y == 2 || block.y == 3 || block.y == 6 || block.y == 7 || block.y == 14))
                        || (block.x == 3 && (block.y == 1 || block.y == 2 || block.y == 11 || block.y == 12 || block.y == 14))
                        || (block.x == 4 && (block.y == 1 || block.y == 2 || block.y == 11 || block.y == 12 || block.y == 14))
                        || (block.x == 5 && (block.y == 1 || block.y == 7 || block.y == 8 || block.y == 9 || block.y == 10 || block.y == 11 || block.y == 14))
                        || (block.x == 6 && (block.y == 8 || block.y == 9 || block.y == 13 || block.y == 14))
                        || (block.x == 7 && (block.y == 1 || block.y == 8 || block.y == 9 || block.y == 11 || block.y == 12 || block.y == 13 || block.y == 14))
                        || (block.x == 8 && (block.y == 1 || block.y == 2 || block.y == 7 || block.y == 8 || block.y == 9))
                        || (block.x == 9 && (block.y != 4))
                            )
                {
                    block.isBlank = true;
                    blockObj.isBlank = true;
                    blockObj.blockImg.gameObject.SetActive(false);
                    listObjBlock.Add(blockObj);
                }
                else
                {
                    block.isBlank = false;
                    blockObj.isBlank = false;
                    listObjBlock.Add(blockObj);
                    blockActive.Add(blockObj);
                }

                blockObj.blockID = block.id;
                blockObj.x = block.x;
                blockObj.y = block.y;
                block.blockObj = blockObj;
            }
        }

        public void SpawnBlockBaronMap() //14 x 9 
        {
            int index = Random.Range(0, contentImg.Count);


            parentGroup.cellSize = new(90, 90);
            parentGroup.constraintCount = width + 2;

            foreach (Block block in this.listBlocks)
            {
                BlockObj blockObj;

                blockObj = Instantiate(blockObjPrefabs, Content);
                if (block.x == 0 || block.x == this.width + 1 || block.y == 0 || block.y == this.height + 1)
                {
                    block.isBlank = true;
                    blockObj.isBlank = true;
                    blockObj.blockImg.gameObject.SetActive(false);
                    listObjBlock.Add(blockObj);
                }
                else if ((block.x == 1 && (block.y != 2 && block.y != 4 && block.y != 11 && block.y != 13))
                        || (block.x == 2 && (block.y == 1 || block.y == 3 || block.y == 12 || block.y == 14))
                        || (block.x == 3 && (block.y == 1 || block.y == 14))
                        || (block.x == 4 && (block.y == 1 || block.y == 2 || block.y == 13 || block.y == 14))
                        || (block.x == 5 && (block.y != 4 && block.y != 5 && block.y != 7 && block.y != 8 && block.y != 10 && block.y != 11))
                        || (block.x == 6 && (block.y == 1 || block.y == 5 || block.y == 10 || block.y == 14))
                        || (block.x == 7 && (block.y == 3 || block.y == 12))
                        || (block.x == 8 && (block.y != 1 && block.y != 2 && block.y != 13 && block.y != 14))
                        || (block.x == 9 && (block.y != 1 && block.y != 2 && block.y != 3 && block.y != 12 && block.y != 13 && block.y != 14))
                            )
                {
                    block.isBlank = true;
                    blockObj.isBlank = true;
                    blockObj.blockImg.gameObject.SetActive(false);
                    listObjBlock.Add(blockObj);
                }
                else
                {
                    block.isBlank = false;
                    blockObj.isBlank = false;
                    listObjBlock.Add(blockObj);
                    blockActive.Add(blockObj);
                }

                blockObj.blockID = block.id;
                blockObj.x = block.x;
                blockObj.y = block.y;
                block.blockObj = blockObj;
            }
        }

        public void SpawnBlockFalonMap() // 16 x 9
        {
            int index = Random.Range(0, contentImg.Count);


            parentGroup.cellSize = new(90, 90);
            parentGroup.constraintCount = width + 2;
            foreach (Block block in this.listBlocks)
            {
                BlockObj blockObj;

                blockObj = Instantiate(blockObjPrefabs, Content);
                if (block.x == 0 || block.x == this.width + 1 || block.y == 0 || block.y == this.height + 1)
                {
                    block.isBlank = true;
                    blockObj.isBlank = true;
                    blockObj.blockImg.gameObject.SetActive(false);
                    listObjBlock.Add(blockObj);
                }
                else if ((block.x == 1 && (block.x == 1 || block.x == 2 || block.x == 3 || block.x == 4 || block.x == 13 || block.x == 14 || block.x == 15 || block.x == 16))
                        || (block.x == 2 && (block.y != 6 && block.y != 7 && block.y != 8 && block.y != 9 && block.y != 10 && block.y != 11))
                        || (block.x == 3 && (block.y != 7 && block.y != 8 && block.y != 9 && block.y != 10))
                        || (block.x == 4 && (block.y != 6 && block.y != 7 && block.y != 8 && block.y != 9 && block.y != 10 && block.y != 11))
                        || (block.x == 5 && (block.y == 1 || block.y == 2 || block.y == 3 || block.y == 14 || block.y == 15 || block.y == 16))
                        || (block.x == 6 && (block.y == 1 || block.y == 2 || block.y == 15 || block.y == 16))
                        || (block.x == 7 && (block.y == 1 || block.y == 16))
                        || (block.x == 8 && (block.y != 1 && block.y != 2 && block.y != 6 && block.y != 7 && block.y != 8 && block.y != 9 && block.y != 10 && block.y != 15 && block.y != 16))
                        || (block.x == 9 && (block.y != 7 && block.y != 8 && block.y != 9 && block.y != 10 && block.y != 11))
                            )
                {
                    block.isBlank = true;
                    blockObj.isBlank = true;
                    blockObj.blockImg.gameObject.SetActive(false);
                    listObjBlock.Add(blockObj);
                }
                else
                {
                    block.isBlank = false;
                    blockObj.isBlank = false;
                    listObjBlock.Add(blockObj);
                    blockActive.Add(blockObj);
                }

                blockObj.blockID = block.id;
                blockObj.x = block.x;
                blockObj.y = block.y;
                block.blockObj = blockObj;
            }
        }



        public void SetBlankBlock(BlockObj blockObj)
        {
            //block.isBlank = true;
            blockObj.isBlank = true;
            blockObj.blockImg.gameObject.SetActive(false);
            listObjBlock.Add(blockObj);
        }


        public void SetImgBlockSpawn()
        {
            int index = Random.Range(0, bgImg.Count - 1);
            Sprite bg = bgImg[index];
            foreach (BlockObj block in this.listObjBlock)
            {
                if (block.isBlank)
                {
                    continue;
                }
                else
                {
                    Sprite blockImg = SetImg();
                    block.SetData(blockImg,bg);
                    block.blockName = blockImg.name;
                }

            }
        }

        public Sprite SetImg()
        {
            int index = Random.Range(0, blockContentImg.Count - 1);
            Sprite img = blockContentImg[index];
            blockContentImg.RemoveAt(index);
            return img;
        }
        #endregion


        #region FIND NEIGHBORS  
        public void FindBlockObjsNeighbors()
        {

            int x, y;
            foreach (BlockObj block in this.listObjBlock)
            {
                x = block.x;
                y = block.y;

                block.neighbors.Add(this.GetBlockByXY(x, y + 1)); // up
                block.up = this.GetBlockByXY(x, y + 1);
                block.neighbors.Add(this.GetBlockByXY(x + 1, y)); // right
                block.right = this.GetBlockByXY(x + 1, y);
                block.neighbors.Add(this.GetBlockByXY(x, y - 1)); // down
                block.down = this.GetBlockByXY(x, y - 1);
                block.neighbors.Add(this.GetBlockByXY(x - 1, y)); // left
                block.left = this.GetBlockByXY(x - 1, y);

            }



            foreach (Block block in this.listBlocks)
            {
                if (block.blockObj.isBlank == true) return;
                block.blockObj.neighbors.Add(block.up.blockObj);
                block.blockObj.neighbors.Add(block.right.blockObj);
                block.blockObj.neighbors.Add(block.down.blockObj);
                block.blockObj.neighbors.Add(block.left.blockObj);
            }
        }

        public virtual BlockObj GetBlockByXY(int x, int y)
        {
            BlockObj blockNeighbor = null;
            foreach (BlockObj block in this.listObjBlock)
            {
                if (block.x == x && block.y == y) blockNeighbor = block;
                else if (x < 0 || x > width + 1 || y < 0 || y > height + 1)
                    blockNeighbor = null;
            }

            return blockNeighbor;
        }
        #endregion


        #region LINK OBJ

        public bool canLink = false;

        public List<BlockObj> lineX1 = new();
        public List<BlockObj> lineY1 = new();

        public List<BlockObj> lineX2 = new();
        public List<BlockObj> lineY2 = new();

        public List<BlockObj> lineXCheck = new();


        public void AddPivotPoint(BlockObj node1, BlockObj node2, BlockObj node3, BlockObj node4, BlockObj node5)
        {
            listBlockLine.Clear();
            if (node1 != null) listBlockLine.Add(node1);
            if (node2 != null) listBlockLine.Add(node2);    
            if (node3 != null) listBlockLine.Add(node3);
            if (node4 != null) listBlockLine.Add(node4);
            if (node5 != null) listBlockLine.Add(node5);
        }

        public void CheckPos()
        {
            img1 = blockSelect1.blockImg.gameObject;
            img2 = blockSelect2.blockImg.gameObject;
            line.gameObject.SetActive(false);
            listBlockLine.Clear();

        

            lineX1 = listObjBlock.Where(t => t.y == blockSelect1.x).ToList();
            lineY1 = listObjBlock.Where(t => t.x == blockSelect1.y).ToList();


            pivot1 = null;
            pivot2 = null;
            pivotTemp = null;


            if (blockSelect1.blockName == blockSelect2.blockName)
            {
                canLink = CheckLineI(blockSelect1,blockSelect2);

                if (!canLink)
                {
                    canLink = CheckingLinkL(blockSelect1, blockSelect2);
                }

                if (!canLink)
                {

                    canLink = CheckingLinkU(blockSelect1, blockSelect2);
                }

                if(!canLink)
                {
                    pivot1 = null;
                    pivot2 = null;
                    pivotTemp = null;
                    canLink = CheckingLineZ(blockSelect1, blockSelect2);
                }

                if(!canLink)
                {
                    pivot1 = null;
                    pivot2 = null;
                    pivotTemp = null;
                    canLink = CheckingLineZ(blockSelect2, blockSelect1);
                    AddPivotPoint(blockSelect2, pivot1,pivotTemp, pivot2, blockSelect1);
                }
            }
            else canLink = false;

            if (canLink)
            {
                if (listBlockLine.Count == 0) AddPivotPoint(blockSelect1, pivot1, pivotTemp , pivot2, blockSelect2);
            }

            DrawLink();
        }

        public BlockObj pivot1 =null;
        public BlockObj pivot2 = null;
        public BlockObj pivotTemp =null;

    
        //check blank block thang hang giua 2 diem start va end
        public bool CheckLineI(BlockObj start, BlockObj end)
        {
            int dis1 = start.x - end.x;
            int dis2 = start.y - end.y;

            List<BlockObj> listTemp = new List<BlockObj>();
            //BlockObj pivotTemp = null;

            if (dis1 == 0 && (dis2 == -1 || dis2 == 1)) canLink = true;
            else if(dis2 == 0 && (dis1 == -1 || dis1 == 1)) canLink = true;
            else
            {
                if(dis1 == 0)
                {
                    if (dis2 < -1)  listTemp = listObjBlock.Where(t => t.x == start.x && t.y >= start.y && t.y <= end.y).ToList();
                    else if(dis2 > 1) listTemp = listObjBlock.Where(t => t.x == start.x && t.y <= start.y && t.y >= end.y).ToList();

                    foreach (var item in listTemp)
                    {
                        if (item != start && item != end) canLink = item.isBlank;
                        else continue;
                        if (!canLink) break;
                    }
                
                }
                if(dis2 == 0)
                {
                    if (dis1 < -1) listTemp = listObjBlock.Where(t => t.y == start.y && t.x >= start.x && t.x <= end.x).ToList();
                    else if (dis1 > 1) listTemp = listObjBlock.Where(t => t.y == start.y && t.x <= start.x && t.x >= end.x).ToList();

                    foreach (var item in listTemp)
                    {
                        if (item != start && item != end) canLink = item.isBlank;
                        else continue;
                        if (!canLink) break;
                    }
                }
            }
            return canLink;
        }
   
        public bool CheckingLinkU(BlockObj start, BlockObj end)
        {
            List<BlockObj> listTemp1 = new ();
            List<BlockObj> listTemp2 = new ();

            int pivotIndex = 0;
            int pivotStartIndex = 0;
            BlockObj pivotBlock = null;

            if (start.left.isBlank && end.left.isBlank)
            {
                listTemp1 = listObjBlock.Where(t => t.y == start.y && t.x < start.x).ToList(); 
                listTemp2 = listObjBlock.Where(t => t.y == end.y && t.x < end.x).ToList();

                if(start.x < end.x)
                {
                    for (int i = end.left.x-1; i >= 0; i--)
                    {
                        if (listTemp2[i].isBlank)
                        {
                            pivotBlock = listTemp2[i];
                        }
                        else break;
                    }
                    if(pivotBlock == null) canLink = false;
                    else
                    {
                            if (pivotBlock.x > start.left.x) canLink = false;
                            else
                            {
                                for(int i=start.left.x; i>=pivotBlock.x; i--)
                                {
                                    if (listTemp1[i].isBlank)
                                    {
                                        canLink = CheckLineI(listTemp1[i], listTemp2[i]);
                                        if(canLink)
                                        {
                                            pivot1 = listTemp1[i];
                                            pivot2 = listTemp2[i];
                                            break;
                                        }
                                    }
                                    else canLink = false;                            
                                }
                            }
                    }
                
                }
                if (start.x > end.x)
                {
                    for (int i = start.left.x-1; i >= 0; i--)
                    {
                        if (listTemp1[i].isBlank)
                        {
                            pivotBlock = listTemp1[i];
                        }
                        else break;
                    }

                    if (pivotBlock == null) canLink = false;
                    else
                    {
                        if (pivotBlock.x > end.left.x) canLink = false;
                        else
                        {
                            for (int i = end.left.x; i >= pivotBlock.x; i--)
                            {
                                if (listTemp2[i].isBlank)
                                {
                                    canLink = CheckLineI(listTemp2[i], listTemp1[i]);
                                    if (canLink)
                                    {
                                        pivot1 = listTemp1[i];
                                        pivot2 = listTemp2[i];
                                        break;
                                    }
                                }
                                else canLink = false;
                            }
                        }
                    }

                }
                if(start.x == end.x)
                {
                    for (int i = start.left.x; i >= 0; i--)
                    {
                        if (listTemp1[i].isBlank && listTemp2[i].isBlank)
                        {
                            canLink = CheckLineI(listTemp2[i], listTemp1[i]);
                            if (canLink)
                            {
                                pivot1 = listTemp1[i];
                                pivot2 = listTemp2[i];
                                break;
                            }
                        }
                        else
                        {
                            canLink = false;
                            break;
                        }
                    }
                }
            }

            if(!canLink)
            {
                if (start.right.isBlank && end.right.isBlank)
                {
                    listTemp1 = listObjBlock.Where(t => t.y == start.y && t.x > start.x).ToList();
                    listTemp2 = listObjBlock.Where(t => t.y == end.y && t.x > end.x).ToList();

                    if (start.x < end.x)
                    {
                        for (int i = 0; i<listTemp1.Count;i++)
                        {
                            if (listTemp1[i].x == end.right.x)
                                pivotStartIndex = i;
                            if (listTemp1[i].isBlank)
                            {
                                pivotBlock = listTemp1[i];
                                pivotIndex = i;
                            }
                            else break;
                        }

                        if (pivotBlock == null) canLink = false;
                        else
                        {
                            if (pivotBlock.x < end.right.x) canLink = false;
                            else
                            {
                                for (int i = 0; i < listTemp2.Count; i++)
                                {
                                    if (listTemp2[i].isBlank)
                                    {
                                        canLink = CheckLineI(listTemp1[pivotStartIndex], listTemp2[i]);
                                        if (canLink)
                                        {
                                            pivot1 = listTemp1[pivotStartIndex];
                                            pivot2 = listTemp2[i];
                                            break;
                                        }
                                        pivotStartIndex++;
                                    }
                                    else canLink = false;
                                }
                            }
                        }

                    }
                    if (start.x > end.x)
                    {
                        for (int i = 0; i < listTemp2.Count; i++)
                        {
                            if (listTemp2[i].x == start.right.x)
                                pivotStartIndex = i;
                            if (listTemp2[i].isBlank)
                            {
                                pivotBlock = listTemp2[i];
                                pivotIndex = i;
                            }
                            else break;
                        }

                        if (pivotBlock == null) canLink = false;
                        else
                        {
                            if (pivotBlock.x < start.right.x) canLink = false;
                            else
                            {
                                for (int i = 0; i < listTemp1.Count; i++)
                                {
                                    if (listTemp1[i].isBlank)
                                    {
                                        canLink = CheckLineI(listTemp2[pivotStartIndex], listTemp1[i]);
                                        if (canLink)
                                        {
                                            pivot1 = listTemp1[i];
                                            pivot2 = listTemp2[pivotStartIndex];
                                            break;
                                        }
                                        pivotStartIndex++;
                                    }
                                    else canLink = false;
                                }
                            }
                        }
                
                    }
                    if (start.x == end.x)
                    {
                        for (int i = 0; i < listTemp1.Count; i++)
                        {
                            if (listTemp1[i].isBlank && listTemp2[i].isBlank)
                            {
                                canLink = CheckLineI(listTemp2[i], listTemp1[i]);
                                if (canLink)
                                {
                                    pivot1 = listTemp1[i];
                                    pivot2 = listTemp2[i];
                                    break;
                                }
                            }
                            else
                            {
                                canLink = false;
                                break;
                            }
                        }
                    }
                }
            }

            if (!canLink)
            {
                if (start.up.isBlank && end.up.isBlank)
                {
                    listTemp1 = listObjBlock.Where(t => t.x == start.x && t.y > start.y).ToList();
                    listTemp2 = listObjBlock.Where(t => t.x == end.x && t.y > end.y).ToList();

                    if (start.y < end.y)
                    {
                        for (int i = 0; i < listTemp1.Count; i++)
                        {
                            if (listTemp1[i].y == end.up.y)
                                pivotStartIndex = i;
                            if (listTemp1[i].isBlank)
                            {
                                pivotBlock = listTemp1[i];
                                pivotIndex = i;
                            }
                            else break;
                        }

                        if (pivotBlock == null) canLink = false;
                        else
                        {
                            if (pivotBlock.y < end.up.y) canLink = false;
                            else
                            {
                                for (int i = 0; i < listTemp2.Count; i++)
                                {
                                    if (listTemp2[i].isBlank)
                                    {
                                        canLink = CheckLineI(listTemp1[pivotStartIndex], listTemp2[i]);
                                        if (canLink)
                                        {
                                            pivot1 = listTemp1[pivotStartIndex];
                                            pivot2 = listTemp2[i];
                                            break;
                                        }
                                        pivotStartIndex++;
                                    }
                                    else canLink = false;
                                }
                            }
                        }
                
                    }
                    if (start.y > end.y)
                    {
                        for (int i = 0; i < listTemp2.Count; i++)
                        {
                            if (listTemp2[i].y == start.up.y)
                                pivotStartIndex = i;
                            if (listTemp2[i].isBlank)
                            {
                                pivotBlock = listTemp2[i];
                                pivotIndex = i;
                            }
                            else break;
                        }

                        if (pivotBlock == null) canLink = false;
                        else
                        {
                            if (pivotBlock.y < start.up.y) canLink = false;
                            else
                            {
                                for (int i = 0; i < listTemp1.Count; i++)
                                {
                                    if (listTemp1[i].isBlank)
                                    {
                                        canLink = CheckLineI(listTemp2[pivotStartIndex], listTemp1[i]);
                                        if (canLink)
                                        {
                                            pivot1 = listTemp1[i];
                                            pivot2 = listTemp2[pivotStartIndex];
                                            break;
                                        }
                                        pivotStartIndex++;
                                    }
                                    else canLink = false;
                                }
                            }
                        }

                    }
                    if (start.y == end.y)
                    {
                        for (int i = 0; i < listTemp1.Count; i++)
                        {
                            if (listTemp1[i].isBlank && listTemp2[i].isBlank)
                            {
                                canLink = CheckLineI(listTemp2[i], listTemp1[i]);
                                if (canLink)
                                {
                                    pivot1 = listTemp1[i];
                                    pivot2 = listTemp2[i];
                                    break;
                                }
                            }
                            else
                            {
                                canLink = false;
                                break;
                            }
                        }
                    }
                }
            }
           
            if(!canLink)
            {
                if (start.down.isBlank && end.down.isBlank)
                {
                    listTemp1 = listObjBlock.Where(t => t.x == start.x && t.y < start.y).ToList();
                    listTemp2 = listObjBlock.Where(t => t.x == end.x && t.y < end.y).ToList();

                    if (start.y < end.y)
                    {
                        for (int i = end.down.y; i >= 0; i--)
                        {
                            if (listTemp2[i].isBlank)
                            {
                                pivotBlock = listTemp2[i];
                            }
                            else break;
                        }

                        if (pivotBlock == null) canLink = false;
                        else
                        {
                            if (pivotBlock.y > start.down.y) canLink = false;
                            else
                            {
                                for (int i = start.down.y; i >= pivotBlock.y; i--)
                                {
                                    if (listTemp1[i].isBlank)
                                    {
                                        canLink = CheckLineI(listTemp1[i], listTemp2[i]);
                                        if (canLink)
                                        {
                                            pivot1 = listTemp1[i];
                                            pivot2 = listTemp2[i];
                                            break;
                                        }
                                    }
                                    else canLink = false;
                                }
                            }
                        }

                    }
                    if (start.y > end.y)
                    {
                        for (int i = start.down.y; i >= 0; i--)
                        {
                            if (listTemp1[i].isBlank)
                            {
                                pivotBlock = listTemp1[i];
                            }
                            else break;
                        }

                        if (pivotBlock == null) canLink = false;
                        else
                        {
                            if (pivotBlock.y > end.down.y) canLink = false;
                            else
                            {
                                for (int i = end.down.y; i >= pivotBlock.y; i--)
                                {
                                    if (listTemp2[i].isBlank)
                                    {
                                        canLink = CheckLineI(listTemp2[i], listTemp1[i]);
                                        if (canLink)
                                        {
                                            pivot1 = listTemp1[i];
                                            pivot2 = listTemp2[i];
                                            break;
                                        }
                                    }
                                    else canLink = false;
                                }
                            }
                        }

                    }
                    if (start.y == end.y)
                    {
                        for (int i = start.down.y; i >= 0; i--)
                        {
                            if (listTemp1[i].isBlank && listTemp2[i].isBlank)
                            {
                                canLink = CheckLineI(listTemp2[i], listTemp1[i]);
                                if (canLink)
                                {
                                    pivot1 = listTemp1[i];
                                    pivot2 = listTemp2[i];
                                    break;
                                }
                            }
                            else
                            {
                                canLink = false;
                                break;
                            }
                        }
                    }
                }
            }
        

            return canLink;
        }

        public bool CheckingLineZ(BlockObj start, BlockObj end)
        {
            int dis1 = start.x - end.x;
            int dis2 = start.y - end.y;

            BlockObj pivotObj1;
            BlockObj pivotObj2;

            if (dis1 != 0 && dis2 != 0)
            {
                if (start.y > end.y)
                {
                    if (start.x < end.x)
                    {
                        pivotObj1 = start.down;
                        pivotObj2 = end.up;
                        for(int i=end.y; i< start.y;i++)
                        {
                            if (pivotObj1.isBlank && pivotObj2.isBlank)
                            {
                                if (pivotObj1.y == pivotObj2.y) canLink = CheckLineI(pivotObj1, pivotObj2);
                                else canLink = CheckingLinkL(pivotObj1, pivotObj2);
                                if (canLink)
                                {
                                    if (pivotObj1.y == pivotObj2.y)
                                    {
                                        pivot1 = pivotObj1;
                                        pivot2= pivotObj2;
                                    }
                                    else
                                    {

                                        pivotTemp = pivot1;

                                        pivot1 = pivotObj1;
                                        pivot2 = pivotObj2;
                                    }

                                    break;  
                                }
                                else
                                {
                                    pivotObj1 = pivotObj1.down;
                                    pivotObj2 = pivotObj2.up;
                                }
                            }
                            else canLink = false;
                        }

                        if(!canLink)
                        {
                            pivotObj1 = start.right;
                            pivotObj2 = end.left;
                            for (int i = start.x; i < end.x; i++)
                            {
                                if (pivotObj1.isBlank && pivotObj2.isBlank)
                                {
                                    if (pivotObj1.x == pivotObj2.x) canLink = CheckLineI(pivotObj1, pivotObj2);
                                    else canLink = CheckingLinkL(pivotObj1, pivotObj2);
                                    if (canLink)
                                    {
                                        if (pivotObj1.x == pivotObj2.x)
                                        {
                                            pivot1 = pivotObj1;
                                            pivot2 = pivotObj2;
                                        }
                                        else
                                        {

                                            pivotTemp = pivot1;

                                            pivot1 = pivotObj1;
                                            pivot2 = pivotObj2;
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        pivotObj1 = pivotObj1.right;
                                        pivotObj2 = pivotObj2.left;
                                    }
                                }
                                else canLink = false;
                            }
                        }
                    }

                    if(start.x > end.x)
                    {
                        pivotObj1 = start.down;
                        pivotObj2 = end.up;
                        for (int i = end.y; i < start.y; i++)
                        {
                            if (pivotObj1.isBlank && pivotObj2.isBlank)
                            {
                                if (pivotObj1.y == pivotObj2.y) canLink = CheckLineI(pivotObj1, pivotObj2);
                                else canLink = CheckingLinkL(pivotObj1, pivotObj2);
                                if (canLink)
                                {
                                    if (pivotObj1.y == pivotObj2.y)
                                    {
                                        pivot1 = pivotObj1;
                                        pivot2 = pivotObj2;
                                    }
                                    else
                                    {
                                        pivotTemp = pivot1;

                                        pivot1 = pivotObj1;
                                        pivot2 = pivotObj2;
                                    }

                                    break;
                                }
                                else
                                {
                                    pivotObj1 = pivotObj1.down;
                                    pivotObj2 = pivotObj2.up;
                                }
                            }
                            else canLink = false;
                        }
                        if (!canLink)
                        {
                            pivotObj1 = start.left;
                            pivotObj2 = end.right;
                            for (int i = start.x; i < end.x; i++)
                            {
                                if (pivotObj1.isBlank && pivotObj2.isBlank)
                                {
                                    if (pivotObj1.x == pivotObj2.x) canLink = CheckLineI(pivotObj1, pivotObj2);
                                    else canLink = CheckingLinkL(pivotObj1, pivotObj2);
                                    if (canLink)
                                    {
                                        if (pivotObj1.x == pivotObj2.x)
                                        {
                                            pivot1 = pivotObj1;
                                            pivot2 = pivotObj2;
                                        }
                                        else
                                        {
                                            pivotTemp = pivot1;

                                            pivot1 = pivotObj1;
                                            pivot2 = pivotObj2;
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        pivotObj1 = pivotObj1.left;
                                        pivotObj2 = pivotObj2.right;
                                    }
                                }
                                else canLink = false;
                            }
                        }
                    }
                }

                if(start.y < end.y)
                {
                    if (start.x < end.x)
                    {
                        pivotObj1 = start.up;
                        pivotObj2 = end.down;
                        for (int i = start.y; i < end.y; i++)
                        {
                            if (pivotObj1.isBlank && pivotObj2.isBlank)
                            {
                                if (pivotObj1.y == pivotObj2.y) canLink = CheckLineI(pivotObj1, pivotObj2);
                                else canLink = CheckingLinkL(pivotObj1, pivotObj2);
                                if (canLink)
                                {
                                    if (pivotObj1.y == pivotObj2.y)
                                    {
                                        pivot1 = pivotObj1;
                                        pivot2 = pivotObj2;
                                    }
                                    else
                                    {
                                        pivotTemp = pivot1;

                                        pivot1 = pivotObj1;
                                        pivot2 = pivotObj2;
                                    }

                                    
                                    break;
                                }
                                else
                                {
                                    pivotObj1 = pivotObj1.up;
                                    pivotObj2 = pivotObj2.down;
                                }
                            }
                            else canLink = false;
                        }

                        if (!canLink)
                        {
                            pivotObj1 = start.right;
                            pivotObj2 = end.left;
                            for (int i = start.x; i < end.x; i++)
                            {
                                if (pivotObj1.isBlank && pivotObj2.isBlank)
                                {
                                    if(pivotObj1.x == pivotObj2.x) canLink = CheckLineI(pivotObj1, pivotObj2);
                                    else canLink = CheckingLinkL(pivotObj1, pivotObj2);
                                    if (canLink)
                                    {
                                        if (pivotObj1.x == pivotObj2.x)
                                        {
                                            pivot1 = pivotObj1;
                                            pivot2 = pivotObj2;
                                        }
                                        else
                                        {
                                            pivotTemp = pivot1;

                                            pivot1 = pivotObj1;
                                            pivot2 = pivotObj2;
                                        }
                                        break;
                                    }
                                    else
                                    {
                                       pivotObj1 = pivotObj1.right;
                                       pivotObj2 = pivotObj2.left;
                                    }
                                }
                                else canLink = false;
                            }
                        }
                    }

                    if (start.x > end.x)
                    {
                        pivotObj1 = start.up;
                        pivotObj2 = end.down;
                        for (int i = start.y; i < end.y; i++)
                        {
                            if (pivotObj1.isBlank && pivotObj2.isBlank)
                            {
                                if (pivotObj1.y == pivotObj2.y) canLink = CheckLineI(pivotObj1, pivotObj2);
                                else canLink = CheckingLinkL(pivotObj1, pivotObj2);
                                if (canLink)
                                {
                                    if (pivotObj1.y == pivotObj2.y)
                                    {
                                        pivot1 = pivotObj1;
                                        pivot2 = pivotObj2;
                                    }
                                    else
                                    {
                                        pivotTemp = pivot1;

                                        pivot1 = pivotObj1;
                                        pivot2 = pivotObj2;
                                    }

                                    break;
                                }
                                else
                                {
                                    pivotObj1 = pivotObj1.down;
                                    pivotObj2 = pivotObj2.up;
                                }
                            }
                            else canLink = false;
                        }
                        if (!canLink)
                        {
                            pivotObj1 = start.left;
                            pivotObj2 = end.right;
                            for (int i = end.x; i < start.x; i++)
                            {
                                if (pivotObj1.isBlank && pivotObj2.isBlank)
                                {
                                    if (pivotObj1.x == pivotObj2.x) canLink = CheckLineI(pivotObj1, pivotObj2);
                                    else canLink = CheckingLinkL(pivotObj1, pivotObj2);
                                    if (canLink)
                                    {
                                        if (pivotObj1.x == pivotObj2.x)
                                        {
                                            pivot1 = pivotObj1;
                                            pivot2 = pivotObj2;
                                        }
                                        else
                                        {
                                            pivotTemp = pivot1;

                                            pivot1 = pivotObj1;
                                            pivot2 = pivotObj2;
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        pivotObj1 = pivotObj1.left;
                                        pivotObj2 = pivotObj2.right;
                                    }
                                }
                                else canLink = false;
                            }
                        }
                    }
                }








            }

        




            return canLink;
        }

        public bool CheckingLinkL(BlockObj start, BlockObj end)
        {
            int dis1 = start.x - end.x;
            int dis2 = start.y - end.y;

            BlockObj pivotTemp1;
            BlockObj pivotTemp2;

            if(dis1 != 0 && dis2 != 0)
            {
                pivotTemp1 = listObjBlock.Find(t => t.x == start.x && t.y == end.y);
                pivotTemp2 = listObjBlock.Find(t => t.x == end.x && t.y == start.y);

                if (pivotTemp1.isBlank)
                {
                    canLink = CheckLineI(start, pivotTemp1);
                    if (canLink) canLink = CheckLineI(end, pivotTemp1);

                    if (canLink)
                    {
                        pivot1 = pivotTemp1;
                        pivot2 = pivotTemp1;
                    }
                }
                else canLink = false;
            
                if(!canLink)
                {
                    if(pivotTemp2.isBlank)
                    {
                        canLink = CheckLineI(start, pivotTemp2);
                        if (canLink) canLink = CheckLineI(end, pivotTemp2);

                        if (canLink)
                        {
                            pivot1 = pivotTemp2;
                            pivot2 = pivotTemp2;
                        }
                    }
                    else canLink = false;
                }
            }

            return canLink;
        }

        public void DrawLink()
        {
            if (canLink)
            {
                blockAmount -= 2;
               // listBlockLine.Insert(0, blockSelect2);
                line.SetActive(true);
                if (listBlockLine.Count > 0)
                {
                    Vector3[] blockPos = new Vector3[listBlockLine.Count];

                    for (int i = 0; i < listBlockLine.Count; i++)
                    {
                        blockPos[i] = listBlockLine[i].transform.position;
                    }

                    DrawLine.Instance.DrawLink(blockPos);
                }

                StartCoroutine(HideBlocks(LevelFinish));

            }
            else ResetBlockSelect();
        
            canLink = false;
           // listBlockLine.Clear();
   
        }


        public bool isLose = false;
        public bool isWin = false;
        public void CheckWinLose()
        {
            if(blockAmount == 0 && currentTime >= 0)
            {
                isWin = true;
            }
            if(currentTime <= 0 && blockAmount >0)
            {
                isLose = true;

            }
            if (isLose) popupLose.SetActive(true);
        }


        public void LevelFinish(bool isEndAnim)
        {
            if(isWin && isEndAnim) popupWin.SetActive(true);
            if(isLose && isEndAnim) popupLose.SetActive(true);
        }


        public void ResetBlockSelect()
        {
            if(blockSelect1 != null && blockSelect2!= null)
            {
                blockSelect2.blockImg.color = Color.white;
                blockSelect1.blockImg.color = Color.white;

                blockSelect1 = null;
                blockSelect2 = null;
            }
        }


        public void ResetBlock()
        {
            StartCoroutine(HideAnim());

            blockSelect1.isBlank = true;
           // img1.SetActive(false);

            blockSelect2.isBlank = true;
            // img2.SetActive(false);

            //img1 = null;
            // img2 = null;

            blockSelect1 = null;
            blockSelect2 = null;
        }

        IEnumerator HideBlocks(Action<bool> callback)
        {

            yield return new WaitForSeconds(.2f);

            line.SetActive(false);
            ResetBlock();

            yield return new WaitForSeconds(2.4f);
            callback(true);
        }
        #endregion



        public ParticleSystem disappearEff;
        public IEnumerator HideAnim()
        {
            Vector3 pos1 = blockSelect1.gameObject.transform.position;
            Vector3 pos2 = blockSelect2.gameObject.transform.position;
            blockSelect1.blockImg.transform.DOScale(1.2f, 0.1f);
            blockSelect2.blockImg.transform.DOScale(1.2f, 0.1f);
            blockSelect1.blockImg.transform.DOScale(0f, 0.2f);
            blockSelect2.blockImg.transform.DOScale(0f, 0.2f);



            yield return new WaitForSeconds(.2f);
            var eff = Instantiate(disappearEff, pos1, Quaternion.identity);
            var eff1 = Instantiate(disappearEff, pos2, Quaternion.identity);

           // eff.Play();
           // eff1.Play();


        }
    }


    public enum MapType
    {
        Normal,
        Lion,
        Dragon,
        Baron,
        Falcon
    }
}
