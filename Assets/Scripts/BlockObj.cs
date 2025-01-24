using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PikachuGame
{
    public class BlockObj : MonoBehaviour
    {
        [Header("Block infor")]
        public int blockID = 0;
        public string blockName;
        public Image blockImg;
        public Image contentImg;
        public bool isBlank;

   
        [Header("Position in field")]
        public int x = 0;
        public int y = 0;

        [Header("Neighbors")]
        public List<BlockObj> neighbors = new List<BlockObj>();
        public BlockObj up; // 0
        public BlockObj right; // 1
        public BlockObj down; // 2
        public BlockObj left; // 3

        public void SetData(Sprite content)       
        {
            contentImg.sprite = content;
            contentImg.SetNativeSize();
        }


        public void OnClickBlock()
        {
            SoundManager.Instance.PlaySfx(SoundName.Click);
            Color colorSelect = new Color32(229,153,153,225);
            this.blockImg.color = colorSelect;

            if(GridManager.Instance.blockSelect1 == null)
            {
                GridManager.Instance.blockSelect1 = this;
            }
            else if(GridManager.Instance.blockSelect2 == null)
            {
                if(GridManager.Instance.blockSelect1.blockID == this.blockID)
                {
                    this.blockImg.color = Color.white;
                    GridManager.Instance.blockSelect1 = null;
                    GridManager.Instance.blockSelect2 = null;
                }
                else
                {
                    GridManager.Instance.blockSelect2 = this;

                    GridManager.Instance.CheckPos();
                }

            }
            else
            {
                GridManager.Instance.blockSelect1 = this;
                GridManager.Instance.blockSelect2 = null;
            }  
        }

        public bool Checking(BlockDirection direction, BlockObj end)
        {
            switch (direction)
            {
                case BlockDirection.Left:
                    if (left == null ) return false;
                    if (left != null && !left.isBlank && left.blockName != end.blockName) return false;
                    if (left == end)
                    {
                        GridManager.Instance.listBlockLine.Add(this);
                        return true;
                    
                    }
                   // bool isChecking = false;

                    if (GridManager.Instance.stepCount < 2)
                    {
                        if (left.y < end.y)
                        {
                            if (left.Checking(BlockDirection.Up, end))
                            {
                                if (GridManager.Instance.stepCount+1 >2 ) return false;
                                return AddNode();
                            }
                        }
                        else if (left.y > end.y)
                        {
                            if (left.Checking(BlockDirection.Down, end))
                            {
                                if (GridManager.Instance.stepCount + 1 > 2) return false;
                                return AddNode();

                            }
                        }
                        if (left.Checking(BlockDirection.Left, end))
                        {
                            GridManager.Instance.listBlockLine.Add(this);
                            return true;
                        }
                    }
                    else return false;
                


                    return false;


                case BlockDirection.Right:
                    if (right == null ) return false;
                    if (right != null && !right.isBlank && right.blockName != end.blockName) return false;
                    if (right == end)
                    {
                        GridManager.Instance.listBlockLine.Add(this);
                        return true;
                    }

                    if (GridManager.Instance.stepCount < 2)
                    {
                        if (right.y < end.y)
                        {
                            if (right.Checking(BlockDirection.Up, end))
                            {
                                if (GridManager.Instance.stepCount + 1 > 2) return false;
                                return AddNode();


                            }
                        }
                        else if (right.y > end.y)
                        {
                            if (right.Checking(BlockDirection.Down, end))
                            {
                                if (GridManager.Instance.stepCount + 1 > 2) return false;
                                return AddNode();

                            }
                        }

                        if (right.Checking(BlockDirection.Right, end))
                        {
                            GridManager.Instance.listBlockLine.Add(this);
                            return true;
                        }
                    }
                    else return false;
                    

                    return false;


                case BlockDirection.Up:
                    if(up==null ) return false;
                    if (up != null && !up.isBlank && up.blockName != end.blockName) return false;
                    if (up == end)
                    {
                        GridManager.Instance.listBlockLine.Add(this);
                        return true;
                    }

                    if (GridManager.Instance.stepCount < 2)
                    {
                   
                            if (up.x < end.x)
                            {

                                if (up.Checking(BlockDirection.Right, end))
                                {
                                if (GridManager.Instance.stepCount + 1 > 2) return false;
                                return AddNode();
                                }
                            }
                            else if (up.x > end.x)
                            {
                                if (up.Checking(BlockDirection.Left, end))
                                {
                                if (GridManager.Instance.stepCount + 1 > 2) return false;
                                return AddNode();

                                }
                            }
                        if (up.Checking(BlockDirection.Up, end))
                        {
                            GridManager.Instance.listBlockLine.Add(this);
                            return true;
                        }
                    }
                    else return false;
                    return false;

                case BlockDirection.Down:
                    if (down == null ) return false;
                    if (down != null && !down.isBlank && down.blockName != end.blockName) return false;
                    if (down == end)
                    {
                        GridManager.Instance.listBlockLine.Add(this);
                        return true;
                    }

                    if (GridManager.Instance.stepCount < 2)
                    {
                        if (down.x < end.x)
                        {
                            if (down.Checking(BlockDirection.Right, end))
                            {
                                if (GridManager.Instance.stepCount + 1 > 2) return false;
                                return AddNode();
                            }
                        }
                        else if (down.x > end.x)
                        {
                            if (down.Checking(BlockDirection.Left, end))
                            {
                                if (GridManager.Instance.stepCount + 1 > 2) return false;
                                return AddNode();
                            }
                        }
                        if (down.Checking(BlockDirection.Down, end))
                        {
                            GridManager.Instance.listBlockLine.Add(this);
                            return true;
                        }                   
                    }
                    else return false;
                    return false;
            }
            return true;

        }

         bool AddNode()
            {
            GridManager.Instance.stepCount++;

            GridManager.Instance.listBlockLine.Add(this);
            return true;
            }



        public enum BlockDirection
        {
            Up, 
            Right,
            Down,
            Left
        }
    }

    public class ResultChecking
    {
        public bool isLink;
        public int step;
    }

}
