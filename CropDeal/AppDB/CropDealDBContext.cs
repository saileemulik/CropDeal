namespace CropDeal.AppDB;
public class CropDealDBContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public CropDealDBContext(DbContextOptions<CropDealDBContext> options) : base(options) { }
    public new DbSet<User> Users { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<Crop> Crops { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<CropListing> CropListings { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<CropRequest> CropRequests { get; set; }
    public DbSet<PriceNegotiationRequest> PriceNegotiations { get; set; }
    public DbSet<PickupSchedule> PickupSchedules { get; set; }
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
 
        //Many-to-One relationship between Report and Admin (User)
        modelBuilder.Entity<Report>()
        .HasOne(r => r.Admin)
        .WithMany()
        .HasForeignKey(r => r.GeneratedBy)
        .OnDelete(DeleteBehavior.Restrict);
 
        //Many-to-One relationship between Crop and Admin (User)
        modelBuilder.Entity<Crop>()
        .HasOne(c => c.Admin)
        .WithMany(u => u.Crops)
        .HasForeignKey(c => c.AdminId)
        .OnDelete(DeleteBehavior.Restrict);
 
        //Many-to-One relationship between ReView and Dealer (User)
        modelBuilder.Entity<Review>()
        .HasOne(r => r.Dealer)
        .WithMany()
        .HasForeignKey(r => r.DealerId)
        .OnDelete(DeleteBehavior.Restrict);
 
        //Many-to-One relationship between Report and Farmer (User)
        modelBuilder.Entity<Review>()
        .HasOne(r => r.Farmer)
        .WithMany()
        .HasForeignKey(r => r.FarmerId)
        .OnDelete(DeleteBehavior.Restrict);
 
        //Many-to-One relationship between Review and Transaction
        modelBuilder.Entity<Review>()
        .HasOne(r => r.Transaction)
        .WithMany()
        .HasForeignKey(r => r.TransactionId)
        .OnDelete(DeleteBehavior.Restrict);
 
        //One-to-One relationship between User and BankAccount
        // modelBuilder.Entity<User>()
        // .HasOne(u => u.BankAccount)
        // .WithOne()
        // .HasForeignKey<User>(u => u.BankAccountId)
        // .OnDelete(DeleteBehavior.Cascade);
 
        //One-to-One relationship between User and BankAccount
        // modelBuilder.Entity<BankAccount>()
        // .HasOne(b => b.User)
        // .WithOne(u => u.BankAccount)
        // .HasForeignKey<BankAccount>(b => b.UserId)
        // .OnDelete(DeleteBehavior.Restrict);
 
        //Many-to-One relationship between CropListing and Farmer (User)
        modelBuilder.Entity<CropListing>()
        .HasOne(cl => cl.Farmer)
        .WithMany()
        .HasForeignKey(cl => cl.FarmerId)
        .OnDelete(DeleteBehavior.Restrict);
 
        //Many-to-One relationship between CropListing and Crop
        modelBuilder.Entity<CropListing>()
        .HasOne(cl => cl.Crop)
        .WithMany()
        .HasForeignKey(cl => cl.CropId)
        .OnDelete(DeleteBehavior.Restrict);
 
        //Many-to-One relationship between Subscription and Dealer (User)
        modelBuilder.Entity<Subscription>()
        .HasOne(s => s.Dealer)
        .WithMany()
        .HasForeignKey(s => s.DealerId)
        .OnDelete(DeleteBehavior.Restrict);
 
        //Many-to-One relationship between Subscription and Crop
        modelBuilder.Entity<Subscription>()
        .HasOne(s => s.CropListing)
        .WithMany()
        .HasForeignKey(s => s.CropListingId)
        .OnDelete(DeleteBehavior.Restrict);
 
        //Many-to-One relationship between Transaction and Dealer (User)
        modelBuilder.Entity<Transaction>()
        .HasOne(t => t.Dealer)
        .WithMany()
        .HasForeignKey(t => t.DealerId)
        .OnDelete(DeleteBehavior.Restrict);
 
        //Many-to-One relationship between Transaction and CropListing
        modelBuilder.Entity<Transaction>()
        .HasOne(t => t.Listing)
        .WithMany()
        .HasForeignKey(t => t.ListingId)
        .OnDelete(DeleteBehavior.Restrict);
 
        modelBuilder.Entity<PickupSchedule>()
    .HasOne(p => p.CropListing)
    .WithMany(c => c.PickupSchedules)
    .HasForeignKey(p => p.ListingId)
    .OnDelete(DeleteBehavior.Restrict);
 
modelBuilder.Entity<PickupSchedule>()
    .HasOne(p => p.Dealer)
    .WithMany()
    .HasForeignKey(p => p.DealerId)
    .OnDelete(DeleteBehavior.Restrict);
 
modelBuilder.Entity<PickupSchedule>()
    .HasOne(p => p.Farmer)
    .WithMany()
    .HasForeignKey(p => p.FarmerId)
    .OnDelete(DeleteBehavior.Restrict);
 
 
        //Many-to-One relationship between Address and User
        modelBuilder.Entity<Address>()
        .HasOne(a => a.User)
        .WithMany()
        .HasForeignKey(a => a.UserId)
        .OnDelete(DeleteBehavior.Restrict);
 
        modelBuilder.Entity<PriceNegotiationRequest>()
    .HasOne(p => p.Farmer)
    .WithMany(u => u.PriceNegotiations)
    .HasForeignKey(p => p.FarmerId)
    .OnDelete(DeleteBehavior.Restrict);
        // or DeleteBehavior.NoAction
 
 
 
    }
 
}
 
 
// //Conversion of Role from int to string
// modelBuilder.Entity<User>()
// .Property(u => u.Role)
// .HasConversion<string>();
 
// //Conversion of Status from int to string
// modelBuilder.Entity<User>()
// .Property(u => u.Status)
// .HasConversion<string>();