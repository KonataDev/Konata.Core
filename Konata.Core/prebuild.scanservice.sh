#!bin/bash

rm Msf/ServiceTouch2.cs || true;

cat > Msf/ServiceTouch2.cs << _EOF_
// This file is automatic generated by script.
// DO NOT EDIT DIRECTLY.

using System;

namespace Konata.Msf
{
    internal abstract partial class Service
    {
        static bool TouchServices()
        {

            return true;
        }
    }
}

_EOF_