using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Protocol
{
    public static class SsoServiceCmd
    {
        public enum Command
        {
            [Description("wtlogin.unknown")]
            WtUnknown = 0,

            [Description("wtlogin.login")]
            WtLoginAuth,

            [Description("wtlogin.exchange")]
            WtLoginExchange,

            [Description("wtlogin.name2uin")]
            WtLoginName2Uin,

            [Description("wtlogin.trans_emp")]
            WtLoginTransEmp
        }

        public static string ToString(Command cmd)
        {
            Type type = cmd.GetType();
            string name = Enum.GetName(type, cmd);

            FieldInfo fieldInfo = type.GetField(name);

            DescriptionAttribute attr =
                Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attr == null)
            {
                throw new Exception("SsoServiceCmd.Command has null field description.");
            }

            return attr.Description;
        }

        public static Command TryParse(string cmd)
        {
            foreach (var element in typeof(Command).GetFields())
            {
                DescriptionAttribute attr
                    = Attribute.GetCustomAttribute(element, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr == null)
                    continue;
                if (attr.Description == cmd)
                {
                    return (Command)element.GetValue(null);
                }
            }

            throw new Exception("Try parsing SsoServiceCmd.Command failed.");
        }
    }
}
