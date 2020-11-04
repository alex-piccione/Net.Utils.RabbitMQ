module IntegrationTests.publisher

open System
open Microsoft.Extensions.Configuration
open NUnit.Framework
open publisher



let secret = ConfigurationBuilder().AddUserSecrets("5a837560-b6ce-4bd1-aefa-187bd319e09a").Build()

let getPublisher() =
    let configuration = {Url=secret.["RabbitMQ:URL"]}
    Publisher(configuration)



[<Test>]
let ``Publish do not raise an error`` () =

    let publisher = getPublisher()

    let message = String.Format( "{when:\"{0:u}\"}", DateTime.UtcNow); 

    publisher.Publish(message)

    Assert.Pass()
