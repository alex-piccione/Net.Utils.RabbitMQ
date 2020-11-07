module secrets

open Microsoft.Extensions.Configuration

let secret = ConfigurationBuilder().AddUserSecrets("5a837560-b6ce-4bd1-aefa-187bd319e09a").Build()
let baseAddress = secret.["RabbitMQ:API base address"]
let vhost = secret.["RabbitMQ:vhost"]
let username = secret.["RabbitMQ:username"]
let password = secret.["RabbitMQ:password"]
let URL = secret.["RabbitMQ:URL"]