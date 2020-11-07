namespace Alex75.Utils.RabbitMQ

open System
open System.Text
open RabbitMQ.Client
open common
open RabbitMQ.Client.Events
open System.Threading
open System.Threading.Tasks


type ConsumerConfig<'a> = {
    Url:string
    Exchange:string
    RoutingKey:string
    Queue:string
    OnReceived: 'a -> unit
    OnError: Exception -> unit
}

type Consumer(config:Config) = 
    
    let factory = lazy(
        let factory = new ConnectionFactory(Uri=Uri(config.Url))
        factory.Uri <- Uri(config.Url)
        factory
    )

    let connection = factory.Value.CreateConnection()
    let channel = connection.CreateModel()

    let mutable stop=false


    member this.StartReceiving<'a>(queue:string, exchange:string, routingKey:string, received: 'a -> unit, onError: Exception -> unit) =

        stop <- false

        // create the queue
        let result = channel.QueueDeclare(queue, durable=true, exclusive=false, autoDelete=false, arguments=null)
        // create the exchange 
        channel.ExchangeDeclare(exchange, ``type``="direct", durable=true, autoDelete=false, arguments=null)
        // and bind it to the exchange
        channel.QueueBind(queue, exchange, routingKey, arguments=null)

        let parseContent (bytes:ReadOnlyMemory<byte>) = common.Deserialize<'a>(bytes)

        let token = CancellationToken()


        let consumer = EventingBasicConsumer(channel)
        consumer.Received.Add(fun event -> 
            try 
                let item:'a = parseContent(event.Body)   
                try 
                    received item
                finally ()
            with e -> onError e             
        )

        async {            
            channel.BasicConsume(queue, autoAck=true, consumer=consumer) |> ignore            
            while not stop do
                Thread.Sleep(500)
        } |> Async.Start

                
    
    member this.StopReceiving() = stop <- true

    interface IDisposable with
        member this.Dispose() =
            channel.Dispose()
            connection.Dispose()


