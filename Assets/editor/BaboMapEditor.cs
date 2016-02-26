using UnityEditor;
using UnityEngine;

public class BaboMapEditor
{

    [CustomEditor(typeof(Map))]
    public class ObjectBuilderEditor : Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            Map map = (Map)target;
            if (GUILayout.Button("Load map from file")) {
                float start = Time.realtimeSinceStartup;
                map.loadFromFile();
                Debug.LogFormat("Map load in {0} s", Time.realtimeSinceStartup - start);
            }

            if (GUILayout.Button("Clear map")) {
                float start = Time.realtimeSinceStartup;
                map.clearMap();
                Debug.LogFormat("Map clean in {0} s", Time.realtimeSinceStartup - start);
            }
        }
    }
}
