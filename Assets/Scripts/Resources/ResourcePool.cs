public class ResourcePool : ObjectPool<Resource>
{
    public override Resource Get()
    {
        Resource resource = base.Get();
        resource.gameObject.SetActive(true);
        resource.Delivered += Release;

        return resource;
    }

    public override void Release(Resource resource)
    {
        resource.Delivered -= Release;
        resource.gameObject.SetActive(false);
        base.Release(resource);
    }
}