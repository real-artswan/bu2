using UnityEditor;
using UnityEngine;

public class PlayersManagerEditor
{

    [CustomEditor(typeof(PlayersManager))]
    public class ObjectBuilderEditor : Editor
    {
        GameObject baboPrefab;

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            PlayersManager manager = (PlayersManager)target;

            baboPrefab = EditorGUILayout.ObjectField("Player model", baboPrefab, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("Add player")) {
                if (baboPrefab != null) {
                    PlayerState player = PlayerState.createSelf(manager.getUniqueID(), baboPrefab);
                    player.playerName = (string)("Player name " + Random.value.ToString());
                }
            }
        }
    }
}
