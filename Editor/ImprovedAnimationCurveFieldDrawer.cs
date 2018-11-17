using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer( typeof(AnimationCurve) )]
public class ImprovedAnimationCurveFieldDrawer : PropertyDrawer
{
    private static AnimationCurve clipBoardAnimationCurve = new AnimationCurve();
    private static SerializedProperty popupTargetAnimationCurveProperty;

    [MenuItem( "CONTEXT/AnimationCurve/Copy Animation Curve" )]
    private static void CopyAnimationCurve( MenuCommand inCommand )
    {
        if ( popupTargetAnimationCurveProperty == null )
            return;
        clipBoardAnimationCurve =
            AnimationCurveCopier.CreateCopy( popupTargetAnimationCurveProperty.animationCurveValue );
    }

    [MenuItem( "CONTEXT/AnimationCurve/Paste Animation Curve" )]
    private static void PasteAnimationCurve( MenuCommand inCommand )
    {
        if ( popupTargetAnimationCurveProperty == null )
            return;
        popupTargetAnimationCurveProperty.serializedObject.Update();
        popupTargetAnimationCurveProperty.animationCurveValue =
            AnimationCurveCopier.CreateCopy( clipBoardAnimationCurve );
        popupTargetAnimationCurveProperty.serializedObject.ApplyModifiedProperties();
    }

    [MenuItem( "CONTEXT/AnimationCurve/Reverse Animation Curve" )]
    private static void ReverseAnimationCurve()
    {
        if ( popupTargetAnimationCurveProperty == null )
            return;

        clipBoardAnimationCurve =
            AnimationCurveCopier.CreateCopy( popupTargetAnimationCurveProperty.animationCurveValue, true );
        PasteAnimationCurve( null );
    }

    public override void OnGUI( Rect inRect, SerializedProperty inProperty, GUIContent inLabel )
    {
        Event evt = Event.current;
        if ( evt.type == EventType.ContextClick )
        {
            Vector2 mousePos = evt.mousePosition;
            if ( inRect.Contains( mousePos ) )
            {
                popupTargetAnimationCurveProperty = inProperty.Copy();
                inProperty.serializedObject.Update();
                EditorUtility.DisplayPopupMenu( new Rect( mousePos.x, mousePos.y, 0, 0 ), "CONTEXT/AnimationCurve/",
                                                null );
            }
        }
        else
        {
            inLabel = EditorGUI.BeginProperty( inRect, inLabel, inProperty );
            EditorGUI.BeginChangeCheck();
            AnimationCurve newCurve = EditorGUI.CurveField( inRect, inLabel, inProperty.animationCurveValue );

            if ( EditorGUI.EndChangeCheck() )
                inProperty.animationCurveValue = newCurve;

            EditorGUI.EndProperty();
        }
    }
}

public static class AnimationCurveCopier
{
    public static void Copy( AnimationCurve from, AnimationCurve to, bool reverseKeys )
    {
        if ( !reverseKeys )
            to.keys = from.keys;
        else
        {
            to.keys = KeyFramesReverser.Reverse( from.keys );
        }

        to.preWrapMode = from.preWrapMode;
        to.postWrapMode = from.postWrapMode;
    }

    public static AnimationCurve CreateCopy( AnimationCurve inSource, bool reverseKeys = false )
    {
        AnimationCurve newCurve = new AnimationCurve();
        Copy( inSource, newCurve, reverseKeys );
        return newCurve;
    }
}

public static class KeyFramesReverser
{
    public static Keyframe[] Reverse( Keyframe[] keyframes )
    {
        List<Keyframe> reversedFrameList = new List<Keyframe>();
        foreach ( Keyframe keyframe in keyframes )
        {
            Keyframe keyframeCopy = new Keyframe
            {
                inTangent = keyframe.inTangent,
                outTangent = keyframe.outTangent,
                tangentMode = keyframe.tangentMode,
                time = keyframe.time,
                value = keyframe.value
            };
            reversedFrameList.Add( keyframeCopy );
        }
        reversedFrameList.Reverse();

        List<Keyframe> finalList = new List<Keyframe>();
        for ( int i = 0; i < keyframes.Length; i++ )
        {
            Keyframe keyframe = keyframes[i];
            Keyframe reversedKeyFrame = reversedFrameList[i];
            
            Keyframe finalKeyFrame = new Keyframe
            {
                inTangent = reversedKeyFrame.inTangent,
                outTangent = reversedKeyFrame.outTangent,
                tangentMode = reversedKeyFrame.tangentMode,
                time = reversedKeyFrame.time,
                value = keyframe.value
            };
            finalList.Add( finalKeyFrame );
        }

        return finalList.ToArray();
    }
}
