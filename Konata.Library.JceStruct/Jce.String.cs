namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct String : IObject
        {
            public Type Type
            {
                get
                {
                    return /*Value == null || Value.Length == 0 ? Type.ZeroTag : */Value.Length <= sbyte.MaxValue ? Type.String1 : Type.String4;
                }
            }

            public string Value { get; set; }

            public String(string value)
            {
                Value = value;
            }

            public static implicit operator string(String value)
            {
                return value.Value;
            }

            public static implicit operator String(string value)
            {
                return new String(value);
            }
        }
    }
}