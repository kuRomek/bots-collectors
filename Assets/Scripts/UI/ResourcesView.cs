using TMPro;
using UnityEngine;

public class ResourcesView : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private TextMeshProUGUI _woodCountText;

    private void OnEnable()
    {
        _base.ResourceDistributor.WoodCountChanged += UpdateWoodCount;
    }

    private void OnDisable()
    {
        _base.ResourceDistributor.WoodCountChanged -= UpdateWoodCount;
    }

    private void UpdateWoodCount()
    {
        _woodCountText.text = _base.WoodCount.ToString();
    }
}
