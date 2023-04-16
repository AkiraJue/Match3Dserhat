using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "LevelInfo", menuName = "Create Level", order = 1)]
public class LevelInfo : ScriptableObject
{
    public int LevelNumber; // Which level is this.
    public int TypeCount; // How many objects does this level contains.
    public float TimeToFinish;// Given time to finish level.
    public List<CollectableObject> ObjectsOnLevel; // The list that we hold object for this level.
}