using UnityEngine;

namespace Invector
{
    [vClassHeader("Comment", false, "icon_v2")]
    public class vComment : vMonoBehaviour
    {
#if UNITY_EDITOR
        [TextAreaAttribute(5, 3000)]
        public string comment;
#endif
    }
}