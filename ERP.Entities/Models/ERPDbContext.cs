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
        public virtual DbSet<Inspection> Inspection { get; set; }
        public virtual DbSet<InspectionDetail> InspectionDetail { get; set; }
        public virtual DbSet<RejectReason> RejectReason { get; set; }

        public virtual DbSet<GRN> GRN { get; set; }
        public virtual DbSet<GRNDetail> GRNDetail { get; set; }
        public virtual DbSet<Rack> Rack { get; set; }
        public virtual DbSet<Row> Row { get; set; }
        public virtual DbSet<Section> Section { get; set; }
        public virtual DbSet<CostSheet> CostSheet { get; set; }
        public virtual DbSet<CostSheetDetail> CostSheetDetail { get; set; }
        public virtual DbSet<IGPType> IGPType { get; set; }

        public virtual DbSet<PurchaseReturn> PurchaseReturn { get; set; }
        public virtual DbSet<PurchaseReturnDetail> PurchaseReturnDetail { get; set; }

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
        public virtual DbSet<AccountGroup> AccountGroup { get; set; }

        #endregion

        #region Sale

        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Zone> Zones { get; set; }
        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<Territory> Territories { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<Dealership> Dealerships { get; set; }
        //public virtual DbSet<Product> Product { get; set; }
        //public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderItems> OrderItems { get; set; }
        public virtual DbSet<OrderProcess> OrderProcess { get; set; }
        public virtual DbSet<MarkShopVisit> MarkShopVisits { get; set; }
        public virtual DbSet<ShopType> ShopType { get; set; }
        public virtual DbSet<ShopRouteFrequency> ShopRouteFrequency { get; set; }
        public virtual DbSet<Vehicle> Vehicle { get; set; }
        public virtual DbSet<PriceGroup> PriceGroup { get; set; }
        public virtual DbSet<PriceGroupDetails> PriceGroupDetails { get; set; }
        public virtual DbSet<DistributorPriceGroup> DistributorPriceGroup { get; set; }
        public virtual DbSet<UserAttendance> UserAttendance { get; set; }
        public virtual DbSet<UserTerritory> UserTerritory { get; set; }
        public virtual DbSet<Templates> Templates { get; set; }
        public virtual DbSet<DSFRoute> DSFRoute { get; set; }
        public virtual DbSet<SalesTarget> SalesTarget { get; set; }
        public virtual DbSet<Issuance> Issuance { get; set; }
        public virtual DbSet<IssuanceDetail> IssuanceDetail { get; set; }
        public virtual DbSet<Dispatch> Dispatch { get; set; }
        public virtual DbSet<DispatchDetail> DispatchDetail { get; set; }
        public virtual DbSet<CancelDispatch> CancelDispatch { get; set; }
        public virtual DbSet<CancelDispatchDetail> CancelDispatchDetail { get; set; }
        public virtual DbSet<SaleMaterial> SaleMaterial { get; set; }
        public virtual DbSet<SaleMaterialDetail> SaleMaterialDetail { get; set; }
        public virtual DbSet<SaleMaterialReturn> SaleMaterialReturn { get; set; }
        public virtual DbSet<SaleMaterialReturnDetail> SaleMaterialReturnDetail { get; set; }
        public virtual DbSet<DealershipType> DealershipType { get; set; }
        public virtual DbSet<SaleReturn> SaleReturn { get; set; }
        public virtual DbSet<SaleReturnDetail> SaleReturnDetail { get; set; }
        public virtual DbSet<ShopOrderReturn> ShopOrderReturn { get; set; }
        public virtual DbSet<ShopOrderReturnDetail> ShopOrderReturnDetail { get; set; }
        public virtual DbSet<RetailOrder> RetailOrder { get; set; }
        public virtual DbSet<RetailOrderItems> RetailOrderItems { get; set; }
        public virtual DbSet<RetailOrderProcess> RetailOrderProcess { get; set; }
        public virtual DbSet<ShopOrder> ShopOrder { get; set; }
        public virtual DbSet<ShopOrderItems> ShopOrderItems { get; set; }
        public virtual DbSet<ShopDispatch> ShopDispatch { get; set; }
        public virtual DbSet<ShopDispatchDetail> ShopDispatchDetail { get; set; }
        public virtual DbSet<RetailOrderReturn> RetailOrderReturn { get; set; }
        public virtual DbSet<RetailOrderReturnDetail> RetailOrderReturnDetail { get; set; }

        #endregion

        #region HR

        public virtual DbSet<EmployeeDesignation> EmployeeDesignation { get; set; }
        public virtual DbSet<EmployeeEducation> EmployeeEducation { get; set; }
        public virtual DbSet<EmployeeGrade> EmployeeGrade { get; set; }
        public virtual DbSet<EmployeeShift> EmployeeShift { get; set; }
        public virtual DbSet<EmployeeType> EmployeeType { get; set; }
        public virtual DbSet<EmployeeBank> EmployeeBank { get; set; }
        public virtual DbSet<EmployeeDocument> EmployeeDocument { get; set; }
        public virtual DbSet<EmployeeDocumentType> EmployeeDocumentType { get; set; }
        public virtual DbSet<EmployeeLeaveGroup> EmployeeLeaveGroup { get; set; }
        public virtual DbSet<EmployeeLeaveType> EmployeeLeaveType { get; set; }
        public virtual DbSet<EmployeeGroupLeaveType> EmployeeGroupLeaveType { get; set; }
        public virtual DbSet<EmployeeGroupLeaveTypeDetail> EmployeeGroupLeaveTypeDetail { get; set; }
        public virtual DbSet<EmployeeWorkingDays> EmployeeWorkingDays { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<EmployeeDevice> EmployeeDevice { get; set; }
        public virtual DbSet<EmployeeOvertimeRate> EmployeeOvertimeRate { get; set; }
        public virtual DbSet<Holiday> Holiday { get; set; }
        public virtual DbSet<EmployeeLeave> EmployeeLeave { get; set; }
        public virtual DbSet<HRYear> HRYear { get; set; }
        public virtual DbSet<EmployeeWorkSiteType> EmployeeWorkSiteType { get; set; }
        public virtual DbSet<Interview> Interview { get; set; }
        public virtual DbSet<InterviewHistory> InterviewHistory { get; set; }
        public virtual DbSet<CandidateEvaluation> CandidateEvaluation { get; set; }
        public virtual DbSet<CandidateScoringScale> CandidateScoringScale { get; set; }
        public virtual DbSet<CandidateEvaluationCategory> CandidateEvaluationCategory { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<SalaryHead> SalaryHead { get; set; }
        public virtual DbSet<EmployeeSalary> EmployeeSalary { get; set; }
        public virtual DbSet<Payroll> Payroll { get; set; }
        public virtual DbSet<PayrollDetail> PayrollDetail { get; set; }
        public virtual DbSet<SalaryTaxSlab> SalaryTaxSlab { get; set; }
        public virtual DbSet<DoctorProfile> DoctorProfile { get; set; }
        public virtual DbSet<DoctorServiceFee> DoctorServiceFee { get; set; }

        #endregion

        #region Transfer

        public virtual DbSet<WarehouseTransfer> WarehouseTransfer { get; set; }
        public virtual DbSet<WarehouseTransferDetail> WarehouseTransferDetail { get; set; }

        #endregion

        #region Appointment

        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<Appointment> Appointment { get; set; }
        public virtual DbSet<PriorityLevel> PriorityLevel { get; set; }
        public virtual DbSet<AppointmentType> AppointmentType { get; set; }
        public virtual DbSet<VisitType> VisitType { get; set; }
        public virtual DbSet<AppointmentStatus> AppointmentStatus { get; set; }
        public virtual DbSet<Triage> Triage { get; set; }
        public virtual DbSet<Consultation> Consultation { get; set; }
        public virtual DbSet<PatientProblem> PatientProblem { get; set; }
        public virtual DbSet<Prescription> Prescription { get; set; }
        public virtual DbSet<AppointmentAttachment> AppointmentAttachment { get; set; }
        public virtual DbSet<LabOrderType> LabOrderType { get; set; }
        public virtual DbSet<LabOrder> LabOrder { get; set; }
        public virtual DbSet<LabTestVariable> LabTestVariable { get; set; }
        public virtual DbSet<LabTestVariableOption> LabTestVariableOption { get; set; }
        public virtual DbSet<LabResult> LabResult { get; set; }
        public virtual DbSet<RadiologyOrder> RadiologyOrder { get; set; }
        public virtual DbSet<AppointmentPayment> AppointmentPayment { get; set; }
        public virtual DbSet<RadiologyType> RadiologyType { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<AppointmentService> AppointmentService { get; set; }

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


            modelBuilder.Entity<Project>()
          .HasOne(c => c.CreatedBy) // Navigation property
          .WithMany() // No inverse navigation
          .HasForeignKey(c => c.CreatedById) // Foreign key
          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);


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
                 .HasIndex(c => new { c.CompanyId, c.Code, c.IsActive })
                 .IsUnique();

            modelBuilder.Entity<SubCategory>()
                .HasIndex(sc => new { sc.CompanyId, sc.Code, sc.IsActive })
                .IsUnique();

            modelBuilder.Entity<ItemType>()
                .HasIndex(it => new { it.CompanyId, it.Code, it.IsActive })
                .IsUnique();

            modelBuilder.Entity<Item>()
                .HasIndex(i => new { i.CompanyId, i.Code, i.IsActive })
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

            modelBuilder.Entity<CategoryStore>(entity =>
            {
                entity.ToTable("CategoryStore");
                entity.HasKey(cs => cs.Id);

                entity.HasOne(cs => cs.Category)
                      .WithMany(c => c.CategoryStores)
                      .HasForeignKey(cs => cs.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cs => cs.Store)
                      .WithMany(s => s.CategoryStores) // only one navigation
                      .HasForeignKey(cs => cs.StoreId)
                      .OnDelete(DeleteBehavior.Restrict);


                modelBuilder.Entity<Department>()
                    .HasOne(c => c.ModifiedBy)
                    .WithMany()
                    .HasForeignKey(c => c.ModifiedById)
                    .OnDelete(DeleteBehavior.Restrict);


            });

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

            #region SALE

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.Dealership)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.DealershipId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Dealership>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Dealership>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(u => u.UserTerritory)
                .WithOne(o => o.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserTerritory>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserTerritory>()
               .HasOne(c => c.ModifiedBy)
               .WithMany()
               .HasForeignKey(c => c.ModifiedById)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(u => u.DSFRoute)
                .WithOne(o => o.DSF)
                .HasForeignKey(u => u.DSFId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DSFRoute>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DSFRoute>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.DSF)
                .HasForeignKey(u => u.DSFId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AspNetUsers>()
              .HasMany(u => u.Attachments)
              .WithOne(o => o.User)
              .HasForeignKey(u => u.UserId)
              .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AspNetUsers>()
              .HasMany(u => u.UserAttendance)
              .WithOne(o => o.User)
              .HasForeignKey(u => u.UserId)
              .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AspNetUsers>()
              .HasMany(u => u.EmployeeDevice)
              .WithOne(o => o.Employee)
              .HasForeignKey(u => u.EmployeeId)
              .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AspNetUsers>()
         .HasMany(u => u.EmployeeSalary)
         .WithOne(o => o.Employee)
         .HasForeignKey(u => u.EmployeeId)
         .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeSalary>()
           .HasOne(c => c.CreatedBy)
           .WithMany()
           .HasForeignKey(c => c.CreatedById)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeSalary>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeDevice>()
                 .HasOne(c => c.CreatedBy)
                 .WithMany()
                 .HasForeignKey(c => c.CreatedById)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeDevice>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserAttendance>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserAttendance>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserAttendance>()
                .HasOne(c => c.ManualBy)
                .WithMany()
                .HasForeignKey(c => c.ManualById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attachments>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attachments>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
           .HasMany(u => u.UserProject)
           .WithOne(o => o.User)
           .HasForeignKey(u => u.UserId)
           .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserProject>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProject>()
               .HasOne(c => c.ModifiedBy)
               .WithMany()
               .HasForeignKey(c => c.ModifiedById)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItems>()
              .HasMany(o => o.DispatchDetails)
              .WithOne(d => d.OrderItem)
              .HasForeignKey(d => d.OrderItemId);

            // Disable Cascade on Status (no cascade delete)
            modelBuilder.Entity<SaleReturn>()
                .HasOne(p => p.Status)
                .WithMany()  // Assuming no navigation property on Status for PurchaseDemand
                .HasForeignKey(p => p.StatusId)
                .OnDelete(DeleteBehavior.NoAction);  // No action on delete for Status

            #endregion

            #region HR

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.EmployeeDesignation)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.EmployeeDesignationId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeDesignation>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeDesignation>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.EmployeeEducation)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.EmployeeEducationId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeEducation>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeEducation>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.EmployeeGrade)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.EmployeeGradeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeGrade>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeGrade>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.EmployeeShift)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.EmployeeShiftId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeShift>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeShift>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.EmployeeType)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.EmployeeTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeType>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeType>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.EmployeeLeaveGroup)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.EmployeeLeaveGroupId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeLeaveGroup>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeLeaveGroup>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.EmployeeBank)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.EmployeeBankId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeBank>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeBank>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(u => u.EmployeeWorkingDays)
                .WithOne(o => o.Employee)
                .HasForeignKey(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeWorkingDays>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeWorkingDays>()
               .HasOne(c => c.ModifiedBy)
               .WithMany()
               .HasForeignKey(c => c.ModifiedById)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(u => u.EmployeeDocument)
                .WithOne(o => o.Employee)
                .HasForeignKey(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeDocument>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeDocument>()
               .HasOne(c => c.ModifiedBy)
               .WithMany()
               .HasForeignKey(c => c.ModifiedById)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.City)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.CityId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<City>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<City>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.EmployeeOvertimeRate)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.EmployeeOvertimeRateId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeOvertimeRate>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeOvertimeRate>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AspNetUsers>()
                .HasOne(u => u.EmployeeWorkSiteType)
                .WithMany(o => o.AspNetUsers)
                .HasForeignKey(u => u.EmployeeWorkSiteTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.Entity<AspNetUsers>()
            //    .HasOne(u => u.Status)
            //    .WithMany(o => o.AspNetUsers)
            //    .HasForeignKey(u => u.StatusId)
            //    .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeWorkSiteType>()
              .HasOne(c => c.CreatedBy)
              .WithMany()
              .HasForeignKey(c => c.CreatedById)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeWorkSiteType>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorProfile>()
          .HasOne(c => c.CreatedBy)
          .WithMany()
          .HasForeignKey(c => c.CreatedById)
          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorProfile>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorProfile>()
                .HasOne(x => x.Doctor)
                .WithOne(x => x.DoctorProfile)
                .HasForeignKey<DoctorProfile>(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region CostSheet

            // CostSheet -> GRNDetail
            modelBuilder.Entity<GRNDetail>()
               .HasOne(g => g.CostSheet)
               .WithMany(c => c.GRNDetails)
               .HasForeignKey(g => g.CostSheetId)
               .OnDelete(DeleteBehavior.Restrict); // or your desired behavior

            // CostSheet → DispatchDetail
            modelBuilder.Entity<DispatchDetail>()
                .HasOne(d => d.CostSheet)
                .WithMany(c => c.DispatchDetail)
                .HasForeignKey(d => d.CostSheetId)
                .OnDelete(DeleteBehavior.Restrict);

            // CostSheet → WarehouseTransferDetail
            modelBuilder.Entity<WarehouseTransferDetail>()
                .HasOne(w => w.CostSheet)
                .WithMany(c => c.WarehouseTransferDetail)
                .HasForeignKey(w => w.CostSheetId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Appointment


            modelBuilder.Entity<Appointment>()
              .HasOne(a => a.Patient)
              .WithMany(u => u.PatientAppointments)
              .HasForeignKey(a => a.PatientId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(u => u.DoctorAppointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(c => c.ConfirmedBy)
                .WithMany()
                .HasForeignKey(c => c.ConfirmedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
               .HasOne(c => c.CreatedBy)
               .WithMany()
               .HasForeignKey(c => c.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
             .HasOne(c => c.ModifiedBy)
             .WithMany()
             .HasForeignKey(c => c.ModifiedById)
             .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Roster

            modelBuilder.Entity<RosterDetail>()
             .HasOne(a => a.Employee)
             .WithMany(u => u.RosterDetail)
             .HasForeignKey(a => a.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RosterDetail>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

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
