namespace QuizGame.Api.Grains;

public interface IHubListGrain : IGrainWithGuidKey
{
    ValueTask AddHub(SiloAddress host, IRemoteGameHub hubReference);
    ValueTask<List<(SiloAddress Host, IRemoteGameHub Hub)>> GetHubs();
}

public class HubListGrain 
   
    : Grain, IHubListGrain
{
    private readonly IClusterMembershipService _clusterMembership;
    private readonly Dictionary<SiloAddress, IRemoteGameHub> _hubs = new();
    private MembershipVersion _cacheMembershipVersion;
    private List<(SiloAddress Host, IRemoteGameHub Hub)>? _cache;

    public HubListGrain(IClusterMembershipService clusterMembership)
    {
        _clusterMembership = clusterMembership;
    }

    public ValueTask AddHub(SiloAddress host, IRemoteGameHub hubReference)
    {
        _cache = null;
        _hubs[host] = hubReference;

        return default;
    }

    public ValueTask<List<(SiloAddress Host, IRemoteGameHub Hub)>> GetHubs() =>
        new(GetCachedHubs());

    private List<(SiloAddress Host, IRemoteGameHub Hub)> GetCachedHubs()
    {
        // Returns a cached list of hubs if the cache is valid, otherwise builds a list of hubs.
        ClusterMembershipSnapshot clusterMembers = _clusterMembership.CurrentSnapshot;
        if (_cache is { } && clusterMembers.Version == _cacheMembershipVersion)
        {
            return _cache;
        }

        // Filter out hosts which are not yet active or have been removed from the cluster.
        var hubs = new List<(SiloAddress Host, IRemoteGameHub Hub)>();
        var toDelete = new List<SiloAddress>();
        foreach (KeyValuePair<SiloAddress, IRemoteGameHub> pair in _hubs)
        {
            SiloAddress host = pair.Key;
            IRemoteGameHub hubRef = pair.Value;
            SiloStatus hostStatus = clusterMembers.GetSiloStatus(host);
            if (hostStatus is SiloStatus.Dead)
            {
                toDelete.Add(host);
            }

            if (hostStatus is SiloStatus.Active)
            {
                hubs.Add((host, hubRef));
            }
        }

        foreach (SiloAddress host in toDelete)
        {
            _hubs.Remove(host);
        }

        _cache = hubs;
        _cacheMembershipVersion = clusterMembers.Version;
        return hubs;
    }
}
