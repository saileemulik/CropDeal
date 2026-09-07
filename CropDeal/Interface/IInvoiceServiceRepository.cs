namespace CropDeal.Interface;

public interface IInvoiceServiceRepository
{
    MemoryStream GenerateInvoicePdf(Transaction txn);
}
