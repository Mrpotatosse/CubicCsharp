using Cubic.utils;
using Cubic.V1.Protocol;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Cubic.formatter
{
    public class JsonProtocolFormatter
    {
        private readonly JsonFormatter formatter;

        public JsonProtocolFormatter()
        {
            formatter = new JsonFormatter(new JsonFormatter.Settings(true, Registry.ProtocolRegistry));
        }

        public JsonFormatter Formatter
        {
            get
            {
                return formatter;
            }
        }

        public string WriteProtocol(IMessage message)
        {
            ProtocolMessage protocolMessage = new ProtocolMessage()
            {
                Payload = Any.Pack(message)
            };

            return Formatter.Format(protocolMessage);
        }
    }
}
