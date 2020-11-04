module publisher


open System
open System.Text
open RabbitMQ.Client


type Config = {Url:string}

type Publisher(config:Config) =


    member this.Publish(message:string, exchange:string, routingKey:string) = 
        let factory = new ConnectionFactory()
        factory.Uri <- Uri(config.Url)
        //factory.Endpoint <- AmqpTcpEndpoint(config.Url)
        use connection = factory.CreateConnection()
        use channel = connection.CreateModel()

        //let result = channel.QueueDeclare(queue=queue, durable=false, exclusive=false, autoDelete = false, arguments = null)

        let body = ReadOnlyMemory(Encoding.UTF8.GetBytes(message))

        channel.BasicPublish(exchange=exchange, routingKey=routingKey, basicProperties=null, body=body)



