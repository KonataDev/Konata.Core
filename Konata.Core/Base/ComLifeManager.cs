using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base
{
    public sealed class ComLifeManager
    {
        private static ComLifeManager instance;

        public static ComLifeManager Instance
        {
            get => instance ?? (instance = new ComLifeManager());
        }

        private ComLifeManager()
        {

        }

        public static void Release()
        {
            instance = null;
        }




        ~ComLifeManager()
        {

        }
    }
}
