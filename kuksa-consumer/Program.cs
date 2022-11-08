using System.Collections.Concurrent;
using Grpc.Core;
using Grpc.Net.Client;
using Sdv.Databroker.V1;

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<ConcurrentQueue<Datapoint>>();
builder.Services.AddHostedService<KuksaService>();
var app = builder.Build();

app.MapGet("/", (ConcurrentQueue<Datapoint> datapoints) => datapoints);
app.Run();

record Datapoint(double Speed, long Time);

sealed class KuksaService : BackgroundService
{
    private ConcurrentQueue<Datapoint> datapoints;

    public KuksaService(ConcurrentQueue<Datapoint> datapoints)
    {
        this.datapoints = datapoints;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
            var channel = GrpcChannel.ForAddress("http://localhost:55555");
            var client = new Broker.BrokerClient(channel);

            var response = client.Subscribe(request: new SubscribeRequest { Query = "SELECT Vehicle.Chassis.SteeringWheel.Angle,Vehicle.CurrentLocation.Timestamp,Vehicle.Speed" });

            while (await response.ResponseStream.MoveNext())
            {
                var timestamp = response.ResponseStream.Current.Fields["Vehicle.CurrentLocation.Timestamp"].Timestamp;
                var milliseconds = (long)(timestamp.Seconds * 1000 + timestamp.Nanos * 0.000001);

                var dp = new Datapoint(response.ResponseStream.Current.Fields["Vehicle.Speed"].FloatValue, milliseconds);

                this.datapoints.Enqueue(dp);

                if (this.datapoints.Count > 200) {
                    // Evict the oldest entry in the queue.
                    this.datapoints.TryDequeue(out var _);
                }
            }
    }
}
