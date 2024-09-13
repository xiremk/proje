/*
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMQConsumer : BackgroundService
{
    private readonly RabbitMQService _rabbitMQService;
    private readonly string _queueName;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumer(RabbitMQService rabbitMQService, string queueName, IServiceProvider serviceProvider)
    {
        _rabbitMQService = rabbitMQService;
        _queueName = queueName;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqps://ocsqrzxi:roPyk0zLnDBX5sGsNWtPoOx9JkARUkY6@cougar.rmq.cloudamqp.com/ocsqrzxi")
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);


            // Dependency injection ile MessageController'ı kullanarak mesajı ekle
            using (var scope = _serviceProvider.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<MessageController>();
                controller.ReceiveMessage(message);
            }

        };

        channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
*/
/*
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMQConsumer : BackgroundService
{
    private readonly RabbitMQService _rabbitMQService;
    private readonly string _queueName;
    public static ConcurrentQueue<string> ReceivedMessages = new ConcurrentQueue<string>();

    public RabbitMQConsumer(RabbitMQService rabbitMQService, string queueName)
    {
        _rabbitMQService = rabbitMQService;
        _queueName = queueName;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqps://ocsqrzxi:roPyk0zLnDBX5sGsNWtPoOx9JkARUkY6@cougar.rmq.cloudamqp.com/ocsqrzxi")
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            ReceivedMessages.Enqueue(message);
        };

        channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
*/
/*
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMQConsumer : BackgroundService
{
    private readonly RabbitMQService _rabbitMQService;
    private readonly string _queueName;
    public static ConcurrentQueue<string> ReceivedMessages = new ConcurrentQueue<string>();

    public RabbitMQConsumer(RabbitMQService rabbitMQService, string queueName)
    {
        _rabbitMQService = rabbitMQService;
        _queueName = queueName;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://ocsqrzxi:roPyk0zLnDBX5sGsNWtPoOx9JkARUkY6@cougar.rmq.cloudamqp.com/ocsqrzxi")
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                ReceivedMessages.Enqueue(message);
                Console.WriteLine($"Received: {message}"); // Log received message
            };

            channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Consumer started.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ExecuteAsync: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}
*/


















/*
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;
using OfficeOpenXml; // EPPlus Kütüphanesi
using Microsoft.Extensions.DependencyInjection;

public class RabbitMQConsumer : BackgroundService
{
 
    private readonly RabbitMQService _rabbitMQService;

    private readonly string _queueName = "excelQueue";  // Kuyruk adı


    private IModel _channel;

    public RabbitMQConsumer(RabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
        _channel = _rabbitMQService.CreateConsumerChannel("excelQueue"); // Burada queueName geçiliyor
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            try
            {
                // Mesaj içeriğini al
                var body = ea.Body.ToArray();

                // Mesajdan dosyayı belleğe yükle
                using (var memoryStream = new MemoryStream(body))
                {
                    // EPPlus kullanarak Excel dosyasını okuma
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // İlk sayfayı al
                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        // Hücreleri oku ve işleyin
                        for (int row = 1; row <= rowCount; row++)
                        {
                            for (int col = 1; col <= colCount; col++)
                            {
                                var cellValue = worksheet.Cells[row, col].Text;
                                Console.WriteLine($"Satır: {row}, Sütun: {col}, Değer: {cellValue}");

                                // Burada işleme kodunuzu yazabilirsiniz




                            }
                        }
                    }
                    // Dosya işleme mantığı
                    ProcessFile(body);


                }

                // Mesaj başarılı şekilde işlendiğinde RabbitMQ'ya bildir
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya işleme hatası: {ex.Message}");
                // Hata durumunda mesajı yeniden kuyruğa almak isteyebilirsiniz
            }
        };

        _channel.BasicConsume(queue: "excelQueue", autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }



    private void ProcessFile(byte[] fileBytes)
    {
        // Burada dosya işleme mantığını ekleyin
        // Örneğin, dosyayı diske kaydedebilir, veritabanına kaydedebilir veya analiz edebilirsiniz.
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", "ProcessedExcel.xlsx");

        File.WriteAllBytes(filePath, fileBytes);

        Console.WriteLine("Dosya başarıyla işlendi ve kaydedildi: " + filePath);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        base.Dispose();
    }






}


*/


using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;
using OfficeOpenXml; // EPPlus Kütüphanesi
using Microsoft.Extensions.DependencyInjection;
using MyExcelUploader.Services; // ExcelService sınıfını kullanmak için

public class RabbitMQConsumer : BackgroundService
{
    private readonly RabbitMQService _rabbitMQService;
    private readonly ExcelService _excelService;
    private readonly string _queueName = "excelQueue";  // Kuyruk adı
    private IModel _channel;

    public RabbitMQConsumer(RabbitMQService rabbitMQService, ExcelService excelService)
    {
        _rabbitMQService = rabbitMQService;
        _excelService = excelService;
        _channel = _rabbitMQService.CreateConsumerChannel(_queueName); // Kuyruk adı geçiliyor
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            /*
            try
            {
                // Mesaj içeriğini al
                var body = ea.Body.ToArray();

                // Mesajdan dosyayı belleğe yükle ve işle
                ProcessFile(body);

                // Mesaj başarılı şekilde işlendiğinde RabbitMQ'ya bildir
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya işleme hatası: {ex.Message}");
                // Hata durumunda mesajı yeniden kuyruğa almak isteyebilirsiniz
            }
        };
        */

            try
            {
                // Mesaj içeriğini al
                var body = ea.Body.ToArray();

                // Mesajdan dosyayı belleğe yükle
                using (var memoryStream = new MemoryStream(body))
                {
                    // EPPlus kullanarak Excel dosyasını okuma
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // İlk sayfayı al
                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        // Hata listesini oluştur
                        var errorList = new List<string>();

                        // Hücreleri oku ve işleyin
                        for (int row = 1; row <= rowCount; row++)
                        {
                            for (int col = 1; col <= colCount; col++)
                            {
                                var cellValue = worksheet.Cells[row, col].Text;

                                // Burada örnek bir hata kontrolü yapılıyor, tarih formatı kontrol ediliyor
                                if (col == 3) // Örneğin, 3. sütun tarih sütunu
                                {
                                    if (!DateTime.TryParse(cellValue, out _))
                                    {
                                        errorList.Add($"Satır: {row}, Sütun: {col} - Geçersiz tarih formatı: {cellValue}");
                                    }
                                }

                                // Burada başka hata kontrolü yapabilirsiniz

                                Console.WriteLine($"Satır: {row}, Sütun: {col}, Değer: {cellValue}");
                            }
                        }

                        // Eğer hata varsa, hataları göster
                        if (errorList.Count > 0)
                        {
                            foreach (var error in errorList)
                            {
                                Console.WriteLine($"Hata: {error}");
                            }
                        }
                        else
                        {
                            // Dosya işleme mantığı
                            ProcessFile(body);
                        }
                    }
                }

                // Mesaj başarılı şekilde işlendiğinde RabbitMQ'ya bildir
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya işleme hatası: {ex.Message}");
                // Hata durumunda mesajı yeniden kuyruğa almak isteyebilirsiniz
            }
        };

        ///////////
        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    private void ProcessFile(byte[] fileBytes)
    {

        // Kaydetme yolu
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        string filePath = Path.Combine(folderPath, "ProcessedExcel.xlsx");

        // Dizin yoksa oluştur
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Dosyayı kaydet
        File.WriteAllBytes(filePath, fileBytes);
        Console.WriteLine("Dosya başarıyla işlendi ve kaydedildi: " + filePath);

        // Dosyayı oku ve işleme
        using (var memoryStream = new MemoryStream(fileBytes))
        {
            var (dataList, errorList) = _excelService.ReadExcelFile(memoryStream);
            Console.WriteLine("Dosya içeriği:");

            // Dosyadaki her bir veriyi işle
            foreach (var data in dataList)
            {
                Console.WriteLine($"Customer: {data.CustomerName}, Başlangıç Tarihi: {data.BaslangicTarihi}, Bitiş Tarihi: {data.BitisTarihi}");
                // Burada işleme kodunuzu yazabilirsiniz (örneğin, veritabanına kaydetme)
            }
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        base.Dispose();
    }
}
