using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private string itemName;
    public string ItemName { get { return itemName; } }
    
    [SerializeField]
    [EnumFlags]
    private SlotCategories acceptableSlots;
    public SlotCategories AcceptableSlots { get { return acceptableSlots; } }
    
    [SerializeField]
    private Camera uiCamera;
    public Camera UICamera { get { return uiCamera; } set { uiCamera = value; } }
    
    [SerializeField]
    private RectTransform rectTransform;
    
    [SerializeField]
    private EventSystem eventSystem;
    public EventSystem EventSystem { get { return eventSystem; } set { eventSystem = value; } }
    
    [SerializeField]
    private GraphicRaycaster graphicsRaycaster;
    public GraphicRaycaster GraphicsRaycaster { get { return graphicsRaycaster; } set { graphicsRaycaster = value; } }
    
    public Slot Slot;
    
    public void OnPointerUp(PointerEventData eventData)
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        
        pointerEventData.position = Input.mousePosition;
        
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        
        graphicsRaycaster.Raycast(pointerEventData, raycastResults);
        
        for (int i = 0; i < raycastResults.Count; i++)
        {
            var targetSlot = raycastResults[i].gameObject.GetComponent<Slot>();
            
            if (targetSlot != null)
            {
                if (targetSlot.Item != null)
                    continue;
                
                if (((int)targetSlot.Category & (int)acceptableSlots) != (int)targetSlot.Category)
                    continue;
                
                targetSlot.Item = this;
                
                Slot.Item = null;
                
                Slot = targetSlot;
                
                transform.SetParent(targetSlot.transform, true);
                
                rectTransform.anchoredPosition = Vector2.zero;
                
                return;
            }
        }
        
        transform.position = Slot.transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 result;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, uiCamera, out result);
        
        rectTransform.position = rectTransform.TransformPoint(result);
        
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f);
    }
}