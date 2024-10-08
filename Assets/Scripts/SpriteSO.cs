using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PikachuGame
{
    [CreateAssetMenu(fileName = "ListSpriteName", menuName = "ScriptableObject/SpriteData", order = 1)]
    public class SpriteSO : ScriptableObject
    {
        public List<Sprite> sprites = new List<Sprite>();
    }
}

