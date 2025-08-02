using DynamicData;
using ManagementWeb.Components.Layout;
using ManagementWeb.Components.Models;
using ManagementWeb.Components.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using ReactiveUI;
using Shared.Data;
using Shared.Models;
using System.Diagnostics.Metrics;
using System.Reactive;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace ManagementWeb.Components.ViewModels
{
    public class MusicianManagerViewModel
    {
        public List<Musician> Musicians { get; set; }
        public Musician SelectedMusician { get; set; }
        public bool SaveBtnDisabled { get; set; } = true;
        public bool EditFieldsDisabled { get; set; } = true;
        public bool EditBtnsDisabled { get; set; } = true;

        public string txtName { get; set; } = string.Empty;
        public string? sltInstrument { get; set; } = null;

        public bool ShowMusicianDrinkHistory { get; set; } = false;

        private readonly IMessageService _messageService;
        private readonly IDialogService _dialogService;
        public MusicianManagerViewModel(IMessageService messageService, IDialogService dialogService)
        {
            _messageService = messageService;
            _dialogService = dialogService;
            Initialize();
        }

        public void Initialize()
        {
            using (var db = new MusicianDbContext())
            {
                Musicians = db.Musicians.AsNoTracking().OrderBy(m => m.Name).ToList();
            }
            SelectedMusician = null;
            txtName = string.Empty;
            sltInstrument = null;
            ShowMusicianDrinkHistory = false;
        }
        private void ShowTopMessage(string message, MessageIntent type)
        {
            _messageService.ShowMessageBar(options =>
            {
                options.Title = message;
                options.Intent = type;
                options.Timeout = 4000;
            });
        }

        public void SaveMusician()
        {
            if (Musicians.FirstOrDefault(m => m.Name == txtName) != null)
            {
                ShowTopMessage("Dieser Name existiert bereits! Anderer Name wählen", MessageIntent.Error);
                return;
            }

            if (txtName == string.Empty || sltInstrument == null)
            {
                ShowTopMessage("Die Felder dürfen nicht leer sein!", MessageIntent.Error);
                return;
            }

            EditBtnsDisabled = false;
            EditFieldsDisabled = true;
            SaveBtnDisabled = true;

            if (SelectedMusician == null)
            {
                CreateNewMusician();
                return;
            }


            using (var db = new MusicianDbContext())
            {
                var changedMusician = db.Musicians.FirstOrDefault(m => m.Name == SelectedMusician.Name);
                if (changedMusician == null)
                    return;
                changedMusician.Name = txtName;
                changedMusician.Instrument = Enum.Parse<Satz>(sltInstrument);
                db.SaveChanges();
            }

            SelectedMusician.Name = txtName;
            SelectedMusician.Instrument = Enum.Parse<Satz>(sltInstrument ?? "");

            SaveMusiciansJson();
            ShowTopMessage($"{SelectedMusician.Name} gespeichert!", MessageIntent.Success);
        }

        private void CreateNewMusician()
        {
            Musicians.Add(new Musician(txtName, Enum.Parse<Satz>(sltInstrument)));
            SelectedMusician = Musicians.FirstOrDefault(m => m.Name == txtName);
            SaveMusiciansJson();
            using (var db = new MusicianDbContext())
            {
                db.Musicians.Add(SelectedMusician);
                db.SaveChanges();
            }
            ShowTopMessage($"{SelectedMusician.Name} erstellt und gespeichert.", MessageIntent.Success);
        }

        private void SaveMusiciansJson()
        {
            var dtoList = Musicians
                .Select(m => new MusicianDTO { Name = m.Name, Instrument = m.Instrument.ToString() })
                .ToList();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = null,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string aktualisiertJson = JsonSerializer.Serialize(dtoList, options);

            var path = Path.Combine(AppContext.BaseDirectory, "Data", "musicians.json");
            File.WriteAllText(path, aktualisiertJson);
        }

        public void MusicianChanged(Musician musician)
        {
            if (musician == null)
            {
                EditBtnsDisabled = true;
                SaveBtnDisabled = true;
                EditFieldsDisabled = true;
                return;
            }

            SelectedMusician = musician;
            txtName = musician.Name;
            sltInstrument = musician.Instrument.ToString();
            EditBtnsDisabled = false;
            SaveBtnDisabled = true;
            EditFieldsDisabled = true;
        }

        public void EditMusician()
        {
            if (SelectedMusician == null)
                return;

            EditBtnsDisabled = true;
            SaveBtnDisabled = false;
            EditFieldsDisabled = false;
        }

        public void CancelEdit()
        {
            EditFieldsDisabled = true;
            SaveBtnDisabled = true;
            EditBtnsDisabled = false;

            if (SelectedMusician == null)
            {
                txtName = "";
                sltInstrument = null;
                ShowTopMessage($"Neuer Musikant erstellen abgebrochen!", MessageIntent.Info);
                return;
            }

            txtName = SelectedMusician.Name;
            sltInstrument = SelectedMusician.Instrument.ToString();
            ShowTopMessage($"Editieren abgebrochen!", MessageIntent.Info);
        }

        public void NewMusician()
        {
            EditFieldsDisabled = false;
            SaveBtnDisabled = false;
            EditBtnsDisabled = true;

            SelectedMusician = null;
            txtName = "";
            sltInstrument = null;
        }

        public async Task DeleteMusician()
        {
            var dialog = await _dialogService.ShowDialogAsync<DeleteCheckDialog>(SelectedMusician, new DialogParameters()
            {
                Height = "240px",
                Title = string.Empty,
                PreventDismissOnOverlayClick = true,
                PreventScroll = true,
            });

            var result = await dialog.Result;
            if (result.Cancelled)
            {
                ShowTopMessage($"Löschen abgebrochen!", MessageIntent.Info);
                return;
            }

            Musicians.Remove(SelectedMusician);
            using (var db = new MusicianDbContext())
            {
                db.Musicians.Remove(SelectedMusician);
                db.SaveChanges();
            }
            SaveMusiciansJson();

            ShowTopMessage($"{SelectedMusician.Name} gelöscht!", MessageIntent.Success);

            SelectedMusician = null;
            txtName = string.Empty;
            sltInstrument = null;
        }

        public void OnRowDoubleClick(Musician musician)
        {
            SelectedMusician = musician;
            ShowMusicianDrinkHistory = true;
        }
    }
}
