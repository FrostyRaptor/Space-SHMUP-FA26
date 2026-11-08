using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
  private BestScore bestScore;
  private Text uiText;

  [Header("Dynamic")]
  public int score = 0;

  static public int bestScoreValue = 0;

  void Start()
  {
    uiText = GetComponent<Text>();
  }

  public void UpdateScore()
  {
    uiText.text = "Score: " + score.ToString("#,0");

    this.UpdateBestScoreValue(score);
  }

  public void UpdateBestScoreValue(int value)
  {
    if (value > bestScoreValue)
    {
      bestScoreValue = value;
    }
  }

  public void ResetBestScoreValue()
  {
    bestScoreValue = 0;
  }

  public void DisplayBestScore()
  {
    GameObject bestScoreGO = GameObject.Find("UIText_BestScore");
    bestScore = bestScoreGO.GetComponent<BestScore>();

    bestScore.DisplayBestScoreValue(bestScoreValue);
  }
}
