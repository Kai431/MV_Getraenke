using ManagementWeb.Components.Models;
using ManagementWeb.Components.ViewModels;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ManagementWeb.Controller
{
    [ApiController]
    [Route("api/downloads")]
    public class DownloadController : ControllerBase
    {
        private readonly DownloadsViewModel _vm;
        private readonly IWebHostEnvironment _env;

        public DownloadController(DownloadsViewModel vm, IWebHostEnvironment env)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            _vm = vm;
            _env = env;
            _vm.Initialize(); // oder falls du DI-basiert aufbereitest
        }

        [HttpGet("rechnung")]
        public IActionResult DownloadBill()
        {
            var report = new BalanceReportDocument(_vm.Musicians);
            var pdfBytes = report.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Abrechnung_{DateTime.Now.ToString("dd.MM.yyyy")}.pdf");
        }

        [HttpGet("emptylist")]
        public IActionResult DownloadEmptyList()
        {
            var report = new EmptyMusicianDocument(_vm.Musicians);
            var pdfBytes = report.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Leere_Musikantenliste.pdf");
        }

        [HttpGet("database/Kühlschranknutzung")]
        public IActionResult DownloadDatabaseKühlschrank()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "Kühlschranknutzung2025.db");

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            // Kopie erstellen in einen temporären Pfad
            var tempFile = Path.GetTempFileName();
            var tempTarget = Path.ChangeExtension(tempFile, ".db");
            System.IO.File.Copy(filePath, tempTarget, overwrite: true);

            // Datei in den Speicher laden
            var fileBytes = System.IO.File.ReadAllBytes(tempTarget);

            // Temporäre Datei löschen
            System.IO.File.Delete(tempTarget);
            System.IO.File.Delete(tempFile); // (Das .tmp-File evtl. auch aufräumen)

            return File(fileBytes, "application/x-sqlite3", "Kühlschranknutzung2025.db");
        }

        [HttpGet("database/Kassa")]
        public IActionResult DownloadDatabaseKassa()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "Kassa.db");

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            // Kopie erstellen in einen temporären Pfad
            var tempFile = Path.GetTempFileName();
            var tempTarget = Path.ChangeExtension(tempFile, ".db");
            System.IO.File.Copy(filePath, tempTarget, overwrite: true);

            // Datei in den Speicher laden
            var fileBytes = System.IO.File.ReadAllBytes(tempTarget);

            // Temporäre Datei löschen
            System.IO.File.Delete(tempTarget);
            System.IO.File.Delete(tempFile); // (Das .tmp-File evtl. auch aufräumen)

            return File(fileBytes, "application/x-sqlite3", "Kassa.db");
        }

        [HttpGet("database/Getränke")]
        public IActionResult DownloadDatabaseGetränke()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "Drinks.db");

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            // Kopie erstellen in einen temporären Pfad
            var tempFile = Path.GetTempFileName();
            var tempTarget = Path.ChangeExtension(tempFile, ".db");
            System.IO.File.Copy(filePath, tempTarget, overwrite: true);

            // Datei in den Speicher laden
            var fileBytes = System.IO.File.ReadAllBytes(tempTarget);

            // Temporäre Datei löschen
            System.IO.File.Delete(tempTarget);
            System.IO.File.Delete(tempFile); // (Das .tmp-File evtl. auch aufräumen)

            return File(fileBytes, "application/x-sqlite3", "Drinks.db");
        }

    }

}
