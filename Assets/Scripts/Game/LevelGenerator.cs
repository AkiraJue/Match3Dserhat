using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : Singleton<LevelGenerator>
{
    private LevelInfo levelData; // ScriptableObject that hold current level data.
    [SerializeField]private List<GameObject> levelObjects; // Objects that will be collectable in this level.
    public List<GameObject> objectsOnScene; // All collectable objects with its count on scene.
    private void Awake()
    {
        InitializeLevel(); // Get asset then create level.
    }
    public void InitializeLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        UIManager.Instance.SetLevelText("Level "+currentLevel);
        if (currentLevel > 10) // I will create 10 level for the CaseStudy. Then repeat levels.
        {
            currentLevel = Random.Range(1, 11);
        }
        GetLevelData(currentLevel);
    }
    private void GetLevelData(int levelNum) // Get levelData which we created from Level Editor.
    {
        levelData = FileUtils.LoadLevel(levelNum);
        LoadLevelObject();
    }
    private void LoadLevelObject() // Loads the objects to be used in this level.
    {
        for (int i = 0; i < levelData.ObjectsOnLevel.Count; i++)
        {
            for (int j = 0; j < levelData.ObjectsOnLevel[i].objectCount; j++)
            {
                levelObjects.Add(FileUtils.LoadCollectableObjects(levelData.ObjectsOnLevel[i].objectType.ToString()));
            }
        }
        CreateLevel();
    }
    private void CreateLevel() // Instantiate level objects.
    {
        for (int i = 0; i < levelObjects.Count; i++)
        {
            objectsOnScene.Add(Instantiate(levelObjects[i], CreateRandomPosition(), levelObjects[i].transform.rotation));
        }
        GameManager.Instance.currentLevelTime = levelData.TimeToFinish;
        StartCoroutine(GameManager.Instance.DelayedGameStart(1f));
        
    }
    public Vector3 CreateRandomPosition() // Create random position in Game Area.
    {
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f,1.5f);
        Vector3 spawnPosition = new Vector3(randomX, 2.4f, randomZ);
        return spawnPosition;
    }
    private void DestroyLevel()
    {
        for (int i = 0; i < objectsOnScene.Count; i++)
        {
            Destroy(objectsOnScene[i]);
        }
        levelData = null;
        objectsOnScene.Clear();
        levelObjects.Clear();
        InputController.Instance.arrivedObject.Clear();
        InputController.Instance.objectsOnStack.Clear();
        UIManager.Instance.timerText.text = "_____";
    }
    public void RestartLevel()
    {
        DestroyLevel();
        InitializeLevel();
    }
}
