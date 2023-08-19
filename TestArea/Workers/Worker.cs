using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;

namespace TestArea.Workers;

public class Worker<T>
    where T : IMessage
{
    private readonly long _id;
    private DeduplicatingProducer? _producer;
    private readonly ActionBlock<T> _messageProcessor;

    public Worker(long id)
    {
        _messageProcessor = new ActionBlock<T>(ProcessMessages, new ExecutionDataflowBlockOptions()
        {
            MaxDegreeOfParallelism = 1,
            BoundedCapacity = 10,
        });

        _id = id;
    }

    public async Task Start()
    {
        var streamSystem = await StreamSystem.Create(
            new StreamSystemConfig
            {
                UserName = "guest",
                Password = "guest",
                Endpoints = new List<EndPoint> { new DnsEndPoint("localhost", 5552) }
            }
        );

        const string name = "jopa";
        await streamSystem.CreateStream(
            new StreamSpec(name)
            {
                MaxAge = TimeSpan.FromHours(6),
                MaxLengthBytes = 40_000_000,
                MaxSegmentSizeBytes = 20_000_000
            }).ConfigureAwait(false);

        _producer = await DeduplicatingProducer.Create(
            new DeduplicatingProducerConfig(streamSystem,
                name,
                _id.ToString())
            {
                ConfirmationHandler = confirmation =>
                {
                    switch (confirmation.Status)
                    {
                        case ConfirmationStatus.Confirmed:
                            // logger.LogInformation("Message confirmed");
                            break;
                        case ConfirmationStatus.ClientTimeoutError:
                        case ConfirmationStatus.StreamNotAvailable:
                        case ConfirmationStatus.InternalError:
                        case ConfirmationStatus.AccessRefused:
                        case ConfirmationStatus.PreconditionFailed:
                        case ConfirmationStatus.PublisherDoesNotExist:
                        case ConfirmationStatus.UndefinedError:
                            // logger.LogError("Message not confirmed with error: {0}", confirmation.Status);
                            break;
                        case ConfirmationStatus.WaitForConfirmation:
                        default:
                            break;
                    }

                    return Task.CompletedTask;
                }
            });

        // var logger = new LoggerFakctory().CreateLogger<Producer>();
    }

    public void PushMessage(T message)
    {
        _messageProcessor.Post(message);
    }

    public void Stop()
    {
        _producer.Close();
    }

    private async Task ProcessMessages(T messageQueue)
    {
        try
        {
            var message = new Message(JsonSerializer.SerializeToUtf8Bytes(messageQueue));

            await _producer.Send((ulong)messageQueue.EventCode, message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}