// namespace CropDeal.AppDB.Seed;
 
// public class DBInitializer
// {
//     public static void SeedDatabase(IServiceProvider serviceProvider)
//         {
//             using var scope = serviceProvider.CreateScope();
//             var context = scope.ServiceProvider.GetRequiredService<CropDealDBContext>();
//             context.Database.Migrate();
 
//             // SeedUsers.Seed(context);
//             SeedCrops.Seed(context);
//             // SeedCropListings.Seed(context);
//             // SeedAddresses.Seed(context);
//             // SeedReviews.Seed(context);
//             // SeedReports.Seed(context);
//             // SeedTransactions.Seed(context);
//         }
// }