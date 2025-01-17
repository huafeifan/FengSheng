using System;
using UnityEngine;

namespace FengSheng
{
    [Serializable]
    public class UICache
    {
        public UICache(string path, float startTime, GameObject gameObject)
        {
            mPath = path;
            mStartTime = startTime;
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
        }

        /// <summary>
        /// 资源路径
        /// </summary>
        [SerializeField]
        private string mPath;
        public string Path { get { return mPath; } }

        /// <summary>
        /// 开始存活时间
        /// </summary>
        [SerializeField]
        private float mStartTime;

        /// <summary>
        /// 结束存活时间
        /// </summary>
        private float mEndTime { get { return mStartTime + 60; } }

        public GameObject gameObject { get; set; }

        public Transform transform { get; set; }

        /// <summary>
        /// 刷新存活时间
        /// </summary>
        public void FlashTime(float startTime)
        {
            mStartTime = startTime;
        }

        /// <summary>
        /// 存活时间是否结束
        /// </summary>
        /// <returns></returns>
        public bool IsOverTime()
        {
            return Time.time > mEndTime;
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
