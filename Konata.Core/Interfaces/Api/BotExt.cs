using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Exceptions.Model;
using Konata.Core.Message.Model;

// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Interfaces.Api;

public static class BotExt
{
    /// <summary>
    /// Bot login
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <returns></returns>
    [KonataApi(1)]
    public static Task<(bool Success, WtLoginEvent Event)> Login(this Bot bot)
        => bot.BusinessComponent.WtExchange.Login();

    /// <summary>
    /// Disconnect the socket and logout
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <returns></returns>
    [KonataApi(1)]
    public static Task<bool> Logout(this Bot bot)
        => bot.BusinessComponent.WtExchange.Logout();

    /// <summary>
    /// Submit Slider ticket while login
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="ticket"><b>[In]</b> Slider ticket</param>
    [KonataApi(1)]
    public static bool SubmitSliderTicket(this Bot bot, [NotNull] string ticket)
        => bot.BusinessComponent.WtExchange.SubmitSliderTicket(ticket);

    /// <summary>
    /// Submit Sms code while login
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="code"><b>[In]</b> Sms code</param>
    [KonataApi(1)]
    public static bool SubmitSmsCode(this Bot bot, [NotNull] string code)
        => bot.BusinessComponent.WtExchange.SubmitSmsCode(code);

    /// <summary>
    /// Get group list
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="forceUpdate"><b>[In]</b> Force update. </param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    [KonataApi(1)]
    public static Task<IReadOnlyList<BotGroup>> GetGroupList(this Bot bot, bool forceUpdate = false)
        => bot.BusinessComponent.CacheSync.GetGroupList(forceUpdate);

    /// <summary>
    /// Get friend list
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="forceUpdate"><b>[In]</b> Force update. </param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    [KonataApi(1)]
    public static Task<IReadOnlyList<BotFriend>> GetFriendList(this Bot bot, bool forceUpdate = false)
        => bot.BusinessComponent.CacheSync.GetFriendList(forceUpdate);

    /// <summary>
    /// Get csrf token <br/>
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <returns></returns>
    [KonataApi(1, experimental: true)]
    public static Task<string> GetCsrfToken(this Bot bot)
        => bot.BusinessComponent.CacheSync.GetCsrfToken();

    /// <summary>
    /// Get online status
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <returns></returns>
    [KonataApi(1, experimental: true)]
    public static OnlineStatusEvent.Type GetOnlineStatus(this Bot bot)
        => bot.BusinessComponent.WtExchange.OnlineType;

    /// <summary>
    /// Image ocr
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="image">The image</param>
    /// <returns></returns>
    [KonataApi(1, experimental: true)]
    public static Task<List<ImageOcrResult>> ImageOcr(this Bot bot, ImageChain image)
        => bot.BusinessComponent.Operation.ImageOcr(image);
    
    // /// <summary>
    // /// Set online status
    // /// </summary>
    // /// <param name="bot"><b>[In]</b> Bot instance</param>
    // /// <param name="status"><b>[In]</b> Online status</param>
    // /// <returns></returns>
    // [KonataApi(1, experimental: true)]
    // public static Task<bool> SetOnlineStatus(this Bot bot, OnlineStatusEvent.Type status)
    //     => bot.BusinessComponent.WtExchange.SetOnlineStatus(status);

    /// <summary>
    /// Is Online
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <returns></returns>
    [KonataApi(1, experimental: true)]
    public static bool IsOnline(this Bot bot)
        => bot.GetOnlineStatus() != OnlineStatusEvent.Type.Offline;
}
