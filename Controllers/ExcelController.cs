using Microsoft.AspNetCore.Mvc;
using MyExcelUploader.Services;
using Microsoft.AspNetCore.Http;
using IdentityServerProject.Models;
using Microsoft.EntityFrameworkCore;
using IdentityServerProject.Data;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;

namespace IdentityServerProject.Controllers
{
    [Route("[controller]")]
    [Authorize]

    public class ExcelController : Controller // Base sınıfını Controller yaparak bir MVC controller'ı tanımlarız
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly ExcelService _excelService;
        private readonly ApplicationDbContext _dbContext; // DbContext için private alan


        public ExcelController(RabbitMQService rabbitMQService, ExcelService excelService, ApplicationDbContext dbContext)
        {
            _rabbitMQService = rabbitMQService;
            _excelService = excelService;
            _dbContext = dbContext; // Constructor ile initialize et

        }

        // Excel dosyası yükleme sayfasını döndüren bir GET metodu
        [HttpGet("UploadExcel")]
        public IActionResult UploadExcel()
        {
            return View();
        }



        [HttpPost("UploadExcel")]
        [Authorize]

        public IActionResult UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Lütfen bir dosya seçin.";
                return RedirectToAction("UploadExcel"); // Kullanıcıyı yükleme sayfasına geri yönlendir
            }

            if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
                file.ContentType != "application/vnd.ms-excel")
            {
                TempData["ErrorMessage"] = "Yüklenen dosya türü geçersiz.";
                return RedirectToAction("UploadExcel");
            }

            // Şablon doğrulaması yap
            if (!_excelService.ValidateExcelFormat(file))
            {
                TempData["ErrorMessage"] = "Yüklenen dosya, gerekli şablon formatında değil.";
                return RedirectToAction("UploadExcel");
            }

            // Dosyanın yükleneceği yol
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Dosya belleğe okunur
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                // Verileri oku ve hataları ayır
                var (dataList, errorList) = _excelService.ReadExcelFile(memoryStream);

                // Eğer hatalar varsa, hataları ön izleme sayfasında göster
                if (errorList.Count > 0)
                {
                    ViewBag.Errors = errorList;
                    return View("PreviewErrors", dataList); // Hatalarla birlikte veriyi ön izleme sayfasına gönder
                }

                // Dosyayı RabbitMQ kuyruğuna ekle
                _rabbitMQService.EnqueueFileForProcessing(file);

                // Eğer hata yoksa kullanıcı onay sayfasına yönlendirilir
                return View("PreviewData", dataList); // Hatasız veriyi ön izleme sayfasına gönder
            }
        


    }
      


        [HttpPost("ConfirmUpload")]
        public IActionResult ConfirmUpload(List<MyDataModel> dataList)
        {
            if (dataList == null || !dataList.Any())
            {
                return BadRequest("Dosya yüklendi");
            }
            // Veritabanına ekleme veya güncelleme işlemi
            foreach (var data in dataList)
            {
                data.BaslangicTarihi = DateTime.SpecifyKind(data.BaslangicTarihi, DateTimeKind.Utc);
                data.BitisTarihi = DateTime.SpecifyKind(data.BitisTarihi, DateTimeKind.Utc);
                data.YüklemeTarihi = DateTime.SpecifyKind(data.YüklemeTarihi, DateTimeKind.Utc);

                var existingRecord = _dbContext.MyDataModels
                    .FirstOrDefault(d => d.CustomerName == data.CustomerName &&
                                         d.BaslangicTarihi == data.BaslangicTarihi &&
                                         d.BitisTarihi == data.BitisTarihi);

                if (existingRecord != null)
                {
                    existingRecord.DosyaAdi = data.DosyaAdi;
                    existingRecord.YüklemeTarihi = data.YüklemeTarihi;
                }
                else
                {
                    _dbContext.MyDataModels.Add(data);
                }
            }

            
            _dbContext.SaveChanges();
            return RedirectToAction("ListFiles"); // Kaydedilen dosyalar sayfasına yönlendir
        }


        [HttpGet("ListFiles")]
        public IActionResult ListFiles()
        {
            // wwwroot/uploads klasöründeki dosyaları listele
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(directoryPath))
            {
                return NotFound("Dosyalar klasörü bulunamadı.");
            }
          
            var files = Directory.GetFiles(directoryPath).Select(Path.GetFileName).ToList();
            return View("ListFiles", files);
        }


        [HttpGet("ViewFile/{fileName}")]
           public IActionResult ViewFile(string fileName)
           {
               var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
               var filePath = Path.Combine(folderPath, fileName);

               if (!System.IO.File.Exists(filePath))
                   return NotFound("Dosya bulunamadı.");

               var fileBytes = System.IO.File.ReadAllBytes(filePath);

               // Excel dosyasını oku ve tabloya dönüştür
               List<MyDataModel> dataList = new List<MyDataModel>();
               List<string> errorList = new List<string>();


               try
               {
                   using (var memoryStream = new MemoryStream(fileBytes))
                   {
                       (dataList, errorList) = _excelService.ReadExcelFile(memoryStream); // Hata listesi ve veri listesi döndürülür
                   }

                   if (errorList.Count > 0)
                   {
                       return View("ShowErrors", "Veri bulunamadı.");
                   }
               }
               catch (Exception ex)
               {
                   return ShowErrors(new List<string> { ex.Message });
               }

               // Veritabanına ekleme veya güncelleme işlemi
               foreach (var data in dataList)
               {
                   // Tarih alanlarını UTC olarak ayarla
                   data.BaslangicTarihi = DateTime.SpecifyKind(data.BaslangicTarihi, DateTimeKind.Utc);
                   data.BitisTarihi = DateTime.SpecifyKind(data.BitisTarihi, DateTimeKind.Utc);
                   data.YüklemeTarihi = DateTime.SpecifyKind(data.YüklemeTarihi, DateTimeKind.Utc);

                   var existingRecord = _dbContext.MyDataModels
                       .FirstOrDefault(d => d.CustomerName == data.CustomerName &&
                                            d.BaslangicTarihi == data.BaslangicTarihi &&
                                            d.BitisTarihi == data.BitisTarihi);

                   if (existingRecord != null)
                   {
                       // Mevcut kaydı güncelle
                       existingRecord.DosyaAdi = data.DosyaAdi;
                       existingRecord.YüklemeTarihi = data.YüklemeTarihi;
                   }
                   else
                   {
                       // Yeni kayıt ekle
                       _dbContext.MyDataModels.Add(data);
                   }

               }

               // Veritabanı değişikliklerini kaydet
               _dbContext.SaveChanges();


               return View("ViewExcel", dataList); // Veriyi yeni bir View'e gönder
           }
        
        public IActionResult ShowErrors(List<string> errors)
        {
            return View(errors);
        }

        public IActionResult PreviewExcelData(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction("UploadExcel");
            }

            var (dataList, errorList) = _excelService.ReadExcelFile(file.OpenReadStream());

            ViewBag.Errors = errorList;

            return View("PreviewErrors", dataList);
        }

/*
        [HttpGet("DownloadFile/{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", fileName);

            if (!System.IO.File.Exists(path))
            {
                return NotFound("Dosya bulunamadı.");
            }

            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        
           
        }
*/


        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            // Excel şablonunun yolu
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Template.xlsx");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Şablon dosyası bulunamadı.");

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template.xlsx");
        }

    }
}


