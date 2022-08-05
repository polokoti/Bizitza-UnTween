using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using FMODUnity;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Level levelConfig;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject bossPrefab;
    [SerializeField] RuntimeAnimatorController enemyAnimator;
    [SerializeField] RuntimeAnimatorController bossAnimator;

    public static int levelStatus = 0;

    public float pushPower = 5f;
    public static int enemyCount = 0;

    public Image blackScreen;
    public static bool isLevelStarted = false;
    public static bool isLevelFailed = false;
    public static bool isTransitioningTo = false;

    public GameObject gameStartPanel;
    public Text startCountdown;

    public GameObject levelOverPanel;
    public Image scoreBox;
    public Text currentScore;
    public Text bonusScore;
    public Text totalScore;
    public Text clickAnywhereText;

    public GameObject gameOverPanel;
    public Text gameOverText;
    public Image retryBg;
    public Text retryText;
    public Image mainMenuBg;
    public Text mainMenuText;

    public GameObject scorePanel;
    public GameObject healthPanel;
    public GameObject pauseButton;

    public int currentHealths;

    public GameObject levelBackgroundSound;
    Coroutine levelBackgroundCoroutine;
    bool isLevelFinished = false;

    public GameObject levelCompleteSound;
    Coroutine levelCompleteCoroutine;

    public GameObject levelFailedSound;
    Coroutine levelFailedCoroutine;

    private float scoreBoxEndPoint;

    private int initCurrentScore = 0;
    private int initBonusScore = 0;
    private int initTotalScore = 0;

    private GestureRecognizer.DrawDetector drawDetector;

    private GameObject player;

    public GameObject BlackScreenFade;

    public Animator GameStartCountDown;
    public Animator LevelOverScene;
    public Animator GameOverScene;
    public Animator MainMenuFade;

    // Start is called before the first frame update
    void Start()
    {
        scoreBoxEndPoint = scoreBox.rectTransform.anchoredPosition.y;
        scoreBox.rectTransform.DOAnchorPosY(scoreBoxEndPoint + 800, 0f);

        drawDetector = FindObjectOfType<GestureRecognizer.DrawDetector>();
        player = GameObject.Find("Player");

        isLevelStarted = false;
        isLevelFailed = false;
        isTransitioningTo = false;

        Sequence initSeq = DOTween.Sequence();

        if (SceneManager.GetActiveScene().buildIndex == CurrentLevelStatic.levelTrainingBuildIndex)
        {
            initSeq
                .Append(blackScreen.DOFade(0f, 2f))
                .AppendCallback(() => {
                    gameStartPanel.SetActive(false);
                    StartCoroutine(StartLevel(levelConfig));
                });
        }
        
        else
        {
            GameStartCountDown.Play("Base Layer.Game Start Count");
            Invoke("DelayStartCount", 5f);
        }

        if (SceneManager.GetActiveScene().buildIndex == CurrentLevelStatic.levelTrainingBuildIndex)
        {
            scorePanel.SetActive(false);
            healthPanel.SetActive(false);
            pauseButton.SetActive(false);
        }
    }
    public void DelayStartCount()
    {
        gameStartPanel.SetActive(false);
        StartCoroutine(StartLevel(levelConfig));
        drawDetector.enabled = true;
    }
     
    IEnumerator StartLevel(Level level)
    {
        levelStatus = 1;
        levelBackgroundCoroutine = StartCoroutine(PlayLevelBackgroundSong());

        if (SceneManager.GetActiveScene().buildIndex == CurrentLevelStatic.levelTrainingBuildIndex)
        {
            isLevelStarted = false;
        }
        else 
        { 
            isLevelStarted = true; 
        }
        
        if (level.GetWaves() != null)
        {
            var waves = level.GetWaves();
            for (int i = 0; i < waves.Count; i++)
            {
                WaveConfig wave = waves[i];
                List<EnemyBehaviour> enemies = wave.GetEnemies();
                float timeToStart = wave.GetTimeToStartWave();
                float timeBtwnSpawn = wave.GetTimeBtwnSpawn();

                yield return new WaitForSeconds(timeToStart);

                for (int l = 0; l < enemies.Count; l++)
                {
                    Vector2 enemySpawnPoint = enemies[l].GetSpawnPoint();

                    GameObject enemy = null;

                    if (enemies[l].GetEnemyAnimator() == enemyAnimator)
                    {
                        enemy = Instantiate(enemyPrefab, new Vector3(enemySpawnPoint.x, enemySpawnPoint.y), Quaternion.identity);
                        enemy.GetComponentInChildren<EnemyController>().AssignEnemyBehaviour(enemies[l]);
                        enemyCount++;
                        
                        yield return new WaitForSeconds(timeBtwnSpawn);

                    } else if (enemies[l].GetEnemyAnimator() == bossAnimator)
                    {
                        enemy = Instantiate(bossPrefab, new Vector3(enemySpawnPoint.x, enemySpawnPoint.y), Quaternion.identity);
                        enemy.GetComponentInChildren<EnemyController>().AssignEnemyBehaviour(enemies[l]);
                        enemyCount++;

                        var bossController = enemy.GetComponentInChildren<EnemyController>();
                        var bossPatternHandler = enemy.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<EnemyPatternHandler>();
                        bossController.enemyType = "Boss";

                        yield return new WaitUntil(() => bossPatternHandler.enemyPatterns.Count <= 0);
                        yield return new WaitForSeconds(0.3f);
                        enemy.transform.DOMoveX(12f, 0.2f);
                        yield return new WaitForSeconds(0.3f);
                        Destroy(enemy);
                    }
                }

                yield return new WaitUntil(() => enemyCount == 0);
            }
        }

        if(level.GetBossWave() != null)
        {
            levelStatus = 2;
            yield return new WaitForSeconds(1f);
            if (SceneManager.GetActiveScene().buildIndex == 4) // Level 1
            {
                player.transform.DOMoveX(player.transform.position.x - 6, 2f);
            }
            else if (SceneManager.GetActiveScene().buildIndex == 5) // Level 2
            {
                player.transform.DOMoveX(player.transform.position.x - 6, 2f);
            }
            yield return new WaitForSeconds(2f);

            var bossWave = level.GetBossWave();
            List<EnemyBehaviour> bosses = bossWave.GetEnemies();
            float timeToStartBoss = bossWave.GetTimeToStartWave();
            float timeBtwnSpawnBoss = bossWave.GetTimeBtwnSpawn();

            yield return new WaitForSeconds(timeToStartBoss);

            Vector2 bossSpawnPoint = bosses[0].GetSpawnPoint();
            var boss = Instantiate(bossPrefab, new Vector3(bossSpawnPoint.x, bossSpawnPoint.y), Quaternion.identity);
            var bossController = boss.GetComponentInChildren<EnemyController>();
            var bossPatternHandler = boss.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<EnemyPatternHandler>();
            bossController.enemyType = "Boss";
            enemyCount++;

            for (int i = 0; i < bosses.Count; i++)
            {
                bossController.AssignEnemyBehaviour(bosses[i]);
                if (i < bosses.Count-1)
                {
                    Debug.Log("Waiting boss hit player");
                    yield return new WaitUntil(() => bossPatternHandler.enemyPatterns.Count <= 0);
                    boss.transform.DOMoveX(8f, 0.2f);
                }
            }
            print("Waiting for boss to die");
            yield return new WaitUntil(() => enemyCount == 0);
        }

        yield return new WaitForSeconds(2f);
        isLevelFinished = true;

        yield return new WaitUntil(() => isLevelFailed == false);
        if (SceneManager.GetActiveScene().buildIndex == CurrentLevelStatic.levelTrainingBuildIndex)
        {
            levelStatus = 3;
            ScoreUnit.score = 0;
            Sequence finishLevel = DOTween.Sequence()
                .Append(blackScreen.DOFade(1f, 1f))
                .AppendInterval(1f)
                .OnComplete(() =>
                {
                    DOTween.KillAll();
                    CurrentLevelStatic.lastBuildIndex = SceneManager.GetActiveScene().buildIndex;
                    ScoreCounter.addScoreDelegate = null;
                    ScoreCounter.resetScoreMultiplierDelegate = null;
                    SceneManager.LoadScene(CurrentLevelStatic.loadingSceneBuildIndex);
                });
        }
        else
        {
            StartCoroutine(PlayLevelCompleteSong());
            ScoreUnit.score = ScoreCounter.score;
            scorePanel.SetActive(false);
            healthPanel.SetActive(false);
            pauseButton.SetActive(false);
            levelOverPanel.SetActive(true);

            int toMaxScore = initCurrentScore + ScoreCounter.score;
            initCurrentScore = toMaxScore;
            int toMaxBonusScore = initBonusScore + (currentHealths * 500);
            initBonusScore = toMaxBonusScore;
            int toMaxTotalScore = initTotalScore + initCurrentScore + initBonusScore;
            initTotalScore = toMaxTotalScore;
            ScoreUnit.score = toMaxTotalScore;

            LevelOverScene.Play("ScoreBoard in");

            yield return new WaitForSeconds(6f);
            yield return new WaitUntil(() => Input.GetKey(KeyCode.Mouse0));

            Sequence finishLevelSeq = DOTween.Sequence();
            finishLevelSeq
                .Append(currentScore.rectTransform.DOScale(0f, 1f).SetEase(Ease.OutExpo))
                .Join(bonusScore.rectTransform.DOScale(0f, 1f).SetEase(Ease.OutExpo))
                .Join(totalScore.rectTransform.DOScale(0f, 1f).SetEase(Ease.OutExpo))
                .Join(scoreBox.rectTransform.DOAnchorPosY(scoreBoxEndPoint + 800, 1f).SetEase(Ease.OutExpo))
                .Join(clickAnywhereText.rectTransform.DOScale(0f, 1f).SetEase(Ease.OutExpo))
                .Join(clickAnywhereText.DOFade(0f, 1f).SetEase(Ease.OutExpo))
                .Append(blackScreen.DOFade(1f, 1f))
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    DOTween.KillAll();
                    CurrentLevelStatic.lastBuildIndex = SceneManager.GetActiveScene().buildIndex;
                    StopAllCoroutines();
                    ScoreCounter.addScoreDelegate = null;
                    ScoreCounter.resetScoreMultiplierDelegate = null;
                    SceneManager.LoadScene(CurrentLevelStatic.loadingSceneBuildIndex);
                });
        }
    }

    private void Update()
    {
        print(initCurrentScore);
        //print("Enemy Count: " + enemyCount);
        currentScore.text = "Score: " + initCurrentScore.ToString();
        bonusScore.text = "Bonus: " + initBonusScore.ToString();
        totalScore.text = "Total\n" + initTotalScore.ToString();
        if (FindObjectOfType<HealthCounter>() != null)
        {
            currentHealths = FindObjectOfType<HealthCounter>().gameObject.transform.childCount;
        }
    }

    public void ResetGame()
    {
        isTransitioningTo = true;
        ScoreCounter.addScoreDelegate = null;
        ScoreCounter.resetScoreMultiplierDelegate = null;
        isLevelFailed = true;
        isLevelFinished = true;
        DOTween.Sequence()
            .Append(blackScreen.DOFade(1f, 1f))
            .AppendInterval(1f)
            .OnComplete(() =>
            {
                DOTween.KillAll();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        
    }

    public void GoToMainMenu()
    {
        BlackScreenFade.SetActive(true);
        Time.timeScale = 1f;
        isTransitioningTo = true;
        ScoreCounter.addScoreDelegate = null;
        ScoreCounter.resetScoreMultiplierDelegate = null;
        isLevelFailed = true;
        isLevelFinished = true;
        MainMenuFade.Play("BaseLayer.Fade To Black");
        Invoke("FadeToBlack", 1f);
    }
    
    public void FadeToBlack()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(2);
    }
    
    public void PlayGameOverAnimation()
    {
        retryBg.gameObject.GetComponent<Button>().interactable = false;
        mainMenuBg.gameObject.GetComponent<Button>().interactable = false;
        gameOverPanel.SetActive(true);
        GameOverScene.Play("GameOver in");
        retryBg.gameObject.GetComponent<Button>().interactable = true;
        mainMenuBg.gameObject.GetComponent<Button>().interactable = true;
    }

    IEnumerator PlayLevelBackgroundSong()
    {
        var studioEventEmitter = levelBackgroundSound.GetComponent<StudioEventEmitter>();
        studioEventEmitter.EventInstance.setVolume(1f);
        levelBackgroundSound.SetActive(true);
        yield return new WaitUntil(() => isLevelFinished || isLevelFailed);
        for (float i = 1f; i > 0f; i -= 0.1f)
        {
            if (i < 0f) 
                i = 0f;
            studioEventEmitter.EventInstance.setVolume(i);
            yield return new WaitForSeconds(0.1f);
        }
        levelBackgroundSound.SetActive(false);
    }

    IEnumerator PlayLevelCompleteSong()
    {
        var studioEventEmitter = levelBackgroundSound.GetComponent<StudioEventEmitter>();
        studioEventEmitter.EventInstance.setVolume(1f);
        levelCompleteSound.SetActive(true);
        yield return null;
    }
}
