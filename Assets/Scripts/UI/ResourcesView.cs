using TMPro;
using UnityEngine;

public class ResourcesView : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private TextMeshProUGUI _woodCountText;

    private void OnEnable()
    {
        _base.OnResourceCollected += UpdateResourceCount;
    }

    private void OnDisable()
    {
        _base.OnResourceCollected -= UpdateResourceCount;
    }

    private void UpdateResourceCount(Resource resource)
    {
        switch (resource)
        {
            case Wood:
                _woodCountText.text = _base.WoodCount.ToString();
                break;
        }
    }
}
