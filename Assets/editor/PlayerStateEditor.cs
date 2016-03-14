using UnityEditor;
using UnityEngine;

public class PlayerStateEditor
{

    [CustomEditor(typeof(PlayerState))]
    public class ObjectBuilderEditor : Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            PlayerState state = (PlayerState)target;
            if (GUILayout.Button("Destroy")) {
                state.destroy();
            }
            state.kills = (short)EditorGUILayout.IntField(state.kills);
            state.score = (short)EditorGUILayout.IntField(state.score);
            state.setTeamID((BaboPlayerTeamID)EditorGUILayout.EnumMaskField(state.getTeamID()));
        }
    }
}
