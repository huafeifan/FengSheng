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
        /// ��ʼ���ʱ��
        /// </summary>
        private float startTime;

        /// <summary>
        /// �������ʱ��
        /// </summary>
        private float endTime { get { return startTime + 60; } }

        public GameObject gameObject { get; set; }

        public Transform transform { get; set; }

        /// <summary>
        /// ˢ�´��ʱ��
        /// </summary>
        public void FlashTime(float startTime)
        {
            this.startTime = startTime;
        }

        /// <summary>
        /// ���ʱ���Ƿ����
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
