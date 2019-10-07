using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class AnimatedFlyingPopup : MonoBehaviour
{
    [SerializeField]
    private float transitionDuration;

    [SerializeField]
    private float stayDuration;
    public float StayDuration { get { return stayDuration; } set { stayDuration = value; } }
    
    [SerializeField]
    private TextMeshPro textComponent;
    
    [SerializeField]
    private float pathLength;
    
    RectTransform rectTransform;
    
    Color transparent = new Color(0f, 0f, 0f, 0f);
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        textComponent.color = transparent;
    }
    
    public void OnEnable()
    {
        Sequence sequence = DOTween.Sequence();
        
        textComponent.color = transparent;
        
        sequence.Append(textComponent.DOColor(Color.white, transitionDuration));
        
        sequence.Join(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + pathLength, transitionDuration + stayDuration + transitionDuration));
        
        sequence.InsertCallback(transitionDuration + stayDuration, FadeBack);
        
        sequence.InsertCallback(transitionDuration + stayDuration + transitionDuration, Hide);
    }
    
    private void FadeBack()
    {
        textComponent.DOColor(transparent, transitionDuration);
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
