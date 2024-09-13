
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using IdentityServerProject.Data;
using IdentityServerProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml; // EPPlus Kütüphanesini kullanarak Excel dosyalarını işlemek için

namespace MyExcelUploader.Services
{
    public class ExcelService
    { 
        // Müşteri adının sadece harflerden oluşup oluşmadığını kontrol eden metod
        public bool IsAlphabetic(string input)
        {
            return input.All(char.IsLetter);
        }

        // Tarihin geçerli bir format olup olmadığını kontrol eden metod
        public bool IsValidDate(string date)
        {
            return DateTime.TryParse(date, out _);
        }

        public (List<MyDataModel>, List<string>) ReadExcelFile(Stream fileStream)
        {
            var dataList = new List<MyDataModel>();
            var errorList = new List<string>(); // Hatalar için bir liste

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var data = new MyDataModel();
                    data.CustomerName = worksheet.Cells[row, 1].Text;

                    // Müşteri adı doğrulaması
                    if (string.IsNullOrWhiteSpace(data.CustomerName))
                    {
                        errorList.Add($"Satır {row}, Sütun 1: İsim boş olamaz.");
                    }
                 
                    // Başlangıç tarihi doğrulaması
                    if (DateTime.TryParse(worksheet.Cells[row, 2].Text, out DateTime baslangicTarihi))
                    {
                        data.BaslangicTarihi = baslangicTarihi;
                    }
                    else
                    {
                        errorList.Add($"Satır {row}, Sütun 2: Geçersiz Başlangıç Tarihi - {worksheet.Cells[row, 2].Text}");
                    }
                 
                    // Bitiş tarihi doğrulaması
                    if (DateTime.TryParse(worksheet.Cells[row, 3].Text, out DateTime bitisTarihi))
                    {
                        data.BitisTarihi = bitisTarihi;
                    }
                    else
                    {
                        errorList.Add($"Satır {row}, Sütun 3: Geçersiz Bitiş Tarihi - {worksheet.Cells[row, 3].Text}");
                    }

                    data.DosyaAdi = worksheet.Cells[row, 4].Text;
                    
                    // Dosya adı doğrulaması
                    if (string.IsNullOrWhiteSpace(data.DosyaAdi))
                    {
                        errorList.Add($"Satır {row}, Sütun 4: Dosya adı boş olamaz.");
                    }

                    //Yükleme tarihi doğrulaması
                    if (DateTime.TryParse(worksheet.Cells[row, 5].Text, out DateTime yuklemeTarihi))
                    {
                        data.YüklemeTarihi = yuklemeTarihi;
                    }
                    else
                    {
                        errorList.Add($"Satır {row}, Sütun 5: Geçersiz Yükleme Tarihi - {worksheet.Cells[row, 5].Text}");
                    }

                    dataList.Add(data);
                }
            }
            // Hataları döndür
            return (dataList, errorList);
        }

        // Şablonun kontrolünü yapacak metod
        public bool ValidateExcelFormat(IFormFile file)
        {
            try
            {
                // Dosyayı belleğe okur
                using (var stream = file.OpenReadStream())
                {
                    // ExcelPackage kullanarak dosyayı aç
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        if (worksheet == null)
                        {
                            return false; // Çalışma sayfası yoksa
                        }

                        // Örneğin, ilk satırda beklenen sütun başlıklarını kontrol et
                        var requiredHeaders = new[] { "Müşteri", "BAŞLANGIÇ TARİHİ", "BİTİŞ TARİHİ", "Dosya Adı", "Yükleme Tarihi" }; // Beklenen sütun başlıkları
                        var headerRow = worksheet.Cells[1, 1, 1, requiredHeaders.Length].Select(cell => cell.Text.Trim()).ToArray();

                        // Beklenen başlıkların varlığını kontrol et
                        if (!requiredHeaders.SequenceEqual(headerRow))
                        {
                            return false;
                        }

                        // Hücrelerdeki boşlukları kontrol et
                        for (int i = 1; i <= requiredHeaders.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(worksheet.Cells[2, i].Text))
                            {
                                return false; // Boş hücre tespit edilirse
                            }
                        }
                    }
                }

                return true; // Format doğru
            }
            catch
            {
                return false; // Herhangi bir hata olursa, format hatalı kabul edilir
            }
        }
    }
}

