using UnityEngine;
using UnityEditor;

public class UnitDataCreator
{
    [MenuItem("Create/New Unit Data")]
    public static void CreateMyAsset()
    {
        UnitData asset = ScriptableObject.CreateInstance<UnitData>();

        AssetDatabase.CreateAsset(asset, "Assets/GameData/Units/NewAbilityInfo.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}