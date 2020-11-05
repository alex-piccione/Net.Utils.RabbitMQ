module consumer

open System
open System.Text
open RabbitMQ.Client
open common




type Consumer(config:Config) =

    let factory = lazy(
        let factory = new ConnectionFactory(Uri=Uri(config.Url))
        factory.Uri <- Uri(config.Url)
        factory
    )

    member this.Consume(queue:string, routingKey:string) =

        let connection = factory.Value.CreateConnection()
        let channel = connection.CreateModel()

        let result = channel.QueueDeclare(queue, durable=true, exclusive=false, autoDelete=false, arguments=null)

        let getResult = channel.BasicGet(queue, autoAck=true)
        if getResult = null then null
        else
            let c = getResult.MessageCount
            let message = Encoding.UTF8.GetString(getResult.Body.Span)

            message


