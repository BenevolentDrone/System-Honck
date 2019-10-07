using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class H0nckerComponent : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    
    [SerializeField]
    private Animator animator;
    
    [SerializeField]
    private GameObject honkMessage;
    
    public void Honk()
    {
        animator.SetTrigger("Honk");
        
        //mainCamera.transform.DOShakePosition(0.5f, new Vector3(0.2f, 0.2f, 0f), 15);
        
        honkMessage.SetActive(true);
    }
    
    /*
    void Update()
    {
        
    }
    
    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.W))
    }
    */
}
