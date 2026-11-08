using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestScore : MonoBehaviour
{
  private Text uiText;

  void Awake()
  {
    uiText = GetComponent<Text>();
  }

  public void DisplayBestScoreValue(int value)
  {
    uiText.text = "Best Score: " + value.ToString("#,0");
  }
}
