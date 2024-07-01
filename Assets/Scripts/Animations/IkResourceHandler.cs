using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IkResourceHandler : MonoBehaviour
{
    private Animator _animator;
    private Resource _resource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK()
    {
        if (_animator != null)
        {
            if (_resource != null)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);

                _animator.SetIKPosition(AvatarIKGoal.LeftHand, _resource.LeftHandle.position);
                _animator.SetIKRotation(AvatarIKGoal.LeftHand, _resource.LeftHandle.rotation);
                _animator.SetIKPosition(AvatarIKGoal.RightHand, _resource.RightHandle.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, _resource.RightHandle.rotation);
            }
            else
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            }
        }
    }

    public void GrabResource(Resource resource)
    {
        _resource = resource;
    }

    public void DropResource()
    {
        _resource = null;
    }
}
