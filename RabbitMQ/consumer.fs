namespace Alex75.Utils.RabbitMQ

open System
open System.Text
open RabbitMQ.Client
open common
open RabbitMQ.Client.Events


type ConsumerConfig = {
    Url:string
    Exchange:string
    RoutingKey:string
    Queue:string
}

type Consumer(config:Config) =

    let factory = lazy(
        let factory = new ConnectionFactory(Uri=Uri(config.Url))
        factory.Uri <- Uri(config.Url)
        factory
    )

    member this.Consume(queue:string, exchange:string, routingKey:string) =

        let connection = factory.Value.CreateConnection()
        let channel = connection.CreateModel()

        // create the queue
        let result = channel.QueueDeclare(queue, durable=true, exclusive=false, autoDelete=false, arguments=null)
        // create teh exchage 
        channel.ExchangeDeclare(exchange, ``type``="direct", durable=true, autoDelete=false, arguments=null)
        // and bind it to the exchange
        channel.QueueBind(queue, exchange, routingKey, arguments=null)

        let getResult = channel.BasicGet(queue, autoAck=true)
        if getResult = null then null
        else
            let c = getResult.MessageCount
            let message = Encoding.UTF8.GetString(getResult.Body.Span)

            message


    member this.StartReceiving<'a>(queue:string, exchange:string, routingKey:string, received: 'a -> unit) =

        use connection = factory.Value.CreateConnection()
        use channel = connection.CreateModel()

        // create the queue
        let result = channel.QueueDeclare(queue, durable=true, exclusive=false, autoDelete=false, arguments=null)
        // create teh exchage 
        channel.ExchangeDeclare(exchange, ``type``="direct", durable=true, autoDelete=false, arguments=null)
        // and bind it to the exchange
        channel.QueueBind(queue, exchange, routingKey, arguments=null)


        let parseContent (bytes:ReadOnlyMemory<byte>) = common.Deserialize<'a>(bytes)


        let consumer = EventingBasicConsumer(channel)
        consumer.Received.Add(fun event -> 
            // todo: catch error and raise event + retry
            let item:'a = parseContent(event.Body)            
            received(item)
        )
        