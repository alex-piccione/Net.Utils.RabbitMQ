# Net.Utils.RabbitMQ
RabbitMQ helper library to publish an receive messages on a RabbitMQ instance. 


Nuget package: \<not yet created\>


## How to use


### Publisher (sender)
``` fsharp
let publisher = Publisher(<RabbitMQ-URL>, <exchange:string>, <routingKey:string>)
// optionally it can create the Queue:
publisher.CreateQueue(<queue:string>)

// publish a string message (can be a JSON string)
publisher.Publish(<message:string>)

// or it will serialize the objet in JSON format
publisher.Publish(<data:obj>)

```

### Consumer (receiver)
``` fsharp
let consumer = consumer.Consumer(<RabbitMQ-URL>, <exchange:string>, <queue:string> <routingKey:string>)


consumer.StartConsuming<'a>( <received: fun () -> a> )



```



## Exchange

### Type

- fanout
- direct