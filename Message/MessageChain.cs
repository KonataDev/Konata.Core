using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message;

public class MessageChain : IEnumerable<BaseChain>
{
    internal List<BaseChain> Chains { get; }

    internal MessageChain()
        => Chains = new();

    internal MessageChain(params BaseChain[] chain)
        => Chains = new(chain.Where(i => i != null));

    /// <summary>
    /// Add chain
    /// </summary>
    /// <param name="chain"></param>
    internal void Add(BaseChain chain)
        => Chains.Add(chain);

    /// <summary>
    /// Add chains
    /// </summary>
    /// <param name="chains"></param>
    internal void AddRange(IEnumerable<BaseChain> chains)
        => Chains.AddRange(chains);

    public IEnumerator<BaseChain> GetEnumerator()
        => Chains.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Convert chain to code string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => Chains.Aggregate("", (current, element) => current + element);

    /// <summary>
    /// Find chains
    /// </summary>
    /// <typeparam name="TChain"></typeparam>
    /// <returns></returns>
    public List<TChain> FindChain<TChain>()
        => Chains.Where(i => i is TChain).Cast<TChain>().ToList();

    /// <summary>
    /// Get a chain
    /// </summary>
    /// <typeparam name="TChain"></typeparam>
    /// <returns></returns>
    public TChain GetChain<TChain>()
        => FindChain<TChain>().FirstOrDefault();

    /// <summary>
    /// Filter the message chain with a chain type
    /// </summary>
    /// <param name="x"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<BaseChain> operator |(MessageChain x, BaseChain.ChainType type)
        => x.Chains.Where(c => c.Type != type);

    /// <summary>
    /// Filter the message chain with a chain type
    /// </summary>
    /// <param name="x"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<BaseChain> operator &(MessageChain x, BaseChain.ChainType type)
        => x.Chains.Where(c => c.Type == type);

    /// <summary>
    /// Filter the message chain with a chain mode
    /// </summary>
    /// <param name="x"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static IEnumerable<BaseChain> operator |(MessageChain x, BaseChain.ChainMode mode)
        => x.Chains.Where(c => c.Mode != mode);

    /// <summary>
    /// Filter the message chain with a chain mode
    /// </summary>
    /// <param name="x"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static IEnumerable<BaseChain> operator &(MessageChain x, BaseChain.ChainMode mode)
        => x.Chains.Where(c => c.Mode == mode);

    public List<BaseChain> this[Range r]
    {
        get
        {
            var (offset, length) = r.GetOffsetAndLength(Chains.Count);
            return Chains.GetRange(offset, length);
        }
    }

    public BaseChain this[int index]
        => Chains[index];

    public List<BaseChain> this[Type type]
        => Chains.Where(c => c.GetType() == type).ToList();

    public List<BaseChain> this[BaseChain.ChainMode mode]
        => Chains.Where(c => c.Mode == mode).ToList();

    public List<BaseChain> this[BaseChain.ChainType type]
        => Chains.Where(c => c.Type == type).ToList();
}
