using UnityEngine;
using UnityEditor;

public class BaboBodyEditor {

    [CustomEditor(typeof(BaboBody))]
    public class ObjectBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BaboBody body = (BaboBody)target;
            if (GUILayout.Button("Apply skin"))
            {
                body.updateSkin(BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR);
            }
        }
    }
}
