using UnityEngine.UI;
using UnityEngine;
using Settings;

namespace UI
{
    class KillScorePopup : BasePopup
    {
        protected override string Title => string.Empty;
        protected override float Width => 0f;
        protected override float Height => 0f;
        protected override float TopBarHeight => 0f;
        protected override float BottomBarHeight => 0f;
        protected override PopupAnimation PopupAnimationType => PopupAnimation.KillPopup;
        protected override float AnimationTime => 0.2f;
        private Text _scoreLabel;
        private Text _backgroundLabel;
        private const float ShakeDistance = 50f;
        private const float ShakeDuration = 1f;
        private const float ShakeDecay = 0.2f;
        private const float DefaultOffset = 100f;
        private bool _shakeFlip;
        private float _shakeTimeLeft;
        private float _currentShakeDistance;
        private float _lastShowTime;

        public override void Setup(BasePanel parent = null)
        {
            base.Setup(parent);
            _scoreLabel = ElementFactory.InstantiateAndBind(transform, "Prefabs/InGame/KillScoreLabel").GetComponent<Text>();
            _backgroundLabel = _scoreLabel.transform.Find("BackgroundLabel").GetComponent<Text>();
        }

        public void Show(int score)
        {
            _shakeTimeLeft = 0f;
    float currentTime = Time.time;
    ElementFactory.SetAnchor(gameObject, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, Vector3.up * DefaultOffset);
    if (currentTime - _lastShowTime < 1.5f)
    {
        _shakeTimeLeft = ShakeDuration;
        _currentShakeDistance = ShakeDistance;
        _shakeFlip = false;
    }
    else
    {
        IsActive = false;
    }
    _lastShowTime = currentTime;
    _scoreLabel.text = score.ToString();
    _backgroundLabel.text = score.ToString();

    Color scoreColorHigh = new Color32(0x33, 0x14, 0x17, 0xFF); // 331417
    Color backgroundColorHigh = new Color32(0xFF, 0x16, 0x00, 0xFF); // FF1600
    Color scoreColorLow = new Color32(0xFF, 0x00, 0x00, 0xFF); // FF0000
    Color backgroundColorLow = new Color32(0xFF, 0xFF, 0x00, 0xFF); // FFFF00

    if (score >= 1000)
    {
        _scoreLabel.color = scoreColorHigh;
        _backgroundLabel.color = backgroundColorHigh;
    }
    else
    {
        _scoreLabel.color = scoreColorLow;
        _backgroundLabel.color = backgroundColorLow;
    }

    int fontSize = 40 + (int)(60f * Mathf.Min(score / 3000f, 1f));
    fontSize = (int)(fontSize * SettingsManager.UISettings.KillScoreScale.Value);
    _scoreLabel.fontSize = fontSize;
    _backgroundLabel.fontSize = fontSize;
    base.Show();
        }

        public void ShowSnapshotViewer(int score)
        {
            _scoreLabel.text = score.ToString();
            _backgroundLabel.text = score.ToString();
            int fontSize = 40;
            _scoreLabel.fontSize = fontSize;
            _backgroundLabel.fontSize = fontSize;
            base.ShowImmediate();
        }

        private void Update()
        {
            if (IsActive && _shakeTimeLeft > 0f)
            {
                _shakeTimeLeft -= Time.deltaTime;
                if (_shakeFlip)
                    ElementFactory.SetAnchor(gameObject, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, Vector3.up * (DefaultOffset + _currentShakeDistance));
                else
                    ElementFactory.SetAnchor(gameObject, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, Vector3.up * (DefaultOffset - _currentShakeDistance));
                _shakeFlip = !_shakeFlip;
                float decay = ShakeDecay * Time.deltaTime * 60f;
                _currentShakeDistance *= (1 - decay);
            }
        }
    }
}
