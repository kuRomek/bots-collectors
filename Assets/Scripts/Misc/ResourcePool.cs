public class ResourcePool : ObjectPool<Resource>
{
    public override Resource Get()
    {
        Resource resource = base.Get();
        resource.gameObject.SetActive(true);
        resource.OnDelivered += Release;

        return resource;
    }

    public override void Release(Resource resource)
    {
        resource.OnDelivered -= Release;
        base.Release(resource);
    }
}