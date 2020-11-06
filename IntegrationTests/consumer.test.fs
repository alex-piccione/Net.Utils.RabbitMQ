module IntegrationTests.consumer

open System
open NUnit.Framework
open FsUnit
open common
open System.Text
open System.Text.Json




[<Test>]
let ``Consume <should> create exchange and queue`` () =

    let queue = "test 1"
    let exchange = "test"
    helper.deleteQueue(queue, false)
    helper.deleteExchange(exchange, false)

    let consumer = helper.createConsumer()

    let message = String.Format("{{when:\"{0:u}\"}}", DateTime.UtcNow); 

    // act
    let message = consumer.Consume(queue, exchange, "key 1")

    
    let queues = helper.listQueues()
    queues |> List.exists (fun x -> fst x = queue) |> should be True  

    let exchanges = helper.listExchanges()
    exchanges |> List.exists (fun x -> x.Name = exchange) |> should be True

    helper.deleteQueue(queue, false)
    helper.deleteExchange(exchange, false)



[<Test>]
let ``StartReceiving consumes published messages`` () =

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

        let publisher = helper.createPublisher()

        let message1 = helper.TestObject("string 1", 1m, DateTime.UtcNow)
        let message2 = helper.TestObject("string 2", 2m, DateTime.UtcNow)
        let message3 = helper.TestObject("string 3", 3m, DateTime.UtcNow)

        // publish
        publisher.Send(JsonSerializer.Serialize(message1), exchange, routingKey)
        publisher.Send(JsonSerializer.Serialize(message2), exchange, routingKey)
        publisher.Send(JsonSerializer.Serialize(message3), exchange, routingKey)

        for i in 0..5 do
            if receivedMessages < 3 then
                Threading.Thread.Sleep(500)
        
        consumer.StopReceiving()
    
        receivedMessages |> should equal 3
    
    finally
        helper.deleteQueue(queue, false)
        helper.deleteExchange(exchange, false)
