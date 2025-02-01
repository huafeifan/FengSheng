using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    public class GameObjectPool : MonoBehaviour
    {
        public GameObject Clone;
        public List<GameObject> UsingList = new List<GameObject>();
        public List<GameObject> Pool = new List<GameObject>();

        public GameObject Get(bool activeSelf)
        {
            if (Pool.Count == 0)
            {
                Pool.Add(GameObject.Instantiate(Clone, transform));
            }

            GameObject result = Pool[Pool.Count - 1];
            Pool.RemoveAt(Pool.Count - 1);

            UsingList.Add(result);
            result.SetActive(activeSelf);
            return result;
        }

        public void Dispose(GameObject obj)
        {
            obj.SetActive(false);
            UsingList.Remove(obj);
            Pool.Add(obj);
        }
    }
}
