# Cubic

Exemple

```csharp
using Cubic.V1.Formatter;
using Cubic.V1.Parser;
using Cubic.V1.Protocol.Auth;
using Google.Protobuf;

JsonProtocolFormatter formatter = new JsonProtocolFormatter();
string json = formatter.WriteProtocol(new HelloAuthRequest());

JsonProtocolParser parser = new JsonProtocolParser();
IMessage message = parser.ReadProtocol(json);

if(message is HelloAuthRequest har)
{
    Console.WriteLine(message.Descriptor.FullName + " " + har);
}
```