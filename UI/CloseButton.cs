using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    [SerializeField]
    private Button button;
    
    [SerializeField]
    private GameObject window;
    
    void Awake()
    {
        button.onClick.AddListener(OnClick);
    }
    
    void OnClick()
    {
        window.SetActive(false);
        
        foreach (Transform child in window.transform)
            if (child.GetComponent<Inventory>() != null)
                child.gameObject.SetActive(false);
    }
}