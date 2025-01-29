using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace GIGA.AutoRadialLayout.QuerySystem
{
    public class RadialLayoutQueryTarget : MonoBehaviour
    {
        [SerializeField]
        private int _uniqueId = -1;
        public int UniqueId { get { return this._uniqueId; } }

        public List<string> tags;

        /// <summary>
        /// Forces the unique Id to a specified value
        /// </summary>
        public void ForceUniqueId(int id)
        {
            this._uniqueId = id;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        #region Query System

        /// <summary>
        /// Returns TRUE if the given tag is found in the tags list.
        /// </summary>
        public bool ContainsTag(string tag, bool caseSensitive)
        {
            if (caseSensitive)
            {
                if (this.tag.Contains(tag))
                    return true;
            }
            else
            {
                foreach (var t in this.tags)
                    if (string.Compare(tag, t, true) == 0)
                        return true;
            }
            return false;
        }

		#endregion

	}
}
