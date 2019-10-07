using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SUPERHONKAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject super;
    
    [SerializeField]
    private GameObject honk;
    
    void OnEnable()
    {
        SUPER();
        
        Sequence sequence = DOTween.Sequence();
        
        sequence.InsertCallback(1.5f, HONK);
        sequence.InsertCallback(3f, SUPER);
        sequence.SetLoops(-1);
    }
    
    private void SUPER()
    {
        super.SetActive(true);
        honk.SetActive(false);
        
        super.transform.localScale = new Vector3(1f, 1.2f, 1f);
        super.transform.DOScaleY(1f, 0.7f);
    }
    
    private void HONK()
    {
        super.SetActive(false);
        honk.SetActive(true);
        
        honk.transform.localScale = new Vector3(1f, 1.2f, 1f);
        honk.transform.DOScaleY(1f, 0.7f);
    }
}
