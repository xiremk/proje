/*
using RabbitMQ.Client;
using System.Text;

public class RabbitMQService
{
    private readonly string _hostname;
    private readonly string _username;
    private readonly string _password;
    private IConnection _connection;

    public RabbitMQService(string cloudAMQPUrl)
    {
        _hostname = "cougar.rmq.cloudamqp.com";
        _username = "ocsqrzxi";
        _password = "roPyk0zLnDBX5sGsNWtPoOx9JkARUkY6";
        CreateConnection();
    }

    private void CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            UserName = _username,
            Password = _password,
            Uri = new Uri("amqps://ocsqrzxi:roPyk0zLnDBX5sGsNWtPoOx9JkARUkY6@cougar.rmq.cloudamqp.com/ocsqrzxi")
        };

        _connection = factory.CreateConnection();
    }

    public void Publish(string message, string queueName)
    {
        using (var channel = _connection.CreateModel())
        {
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }
}

*/

using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;

public class RabbitMQService
{
    private readonly string _hostname;
    private readonly string _username;
    private readonly string _password;
    private IConnection _connection;

    public RabbitMQService(string cloudAMQPUrl)
    {
        _hostname = "cougar.rmq.cloudamqp.com";
        _username = "ocsqrzxi";
        _password = "roPyk0zLnDBX5sGsNWtPoOx9JkARUkY6";
        CreateConnection();
    }

    private void CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            UserName = _username,
            Password = _password,
            Uri = new Uri("amqps://ocsqrzxi:roPyk0zLnDBX5sGsNWtPoOx9JkARUkY6@cougar.rmq.cloudamqp.com/ocsqrzxi")
        };

        _connection = factory.CreateConnection();
    }

    public IModel CreateConsumerChannel(string queueName)
    {
        var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        return channel;
    }

    // Yeni metod
    public void EnqueueFileForProcessing(IFormFile file)
    {
        using (var channel = _connection.CreateModel())
        {
            channel.QueueDeclare(queue: "excelQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            // Dosyayı byte dizisine çevir
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                var fileBytes = memoryStream.ToArray();

                // Byte dizisini RabbitMQ kuyruğuna ekle
                var body = Encoding.UTF8.GetBytes(Convert.ToBase64String(fileBytes));
                channel.BasicPublish(exchange: "", routingKey: "excelQueue", basicProperties: null, body: body);
            }
        }
    }
    }
