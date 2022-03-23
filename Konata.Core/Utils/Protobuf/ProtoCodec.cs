using System;
using System.Collections.Generic;
using System.Linq;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf.ProtoModel;

namespace Konata.Core.Utils.Protobuf;

internal class ProtobufEncoder
{
    private readonly Dictionary<uint, (ProtoType, List<byte[]>)> _leaves = new();

    private ProtobufEncoder() { }

    public byte[] Marshal()
    {
        var buf = new ByteBuffer();
        foreach (var node in _leaves)
        {
            var tag = node.Key;
            var (type, list) = node.Value;
            var tagType = tag << 3 | (uint) type;
            list.ForEach(data =>
            {
                buf.PutBytes(ByteConverter.NumberToVarint(tagType));
                if (type == ProtoType.LengthDelimited)
                    buf.PutBytes(ByteConverter.NumberToVarint(data.Length));
                buf.PutBytes(data);
            });
        }

        return buf.GetBytes();
    }

    private void _save(uint tag, ProtoType type, byte[] data)
    {
        if (!_leaves.ContainsKey(tag))
            _leaves.Add(tag, (type, new()));
        var (_, list) = _leaves[tag];
        list.Add(data);
    }
    
    public ProtobufEncoder AddLeafVarInt(uint tag, long value)
    {
        _save(tag, ProtoType.VarInt, ByteConverter.NumberToVarint(value));
        return this;
    }

    public ProtobufEncoder AddLeafString(uint tag, string value)
    {
        _save(tag, ProtoType.LengthDelimited, ProtoLengthDelimited.Create(value));
        return this;
    }
    
    public ProtobufEncoder AddLeafBytes(uint tag, byte[] value)
    {
        _save(tag, ProtoType.LengthDelimited, ProtoLengthDelimited.Create(value));
        return this;
    }
    
    public ProtobufEncoder AddLeafBuffer(uint tag, ByteBuffer value)
    {
        _save(tag, ProtoType.LengthDelimited, ProtoLengthDelimited.Create(value.GetBytes()));
        return this;
    }

    public object this[uint tag]
    {
        set
        {
            switch (value)
            {
                case byte b:
                    AddLeafVarInt(tag, b);
                    break;
                case short s:
                    AddLeafVarInt(tag, s);
                    break;
                case ushort us:
                    AddLeafVarInt(tag, us);
                    break;
                case int i:
                    AddLeafVarInt(tag, i);
                    break;
                case uint ui:
                    AddLeafVarInt(tag, ui);
                    break;
                case long l:
                    AddLeafVarInt(tag, l);
                    break;
                case string str:
                    AddLeafString(tag, str);
                    break;
                case byte[] bytes:
                    AddLeafBytes(tag, bytes);
                    break;
                case ByteBuffer buf:
                    AddLeafBuffer(tag, buf);
                    break;
                case ProtobufEncoder pbEncoder:
                    AddLeafBytes(tag, pbEncoder.Marshal());
                    break;
                default:
                    throw new Exception("Unsupported type");
            }
        }
    }

    public static ProtobufEncoder Create()
        => new();
}

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
            if (_leaves.Count == 0)
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
                var tagType = ProtoVarInt.Create(buffer.TakeVarIntBytes(out var _)).Value;
                var tag = (uint) (tagType >> 3);
                var type = (ProtoType) (tagType & 0b111);
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
    
    public static ProtobufDecoder Create(ByteBuffer b) => new(b.GetBytes());

}
