﻿using Paladin.Framework.Attributes;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.TextJuicer.Modifiers
{
    
    [AddComponentMenu( "UI/Text Juicer Modifiers/Transform Modifier", 11 )]
    public sealed class TextJuicerTransformModifier : TextJuicerVertexModifier
    {
        [Header( "Animation Parameters" )]
        [SerializeField]
        private bool animateScale;

        [SerializeField]
        [ShowIf( "animateScale", true )]
        private AnimationCurve scaleCurve = new AnimationCurve( new Keyframe( 0, 0 ), new Keyframe( 1, 1 ) );
        
        [SerializeField]
        [ShowIf( "animateScale", true )]
        private Vector3 scaleModifier;

        [SerializeField]
        [ShowIf( "animateScale", true )]
        private bool applyOnX;

        [SerializeField]
        [ShowIf( "animateScale", true )]
        private bool applyOnY;
        
        [SerializeField]
        [ShowIf( "animateScale", true )]
        private bool applyOnZ;
        

        [SerializeField]
        private bool animatePosition;

        [SerializeField]
        [ShowIf( "animatePosition", true )]
        private Vector3 positionMultiplier;

        [SerializeField]
        [ShowIf( "animatePosition", true )]
        private AnimationCurve positionCurve = new AnimationCurve( new Keyframe( 0, 0 ), new Keyframe( 1, 1 ) );

        [SerializeField]
        private bool animateRotation;

        [SerializeField]
        [ShowIf( "animateRotation", true )]
        private AnimationCurve rotationCurve = new AnimationCurve( new Keyframe( 0, 0 ), new Keyframe( 1, 1 ) );

        [SerializeField]
        [ShowIf( "animateRotation", true )]
        private Vector3 rotationMultiplier;
        
        private bool modifyGeomery;
        private bool modifyVertex;

        public override bool ModifyGeomery
        {
            get
            {
                return true;
            }
        }
        public override bool ModifyVertex
        {
            get
            {
                return false;
            }
        }

        public override void ModifyCharacter( CharacterData characterData, TMP_Text textComponent, TMP_TextInfo textInfo,
            TMP_MeshInfo[] meshInfo )
        {
            int materialIndex = characterData.MaterialIndex;

            int vertexIndex = characterData.VertexIndex;

            Vector3[] sourceVertices = meshInfo[materialIndex].vertices;

            Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

            Vector3 offset = charMidBasline;

            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

            destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
            destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
            destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
            destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

            Vector3 finalScale = Vector3.one;
            if ( animateScale )
            {
                if ( applyOnX )
                {
                    finalScale.x = scaleModifier.x * scaleCurve.Evaluate( characterData.Progress );
                }

                if ( applyOnY )
                {
                    finalScale.y = scaleModifier.y * scaleCurve.Evaluate( characterData.Progress );
                }

                if ( applyOnZ )
                {                    
                    finalScale.z = scaleModifier.z * scaleCurve.Evaluate( characterData.Progress );
                }
            }

            Vector3 finalPosition = Vector3.zero;
            if ( animatePosition )
                finalPosition = positionMultiplier * positionCurve.Evaluate( characterData.Progress );

            Quaternion finalQuaternion = Quaternion.identity;
            if ( animateRotation )
                finalQuaternion =
                    Quaternion.Euler( rotationMultiplier * rotationCurve.Evaluate( characterData.Progress ) );

            Matrix4x4 matrix = Matrix4x4.TRS( finalPosition, finalQuaternion, finalScale );

            destinationVertices[vertexIndex + 0] =
                matrix.MultiplyPoint3x4( destinationVertices[vertexIndex + 0] );
            destinationVertices[vertexIndex + 1] =
                matrix.MultiplyPoint3x4( destinationVertices[vertexIndex + 1] );
            destinationVertices[vertexIndex + 2] =
                matrix.MultiplyPoint3x4( destinationVertices[vertexIndex + 2] );
            destinationVertices[vertexIndex + 3] =
                matrix.MultiplyPoint3x4( destinationVertices[vertexIndex + 3] );

            destinationVertices[vertexIndex + 0] += offset;
            destinationVertices[vertexIndex + 1] += offset;
            destinationVertices[vertexIndex + 2] += offset;
            destinationVertices[vertexIndex + 3] += offset;
        }
    }
}