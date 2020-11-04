namespace RabbitMQ

open System
open publisher

module Say =
    let hello name =
        printfn "Hello %s" name


//        {
//  "info": "cloudamqp.com free subsription",
//  "RabbitMQ:URL": "amqps://gydvvzsv:EnvKV0EkBJjF8QqrPUlu1bmhnU4aIdsF@rattlesnake.rmq.cloudamqp.com/gydvvzsv"
//}{
//  "info": "cloudamqp.com free subsription",
//  "RabbitMQ:URL": "amqps://gydvvzsv:EnvKV0EkBJjF8QqrPUlu1bmhnU4aIdsF@rattlesnake.rmq.cloudamqp.com/gydvvzsv"
//}
        
        //let publisher = publisher.Publisher(configuration)

        //let message = sprintf "{when:\"%s\"}" (DateTime.UtcNow.ToString("u"))

        //publisher.publish(message)

        //()
