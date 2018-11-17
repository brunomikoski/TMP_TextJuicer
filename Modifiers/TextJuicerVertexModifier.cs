using System.Globalization;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.TextJuicer.Modifiers
{
    [RequireComponent(typeof(TMP_TextJuicer))]
    public abstract class TextJuicerVertexModifier : MonoBehaviour
    {
        
        public abstract bool ModifyGeomery { get; }
        public abstract bool ModifyVertex { get; }

        private TMP_TextJuicer cachedTextJuicer;
        private TMP_TextJuicer TextJuicer
        {
            get
            {
                if ( cachedTextJuicer == null )
                    cachedTextJuicer = GetComponent<TMP_TextJuicer>();
                return cachedTextJuicer;
            }
        }

        private void OnValidate()
        {
            TextJuicer.SetDirty();
        }

        public abstract void ModifyCharacter( CharacterData characterData, TMP_Text textComponent, TMP_TextInfo textInfo,
            TMP_MeshInfo[] meshInfo );
    }
}
