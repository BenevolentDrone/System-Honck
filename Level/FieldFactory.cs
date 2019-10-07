using UnityEngine;
using UnityEngine.AI;

public class FieldFactory : MonoBehaviour
{
    [SerializeField]
    private Texture2D tilesMap;
    
    [SerializeField]
    private TileColorLegend tileColorLegend;
    
    [SerializeField]
    private GameObject tilePrefab;
    
    [SerializeField]
    private Texture2D itemsMap;
    
    [SerializeField]
    private PrefabColorLegend prefabColorLegend;
    
    [SerializeField]
    private TileField tileField;
    
    [SerializeField]
    private float maxColorDistance;
    
    [SerializeField]
    private WorldContext worldContext;

    [SerializeField]
    private GameObject navMesh;

    [SerializeField]
    private GameObject obstaclePrefab;
    
    void Awake()
    {
        for (int y = 0; y < tilesMap.height; y++)
            for (int x = 0; x < tilesMap.width; x++)
            {
                Color pixel = tilesMap.GetPixel(x, y);

                if (new Vector3(pixel.r, pixel.g, pixel.b).magnitude < maxColorDistance)
                {
                    var targetPosition = tileField.ToVector2Coordinates(x, y);

                    var obstacleInstance = GameObject.Instantiate(
                            obstaclePrefab,
                            new Vector3(targetPosition.x, targetPosition.y, 0f),
                            Quaternion.identity) as GameObject;

                    continue;
                }

                foreach (var pair in tileColorLegend.Data)
                {
                    Color targetColor = pair.Color;
                    
                    Vector3 distance = new Vector3(
                        targetColor.r - pixel.r,
                        targetColor.g - pixel.g,
                        targetColor.b - pixel.b);
                    
                    if (distance.magnitude < maxColorDistance)
                    {
                        var targetPosition = tileField.ToVector2Coordinates(x, y);
                        
                        var tileInstance = GameObject.Instantiate(
                            tilePrefab, 
                            new Vector3(targetPosition.x, targetPosition.y, 0f),
                            Quaternion.identity) as GameObject;
                        
                        var tileComponent = tileInstance.GetComponent<Tile>();
                        
                        tileComponent.Coordinates = tileField.ToIntCoordinates(x, y);
                        
                        tileComponent.Surface = pair.Surface;
                        
                        tileField.AddTile(tileComponent);
                        
                        break;
                    }
                }
            }
        
        for (int y = 0; y < itemsMap.height; y++)
            for (int x = 0; x < itemsMap.width; x++)
            {
                Color pixel = itemsMap.GetPixel(x, y);
                
                foreach (var pair in prefabColorLegend.Data)
                {
                    Color targetColor = pair.Color;
                    
                    Vector3 distance = new Vector3(
                        targetColor.r - pixel.r,
                        targetColor.g - pixel.g,
                        targetColor.b - pixel.b);
                    
                    if (distance.magnitude < maxColorDistance)
                    {
                        var targetPosition = tileField.ToVector2Coordinates(x, y);
                        
                        var prefabInstance = GameObject.Instantiate(
                            pair.Prefab, 
                            new Vector3(targetPosition.x, targetPosition.y, 0f),
                            Quaternion.identity) as GameObject;
                        
                        prefabInstance.GetComponent<Entity>().WorldContext = worldContext;
                        
                        break;
                    }
                }
            }

        Vector2 furthestTileCoordinates = tileField.ToVector2Coordinates(tilesMap.width, tilesMap.height);

        Vector3 center = new Vector3(furthestTileCoordinates.x / 2f, 0f, furthestTileCoordinates.y / 2f);

        Vector2 sizeVector2 = tileField.ToVector2Coordinates(tilesMap.width + 1, tilesMap.height + 1);

        Vector3 size = new Vector3(sizeVector2.x, 0.1f, sizeVector2.y);

        var navMeshBox = navMesh.GetComponent<BoxCollider>();

        navMeshBox.center = center;

        navMeshBox.size = size;

        var navMeshComponent = navMesh.GetComponent<NavMeshSurface>();

        navMeshComponent.BuildNavMesh();
    }
}
