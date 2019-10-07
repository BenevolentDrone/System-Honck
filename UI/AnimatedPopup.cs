using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class AnimatedPopup : MonoBehaviour
{
    [SerializeField]
    private bool leftToRight;
    public bool LeftToRight { get { return leftToRight; } set { leftToRight = value; } }

    [SerializeField]
    private float transitionDuration;

    [SerializeField]
    private float stayDuration;
    public float StayDuration { get { return stayDuration; } set { stayDuration = value; } }

    RectTransform rectTransform;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void OnEnable()
    {
        rectTransform.SetPivot(leftToRight ? new Vector2(0f, 0.5f) : new Vector2(1f, 0.5f));
        
        rectTransform.localScale = new Vector3(0f, 1f, 1f);
        
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(rectTransform.DOScaleX(1f, transitionDuration));
        
        sequence.InsertCallback(transitionDuration + stayDuration, SlideBack);
        
        sequence.InsertCallback(transitionDuration + stayDuration + transitionDuration, Hide);
    }
    
    private void SlideBack()
    {
        rectTransform.SetPivot(leftToRight ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f));
        
        rectTransform.DOScaleX(0f, transitionDuration);
    }
    
    private void Hide()
    {
        rectTransform.SetPivot(new Vector2(0.5f, 0.5f));

        gameObject.SetActive(false);
    }
}
