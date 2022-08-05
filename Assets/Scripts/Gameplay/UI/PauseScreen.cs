using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    public GameObject Pause_Menu;

    bool GameIsPaused = false;

    public GameObject backgroundMusic;

    [SerializeField] private float pausePanelEndPoint = 0f;

    public Image pauseMenuBG;
    public RectTransform pauseMenuPanel;

    private Sequence pauseMenuSequence;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        //Pause_Menu.SetActive(true);
        Time.timeScale = 0f;

        //pauseMenuSequence = DOTween.Sequence().SetAutoKill(false).SetUpdate(true);
        //pauseMenuSequence.Append(pauseMenuBG.DOFade(1f, 0.3f)).SetUpdate(true);
        //pauseMenuSequence.Join(pauseMenuPanel.DOLocalMoveY(pausePanelEndPoint, 0.3f).SetEase(Ease.OutQuint)).SetUpdate(true);

        FMODUnity.StudioEventEmitter audioEvent = backgroundMusic.GetComponent<FMODUnity.StudioEventEmitter>();
        audioEvent.EventInstance.setVolume(0.3f);

    }

    public void Resume()
    {
        StartCoroutine(Delay());
        //pauseMenuSequence.PlayBackwards();
        Time.timeScale = 1f;

        FMODUnity.StudioEventEmitter audioEvent = backgroundMusic.GetComponent<FMODUnity.StudioEventEmitter>();
        audioEvent.EventInstance.setVolume(1f);
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.4f);
        //Pause_Menu.SetActive(false);
    }
}
