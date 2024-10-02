using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PikachuGame
{
    [Serializable]
    public class Block
    { 
        public int id; 

        public int x = 0;
        public int y = 0;

        public bool isBlank = true;

        public BlockObj blockObj;

        [Header("Block neighbor")]
        public Block up;
        public Block right;
        public Block down;
        public Block left;

    }
}

