using UnityEngine;
using UnityEditor;

public class MakeEffectObject
{
    [MenuItem("Effects/Create/New Effect")]
    public static void CreateMyAsset()
    {
        Effect asset = ScriptableObject.CreateInstance<Effect>();

        AssetDatabase.CreateAsset(asset, "Assets/Artwork/Effects/NewEffect.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}