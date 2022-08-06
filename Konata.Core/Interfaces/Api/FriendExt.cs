using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;
using Konata.Core.Exceptions.Model;
using Konata.Core.Message;

// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Interfaces.Api;

public static class FriendExt
{
    /// <summary>
    /// Send the message to a friend
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="friendUin"><b>[In]</b> Friend uin. </param>
    /// <param name="builder"><b>[In]</b> Message chain builder. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    [KonataApi(1)]
    public static Task<bool> SendFriendMessage(this Bot bot, uint friendUin, MessageBuilder builder)
        => bot.BusinessComponent.Messaging.SendFriendMessage(friendUin, builder.Build());

    /// <summary>
    /// Send the message to a friend
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="friendUin"><b>[In]</b> Friend uin. </param>
    /// <param name="message"><b>[In]</b> Text message. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    [KonataApi(1)]
    public static Task<bool> SendFriendMessage(this Bot bot, uint friendUin, string message)
        => bot.SendFriendMessage(friendUin, new MessageBuilder(message));

    /// <summary>
    /// Send the message to a friend
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="friendUin"><b>[In]</b> Friend uin. </param>
    /// <param name="chains"><b>[In]</b> Message chains. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    [KonataApi(1)]
    public static Task<bool> SendFriendMessage(this Bot bot, uint friendUin, params BaseChain[] chains)
        => bot.SendFriendMessage(friendUin, new MessageBuilder(chains));

    /// <summary>
    /// Send the message to a friend
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="friendUin"><b>[In]</b> Friend uin. </param>
    /// <param name="chain"><b>[In]</b> Message chain. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    [KonataApi(1)]
    public static Task<bool> SendFriendMessage(this Bot bot, uint friendUin, MessageChain chain)
        => bot.SendFriendMessage(friendUin, new MessageBuilder(chain));
    
    /// <summary>
    /// Poke Friend
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="friendUin"><b>[In]</b> Friend uin. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    [KonataApi(1, experimental: true)]
    public static Task<bool> SendFriendPoke(this Bot bot, uint friendUin)
        => bot.BusinessComponent.Operation.FriendPoke(friendUin);
    
    /// <summary>
    /// Process friend request message
    /// </summary>
    /// <param name="bot"><b>[In]</b> <see cref="Bot"/> instance</param>
    /// <param name="reqUin"><b>[In]</b> Group uin</param>
    /// <param name="token"><b>[In]</b> Request <see cref="FriendRequestEvent.Token"/></param>
    /// <returns></returns>
    [KonataApi(1)]
    public static Task<bool> ApproveFriendRequest(this Bot bot, uint reqUin, long token)
        => bot.BusinessComponent.Operation.ApproveFriendRequest(reqUin, token);

    /// <summary>
    /// Process friend request message
    /// </summary>
    /// <param name="bot"><b>[In]</b> <see cref="Bot"/> instance</param>
    /// <param name="reqUin"><b>[In]</b> Group uin</param>
    /// <param name="token"><b>[In]</b> Request <see cref="FriendRequestEvent.Token"/></param>
    /// <param name="preventRequest"><b>[In] [Opt]</b> Prevent this request</param>
    /// <returns></returns>
    [KonataApi(1)]
    public static Task<bool> DeclineFriendRequest(this Bot bot, uint reqUin, long token, bool preventRequest = false)
        => bot.BusinessComponent.Operation.DeclineFriendRequest(reqUin, token, preventRequest);
    
    [KonataApi(1, experimental: true)]
    public static Task<string> GetOfflineFileUrl(this Bot bot, FileChain chain)
        => bot.BusinessComponent.Operation.GetOfflineFileUrl(chain.FileUuid);
}
