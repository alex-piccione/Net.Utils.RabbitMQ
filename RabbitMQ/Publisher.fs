namespace Alex75.Utils.RabbitMQ

open System
open System.Text
open RabbitMQ.Client
open common


type Publisher(url:string, exchange:string, queue:string option) =    
    
    //let factory = lazy(
    //    let factory = new ConnectionFactory(Uri=Uri(url))
    //    //factory.Uri <- Uri(config.Url)
    //    factory
    //)

    let factory = new ConnectionFactory(Uri=Uri(url))
    let connection = factory.CreateConnection()
    let channel = connection.CreateModel()

    do
        channel.ExchangeDeclare(exchange, ``type``="direct", durable=true, autoDelete=false, arguments=null)
        if queue.IsSome then
            let result = channel.QueueDeclare(queue=queue.Value, durable=false, exclusive=false, autoDelete = false, arguments = null)
            if result = null then failwithf "Failed to create queue \"%s\"." queue.Value

    new(url:string, exchange:string) =
        new Publisher(url, exchange, None)


    member this.CreateQueue(queue:string) =
        let result = channel.QueueDeclare(queue=queue, durable=false, exclusive=false, autoDelete = false, arguments = null)
        if result = null then failwithf "Failed to create queue \"%s\"." queue


    member this.Send(message:string, exchange:string, routingKey:string) = 
        let body = ReadOnlyMemory(Encoding.UTF8.GetBytes(message))
        channel.BasicPublish(exchange=exchange, routingKey=routingKey, basicProperties=null, body=body)

    
    interface IDisposable with
        member this.Dispose() =
            channel.Dispose()
            connection.Dispose()