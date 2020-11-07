[<NUnit.Framework.Category("Consumer")>]
module IntegrationTests.consumer

open System
open System.Text.Json
open NUnit.Framework
open FsUnit
open Alex75.Utils.RabbitMQ


[<Test>]
let ``StartReceiving <should> consume published messages`` () =

    use consumer = helper.createConsumer()

    let n = Random().Next(100).ToString()

    let queue       = "test-queue-"+n
    let exchange    = "test-exhange-"+n
    let routingKey  = "test-key-"+n

    helper.deleteQueue(queue, false)
    helper.deleteExchange(exchange, false)

    try 

        let mutable receivedMessages = 0

        // start receiving
        let onReceived = (fun _ -> receivedMessages <- receivedMessages+ 1)
        let onError = ignore
        consumer.StartReceiving(queue, exchange, routingKey, onReceived, onError)

        use publisher = new Publisher(secrets.URL, exchange)

        let message1 = helper.TestObject("string 1", 1m, DateTime.UtcNow)
        let message2 = helper.TestObject("string 2", 2m, DateTime.UtcNow)
        let message3 = helper.TestObject("string 3", 3m, DateTime.UtcNow)

        // publish
        publisher.Send(JsonSerializer.Serialize(message1), exchange, routingKey)
        publisher.Send(JsonSerializer.Serialize(message2), exchange, routingKey)
        publisher.Send(JsonSerializer.Serialize(message3), exchange, routingKey)

        // wait until we have a result
        for i in 0..5 do
            if receivedMessages < 3 then
                Threading.Thread.Sleep(200)
        
        consumer.StopReceiving()
    
        receivedMessages |> should equal 3
    
    finally       

        helper.deleteQueue(queue, false)
        helper.deleteExchange(exchange, false)
