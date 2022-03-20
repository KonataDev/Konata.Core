using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Message;

// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable PossibleMultipleEnumeration

namespace Konata.Core.Components.Logics.Model;

[EventSubscribe(typeof(OnlineStatusEvent))]
[EventSubscribe(typeof(FriendMessageEvent))]
[BusinessLogic("Mesage Filter Logic", "Filter duplicate push events.")]
internal class MessageFilterLogic : BaseLogic
{
    private const string TAG = "Mesage Filter Logic";
    private const string ScheduleSyncServerTime = "Logic.Filter.SyncServerTime";
    private const string ScheduleCacheClear = "Logic.Filter.CacheClear";

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

            // private message coming
            case FriendMessageEvent friend:
                await FilterMessages(friend);
                return;
        }
    }

    private Task FilterMessages(FriendMessageEvent e)
    {
        // Ignore if server time has not corrected
        if (_serverTimeOffset == int.MaxValue)
            return Task.FromResult(false);

        // Ignore self messages
        if (e.SelfUin == e.Message.Sender.Uin)
            return Task.FromResult(false);

        // Ignore messages which exceed 30 sec
        if (GetLocalServerTime() - e.Message.Time > 30)
            return Task.FromResult(false);

        // Check the cache
        var time = e.Message.Time;
        var msgid = GetMessageId(e.Message);
        var item = _cache.GetOrAdd(time, _ => new());
        {
            lock (item)
            {
                // Add new message id
                if (!item.Contains(msgid))
                {
                    item.Add(msgid);
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
    /// Get message unique id for filtering
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private static long GetMessageId(MessageStruct s)
        => (long) s.Sender.Uin << 32 | s.Random;

    /// <summary>
    /// On sync server time
    /// </summary>
    private async void OnSyncServerTime()
    {
        // Get server time
        if (_serverTimeOffset == int.MaxValue)
        {
            var server = (await GetServerTime()).ServerTime;
            var current = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _serverTimeOffset = server - current;
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
        var list = _cache.Where(s => time - s.Key > 30).Select(s => s.Key).ToList();
        foreach (var i in list)
        {
            _cache.TryRemove(i, out _);
            Console.WriteLine($"deleted {i}");
        }

        Context.LogV(TAG, $"Cleared {list.Count} cache(s).");
    }

    #region Stub methods

    private Task<CorrectTimeEvent> GetServerTime()
        => Context.SendPacket<CorrectTimeEvent>(CorrectTimeEvent.Create());

    #endregion
}
