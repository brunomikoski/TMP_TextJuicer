﻿using System.Collections;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.TextJuicer
{
    [CustomEditor(typeof(TMP_TextJuicer), true)]
    public sealed class TMP_TextJuicerInspector : Editor
    {
        private SerializedProperty animationControlledSerializedProperty;
        private TMP_TextJuicer textJuicer;
        private SerializedProperty playWhenReadySerializedProperty;
        private SerializedProperty loopSerializedProperty;
        private SerializedProperty playForeverSerializedProperty;
        private SerializedProperty durationSerializedProperty;
        private SerializedProperty delaySerializedProperty;
        private SerializedProperty progressSerializedProperty;
        private Coroutine playingRoutine;

        private void OnEnable()
        {
            textJuicer = (TMP_TextJuicer)target;

            durationSerializedProperty =  serializedObject.FindProperty("duration");
            delaySerializedProperty =  serializedObject.FindProperty("delay");
            progressSerializedProperty =  serializedObject.FindProperty("progress");
            playWhenReadySerializedProperty =  serializedObject.FindProperty("playWhenReady");
            loopSerializedProperty = serializedObject.FindProperty("loop");
            playForeverSerializedProperty =  serializedObject.FindProperty("playForever");
            animationControlledSerializedProperty =  serializedObject.FindProperty("animationControlled");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(durationSerializedProperty);
            EditorGUILayout.PropertyField(delaySerializedProperty);
            EditorGUILayout.PropertyField(animationControlledSerializedProperty);
            if (animationControlledSerializedProperty.boolValue)
            {
                EditorGUILayout.PropertyField(progressSerializedProperty);
            }
            else
            {
                EditorGUILayout.PropertyField(playWhenReadySerializedProperty);
                EditorGUILayout.PropertyField(loopSerializedProperty);
                EditorGUILayout.PropertyField(playForeverSerializedProperty);
            }

            serializedObject.ApplyModifiedProperties();

            if (!textJuicer.IsPlaying)
            {
                if (GUILayout.Button("Play"))
                {
                    textJuicer.Play();
                }
            }
            else
            {
                if (GUILayout.Button("Stop"))
                {
                    textJuicer.Stop();
                }
            }
        }
    }
}