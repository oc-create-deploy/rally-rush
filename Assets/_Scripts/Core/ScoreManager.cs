

using System;
using _Scripts.Controllers;
using _Scripts.Utilities;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    // LEVEL END PARAMETERS
    public Transform point1Location;
    public Transform point100Location;
    public TMP_Text upperScoreText;
    public TextMeshProUGUI endGameScoreText;

    public int bestScore;
    public int lastScore;
    private void Start() { bestScore = PlayerPrefs.GetInt("rallyBestScore", 0); }

    private float CalculateLevelEndBonus(GameObject boss)
    {
        float zDiff = point100Location.transform.position.z - point1Location.transform.position.z;
        float pointPerLocation = zDiff / Constants.MAX_SCORE;
        float bossTransformZ = boss.transform.position.z;
        return Mathf.FloorToInt(Mathf.Clamp((bossTransformZ - point1Location.position.z) / Mathf.Max(pointPerLocation, .001f), 0, Constants.MAX_SCORE));
    }

    public void EndCrowdRelayScore(int earnedReward)
    {
        lastScore=CorridorController.Instance.score+earnedReward;
        bestScore=Mathf.Max(bestScore,lastScore);
        PlayerPrefs.SetInt("rallyBestScore",bestScore);PlayerPrefs.Save();
        endGameScoreText.text=lastScore+" / BEST "+bestScore+"\nCROWD RELAY +"+earnedReward;
    }

    public void EndGamePopupScore(GameObject boss)
    {
        lastScore = Mathf.RoundToInt(CorridorController.Instance.score + CalculateLevelEndBonus(boss));
        bestScore = Mathf.Max(bestScore, lastScore);
        PlayerPrefs.SetInt("rallyBestScore", bestScore);
        PlayerPrefs.Save();
        endGameScoreText.text = lastScore + " / BEST " + bestScore;
    }
}
