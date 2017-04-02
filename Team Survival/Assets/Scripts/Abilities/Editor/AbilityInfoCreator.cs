using UnityEngine;
using UnityEditor;

public class AbilityInfoCreator
{
    [MenuItem("Abilities/Create/New Ability Info")]
    public static void CreateMyAsset()
    {
        AbilityInfo asset = ScriptableObject.CreateInstance<AbilityInfo>();

        AssetDatabase.CreateAsset(asset, "Assets/Scripts/Abilities/AbilityData/NewAbilityInfo.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}