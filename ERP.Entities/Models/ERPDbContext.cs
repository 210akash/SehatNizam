namespace ERP.Entities.Models
{
    using Microsoft.EntityFrameworkCore;

    public partial class ERPDbContext : DbContext
    {
        public ERPDbContext()
        {
        }

        public ERPDbContext(DbContextOptions<ERPDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }

        #region Inventory & Purchase 

        public virtual DbSet<ErrorLogs> ErrorLogs { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<UOM> UOM { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<SubCategory> SubCategory { get; set; }
        public virtual DbSet<ItemType> ItemType { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<CategoryStore> CategoryStore { get; set; }
        public virtual DbSet<Priority> Priority { get; set; }
        public virtual DbSet<IndentType> IndentType { get; set; }
        public virtual DbSet<IndentRequest> IndentRequest { get; set; }
        public virtual DbSet<IndentRequestDetail> IndentRequestDetail { get; set; }
        public virtual DbSet<PurchaseDemand> PurchaseDemand { get; set; }
        public virtual DbSet<PurchaseDemandDetail> PurchaseDemandDetail { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<VendorType> VendorType { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetail { get; set; }
        public virtual DbSet<PaymentMode> PaymentMode { get; set; }
        public virtual DbSet<ShipmentMode> ShipmentMode { get; set; }
        public virtual DbSet<ComparativeStatement> ComparativeStatement { get; set; }
        public virtual DbSet<ComparativeStatementDetail> ComparativeStatementDetail { get; set; }
        public virtual DbSet<ComparativeStatementVendor> ComparativeStatementVendor { get; set; }
        public virtual DbSet<DeliveryTerms> DeliveryTerms { get; set; }
        public virtual DbSet<GST> GST { get; set; }
        public virtual DbSet<IGP> IGP { get; set; }
        public virtual DbSet<IGPDetails> IGPDetails { get; set; }

        #endregion

        #region Accounting

        public virtual DbSet<AccountCategory> AccountCategory { get; set; }
        public virtual DbSet<AccountSubCategory> AccountSubCategory { get; set; }
        public virtual DbSet<AccountType> AccountType { get; set; }
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<VoucherType> VoucherType { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<TransactionDetail> TransactionDetail { get; set; }
        public virtual DbSet<TransactionDocument> TransactionDocument { get; set; }
        public virtual DbSet<AccountHead> AccountHead { get; set; }
        public virtual DbSet<AccountFlow> AccountFlow { get; set; }
        
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().HasQueryFilter(i => !i.IsDelete);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDelete);
            modelBuilder.Entity<SubCategory>().HasQueryFilter(sc => !sc.IsDelete);
            modelBuilder.Entity<ItemType>().HasQueryFilter(it => !it.IsDelete);
            modelBuilder.Entity<Department>().HasQueryFilter(dt => !dt.IsDelete);
            modelBuilder.Entity<Vendor>().HasQueryFilter(vr => !vr.IsDelete);

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            // Configure the one-to-many relationship between Office and AspNetUsers
            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.Department)       // AspNetUsers has one Office
                .WithMany(o => o.Users)      // Office has many AspNetUsers
                .HasForeignKey(u => u.DepartmentId)  // Foreign key is OfficeId in AspNetUsers
                .OnDelete(DeleteBehavior.SetNull); // Optional: Set OfficeId to null if Office is deleted

            // Configure the one-to-many relationship between Office and AspNetUsers
            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.Store)       // AspNetUsers has one Office
                .WithMany(o => o.Users)      // Office has many AspNetUsers
                .HasForeignKey(u => u.StoreId)  // Foreign key is OfficeId in AspNetUsers
                .OnDelete(DeleteBehavior.SetNull); // Optional: Set OfficeId to null if Office is deleted

            modelBuilder.Entity<Company>()
                .HasOne(c => c.CreatedBy) // Navigation property
                .WithMany() // No inverse navigation
                .HasForeignKey(c => c.CreatedById) // Foreign key
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasOne(c => c.CreatedBy) // Navigation property
                .WithMany() // No inverse navigation
                .HasForeignKey(c => c.CreatedById) // Foreign key
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Store>()
         .HasOne(c => c.CreatedBy) // Navigation property
         .WithMany() // No inverse navigation
         .HasForeignKey(c => c.CreatedById) // Foreign key
         .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Store>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                 .HasIndex(c => new { c.CompanyId, c.Code })
                 .IsUnique();

            modelBuilder.Entity<SubCategory>()
                .HasIndex(sc => new { sc.CompanyId, sc.Code })
                .IsUnique();

            modelBuilder.Entity<ItemType>()
                .HasIndex(it => new { it.CompanyId, it.Code })
                .IsUnique();

            modelBuilder.Entity<Item>()
                .HasIndex(i => new { i.CompanyId, i.Code })
                .IsUnique();

            modelBuilder.Entity<ItemType>()
    .HasOne(it => it.Company)
    .WithMany() // Or specify navigation property
    .HasForeignKey(it => it.CompanyId)
    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<SubCategory>()
                .HasOne(sc => sc.Company)
                .WithMany() // Or specify navigation property
                .HasForeignKey(sc => sc.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Company)
                .WithMany() // Or specify navigation property
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Company)
                .WithMany() // Or specify navigation property
                .HasForeignKey(i => i.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasMany(c => c.ItemTypes)
                .WithOne(it => it.Company)
                .HasForeignKey(it => it.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Company>()
       .HasMany(c => c.Item)
       .WithOne(it => it.Company)
       .HasForeignKey(it => it.CompanyId)
       .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Company>()
       .HasMany(c => c.SubCategories)
       .WithOne(it => it.Company)
       .HasForeignKey(it => it.CompanyId)
       .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
             .HasMany(c => c.Categories)
             .WithOne(it => it.Company)
             .HasForeignKey(it => it.CompanyId)
             .OnDelete(DeleteBehavior.Restrict);

            // Configure the many-to-many relationship between Category and Store
            modelBuilder.Entity<CategoryStore>()
                .HasKey(cs => new { cs.CategoryId, cs.StoreId }); // Composite key

            modelBuilder.Entity<CategoryStore>()
                .HasOne(cs => cs.Category)
                .WithMany(c => c.CategoryStores)
                .HasForeignKey(cs => cs.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CategoryStore>()
                .HasOne(cs => cs.Store)
                .WithMany(s => s.CategoryStores)
                .HasForeignKey(cs => cs.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            #region Purchase Demand 

            // Disable Cascade on Status (no cascade delete)
            modelBuilder.Entity<PurchaseDemand>()
                .HasOne(p => p.Status)
                .WithMany()  // Assuming no navigation property on Status for PurchaseDemand
                .HasForeignKey(p => p.StatusId)
                .OnDelete(DeleteBehavior.NoAction);  // No action on delete for Status

            // Disable Cascade on Priority (no cascade delete)
            modelBuilder.Entity<PurchaseDemand>()
                .HasOne(p => p.Priority)
                .WithMany()  // Assuming no navigation property on Priority for PurchaseDemand
                .HasForeignKey(p => p.PriorityId)
                .OnDelete(DeleteBehavior.NoAction);  // No action on delete for Priority

            // Disable Cascade on Location (no cascade delete)
            modelBuilder.Entity<PurchaseDemand>()
                .HasOne(p => p.Location)
                .WithMany()  // Assuming no navigation property on Location for PurchaseDemand
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.NoAction);  // No action on delete for Location

            // Configure PurchaseDemandDetail relationships
            modelBuilder.Entity<PurchaseDemandDetail>()
                .HasOne(p => p.PurchaseDemand)
                .WithMany(p => p.PurchaseDemandDetail)
                .HasForeignKey(p => p.PurchaseDemandId)
                .OnDelete(DeleteBehavior.NoAction);  // Disable cascading delete here

            #endregion

            #region Purchase order

            // Configure PurchaseDemandDetail relationships
            modelBuilder.Entity<PurchaseOrderDetail>()
                .HasOne(p => p.PurchaseOrder)
                .WithMany(p => p.PurchaseOrderDetail)
                .HasForeignKey(p => p.PurchaseOrderId)
                .OnDelete(DeleteBehavior.NoAction);  // Disable cascading delete here

            // Configure ComparativeStatementVendor relationship
            modelBuilder.Entity<PurchaseOrderDetail>()
                .HasOne(p => p.ComparativeStatementVendor)
               .WithMany(p => p.PurchaseOrderDetail)
                .HasForeignKey(p => p.ComparativeStatementVendorId)
                .OnDelete(DeleteBehavior.NoAction); // Disable cascading delete for ComparativeStatementVendor

            #endregion

            #region Comparative Statement


            #endregion

            #region Accounting 
            modelBuilder.Entity<AccountSubCategory>()
            .HasOne(sc => sc.Company)
            .WithMany() // Or specify navigation property
            .HasForeignKey(sc => sc.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccountCategory>()
                .HasOne(c => c.Company)
                .WithMany() // Or specify navigation property
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasOne(i => i.Company)
                .WithMany() // Or specify navigation property
                .HasForeignKey(i => i.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasOne(td => td.AccountFlow)
                .WithMany(t => t.Accounts)
                .HasForeignKey(td => td.AccountFlowId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<AccountCategory>()
               .HasOne(td => td.AccountHead)
               .WithMany(t => t.AccountCategorys)
               .HasForeignKey(td => td.AccountHeadId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<TransactionDetail>()
                .HasOne(td => td.Transaction)
                .WithMany(t => t.TransactionDetails)
                .HasForeignKey(td => td.TransactionId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
                
            modelBuilder.Entity<Transaction>()
                .HasOne(td => td.VoucherType)
                .WithMany(t => t.Transactions)
                .HasForeignKey(td => td.VoucherTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            #endregion

            this.OnModelCreatingPartial(modelBuilder);

        }

        /// <summary>
        /// On Model Creating Partial
        /// </summary>
        /// <param name="modelBuilder">The model builder</param>
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
