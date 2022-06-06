using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Events.Model;

// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable PossibleMultipleEnumeration

namespace Konata.Core.Components.Logics.Model;

[EventSubscribe(typeof(OnlineStatusEvent))]
[EventSubscribe(typeof(FilterableEvent))]
[BusinessLogic("Mesage Filter Logic", "Filter duplicate push events.")]
internal class MessageFilterLogic : BaseLogic
{
    private const string TAG = "Mesage Filter Logic";
    private const string ScheduleSyncServerTime = "Logic.Filter.SyncServerTime";
    private const string ScheduleCacheClear = "Logic.Filter.CacheClear";
    private const int FilterCacheTime = 15;

    private readonly ConcurrentDictionary<uint, HashSet<long>> _cache;
    private long _serverTimeOffset;

    internal MessageFilterLogic(BusinessComponent context)
        : base(context)
    {
        _cache = new();
        _serverTimeOffset = int.MaxValue;
    }

    public override async Task Incoming(ProtocolEvent e)
    {
        switch (e)
        {
            // Bot is online
            case OnlineStatusEvent {EventType: OnlineStatusEvent.Type.Online}:
                ScheduleComponent.Interval(ScheduleCacheClear, 30 * 1000, OnFilterCacheClear);
                ScheduleComponent.Interval(ScheduleSyncServerTime, 1800 * 1000, OnSyncServerTime);
                OnSyncServerTime();
                return;

            // Bot offline
            case OnlineStatusEvent {EventType: OnlineStatusEvent.Type.Offline}:
                ScheduleComponent.Cancel(ScheduleCacheClear);
                ScheduleComponent.Cancel(ScheduleSyncServerTime);
                return;

            // Filter duplicate message
            case FilterableEvent filterable:
                await FilterMessages(filterable);
                return;
        }
    }

    private Task FilterMessages(FilterableEvent e)
    {
        // Ignore if server time has not corrected
        if (_serverTimeOffset == int.MaxValue)
            return Task.FromResult(false);

        // Ignore messages which exceed 15 sec
        if (GetLocalServerTime() - e.FilterTime > FilterCacheTime)
            return Task.FromResult(false);

        // Check the cache
        var time = e.FilterTime;
        var msghash = e.GetFilterIdenfidentor();
        var item = _cache.GetOrAdd(time, _ => new());
        {
            lock (item)
            {
                // Add new message id
                if (!item.Contains(msghash))
                {
                    item.Add(msghash);
                    Context.PostEventToEntity(e);
                }

                // Ignore duplicate message
                else Context.LogW(TAG, "Dropped duplicate message.");
            }
        }

        return Task.CompletedTask;
    }

    private long GetLocalServerTime()
        => DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _serverTimeOffset;

    /// <summary>
    /// On sync server time
    /// </summary>
    private async void OnSyncServerTime()
    {
        try
        {
            // Get server time
            var result = await Context.SendPacket<CorrectTimeEvent>(CorrectTimeEvent.Create());
            {
                var current = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                _serverTimeOffset = result.ServerTime - current;

                Context.LogI(TAG, $"Server diff time: {_serverTimeOffset}");
            }
        }
        catch
        {
            // do nothing
        }
    }

    /// <summary>
    /// Clear cache every 30 sec
    /// </summary>
    private void OnFilterCacheClear()
    {
        Context.LogV(TAG, "OnFilterCacheClear");

        // Remove cache messages
        var time = GetLocalServerTime();
        var list = _cache.Where(s => time - s.Key > FilterCacheTime).Select(s => s.Key).ToList();
        foreach (var i in list)
        {
            _cache.TryRemove(i, out _);
            //Console.WriteLine($"deleted {i}");
        }

        if (list.Count != 0)
            Context.LogV(TAG, $"Cleared {list.Count} cache(s).");
    }
}
