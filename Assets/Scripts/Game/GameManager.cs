using System;
using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameStatus
    {
        PLAY,
        FAIL,
        FINISH
    }
    public GameStatus gameStatus;
    public float currentLevelTime;

    void Update()
    {
        DecreaseTimer();
        CheckIfGameEnd();
    }
    private void DecreaseTimer()
    {
        if (GameIsPlaying() && !BoosterManager.Instance.timeFreezed)
        {
            currentLevelTime -= Time.deltaTime;
            TimeSpan duration = TimeSpan.FromSeconds(currentLevelTime);
            UIManager.Instance.SetTimerText(duration);
            if (currentLevelTime <= 0)
            {
                UIManager.Instance.ShowTimeIsUpPopUp();
            }
        }
    }
    private void CheckIfGameEnd()
    {
        if (LevelGenerator.Instance.objectsOnScene.Count == 0)
        {
            UIManager.Instance.ShowNextlevelPopUp();
        }
    }
    public IEnumerator DelayedGameStart(float t)
    {
        yield return new WaitForSeconds(t);
        gameStatus = GameStatus.PLAY;
        BoosterManager.Instance.BoostersInteractibility(true);
    }
    public bool GameIsPlaying()
    {
        return gameStatus == GameStatus.PLAY? true:false;
    }
}
