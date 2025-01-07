using UnityEngine;

namespace FengSheng
{
    public class UICache
    {
        public UICache(float startTime, GameObject gameObject)
        {
            StartTime = startTime;
            GameObject = gameObject;
            SetEndTime();
        }
        public float StartTime { get; private set; }
        public float EndTime { get; private set; }
        public GameObject GameObject { get; set; }

        private void SetEndTime()
        {
            EndTime = StartTime + 60;
        }

        public bool IsOverTime()
        {
            return Time.time > EndTime;
        }

        public void Destory()
        {
            if (GameObject != null)
            {
                GameObject.Destroy(GameObject);
            }
        }
    }
}
