using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _modeText;

    private Tween _textTweening;

    public Tween Progressing { get; private set; }

    public enum Mode
    {
        Recruiting,
        Building
    }

    public void BeginMaking(Mode mode, float makingTime)
    {
        gameObject.SetActive(true);
        _modeText.text = mode.ToString();
        _textTweening = _modeText.DOText("...", 2f).SetRelative().SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        Progressing = _slider.DOValue(1f, makingTime).SetEase(Ease.Linear);
        Progressing.onComplete += Complete;
    }

    private void Complete()
    {
        Progressing.onComplete -= Complete;
        _textTweening.Kill();
        gameObject.SetActive(false);
        _slider.value = 0f;
    }
}
