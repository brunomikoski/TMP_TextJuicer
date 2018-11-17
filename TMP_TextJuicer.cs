using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.TextJuicer.Modifiers;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.TextJuicer
{
    [ExecuteInEditMode]
    [AddComponentMenu( "UI/Text Juicer" )]
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_TextJuicer : MonoBehaviour
    {
        private TMP_Text cachedTextComponent;
        private TMP_Text TextComponent
        {
            get
            {
                if ( cachedTextComponent == null )
                    cachedTextComponent = GetComponent<TMP_Text>();
                return cachedTextComponent;
            }
        }

        private RectTransform rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = (RectTransform)transform;
                return rectTransform;
            }
        }

        [SerializeField]
        private float duration = 1.0f;

        [SerializeField]
        private float delay = 0.01f;

        [SerializeField]
        [Range( 0.0f, 1.0f )]
        private float progress;

        [SerializeField]
        private bool playWhenReady = true;

        [SerializeField]
        private bool loop;

        [SerializeField]
        private bool playForever;

        [SerializeField]
        private bool animationControlled;


        public string Text
        {
            get { return TextComponent.text; }
            set
            {
                TextComponent.text = value;
                SetDirty();
                UpdateIfDirty();
            }
        }

        private bool isDirty = true;

        private CharacterData[] charactersData;
        private float internalTime;
        private float realTotalAnimationTime;

        private TextJuicerVertexModifier[] vertexModifiers;

        private TMP_MeshInfo[] cachedMeshInfo;
        private TMP_TextInfo textInfo;
        private string cachedText = string.Empty;

        private bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
        }

        private bool updateGeometry;
        private bool updateVertexData;

        private bool dispatchedAfterReadyMethod;

        #region Unity Methods

        private void OnValidate()
        {
            cachedText = string.Empty;
            SetDirty();
        }

        protected void OnEnable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add( OnTextChanged );
        }

        private void OnDisable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove( OnTextChanged );
        }

        public void Update()
        {
            if ( !IsAllComponentsReady() )
                return;

            UpdateIfDirty();

            if ( !dispatchedAfterReadyMethod )
            {
                AfterIsReady();
                dispatchedAfterReadyMethod = true;
            }

            UpdateTime();
            CheckProgress();
            if ( isPlaying || animationControlled )
                ApplyModifiers();
        }

        #endregion

        #region Interaction Methods
        public void Restart()
        {
            internalTime = 0;
        }

        public void Play()
        {
            Play( true );
        }

        public void Play( bool fromBeginning = true )
        {
            if ( !IsAllComponentsReady() )
            {
                playWhenReady = true;
                return;
            }
            if ( fromBeginning )
                Restart();

            isPlaying = true;
        }

        public void Complete()
        {
            if ( isPlaying )
                progress = 1.0f;
        }

        public void Stop()
        {
            isPlaying = false;
        }

        public void SetProgress( float targetProgress )
        {
            progress = targetProgress;
            internalTime = progress * realTotalAnimationTime;
            UpdateTime();
            ApplyModifiers();
        }

        public void SetPlayForever( bool shouldPlayForever )
        {
            playForever = shouldPlayForever;
        }

        public IEnumerator WaitForCompletionEnumerator()
        {
            if (!IsAllComponentsReady() || isPlaying && progress < 1.0f )
                yield return null;
        }

        #endregion

        #region Internal
        private void AfterIsReady()
        {
            if ( playWhenReady )
                Play();
            else
                SetProgress( progress );
        }

        private bool IsAllComponentsReady()
        {
            if ( TextComponent == null )
                return false;

            if ( TextComponent.textInfo == null )
                return false;

            if ( TextComponent.mesh == null )
                return false;

            if ( TextComponent.textInfo.meshInfo == null )
                return false;
            return true;
        }

        private void OnTextChanged( Object obj )
        {
            TMP_Text changedTextComponent = obj as TMP_Text;
            if ( changedTextComponent == null )
                return;

            if ( changedTextComponent != TextComponent )
                return;

            if ( string.Equals( cachedText, changedTextComponent.text ) )
                return;
            SetDirty();
        }

        private void ApplyModifiers()
        {
            if (charactersData == null)
                return;
            for ( int i = 0; i < charactersData.Length; i++ )
                ModifyCharacter( i, cachedMeshInfo );

            if ( updateGeometry )
            {
                for ( int i = 0; i < textInfo.meshInfo.Length; i++ )
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    TextComponent.UpdateGeometry( textInfo.meshInfo[i].mesh, i );
                }
            }

            if ( updateVertexData )
                TextComponent.UpdateVertexData( TMP_VertexDataUpdateFlags.Colors32 );
       }

        private void ModifyCharacter( int info, TMP_MeshInfo[] meshInfo )
        {
            for ( int i = 0; i < vertexModifiers.Length; i++ )
            {
                vertexModifiers[i].ModifyCharacter( charactersData[info],
                                                    TextComponent,
                                                    textInfo,
                                                    meshInfo );
            }
        }

        private void CheckProgress()
        {
            if ( isPlaying )
            {
                if ( internalTime + Time.deltaTime <= realTotalAnimationTime || playForever )
                    internalTime += Time.deltaTime;
                else
                {
                    if ( loop )
                    {
                        internalTime = 0;
                        UpdateTime();
                    }
                    else
                    {
                        internalTime = realTotalAnimationTime;
                        progress = 1.0f;
                        Stop();
                        OnAnimationCompleted();
                    }
                }
            }
        }

        private void OnAnimationCompleted()
        {
        }

        private void UpdateTime()
        {
            if ( !isPlaying || animationControlled )
                internalTime = progress * realTotalAnimationTime;
            else
                progress = internalTime / realTotalAnimationTime;

            if ( charactersData == null )
                return;

            for ( int i = 0; i < charactersData.Length; i++ )
                charactersData[i].UpdateTime( internalTime );
        }

        private void UpdateIfDirty()
        {
            if ( !isDirty )
                return;

            if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
                return;

            TextJuicerVertexModifier[] currentComponents = GetComponents<TextJuicerVertexModifier>();
            if ( vertexModifiers == null || vertexModifiers != currentComponents )
            {
                vertexModifiers = currentComponents;

                for ( int i = 0; i < vertexModifiers.Length; i++ )
                {
                    TextJuicerVertexModifier vertexModifier = vertexModifiers[i];

                    if ( !updateGeometry && vertexModifier.ModifyGeomery )
                        updateGeometry = true;

                    if ( !updateVertexData && vertexModifier.ModifyVertex )
                        updateVertexData = true;
                }
            }

            if ( string.IsNullOrEmpty( cachedText ) || !cachedText.Equals( TextComponent.text ) )
            {
                TextComponent.ForceMeshUpdate();
                textInfo = TextComponent.textInfo;
                cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

                List<CharacterData> newCharacterDataList = new List<CharacterData>();
                int indexCount = 0;
                for ( int i = 0; i < textInfo.characterCount; i++ )
                {
                    char targetCharacter = textInfo.characterInfo[i].character;
                    if ( targetCharacter == ' ' )
                        continue;

                    CharacterData characterData = new CharacterData( indexCount,
                                                                     delay * indexCount,
                                                                     duration,
                                                                     playForever,
                                                                     textInfo.characterInfo[i]
                                                                             .materialReferenceIndex,
                                                                     textInfo.characterInfo[i].vertexIndex );
                    newCharacterDataList.Add( characterData );
                    indexCount += 1;
                }

                charactersData = newCharacterDataList.ToArray();
                realTotalAnimationTime = duration +
                                         charactersData.Length * delay;

                cachedText = TextComponent.text;
            }

            isDirty = false;
        }

        public void SetDirty()
        {
            isDirty = true;
        }
        #endregion
    }
}
