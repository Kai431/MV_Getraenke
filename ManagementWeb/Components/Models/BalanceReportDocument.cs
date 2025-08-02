using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Shared.Models;

namespace ManagementWeb.Components.Models
{
    public class BalanceReportDocument : IDocument
    {
        private readonly List<Musician> _musicians;

        public BalanceReportDocument(List<Musician> musicians)
        {
            _musicians = musicians;
            var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Fonts", "dehintedFont.ttf");
            FontManager.RegisterFont(File.OpenRead(fontPath));
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var leftColumn = _musicians.Take(39).ToList();
            var rightColumn = _musicians.Skip(39).ToList();

            container.Page(page =>
            {
                page.DefaultTextStyle(x => x.FontFamily("Font").FontSize(10));
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);

                page.Header()
                    .Text($"Abrechnung Stand: {DateTime.Now.Date.ToString("dd.MM.yyyy")}")
                    .SemiBold().FontSize(15).FontColor(Colors.Black).AlignCenter();

                page.Content().PaddingTop(10, Unit.Point).Row(row =>
                {
                    // Linke Spalte
                    row.RelativeItem().Column(col =>
                    {
                        CreateTable(col, leftColumn);
                    });

                    // Rechte Spalte
                    row.RelativeItem().Column(col =>
                    {
                        CreateTable(col, rightColumn);
                    });
                });
            });
        }

        private void CreateTable(ColumnDescriptor col, List<Musician> musicians)
        {
            col.Item().AlignCenter().Width(9, Unit.Centimetre).Table(table =>
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
                foreach (var m in musicians)
                {
                    table.Cell().Element(CellStyle).Text(m.Name);
                    table.Cell().Element(CellStyle)
                        .AlignRight()
                        .Text(m.outBalance.ToString("F2") + " €");
                }

                static IContainer CellStyle(IContainer container)
                    => container.Border(1).Padding(3);
            });
        }
    }
}
