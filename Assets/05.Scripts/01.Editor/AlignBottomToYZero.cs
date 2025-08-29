// Assets/Editor/AlignBottomToYZero.cs
using UnityEngine;
using UnityEditor;

public class AlignBottomToYZero : EditorWindow //Unity 에디터용 유틸리티 스크립트
{
    [MenuItem("Tools/Align Bottom To Y=0 (Selected)")]
    static void Align()
    {
        foreach (var go in Selection.gameObjects)
        {
            if (!go.activeInHierarchy) continue;

            // Bounds 우선순위: Collider -> Renderer -> MeshFilter
            bool ok = false;
            Bounds b = new Bounds();

            var col = go.GetComponentInChildren<Collider>();
            if (col)
            {
                b = col.bounds; ok = true;
            }
            else
            {
                var rend = go.GetComponentInChildren<Renderer>();
                if (rend) { b = rend.bounds; ok = true; }
                else
                {
                    var mf = go.GetComponentInChildren<MeshFilter>();
                    if (mf && mf.sharedMesh) { b = mf.sharedMesh.bounds; ok = true; }
                }
            }

            if (!ok) continue;

            // 월드에서 아랫면을 0으로
            float delta = b.min.y;               // 현재 아랫면의 월드 Y
            Vector3 pos = go.transform.position; // 루트 기준 이동
            pos.y -= delta;                      // 아래면을 0으로 보정
            Undo.RecordObject(go.transform, "Align Bottom");
            go.transform.position = pos;
        }
    }
}
