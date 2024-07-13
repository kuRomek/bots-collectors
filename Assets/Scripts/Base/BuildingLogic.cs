using UnityEngine;

[RequireComponent(typeof(Base))]
public class BuildingLogic : MonoBehaviour
{
    [SerializeField] private Material _underConstructionMaterial;
    [SerializeField] private int _buildingCost;
    [SerializeField] private float _buildingDuration;

    private Base _base;
    private Worker _builder;
    private Material _defaultMaterial;

    public Worker Builder => _builder;
    public bool HasBuilder => _builder != null;
    public int BuildingCost => _buildingCost;
    public float BuildingDuration => _buildingDuration;

    private void Awake()
    {
        _base = GetComponent<Base>();
        _defaultMaterial = _base.Model.material;
    }

    public void SetBuilder(Worker builder)
    {
        _builder = builder; 
        _builder.OnResourceDelivered -= _base.ResourceDistributor.GetResource;
        _builder.OnBaseBuilding += RemoveBuilder;
        _builder.GoBuildBase(_base.Flag.transform.position);
    }

    public void RemoveBuilder()
    {
        RemoveFlag();
        _builder.OnBaseBuilding -= RemoveBuilder;
        _builder = null;
    }

    public void SetBuildingState()
    {
        _base.Model.material = _underConstructionMaterial;
    }

    public void SetBuiltState()
    {
        _base.Model.material = _defaultMaterial;
    }

    private void RemoveFlag()
    {
        _base.Flag.Reset();
        _base.Flag.gameObject.SetActive(false);
    }

    public void Reset()
    {
        _base.Model.material = _defaultMaterial;
    }
}
