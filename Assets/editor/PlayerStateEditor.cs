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
            state.playerStatistic.kills = (short)EditorGUILayout.IntField(state.playerStatistic.kills);
            state.playerStatistic.score = (short)EditorGUILayout.IntField(state.playerStatistic.score);
            state.setTeamID((BaboPlayerTeamID)EditorGUILayout.EnumMaskField(state.getTeamID()));
        }
    }
}
