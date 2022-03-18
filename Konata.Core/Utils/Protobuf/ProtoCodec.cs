using System;
using System.Collections.Generic;
using System.Linq;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf.ProtoModel;

namespace Konata.Core.Utils.Protobuf;

internal class ProtobufDecoder
{
    private readonly List<byte[]> _raw;
    private bool _isTree = true;
    private bool _isNumber;
    private long _valueAsNumber;
    private readonly Dictionary<uint, (ProtoType, List<byte[]>)> _leaves = new (); // <tag, (type, raw)>

    private ProtobufDecoder(byte[] raw)
    {
        _raw = new (){raw};
    }
    
    private ProtobufDecoder(List<byte[]> raw)
    {
        _raw = raw;
    }

    public List<ProtobufDecoder> AsLeaves()
    {
        var list = new List<ProtobufDecoder>();
        foreach (var pbDecoder in _raw)
        {
            list.Add(new(pbDecoder));
        }
        return list;
    }

    public string AsString()
    {
        return ProtoLengthDelimited.Create(AsBytes()).ToString();
    }

    public byte[] AsBytes()
    {
        return _raw.First();
    }

    public ByteBuffer AsBuffer()
    {
        return new ByteBuffer(AsBytes());
    }

    public long AsNumber()
    {
        if (!_isNumber)
            throw new Exception("A tree can not be converted to a number");
        return _valueAsNumber;
    }

    public ProtobufDecoder this[uint tag]
    {
        get
        {
            if (!_isTree)
                throw new Exception("A number can not be decoded as a tree");
            if (!_leaves.ContainsKey(tag))
                _setLeaves();
            if (!_leaves.ContainsKey(tag))
                throw new Exception("This tree does not contain tag: " + tag.ToString());
            var (type, list) = _leaves[tag];
            var raw = list.First();
            var pbDecoder = new ProtobufDecoder(list);
            switch (type)
            {
                case ProtoType.VarInt:
                    pbDecoder._isTree = false;
                    pbDecoder._isNumber = true;
                    pbDecoder._valueAsNumber = ProtoVarInt.Create(raw).Value;
                    break;

                case ProtoType.Bit32:
                    pbDecoder._isTree = false;
                    pbDecoder._isNumber = true;
                    pbDecoder._valueAsNumber = ProtoBit32.Create(raw).Value;
                    break;

                case ProtoType.Bit64:
                    pbDecoder._isTree = false;
                    pbDecoder._isNumber = true;
                    pbDecoder._valueAsNumber = ProtoBit64.Create(raw).Value;
                    break;

                default:
                case ProtoType.LengthDelimited:
                    pbDecoder._isTree = true;
                    pbDecoder._isNumber = false;
                    break;
            }
            return pbDecoder;
        }
    }

    private void _setLeaves()
    {
        var buffer = new ByteBuffer(_raw.First());
        {
            while (buffer.RemainLength > 0)
            {
                var tagtype = ProtoVarInt.Create(buffer.TakeVarIntBytes(out var _)).Value;
                var tag = (uint) (tagtype >> 3);
                var type = (ProtoType) (tagtype & 0b111);
                var raw = new byte[0];

                switch (type)
                {
                    case ProtoType.VarInt:
                        buffer.TakeVarIntBytes(out raw);
                        break;

                    case ProtoType.Bit32:
                        buffer.TakeBytes(out raw, 4);
                        break;

                    case ProtoType.Bit64:
                        buffer.TakeBytes(out raw, 8);
                        break;

                    default:
                    case ProtoType.LengthDelimited:
                        buffer.TakeBytes(out raw, (uint) buffer.TakeVarIntValueLE(out var _));
                        break;
                }
                if (!_leaves.ContainsKey(tag))
                    _leaves.Add(tag, (type, new(){raw}));
                else
                {
                    var (_, list) = _leaves[tag];
                    list.Add(raw);
                }
            }
        }
    }

    public static ProtobufDecoder Create(byte[] raw) => new(raw);
}
