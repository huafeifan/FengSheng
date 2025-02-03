using System.Collections;
using UnityEngine;

namespace FengSheng
{
    public class FengShengManager : MonoBehaviour
    {
        public virtual IEnumerator Register() 
        {
            yield return null;
        }

        public virtual IEnumerator Unregister() 
        {
            yield return null;
        }

    }
}
