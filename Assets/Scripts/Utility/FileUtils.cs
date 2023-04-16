using UnityEngine;

public static class FileUtils
{
    public static LevelInfo LoadLevel(int levelNumber)
    {
        return Resources.Load<LevelInfo>($"Levels/{levelNumber}");
    }
    public static Texture LoadTexture(string fileName)
    {
        return Resources.Load($"Editor/{fileName}") as Texture;
    }
    public static GameObject LoadCollectableObjects(string fileName)
    {
        return Resources.Load($"CollectableObjects/{fileName}") as GameObject;
    }
}
