// namespace CropDeal.AppDB.Seed;
 
// public class SeedCrops
// {
//     public static void Seed(CropDealDBContext context)
//     {
//         var admin = context.Users.FirstOrDefault(u => u.Role == UserRole.Admin);
//         if (admin == null)
//         {
//             return;
//         }
//         if (!context.Crops.Any())
//         {
//             var crops = new List<Crop>
//                 {
//                     new Crop
//                     {
//                         Id = Guid.NewGuid(),
//                         Name = "Wheat",
//                         AdminId = admin.Id,
//                         Type = CropTypeEnum.Grain,
//                         CreatedAt = DateTime.UtcNow,
//                         UpdatedAt = DateTime.UtcNow
//                     },
//                     new Crop
//                     {
//                         Id = Guid.NewGuid(),
//                         Name = "Tomato",
//                         AdminId = admin.Id,
//                         Type = CropTypeEnum.Vegetable,
//                         CreatedAt = DateTime.UtcNow,
//                         UpdatedAt = DateTime.UtcNow
//                     },
//                     new Crop
//                     {
//                         Id = Guid.NewGuid(),
//                         Name = "Apple",
//                         AdminId = admin.Id,
//                         Type = CropTypeEnum.Fruit,
//                         CreatedAt = DateTime.UtcNow,
//                         UpdatedAt = DateTime.UtcNow
//                     },
//                     new Crop
//                     {
//                         Id = Guid.NewGuid(),
//                         Name = "Rice",
//                         AdminId = admin.Id,
//                         Type = CropTypeEnum.Grain,
//                         CreatedAt = DateTime.UtcNow,
//                         UpdatedAt = DateTime.UtcNow
//                     }
//                 };
 
//             context.Crops.AddRange(crops);
//             context.SaveChanges();
//         }
//     }
// }