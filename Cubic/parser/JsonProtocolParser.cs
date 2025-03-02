using Cubic.V1.Protocol;
using Cubic.V1.Utils;
using Google.Protobuf;

namespace Cubic.V1.Parser
{
    public class JsonProtocolParser
    {
        private readonly JsonParser parser;

        public JsonProtocolParser()
        {
            parser = new JsonParser(new JsonParser.Settings(100, Registry.ProtocolRegistry));
        }

        public JsonParser Parser
        {
            get
            {
                return parser;
            }
        }

        public IMessage ReadProtocol(string json)
        {
            ProtocolMessage message = Parser.Parse<ProtocolMessage>(json);
            return message.Payload.Unpack(Registry.ProtocolRegistry);
        }
    }
}
