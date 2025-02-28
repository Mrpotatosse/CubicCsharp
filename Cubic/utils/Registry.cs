using Cubic.V1.Database.Model;
using Cubic.V1.Protocol;
using Cubic.V1.Protocol.Auth;
using Cubic.V1.Protocol.Event;
using Cubic.V1.Protocol.Http;
using Cubic.V1.Protocol.World;
using Google.Protobuf.Reflection;

namespace Cubic.utils
{
    public sealed class Registry
    {
        private Registry() { }

        public static readonly TypeRegistry ProtocolRegistry = TypeRegistry.FromFiles(
            AuthReflection.Descriptor,
            WorldReflection.Descriptor,
            EventReflection.Descriptor,
            ProtocolReflection.Descriptor,
            HttpReflection.Descriptor


        );

        public static readonly TypeRegistry DatabaseRegistry = TypeRegistry.FromFiles(
            ModelReflection.Descriptor
            );
    }
}
