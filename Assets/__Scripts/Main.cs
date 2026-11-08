using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
  static private Main S;
  static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;
  static private int remainingLives = -1;

  private BoundsCheck bndCheck;
  private LifeCounter lifeCounter;
  private ScoreCounter scoreCounter;
  private GameObject scoreCounterGO;

  [Header("Inscribed")]
  public bool spawnEnemies = true;
  public GameObject[] prefabEnemies;
  public float enemySpawnPerSecond = 0.5f;
  public float enemyInsetDefault = 1.5f;
  public float gameRestartDelay = 2;
  public GameObject prefabPowerUp;
  public WeaponDefinition[] weaponDefinitions;
  public eWeaponType[] powerUpFrequency = new eWeaponType[]
  {
    eWeaponType.blaster, eWeaponType.blaster, eWeaponType.spread, eWeaponType.shield
  };

  [Header("GameOver Screen")]
  public GameObject gameOverScreen;

  void Awake()
  {
    S = this;

    bndCheck = GetComponent<BoundsCheck>();

    GameObject lifeCounterGO = GameObject.Find("UIImage_LifeParent");
    lifeCounter = lifeCounterGO.GetComponent<LifeCounter>();

    scoreCounterGO = GameObject.Find("UIText_Score");
    scoreCounter = scoreCounterGO.GetComponent<ScoreCounter>();

    if (scoreCounter == null)
    {
      Debug.Log("scoreCounter wasn't found.");
    }

    if (remainingLives < 0)
    {
      remainingLives = lifeCounter.maxLives;
    }

    lifeCounter.StartLives(remainingLives);

    if (gameOverScreen != null)
    {
      gameOverScreen.SetActive(false);
    }

    Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);

    WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
    foreach (WeaponDefinition def in weaponDefinitions)
    {
      WEAP_DICT[def.type] = def;
    }
  }

  public void SpawnEnemy()
  {
    if (!spawnEnemies)
    {
      Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
      return;
    }

    int ndx = Random.Range(0, prefabEnemies.Length);
    GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

    float enemyInset = enemyInsetDefault;
    if (go.GetComponent<BoundsCheck>() != null)
    {
      enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
    }

    Vector3 pos = Vector3.zero;
    float xMin = -bndCheck.camWidth + enemyInset;
    float xMax = bndCheck.camWidth - enemyInset;
    pos.x = Random.Range(xMin, xMax);
    pos.y = bndCheck.camHeight + enemyInset;
    go.transform.position = pos;

    Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
  }

  void DelayedRestart()
  {
    Invoke(nameof(Restart), gameRestartDelay);
  }

  void Restart()
  {
    SceneManager.LoadScene("__Scene_0");
  }

  static public void HERO_DIED()
  {
    if (S == null) return;

    remainingLives = S.lifeCounter.LoseLife();

    if (remainingLives > 0)
    {
      S.DelayedRestart();
    }
    else
    {
      S.ShowGameOver();
    }
  }

  private void ShowGameOver()
  {
    spawnEnemies = false;
    CancelInvoke(nameof(SpawnEnemy));

    if (gameOverScreen != null)
    {
      gameOverScreen.SetActive(true);
      scoreCounter.DisplayBestScore();

      scoreCounterGO.SetActive(false);
    }
    else
    {
      Debug.Log("GameOver Screen has not been assigned");
    }
  }

  public void StartNewGame()
  {
    remainingLives = -1;
    scoreCounter.ResetBestScoreValue();
    SceneManager.LoadScene("__Scene_0");
  }

  /// <summary>
  /// Static function that gets a WeaponDefinition from the WEAP_DICT static
  ///   protected field of the Main class.
  /// </summary>
  /// <returns>The WeaponDefinition, or if there is no WeaponDefinition with
  ///   the eWeaponType passed in, returns a new WeaponDefinition with a 
  ///   eWeaponType of eWeaponType.none</returns>
  /// <param name="wt">The eWeaponType of the  desired WeeaponDefinition</param>
  static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
  {
    if (WEAP_DICT.ContainsKey(wt))
    {
      return WEAP_DICT[wt];
    }
    return new WeaponDefinition();
  }

  /// <summary>
  /// Called by an Enemy ship whenever it is destroyed. It sometimes creates
  ///   a PowerUp in place of the destroyed ship.
  /// </summary>
  /// <param name="e">The Enemy that was destroyed</param>
  static public void SHIP_DESTROYED(Enemy e)
  {
    if (Random.value <= e.powerUpDropChance)
    {
      int ndx = Random.Range(0, S.powerUpFrequency.Length);
      eWeaponType pUpType = S.powerUpFrequency[ndx];

      GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
      PowerUp pUp = go.GetComponent<PowerUp>();
      pUp.SetType(pUpType);

      pUp.transform.position = e.transform.position;
    }
  }

}
