using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using XLua;

namespace FengSheng
{
    public class Loading
    {
        private Transform transform;
        private Slider mSlider;
        private Text mText;
        private Text mProgressText;
        private Transform mRotate;
        private Tweener mTweener;

        public Loading (Transform transform)
        {
            this.transform = transform;
            mRotate = transform.Find("Image");
            mText = transform.Find("Tip").GetComponent<Text>();
            mSlider = transform.Find("Slider").GetComponent<Slider>();
            mProgressText = transform.Find("Slider/ProgressText").GetComponent<Text>();
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
            string tips = null;

            if (obj is LoadingEventPackage)
            {
                var data = obj as LoadingEventPackage;
                if (data != null)
                {
                    float progress = GetProgress(data);
                    mSlider.value = progress;
                    mText.text = data.Tips;
                    mProgressText.text = (progress * 100).ToString("F0") + "%";

                    tips = data.Tips;
                }
            }
            else if (obj is LuaTable)
            {
                var data = obj as LuaTable;
                if (data != null)
                {
                    float progress = GetProgress(data);
                    mSlider.value = progress;
                    mText.text = data.Get<string>("tips");
                    mProgressText.text = (progress * 100).ToString("F0") + "%";

                    tips = mText.text;
                }
            }

            if (string.IsNullOrEmpty(tips))
            {
                return;
            }
            else if(tips == "success")
            {
                mText.text = string.Empty;
                transform.gameObject.SetActive(false);
            }
            else if (tips == "start")
            {
                mText.text = string.Empty;
                Start();
            }
        }

        private float GetProgress(LoadingEventPackage data)
        {
            return data.Progress;
        }

        private float GetProgress(LuaTable data)
        {
            return data.Get<float>("progress");
        }

    }
}
