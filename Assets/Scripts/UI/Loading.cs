using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FengSheng
{
    public class Loading
    {
        public const string Progress_Hotfix = "Loading_Hotfix";

        private Transform transform;
        private Slider mSlider;
        private Text mText;
        private Transform mRotate;
        private Tweener mTweener;

        public Loading (Transform transform)
        {
            this.transform = transform;
            mRotate = transform.Find("Image");
            mText = transform.Find("Tip").GetComponent<Text>();
            mSlider = transform.Find("Slider").GetComponent<Slider>();
        }

        public void Start()
        {
            transform.gameObject.SetActive(true);
            if (mTweener != null)
            {
                mTweener.Kill();
            }
            mTweener = mRotate.DORotate(Vector3.forward * 360, 1, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        }

        public void Dispose()
        {
            if (mSlider != null)
            {
                mSlider.value = 0;
            }

            if (mTweener != null)
            {
                mTweener.Kill();
            }

            RemoveListener();
        }

        public void AddListener()
        {
            EventManager.Instance.AddListener(EventManager.Event_LoadingProgress, OnProgressChange, "CSharp.Loading.OnProgressChange");
        }

        private void RemoveListener()
        {
            EventManager.Instance.RemoveListener(EventManager.Event_LoadingProgress, OnProgressChange);
        }

        private void OnProgressChange(object obj)
        {
            var data = obj as LoadingEventPackage;
            if (data != null)
            {
                mSlider.value = GetProgress(data);
                mText.text = data.Tips;

                if (data.Tips == "success")
                {
                    transform.gameObject.SetActive(false);
                }
                else if (data.Tips == "start")
                {
                    Start();
                }
            }
        }

        private float GetProgress(LoadingEventPackage data)
        {
            return data.Progress;
        }

    }
}
