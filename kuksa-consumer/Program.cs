using System.Text.Json;
using Grpc.Core;
using Grpc.Net.Client;
using Sdv.Databroker.V1;

var channel = GrpcChannel.ForAddress("http://localhost:55555");
var client = new Broker.BrokerClient(channel);

var response = client.Subscribe(request: new SubscribeRequest { Query = "SELECT Vehicle.Chassis.SteeringWheel.Angle,Vehicle.CurrentLocation.Timestamp,Vehicle.Speed" });

while (await response.ResponseStream.MoveNext())
{
    var data = JsonSerializer.Serialize(new
    {
        SteeringWheelAngle = response.ResponseStream.Current.Fields["Vehicle.Chassis.SteeringWheel.Angle"].Int32Value,
        VehicleSpeed = response.ResponseStream.Current.Fields["Vehicle.Speed"].FloatValue,
        Timestamp = response.ResponseStream.Current.Fields["Vehicle.CurrentLocation.Timestamp"].Timestamp
    });

    // Pass the data to the consuming application via standard output
    Console.WriteLine(data);
}
