// Assets/Editor/AlignBottomToYZero.cs
using UnityEngine;
using UnityEditor;

public class AlignBottomToYZero : EditorWindow //Unity �����Ϳ� ��ƿ��Ƽ ��ũ��Ʈ
{
    [MenuItem("Tools/Align Bottom To Y=0 (Selected)")]
    static void Align()
    {
        foreach (var go in Selection.gameObjects)
        {
            if (!go.activeInHierarchy) continue;

            // Bounds �켱����: Collider -> Renderer -> MeshFilter
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

            // ���忡�� �Ʒ����� 0����
            float delta = b.min.y;               // ���� �Ʒ����� ���� Y
            Vector3 pos = go.transform.position; // ��Ʈ ���� �̵�
            pos.y -= delta;                      // �Ʒ����� 0���� ����
            Undo.RecordObject(go.transform, "Align Bottom");
            go.transform.position = pos;
        }
    }
}
