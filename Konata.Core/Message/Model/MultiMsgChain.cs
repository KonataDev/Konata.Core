using System.Collections;
using System.Collections.Generic;
using Konata.Core.Events.Model;
using Konata.Core.Utils;
using Konata.Core.Utils.Extensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Konata.Core.Message.Model;

public class MultiMsgChain : XmlChain, IEnumerable<MessageStruct>
{
    /// <summary>
    /// Messages
    /// </summary>
    internal List<MessageStruct> Messages { get; }

    /// <summary>
    /// MultiMsg up information
    /// </summary>
    internal MultiMsgUpInfo MultiMsgUpInfo { get; private set; }

    /// <summary>
    /// Packed data
    /// </summary>
    internal byte[] PackedData { get; private set; }

    /// <summary>
    /// File name guid
    /// </summary>
    internal string FileName { get; }

    public MultiMsgChain() : base("")
    {
        Messages = new();
        FileName = Guid.GenerateString();
        Type = ChainType.MultiMsg;
    }

    public IEnumerator<MessageStruct> GetEnumerator()
        => Messages.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Add message
    /// </summary>
    /// <param name="reference"></param>
    public void Add(MessageStruct reference)
    {
        Messages.Add(reference);
        Content = ToString();
    }

    public void Add(((uint uin, string name) source, MessageChain chain) message)
        => Add(new MessageStruct(message.source.uin, message.source.name, message.chain));

    public void Add(((uint uin, string name) source, BaseChain chain) message)
        => Add((message.source, new MessageChain(message.chain)));

    public void Add(((uint uin, string name) source, string text) message)
        => Add((message.source, new MessageChain(TextChain.Create(message.text))));
    
    /// <summary>
    /// Add message
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    public MultiMsgChain AddMessage(MessageStruct reference)
    {
        Add(reference);
        return this;
    }

    /// <summary>
    /// Add message
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public MultiMsgChain AddMessage(uint uin, string name, MessageChain message)
    {
        Messages.Add(new(uin, name, message));
        Content = ToString();
        return this;
    }

    /// <summary>
    /// Add message
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public MultiMsgChain AddMessage(uint uin, string name, BaseChain chain)
        => AddMessage(uin, name, new MessageChain(chain));

    /// <summary>
    /// Set upload info
    /// </summary>
    /// <param name="info"></param>
    /// <param name="packed"></param>
    internal void SetMultiMsgUpInfo(MultiMsgUpInfo info, byte[] packed)
    {
        MultiMsgUpInfo = info;
        PackedData = packed;
        Content = ToString();
    }

    // internal void SetGuid(byte[] guid)
    //     => Guid = guid.ToHex();

    /// <summary>
    /// Create MultiMsg chain
    /// </summary>
    public static MultiMsgChain Create()
        => new();

    public override string ToString()
        => "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +

           // Msg
           "<msg serviceID=\"35\" templateID=\"1\" action=\"viewMultiMsg\" brief=\"[聊天记录]\" " +
           $"m_resid=\"{MultiMsgUpInfo?.MsgResId}\" " +
           $"m_fileName=\"{FileName}\" tSum=\"1\" sourceMsgId=\"0\" " +
           "url=\"\" flag=\"3\" adverSign=\"0\" multiMsgFlag=\"0\">" +

           // Message preview
           "<item layout=\"1\" advertiser_id=\"0\" aid=\"0\">" +
           "<title size=\"34\" maxLines=\"2\" lineSpace=\"12\">转发的聊天记录</title>" +
           GetPreviewString() +
           "<hr hidden=\"false\" style=\"0\" />" +
           $"<summary size=\"26\" color=\"#777777\">查看{Messages.Count}条转发消息</summary>" +
           "</item>" +

           // Banner
           "<source name=\"聊天记录\" icon=\"\" action=\"\" appid=\"-1\" />" +
           "</msg>";

    private string GetPreviewString()
    {
        var preview = "";
        var limit = 4;
        foreach (var msgstu in Messages)
        {
            if (--limit < 0) break;
            preview += "<title size=\"26\" color=\"#777777\" maxLines=\"2\" lineSpace=\"12\">" +
                       $"{msgstu.Sender.Name}: {msgstu.Chain[0]?.ToPreviewString()}</title>";
        }

        return preview;
    }

    internal override string ToPreviewString()
        => "[聊天记录]";
}
