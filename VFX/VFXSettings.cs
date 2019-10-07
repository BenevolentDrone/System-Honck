using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "VFXSettings", menuName = "Settings/VFX settings", order = 1)]
public class VFXSettings : ScriptableObject
{
    public VFXSettingsEntry[] VFX;
}