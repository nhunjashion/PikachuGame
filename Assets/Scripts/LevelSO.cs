using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "LevelID", menuName = "ScriptableObject/Level", order = 2)]
public class LevelSO : ScriptableObject
{
    public int id;
    public int level;
    public int time;
    public MapType mapType;

    public int width;
    public int height;

    public int pokemonCount;

}
