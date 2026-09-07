namespace CropDeal.Repository;

public class InvoiceServiceRepository : IInvoiceServiceRepository
{
    public MemoryStream GenerateInvoicePdf(Transaction txn)
    {
        try
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.Content().Column(col =>
                    {
                        // Logo and Title
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(innerCol =>
                            {
                                innerCol.Item()
       .Width(120)
       .Height(60)
       .Image("wwwroot/download.jpg", ImageScaling.FitArea);

                                // Path to your logo
                            });

                            row.RelativeItem().Column(innerCol =>
                            {
                                innerCol.Item().AlignCenter().Text("INVOICE")
                                    .FontSize(24).Bold().FontColor(Colors.Green.Medium);
                            });
                        });

                        col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Green.Lighten3);

                        // Transaction Details Table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            void AddRow(string label, string value)
                            {
                                table.Cell().Element(CellStyle).Text(label).FontColor(Colors.Green.Darken1).Bold();
                                table.Cell().Element(CellStyle).Text(value).FontColor(Colors.Black);
                            }

                            AddRow("Transaction ID", txn.Id.ToString());
                            AddRow("Listing ID", txn.ListingId.ToString());
                            AddRow("Quantity", txn.Quantity.ToString());
                            AddRow("Total Price", $"₹{txn.TotalPrice}");
                            AddRow("Status", txn.Status.ToString());
                            AddRow("Paid At", txn.PaidAt?.ToLocalTime().ToString("f") ?? "Pending");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .PaddingVertical(5)
                                    .PaddingHorizontal(5);
                            }
                        });

                        col.Item().PaddingTop(30).AlignCenter().Text("Thank you for using CropDeal!")
                            .FontColor(Colors.Green.Darken2).Italic();
                    });
                });
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;
            return stream;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Invoice PDF generation failed: {ex.Message}");
            throw;
        }
    }


}

