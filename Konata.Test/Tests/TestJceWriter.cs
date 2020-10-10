using System;
using Konata.Utils;

namespace Konata.Test.Tests
{
    public class TestJceWriter : Test
    {
        public override bool Run()
        {
            var jce = new JceWriter();
            jce.PutJceByte(0, 1);
            jce.PutJceShort(1, 2);
            jce.PutJceInt(2, 3);
            jce.PutJceLong(3, 4);
            {
                var jce2 = new JceWriter();
                jce2.PutJceString4(0, "Hello JCE");
                jce2.PutJceInt(1, 233);

                jce.PutJceWriter(4, jce2);
            }
            jce.PutJceLong(5, 4);

            return true;
        }
    }
}
