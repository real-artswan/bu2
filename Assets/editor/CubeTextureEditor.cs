using UnityEditor;
using UnityEngine;

public class CubeTextureEditor
{
    [CustomEditor(typeof(MeshFilter))]
    public class ObjectBuilderEditor : Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (GUILayout.Button("Fix cube texture scaling")) {
                MeshFilter mf = (MeshFilter)target;
                Mesh meshCopy = Instantiate(mf.sharedMesh);
                BaboUtils.fixScaledMeshUVs(meshCopy, mf.gameObject.transform.localScale);
                mf.mesh = meshCopy;
            }
        }
    }
}
