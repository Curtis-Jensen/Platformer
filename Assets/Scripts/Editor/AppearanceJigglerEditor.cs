using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AppearanceJiggler))]
public class AppearanceJigglerEditor : Editor
{
    // -------------------------------------------------------
    // OnInspectorGUI()
    // Draws the default inspector fields (tilt, stretch), then
    // adds a "Jiggle" button that triggers a random appearance
    // change directly in the editor without entering play mode.
    // -------------------------------------------------------
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AppearanceJiggler jiggler = (AppearanceJiggler)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Jiggle"))
        {
            // Cache the SpriteRenderer since Awake hasn't run in edit mode
            SpriteRenderer sr = jiggler.GetComponent<SpriteRenderer>();

            float rotZ = Random.Range(-jiggler.tilt, jiggler.tilt);
            Undo.RecordObject(jiggler.transform, "Jiggle");
            jiggler.transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);

            float scaleX = Mathf.Max(0f, jiggler.baseScale.x + Random.Range(-jiggler.stretch, jiggler.stretch));
            float scaleY = Mathf.Max(0f, jiggler.baseScale.y + Random.Range(-jiggler.stretch, jiggler.stretch));
            jiggler.transform.localScale = new Vector3(scaleX, scaleY, 1f);

            if (sr != null)
            {
                Undo.RecordObject(sr, "Jiggle");
                sr.flipX = Random.value > 0.5f;
            }

            EditorUtility.SetDirty(jiggler.gameObject);
        }
    }
}
