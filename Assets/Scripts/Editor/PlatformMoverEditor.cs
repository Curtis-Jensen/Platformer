using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlatformMover))]
public class PlatformMoverEditor : Editor
{
    // -------------------------------------------------------
    // OnInspectorGUI()
    // Draws the default inspector, then adds a "Save Waypoint"
    // button that appends the platform's current world position
    // to additionalWaypoints. Move the platform in the scene,
    // hit the button, repeat.
    // -------------------------------------------------------
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlatformMover mover = (PlatformMover)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Save Waypoint"))
        {
            SerializedProperty waypoints = serializedObject.FindProperty("additionalWaypoints");
            waypoints.arraySize++;
            waypoints.GetArrayElementAtIndex(waypoints.arraySize - 1)
                     .vector2Value = (Vector2)mover.transform.position;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
