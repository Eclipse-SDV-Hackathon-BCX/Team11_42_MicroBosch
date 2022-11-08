using Grpc.Core;
using Grpc.Net.Client;
using Sdv.Databroker.V1;

var channel = GrpcChannel.ForAddress("http://localhost:55555");
var client = new Broker.BrokerClient(channel);

var response = client.Subscribe(request: new SubscribeRequest { Query = "SELECT Vehicle.Chassis.SteeringWheel.Angle,Vehicle.CurrentLocation.Timestamp,Vehicle.Speed" });

while (await response.ResponseStream.MoveNext())
{
    Console.WriteLine(response.ResponseStream.Current);
    // "Greeting: Hello World" is written multiple times
}