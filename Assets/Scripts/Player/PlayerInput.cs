using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const string Fire1 = nameof(Fire1);
    private const string Fire2 = nameof(Fire2);
    private const string Ground = nameof(Ground);

    [SerializeField] private Camera _camera;

    private Ray _ray;
    private Base _highlightedBase;
    private Base _selectedBase;

    private void Update()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_ray, out RaycastHit hit, float.PositiveInfinity))
        {
            MoveFlag(hit);
            ControlSelection(hit);
        }
    }

    private void MoveFlag(RaycastHit hit)
    {
        if (_selectedBase != null && hit.collider.gameObject.layer == LayerMask.NameToLayer(Ground))
        {
            _selectedBase.Flag.transform.position = hit.point;

            if (Input.GetButtonDown(Fire1))
            {
                PutFlag();
                UnselectBase();
            }
        }
    }

    private void PutFlag()
    {
        _selectedBase.Flag.Install();
    }

    private void ControlSelection(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out Base @base) && @base != _selectedBase)
        {
            _highlightedBase = @base;
            _highlightedBase.SelectionEffects.Highlight();

            if (Input.GetButtonDown(Fire1))
                SelectBase();
        }
        else if (_highlightedBase != null)
        {
            _highlightedBase.SelectionEffects.RemoveOutline();
            _highlightedBase = null;
        }
        else
        {
            if (Input.GetButtonDown(Fire2))
                UnselectBase();
        }
    }

    private void SelectBase()
    {
        _highlightedBase.SelectionEffects.Select();
        _selectedBase = _highlightedBase;

        _selectedBase.Flag.SetPotentialState();
        _selectedBase.Flag.Reset();

        if (_selectedBase.HasBuilder)
            _selectedBase.WorkersBehavior.CancelBuilding();

        _highlightedBase = null;
    }

    private void UnselectBase()
    {
        if (_selectedBase != null)
        {
            if (_selectedBase.Flag.HasBeenInstalled == false)
            {
                _selectedBase.Flag.Reset();
                _selectedBase.Flag.gameObject.SetActive(false);
            }

            _selectedBase.SelectionEffects.RemoveOutline();
            _selectedBase = null;
        }
    }
}
