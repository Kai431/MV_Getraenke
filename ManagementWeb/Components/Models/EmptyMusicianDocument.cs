using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Shared.Models;

namespace ManagementWeb.Components.Models
{
    public class EmptyMusicianDocument : IDocument
    {
        private readonly List<Musician> _musicians;

        public EmptyMusicianDocument(List<Musician> musicians)
        {
            _musicians = musicians;
            var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Fonts", "dehintedFont.ttf");
            FontManager.RegisterFont(File.OpenRead(fontPath));
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontFamily("Font").FontSize(10));

                page.Header()
                    .Text($"Musikanten {DateTime.Now.Date.ToString("dd.MM.yyyy")}")
                    .SemiBold().FontSize(15).FontColor(Colors.Black).AlignCenter();

                page.Content().PaddingTop(10, Unit.Point).Table(table=>
                {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Name
                            columns.RelativeColumn(1); // Betrag
                        });

                        // Tabellenkopf
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Name").Bold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Betrag (€)").Bold();
                        });

                        // Zeilen
                        foreach (var m in _musicians)
                        {
                            table.Cell().Element(CellStyle).Text(m.Name);
                            table.Cell().Element(CellStyle)
                                .AlignRight()
                                .Text("");
                        }

                        static IContainer CellStyle(IContainer container)
                            => container.Border(1).Padding(3);
                });
            });
        }
    }
}
