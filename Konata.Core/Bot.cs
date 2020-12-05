using System;

using Konata.Core.Manager;
using Konata.Runtime.Base;

namespace Konata.Core
{
    public class Bot : Entity
    {
        public Bot(uint botUin, string botPassword)
        {
            AddComponent<ConfigManager>();
            AddComponent<SsoInfoManager>();
            AddComponent<UserSigManager>().InitializeProfile(botUin, botPassword);
        }
    }
}
