using UnityEngine;

namespace FengSheng
{
    public class UICache
    {
        public UICache(float startTime, GameObject gameObject)
        {
            this.startTime = startTime;
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
        }

        /// <summary>
        /// 开始存活时间
        /// </summary>
        private float startTime;

        /// <summary>
        /// 结束存活时间
        /// </summary>
        private float endTime { get { return startTime + 60; } }

        public GameObject gameObject { get; set; }

        public Transform transform { get; set; }

        /// <summary>
        /// 刷新存活时间
        /// </summary>
        public void FlashTime(float startTime)
        {
            this.startTime = startTime;
        }

        /// <summary>
        /// 存活时间是否结束
        /// </summary>
        /// <returns></returns>
        public bool IsOverTime()
        {
            return Time.time > endTime;
        }

        public void Destory()
        {
            if (gameObject != null)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
