using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;
    
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    [SerializeField]
    private SurfaceSettingsDatabase settingsDatabase;
    
    //[SerializeField]
    //private TileField tileField;
    
    public int Coordinates;
    
    [SerializeField]
    private SurfaceTypes surface;
    public SurfaceTypes Surface
    {
        get { return surface; }
        set
        {
            surface = value;
            
            var currentSurfaceSettings = settingsDatabase.GetSurfaceSettings(value);
            
            spriteRenderer.sprite = currentSurfaceSettings.Sprite;
            
            spriteRenderer.color = currentSurfaceSettings.Color;
        }
    }
    
    public List<Entity> Entities = new List<Entity>();
    
    /*
    void Start()
    {
        Coordinates = tileField.ToIntCoordinates(rectTransform.anchoredPosition);
        
        Surface = surface;
        
        tileField.AddTile(this);
    }
    */
}