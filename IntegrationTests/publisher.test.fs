module IntegrationTests.publisher

open System
open Microsoft.Extensions.Configuration
open NUnit.Framework
open common
open publisher



let getPublisher() =
    let secret = ConfigurationBuilder().AddUserSecrets("5a837560-b6ce-4bd1-aefa-187bd319e09a").Build()
    let configuration = {Url=secret.["RabbitMQ:URL"]}
    new Publisher(configuration)


[<Test>]
let ``Publish will create the queue`` () =

    helper.deleteQueue("test", false)

    use publisher = getPublisher()

    let message = String.Format( "{{when:\"{0:u}\"}}", DateTime.UtcNow); 

    publisher.Publish(message, "test", "")

    Assert.Pass()


[<Test>]
let ``Publish do not raise an error`` () =

    use publisher = getPublisher()

    let message = String.Format( "{{when:\"{0:u}\"}}", DateTime.UtcNow); 

    publisher.Publish(message, "test", "")

    Assert.Pass()
