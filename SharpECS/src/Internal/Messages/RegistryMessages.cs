using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Internal.Messages
{
    internal readonly record struct RegistryDisposedMessage(ushort RegistryID);
}
