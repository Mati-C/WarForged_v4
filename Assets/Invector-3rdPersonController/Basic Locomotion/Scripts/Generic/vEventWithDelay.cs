using System.Collections;
using UnityEngine;

namespace Invector.Utils
{
    [vClassHeader("Events With Delay")]
    public class vEventWithDelay : vMonoBehaviour
    {
        [SerializeField] private vEventWithDelayObject[] events;
        public void DoEvents()
        {
            for (int i = 0; i < events.Length; i++)
                StartCoroutine(DoEventWithDelay(events[i]));
        }

        public void DoEvent(int index)
        {

            if (index < events.Length && events.Length > 0) StartCoroutine(DoEventWithDelay(events[index]));
        }

        IEnumerator DoEventWithDelay(vEventWithDelayObject _event)
        {
            yield return new WaitForSeconds(_event.delay);
            _event.onDoEvent.Invoke();
        }

        [System.Serializable]
        public class vEventWithDelayObject
        {
            public float delay;
            public UnityEngine.Events.UnityEvent onDoEvent;
        }
    }
}