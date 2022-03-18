using System;
using System.Collections.Generic;
using System.Linq;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf.ProtoModel;

namespace Konata.Core.Utils.Protobuf;

internal class ProtobufDecoder
{
    private readonly List<byte[]> _raw;
    private long? _valueAsNumber;
    private readonly Dictionary<uint, (ProtoType, List<byte[]>)> _leaves = new();

    private ProtobufDecoder(byte[] raw)
    {
        _raw = new(){raw};
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
        if (_valueAsNumber != null)
            throw new Exception("Number can not be read as LengthDelimited");
        return _raw.First();
    }

    public ByteBuffer AsBuffer()
    {
        return new ByteBuffer(AsBytes());
    }

    public long AsNumber()
    {
        if (_valueAsNumber == null)
            throw new Exception("LengthDelimited can not be read as Number");
        return (long) _valueAsNumber;
    }

    public ProtobufDecoder this[uint tag]
    {
        get
        {
            if (_valueAsNumber != null)
                throw new Exception("Number can not be read as LengthDelimited");
            if (!_leaves.ContainsKey(tag))
                _setLeaves();
            if (!_leaves.ContainsKey(tag))
                throw new Exception("This LengthDelimited does not contain tag: " + tag.ToString());
            var (type, list) = _leaves[tag];
            var raw = list.First();
            var pbDecoder = new ProtobufDecoder(list);
            switch (type)
            {
                case ProtoType.VarInt:
                    pbDecoder._valueAsNumber = ProtoVarInt.Create(raw).Value;
                    break;

                case ProtoType.Bit32:
                    pbDecoder._valueAsNumber = ProtoBit32.Create(raw).Value;
                    break;

                case ProtoType.Bit64:
                    pbDecoder._valueAsNumber = ProtoBit64.Create(raw).Value;
                    break;

                default:
                case ProtoType.LengthDelimited:
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
