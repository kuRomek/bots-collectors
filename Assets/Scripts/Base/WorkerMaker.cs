using System;
using UnityEngine;

public class WorkerMaker : MonoBehaviour
{
    [SerializeField] private float _makingTime;
    [SerializeField] private int _makingCost;
    [SerializeField] private Base _base;
    [SerializeField] private Worker _worker;

    public event Action WorkerMade;
    
    public int Cost => _makingCost;

    public void BeginRecruiting()
    {
        gameObject.SetActive(true);
        _base.ProgressBar.BeginMaking(ProgressBar.Mode.Recruiting, _makingTime);
        _base.ProgressBar.Progressing.onComplete += FinishRecruiting;
    }

    private void FinishRecruiting()
    {
        _base.ProgressBar.Progressing.onComplete -= FinishRecruiting;
        WorkerMade?.Invoke();
    }

    public Worker Recruit()
    {
        Worker worker = Instantiate(_worker, _base.Flag.DefaultPosition + transform.position, Quaternion.identity);
        worker.gameObject.SetActive(true);
        return worker;
    }
}
