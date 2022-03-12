using System.Collections.Generic;

namespace Konata.Core.Events.Model;

internal class PbGetMessageEvent : ProtocolEvent
{
	/// <summary>
	/// <b>[In]</b> <br/>
	/// Sync cookie <br/>
	/// </summary>
	public byte[] SyncCookie { get; }

	/// <summary>
	/// <b>[In]</b> <br/>
	/// Inner events <br/>
	/// </summary>
	public List<ProtocolEvent> InnerEvent { get; }

	private PbGetMessageEvent(byte[] syncCookie)
		  : base(6000, true)
	{
		SyncCookie = syncCookie;
	}

	private PbGetMessageEvent(int result, byte[] cookie,
		 List<ProtocolEvent> inner) : base(result)
	{
		SyncCookie = cookie;
		InnerEvent = inner;
	}

	/// <summary>
	/// Construct event request
	/// </summary>
	/// <param name="syncCookie"></param>
	/// <returns></returns>
	internal static PbGetMessageEvent Create(byte[] syncCookie)
		=> new(syncCookie);

	internal static PbGetMessageEvent Result(int result, byte[] cookie,
		List<ProtocolEvent> events) => new(result, cookie, events);
}
