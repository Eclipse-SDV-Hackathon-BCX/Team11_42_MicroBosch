using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Grpc.Core;
using Grpc.Net.Client;
using Sdv.Databroker.V1;

var app = WebApplication.CreateBuilder().Build();
app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            var channel = GrpcChannel.ForAddress("http://localhost:55555");
            var client = new Broker.BrokerClient(channel);

            var response = client.Subscribe(request: new SubscribeRequest { Query = "SELECT Vehicle.Chassis.SteeringWheel.Angle,Vehicle.CurrentLocation.Timestamp,Vehicle.Speed" });

            while (await response.ResponseStream.MoveNext())
            {
                await webSocket.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
                {
                    SteeringWheelAngle = response.ResponseStream.Current.Fields["Vehicle.Chassis.SteeringWheel.Angle"].Int32Value,
                    VehicleSpeed = response.ResponseStream.Current.Fields["Vehicle.Speed"].FloatValue,
                    Timestamp = response.ResponseStream.Current.Fields["Vehicle.CurrentLocation.Timestamp"].Timestamp
                })),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

                Console.WriteLine(response.ResponseStream.Current);
            }
        }
        else
        {
            await next(context);
        }
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

app.Run();
