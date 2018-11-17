using UnityEditor;

namespace BrunoMikoski.TextJuicer.Modifiers
{
    [CustomEditor(typeof(TextJuicerTransformModifier))]
    public sealed class TextJuiceTransformInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            SerializedProperty animateScaleProperty = serializedObject.FindProperty("animateScale");
            EditorGUILayout.PropertyField(animateScaleProperty);
            if (animateScaleProperty.boolValue)
            {
                SerializedProperty scaleCurveProperty = serializedObject.FindProperty("scaleCurve");
                EditorGUILayout.PropertyField(scaleCurveProperty);

                SerializedProperty scaleModifierProperty = serializedObject.FindProperty("scaleModifier");
                EditorGUILayout.PropertyField(scaleModifierProperty);

                SerializedProperty applyOnXProperty = serializedObject.FindProperty("applyOnX");

                EditorGUILayout.PropertyField(applyOnXProperty);

                SerializedProperty applyOnYProperty = serializedObject.FindProperty("applyOnY");
                EditorGUILayout.PropertyField(applyOnYProperty);

                SerializedProperty applyOnZProperty = serializedObject.FindProperty("applyOnZ");
                EditorGUILayout.PropertyField(applyOnZProperty);
            }

            SerializedProperty animatePositionProperty = serializedObject.FindProperty("animatePosition");
            EditorGUILayout.PropertyField(animatePositionProperty);
            if (animatePositionProperty.boolValue)
            {
                SerializedProperty positionMultiplierProperty = serializedObject.FindProperty("positionMultiplier");
                EditorGUILayout.PropertyField(positionMultiplierProperty);

                SerializedProperty positionCurveProperty = serializedObject.FindProperty("positionCurve");
                EditorGUILayout.PropertyField(positionCurveProperty);
            }

            SerializedProperty animateRotationProperty = serializedObject.FindProperty("animateRotation");
            EditorGUILayout.PropertyField(animateRotationProperty);
            if (animateRotationProperty.boolValue)
            {
                SerializedProperty rotationCurveProperty = serializedObject.FindProperty("rotationCurve");
                EditorGUILayout.PropertyField(rotationCurveProperty);

                SerializedProperty rotationMultiplierProperty = serializedObject.FindProperty("rotationMultiplier");
                EditorGUILayout.PropertyField(rotationMultiplierProperty);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
