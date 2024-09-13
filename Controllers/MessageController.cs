/*
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

public class MessageController : Controller
{
    private readonly RabbitMQService _rabbitMQService;
    private static List<string> _messages = new List<string>();

    public MessageController(RabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
    }

    public IActionResult Index()
    {
   //     while (RabbitMQConsumer.ReceivedMessages.TryDequeue(out var message))
   //     {
   //         _messages.Add("Received: " + message);
   //     }
        return View(_messages);
    }

    [HttpPost]
    public IActionResult SendMessage(string message)
    {
        _rabbitMQService.Publish(message, "hello"); // Kuyruk adı "hello"
        _messages.Add("Sent: " + message);
        return RedirectToAction("Index");
    }
}
*/