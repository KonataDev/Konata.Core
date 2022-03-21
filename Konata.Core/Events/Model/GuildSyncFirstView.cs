// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

internal class GuildSyncFirstView : ProtocolEvent
{
    public long GuildId { get; }

    private GuildSyncFirstView() : base(6000, true)
    {
    }

    private GuildSyncFirstView(long guildId) : base(0)
    {
        GuildId = guildId;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal static GuildSyncFirstView Create()
        => new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    internal static GuildSyncFirstView Result(long guildId)
        => new(guildId);
}
