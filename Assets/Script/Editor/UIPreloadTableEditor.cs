using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(UIPreloadTable))]
public class UIPreloadTableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Validate Entries"))
        {
            var table = (UIPreloadTable)target;
            foreach(var entry in table.entries)
            {
                var path = AssetDatabase.GUIDToAssetPath(entry.prefabRef.AssetGUID);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if(go == null || go.GetComponent<UIView>() == null)
                {
                    Debug.LogError($"{path} 에 UIView 컴포넌트가 없습니다.", go);
                }
                else
                {
                    Debug.Log($"[Validate OK] '{path} 검증 완료", go);
                }
            }
        }
    }
}
#endif