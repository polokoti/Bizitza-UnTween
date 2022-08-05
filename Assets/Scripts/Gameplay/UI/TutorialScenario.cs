using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TutorialScenario : MonoBehaviour
{
    public GameObject secondTitle;
    public Image handCursor;
    public Image patternToDrawHorizontal;
    public Image patternToDrawVertical;

    [SerializeField] private Vector2 drawingHorizontalStartPoint;
    [SerializeField] private Vector2 drawingHorizontalEndPoint;
    [SerializeField] private Vector2 drawingVerticalStartPoint;
    [SerializeField] private Vector2 drawingVerticalEndPoint;
    [SerializeField] private Vector2 startPoint;
    [SerializeField] private Vector2 finishPoint;

    public bool isHorizontalDraw = false;
    public bool isVerticalDraw = false;

    private Sequence horizontalDraw;
    private Sequence verticalDraw;

    public Animator TutorialAnim;

    // Start is called before the first frame update
    void Start()
    {
        handCursor.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (!isHorizontalDraw)
        {
            TutorialAnim.SetTrigger("First Scene");

            isHorizontalDraw = true;
            LevelManager.isLevelStarted = true;
        }
        else if (isHorizontalDraw && !isVerticalDraw)
        {
            TutorialAnim.SetTrigger("Second Scene");
            secondTitle.SetActive(true);

            isVerticalDraw = true;
        }
    }

    private void OnDisable()
    {
        if (horizontalDraw.IsPlaying() && isHorizontalDraw)
        {
            //horizontalDraw.Complete();
            //horizontalDraw.Kill();
            //patternToDrawHorizontal.DOFade(1f, 0f);
            //patternToDrawHorizontal.rectTransform.DOScaleX(0f, 0f);
            handCursor.enabled = false;
        }

        else if (verticalDraw.IsPlaying() && isVerticalDraw)
        {
            //verticalDraw.Complete();
            //verticalDraw.Kill();
            //patternToDrawVertical.DOFade(1f, 0f);
            //patternToDrawVertical.rectTransform.DOScaleY(1f, 0f);
            handCursor.enabled = false;
        }
    }

    public void HideTutorialPanel()
    {
        gameObject.SetActive(false);
    }

    public void ShowTutorialPanel()
    {
        gameObject.SetActive(true);
    }
}
