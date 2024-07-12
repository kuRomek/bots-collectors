using UnityEngine;

[RequireComponent(typeof(Outline))]
public class SelectionEffects : MonoBehaviour
{
    private Outline _outline;
    private Color _highlightColor = Color.white;
    private Color _selectedColor = Color.yellow;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    public void Highlight()
    {
        _outline.enabled = true;
        _outline.OutlineColor = _highlightColor;
    }

    public void Select()
    {
        _outline.enabled = true;
        _outline.OutlineColor = _selectedColor;
    }

    public void RemoveOutline()
    {
        _outline.enabled = false;
    }
}
