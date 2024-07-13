using System;
using UnityEngine;

public class Flag : MonoBehaviour
{
    [SerializeField] private Material _potentialMaterial;
    [SerializeField] private Material _installedMaterial;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private LineRenderer _dottedLine;
    [SerializeField] private AudioPlayer _audioGeneral;
    [SerializeField] private AudioClip _installedSoundEffect;

    private float _groundOffset = 0.01f;

    public event Action Installed;
    public event Action Removed;

    public Vector3 DefaultPosition { get; private set; }
    public bool HasBeenInstalled { get; private set; } = false;

    private void Awake()
    {
        DefaultPosition = transform.localPosition;
        Vector3 basePosition = GetComponentInParent<Base>().transform.position;
        _dottedLine.SetPosition(0, new Vector3(basePosition.x, _groundOffset, basePosition.z));
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _dottedLine.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        HasBeenInstalled = false;
        _dottedLine.gameObject.SetActive(false);
    }

    private void Update()
    {
        _dottedLine.SetPosition(1, new Vector3(transform.position.x, _groundOffset, transform.position.z));
    }

    public void SetPotentialState()
    {
        _renderer.material = _potentialMaterial;

        if (HasBeenInstalled)
            Removed?.Invoke();
    }

    public void Install()
    {
        _renderer.material = _installedMaterial;
        HasBeenInstalled = true;
        Installed?.Invoke();
        _audioGeneral.PlayClip(_installedSoundEffect);
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        transform.localPosition = DefaultPosition;
        HasBeenInstalled = false;
    }
}