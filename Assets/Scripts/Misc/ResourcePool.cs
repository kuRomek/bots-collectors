public class ResourcePool : ObjectPool<Resource>
{
    public override Resource Get()
    {
        Resource resource = base.Get();
        resource.gameObject.SetActive(true);
        resource.OnResourcePickedUp += Release;

        return resource;
    }

    public override void Release(Resource resource)
    {
        resource.OnResourcePickedUp -= Release;
        base.Release(resource);
    }
}