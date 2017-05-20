using UnityEngine;
using UnityEditor;

public class EffectObjectCreator
{
    [MenuItem("Create/New Effect")]
    public static void CreateMyAsset()
    {
        Effect asset = ScriptableObject.CreateInstance<Effect>();

        AssetDatabase.CreateAsset(asset, "Assets/GameData/Effects/NewEffect.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}