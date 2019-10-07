using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SurfaceSettingsDatabase", menuName = "Database/Surface settings database", order = 1)]
public class SurfaceSettingsDatabase : ScriptableObject
{
    private Dictionary<SurfaceTypes, SurfaceSettings> database;
    
    public SurfaceSettings[] data;
    
    public void Initialize()
    {
        database = new Dictionary<SurfaceTypes, SurfaceSettings>();
        
        foreach (var settings in data)
            database.Add(settings.Name, settings);
    }
    
    public SurfaceSettings GetSurfaceSettings(SurfaceTypes type)
    {
        if (database == null)
            Initialize();
        
        return database[type];
    }
}