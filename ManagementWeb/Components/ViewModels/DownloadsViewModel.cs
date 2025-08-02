using ManagementWeb.Components.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using Shared.Data;
using Shared.Models;

namespace ManagementWeb.Components.ViewModels
{
    public class DownloadsViewModel
    {
        public List<Musician> Musicians { get; set; } = new List<Musician>();

        public DownloadsViewModel()
        {
        }

        public void Initialize()
        {
            using var db = new MusicianDbContext();
            Musicians = db.Musicians.AsNoTracking()
                .OrderBy(m => m.Name)
                .ToList();
        }
    }
}
