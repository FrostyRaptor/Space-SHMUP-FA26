using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCounter : MonoBehaviour
{
  [Header("Inscribed")]
  [SerializeField] private GameObject[] lifeSprites;

  private int remainingLives;
  public int maxLives => lifeSprites.Length;

  public void StartLives(int lives)
  {
    remainingLives = Mathf.Clamp(lives, 0, maxLives);
    UpdateLivesDisplay();
  }

  public int LoseLife()
  {
    if (remainingLives > 0)
    {
      remainingLives--;
      UpdateLivesDisplay();
    }
    return remainingLives;
  }

  public void UpdateLivesDisplay()
  {
    for (int i = 0; i < lifeSprites.Length; i++)
    {
      lifeSprites[i].SetActive(i < remainingLives);
    }
  }

  public int GetLives()
  {
    return remainingLives;
  }
}
