namespace ERP.Mediator.AutoMapper.Configuration
{
    using ERP.BusinessModels.BaseVM;
    using ERP.BusinessModels.ParameterVM;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Entities.ComplexTypes;
    using ERP.Entities.Models;
    using ERP.Mediator.Mediator.Account.Command;
    using ERP.Mediator.Mediator.AccountCategory.Command;
    using ERP.Mediator.Mediator.AccountFlow.Command;
    using ERP.Mediator.Mediator.AccountGroup.Command;
    using ERP.Mediator.Mediator.AccountHead.Command;
    using ERP.Mediator.Mediator.AccountSubCategory.Command;
    using ERP.Mediator.Mediator.AccountType.Command;
    using ERP.Mediator.Mediator.Area.Command;
    using ERP.Mediator.Mediator.Auth.Command;
    using ERP.Mediator.Mediator.Category.Command;
    using ERP.Mediator.Mediator.City.Command;
    using ERP.Mediator.Mediator.Company.Command;
    using ERP.Mediator.Mediator.ComparativeStatement.Command;
    using ERP.Mediator.Mediator.CostSheet.Command;
    using ERP.Mediator.Mediator.Currency.Command;
    using ERP.Mediator.Mediator.Dealership.Command;
    using ERP.Mediator.Mediator.DeliveryTerms.Command;
    using ERP.Mediator.Mediator.Department.Command;
    using ERP.Mediator.Mediator.Device.Command;
    using ERP.Mediator.Mediator.Dispatch.Command;
    using ERP.Mediator.Mediator.EmployeeBank.Command;
    using ERP.Mediator.Mediator.EmployeeDesignation.Command;
    using ERP.Mediator.Mediator.EmployeeDocumentType.Command;
    using ERP.Mediator.Mediator.EmployeeEducation.Command;
    using ERP.Mediator.Mediator.EmployeeGrade.Command;
    using ERP.Mediator.Mediator.EmployeeLeave.Command;
    using ERP.Mediator.Mediator.EmployeeLeaveGroup.Command;
    using ERP.Mediator.Mediator.EmployeeLeaveType.Command;
    using ERP.Mediator.Mediator.EmployeeOvertimeRate.Command;
    using ERP.Mediator.Mediator.EmployeeShift.Command;
    using ERP.Mediator.Mediator.EmployeeType.Command;
    using ERP.Mediator.Mediator.EmployeeWorkSiteType.Command;
    using ERP.Mediator.Mediator.GRN.Command;
    using ERP.Mediator.Mediator.GST.Command;
    using ERP.Mediator.Mediator.Holiday.Command;
    using ERP.Mediator.Mediator.HRYear.Command;
    using ERP.Mediator.Mediator.IGP.Command;
    using ERP.Mediator.Mediator.IGPType.Command;
    using ERP.Mediator.Mediator.IndentRequest.Command;
    using ERP.Mediator.Mediator.IndentType.Command;
    using ERP.Mediator.Mediator.Inspection.Command;
    using ERP.Mediator.Mediator.Interview.Command;
    using ERP.Mediator.Mediator.Issuance.Command;
    using ERP.Mediator.Mediator.Item.Command;
    using ERP.Mediator.Mediator.ItemType.Command;
    using ERP.Mediator.Mediator.Location.Command;
    using ERP.Mediator.Mediator.PaymentMode.Command;
    using ERP.Mediator.Mediator.PriceGroup.Command;
    using ERP.Mediator.Mediator.PrimaryOrder.Command;
    using ERP.Mediator.Mediator.Priority.Command;
    using ERP.Mediator.Mediator.Project.Command;
    using ERP.Mediator.Mediator.PurchaseDemand.Command;
    using ERP.Mediator.Mediator.PurchaseOrder.Command;
    using ERP.Mediator.Mediator.PurchaseReturn.Command;
    using ERP.Mediator.Mediator.Rack.Command;
    using ERP.Mediator.Mediator.Region.Command;
    using ERP.Mediator.Mediator.RejectReason.Command;
    using ERP.Mediator.Mediator.RetailOrder.Command;
    using ERP.Mediator.Mediator.RetailOrderReturn.Command;
    using ERP.Mediator.Mediator.Route.Command;
    using ERP.Mediator.Mediator.Row.Command;
    using ERP.Mediator.Mediator.SaleMaterial.Command;
    using ERP.Mediator.Mediator.SaleMaterialReturn.Command;
    using ERP.Mediator.Mediator.SaleReturn.Command;
    using ERP.Mediator.Mediator.SalesTarget.Command;
    using ERP.Mediator.Mediator.Section.Command;
    using ERP.Mediator.Mediator.ShipmentMode.Command;
    using ERP.Mediator.Mediator.Shop.Command;
    using ERP.Mediator.Mediator.ShopDispatch.Command;
    using ERP.Mediator.Mediator.ShopOrder.Command;
    using ERP.Mediator.Mediator.ShopOrderReturn.Command;
    using ERP.Mediator.Mediator.ShopType.Command;
    using ERP.Mediator.Mediator.Store.Command;
    using ERP.Mediator.Mediator.SubCategory.Command;
    using ERP.Mediator.Mediator.Templates.Command;
    using ERP.Mediator.Mediator.Territory.Command;
    using ERP.Mediator.Mediator.Transaction.Command;
    using ERP.Mediator.Mediator.UOM.Command;
    using ERP.Mediator.Mediator.UserAttendance.Command;
    using ERP.Mediator.Mediator.UserTerritory.Command;
    using ERP.Mediator.Mediator.Vehicle.Command;
    using ERP.Mediator.Mediator.Vendor.Command;
    using ERP.Mediator.Mediator.WarehouseTransfer.Command;
    using ERP.Mediator.Mediator.Zone.Command;
    using global::AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using System.Linq;

    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<AspNetUsers, AspNetUsersModel>()
        .ForMember(d => d.Code, opt => opt.MapFrom(s => s.Code))
        .ReverseMap();
            this.CreateMap<RegisterCommand, AspNetUsersModel>().ReverseMap();
            this.CreateMap<RegisterCommand, AspNetUsers>().ReverseMap();
            this.CreateMap<AspNetRoles, AspNetRolesModel>().ReverseMap();
            this.CreateMap<IdentityResult, IdentityResponse>();

            this.CreateMap<AspNetUsersModel, UpdateCommand>().ReverseMap();
            this.CreateMap<AspNetUsers, UpdateCommand>().ReverseMap();

            this.CreateMap<spGetUsers, GetUserResponse>();
            this.CreateMap<spGetTotalUserRoles, GetTotalUserRolesResponse>();
            this.CreateMap<AspNetUsers, AspnetUserModelResponse>();

            this.CreateMap<AspNetUsers, GetAllUsers>()
                  .ForMember(d => d.CompanyId, opt => opt.MapFrom(s => s.Department.CompanyId))
                .ReverseMap();
            this.CreateMap<AspNetUsers, GetUser>().ReverseMap();
            this.CreateMap<AspNetUsers, GetEmployee>().ReverseMap();
            this.CreateMap<AspNetRoles, GetRoles>().ReverseMap();
            this.CreateMap<Status, GetStatus>().ReverseMap();

            this.CreateMap<Vendor, SaveVendorCommand>().ReverseMap();
            this.CreateMap<Vendor, GetVendor>().ReverseMap();

            this.CreateMap<Company, SaveCompanyCommand>().ReverseMap();
            this.CreateMap<Company, GetCompany>().ReverseMap();

            this.CreateMap<Department, SaveDepartmentCommand>().ReverseMap();
            this.CreateMap<Department, GetDepartment>().ReverseMap();

            this.CreateMap<Store, GetStore>().ReverseMap();
            this.CreateMap<Store, SaveStoreCommand>().ReverseMap();

            this.CreateMap<UOM, GetUOM>().ReverseMap();
            this.CreateMap<UOM, SaveUOMCommand>().ReverseMap();

            this.CreateMap<Category, GetCategory>().ReverseMap();
            this.CreateMap<Category, SaveCategoryCommand>().ReverseMap();

            this.CreateMap<SubCategory, GetSubCategory>().ReverseMap();
            this.CreateMap<SubCategory, SaveSubCategoryCommand>().ReverseMap();

            this.CreateMap<ItemType, GetItemType>().ReverseMap();
            this.CreateMap<ItemType, SaveItemTypeCommand>().ReverseMap();

            this.CreateMap<Item, GetItem>().ReverseMap();
            this.CreateMap<Item, SaveItemCommand>().ReverseMap();

            this.CreateMap<Location, GetLocation>().ReverseMap();
            this.CreateMap<Location, SaveLocationCommand>().ReverseMap();

            this.CreateMap<Project, GetProject>().ReverseMap();
            this.CreateMap<Project, SaveProjectCommand>().ReverseMap();
            this.CreateMap<ProjectStore, GetProjectStore>().ReverseMap();
            this.CreateMap<CategoryStore, GetCategoryStore>().ReverseMap();

            this.CreateMap<IndentRequest, GetIndentRequest>().ReverseMap();
            this.CreateMap<IndentRequestDetail, GetIndentRequestDetail>().ReverseMap();
            this.CreateMap<IndentRequest, SaveIndentRequestCommand>().ReverseMap();
            this.CreateMap<IndentRequestDetail, SaveIndentRequestDetailCommand>().ReverseMap();
            this.CreateMap<IndentRequest, GetDropDown>()
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.IndentType.Name))
                .ForMember(d => d.Date, opt => opt.MapFrom(s => s.ApprovedDate))
                .ReverseMap();

            this.CreateMap<IndentType, SaveIndentTypeCommand>().ReverseMap();
            this.CreateMap<IndentType, GetIndentType>().ReverseMap();

            this.CreateMap<Priority, SavePriorityCommand>().ReverseMap();
            this.CreateMap<Priority, GetPriority>().ReverseMap();

            this.CreateMap<PurchaseDemand, GetPurchaseDemand>().ReverseMap();
            this.CreateMap<PurchaseDemand, GetDropDown>().ReverseMap();
            this.CreateMap<PurchaseDemandDetail, GetPurchaseDemandDetail>().ReverseMap();
            this.CreateMap<PurchaseDemand, SavePurchaseDemandCommand>().ReverseMap();
            this.CreateMap<PurchaseDemandDetail, SavePurchaseDemandDetailCommand>().ReverseMap();
            
            this.CreateMap<Currency, GetCurrency>().ReverseMap();
            this.CreateMap<Currency, SaveCurrencyCommand>().ReverseMap();

            this.CreateMap<ShipmentMode, GetShipmentMode>().ReverseMap();
            this.CreateMap<ShipmentMode, SaveShipmentModeCommand>().ReverseMap();

            this.CreateMap<PaymentMode, GetPaymentMode>().ReverseMap();
            this.CreateMap<PaymentMode, SavePaymentModeCommand>().ReverseMap();


            this.CreateMap<ComparativeStatement, GetComparativeStatement>().ReverseMap();
            this.CreateMap<ComparativeStatement, GetDropDown>().ReverseMap();
            this.CreateMap<ComparativeStatementDetail, GetComparativeStatementDetail>().ReverseMap();
            this.CreateMap<ComparativeStatementVendor, GetComparativeStatementVendor>().ReverseMap();

            this.CreateMap<ComparativeStatement, SaveComparativeStatementCommand>().ReverseMap();
            this.CreateMap<ComparativeStatementDetail, SaveComparativeStatementDetailCommand>().ReverseMap();
            this.CreateMap<ComparativeStatementVendor, SaveComparativeStatementVendorCommand>().ReverseMap();

            this.CreateMap<DeliveryTerms, GetDeliveryTerms>().ReverseMap();
            this.CreateMap<DeliveryTerms, SaveDeliveryTermsCommand>().ReverseMap();

            this.CreateMap<GST, GetGST>().ReverseMap();
            this.CreateMap<GST, SaveGSTCommand>().ReverseMap();


            this.CreateMap<PurchaseOrder, GetPurchaseOrder>().ReverseMap();
            this.CreateMap<PurchaseOrder, GetDropDown>().ReverseMap();
            this.CreateMap<PurchaseOrderDetail, GetPurchaseOrderDetail>().ReverseMap();
            this.CreateMap<PurchaseOrder, SavePurchaseOrderCommand>().ReverseMap();
            this.CreateMap<PurchaseOrderDetail, SavePurchaseOrderDetailCommand>().ReverseMap();

            this.CreateMap<IGP, GetIGP>().ReverseMap();
            this.CreateMap<IGP, GetDropDown>().ReverseMap();
            this.CreateMap<IGPDetails, GetIGPDetails>().ReverseMap();

            this.CreateMap<IGP, SaveIGPCommand>().ReverseMap();
            this.CreateMap<IGPDetails, SaveIGPDetailsCommand>().ReverseMap();

            this.CreateMap<GRN, GetGRN>().ReverseMap();
            this.CreateMap<GRNDetail, GetGRNDetail>().ReverseMap();

            this.CreateMap<GRN, SaveGRNCommand>().ReverseMap();
            this.CreateMap<GRNDetail, SaveGRNDetailCommand>().ReverseMap();

            this.CreateMap<AccountCategory, GetAccountCategory>().ReverseMap();
            this.CreateMap<AccountCategory, SaveAccountCategoryCommand>().ReverseMap();

            this.CreateMap<AccountSubCategory, GetAccountSubCategory>().ReverseMap();
            this.CreateMap<AccountSubCategory, SaveAccountSubCategoryCommand>().ReverseMap();

            this.CreateMap<AccountType, GetAccountType>().ReverseMap();
            this.CreateMap<AccountType, SaveAccountTypeCommand>().ReverseMap();

            this.CreateMap<Account, GetAccount>().ReverseMap();
            this.CreateMap<Account, SaveAccountCommand>().ReverseMap();

            this.CreateMap<AccountGroup, GetAccountGroup>().ReverseMap();
            this.CreateMap<AccountGroup, SaveAccountGroupCommand>().ReverseMap();

            this.CreateMap<AccountHead, GetAccountHead>().ReverseMap();
            this.CreateMap<AccountHead, SaveAccountHeadCommand>().ReverseMap();

            this.CreateMap<AccountFlow, GetAccountFlow>().ReverseMap();
            this.CreateMap<AccountFlow, SaveAccountFlowCommand>().ReverseMap();

            this.CreateMap<Transaction, GetTransaction>().ReverseMap();
            this.CreateMap<TransactionDetail, GetTransactionDetail>().ReverseMap();
            this.CreateMap<TransactionDocument, GetTransactionDocument>().ReverseMap();
            this.CreateMap<Transaction, SaveTransactionCommand>().ReverseMap();
            this.CreateMap<TransactionDetail, SaveTransactionDetailCommand>().ReverseMap();
            this.CreateMap<TransactionDocument, SaveTransactionDocumentCommand>().ReverseMap();

            this.CreateMap<Zone, GetZone>().ReverseMap();
            this.CreateMap<Zone, SaveZoneCommand>().ReverseMap();

            this.CreateMap<Territory, GetTerritory>().ReverseMap();
            this.CreateMap<Territory, SaveTerritoryCommand>().ReverseMap();

            this.CreateMap<Dealership, GetDealership>().ReverseMap();
            this.CreateMap<Dealership, SaveDealershipCommand>().ReverseMap();

            this.CreateMap<Shop, GetShop>().ReverseMap();
            this.CreateMap<Shop, SaveShopCommand>().ReverseMap();

            this.CreateMap<Region, GetRegionLite>().ReverseMap();
            this.CreateMap<Zone, GetZoneLite>().ReverseMap();
            this.CreateMap<Area, GetAreaLite>().ReverseMap();
            this.CreateMap<Territory, GetTerritoryLite>().ReverseMap();

            this.CreateMap<Dealership, GetDealershipLite>()
                .ForMember(d => d.TerritoryName, opt => opt.MapFrom(s => s.Territory.Name))
                .ForMember(d => d.AreaName, opt => opt.MapFrom(s => s.Territory.Area.Name))
                .ForMember(d => d.ZoneName, opt => opt.MapFrom(s => s.Territory.Area.Zone.Name))
                .ForMember(d => d.RegionName, opt => opt.MapFrom(s => s.Territory.Area.Zone.Region.Name))
                .ReverseMap();

            this.CreateMap<Shop, GetShopLite>()
                .ForMember(d => d.DistributorName, opt => opt.MapFrom(s => s.Territory.Dealership.FirstOrDefault(x => x.IsActive == true && x.IsDelete == false).Name))
                .ForMember(d => d.Image, opt => opt.MapFrom(s => s.Attachments.FirstOrDefault(x => x.IsActive == true && x.IsDelete == false).ImageName))
                .ForMember(d => d.TerritoryName, opt => opt.MapFrom(s => s.Territory.Name))
                .ForMember(d => d.AreaName, opt => opt.MapFrom(s => s.Territory.Area.Name))
                .ForMember(d => d.ZoneName, opt => opt.MapFrom(s => s.Territory.Area.Zone.Name))
                .ForMember(d => d.RegionName, opt => opt.MapFrom(s => s.Territory.Area.Zone.Region.Name))
                .ReverseMap();

            this.CreateMap<Route, GetRoute>().ReverseMap();
            this.CreateMap<Route, SaveRouteCommand>().ReverseMap();

            this.CreateMap<RouteShop, GetRouteShop>().ReverseMap();
            this.CreateMap<DSFRoute, GetDSFRoute>().ReverseMap();

            this.CreateMap<Region, GetRegion>().ReverseMap();
            this.CreateMap<Region, SaveRegionCommand>().ReverseMap();

            this.CreateMap<Area, GetArea>().ReverseMap();
            this.CreateMap<Area, SaveAreaCommand>().ReverseMap();

            this.CreateMap<ShopType, GetShopType>().ReverseMap();
            this.CreateMap<ShopType, SaveShopTypeCommand>().ReverseMap();

            this.CreateMap<ShopRouteFrequency, GetShopRouteFrequency>().ReverseMap();

            this.CreateMap<UserTerritory, GetUserTerritory>().ReverseMap();
            this.CreateMap<AspNetUsers, GetUsers>()
                .ForMember(d => d.RoleId, opt => opt.MapFrom(s => s.AspNetUserRoles.FirstOrDefault().RoleId))
                .ReverseMap();

            this.CreateMap<Inspection, GetInspection>().ReverseMap();
            this.CreateMap<Inspection, GetDropDown>().ReverseMap();
            this.CreateMap<InspectionDetail, GetInspectionDetail>()
                .ForMember(d => d.Approved, opt => opt.MapFrom(s => s.IGPDetail.Received - s.Rejected))
                .ReverseMap();
            this.CreateMap<Inspection, SaveInspectionCommand>().ReverseMap();
            this.CreateMap<InspectionDetail, SaveInspectionDetailCommand>().ReverseMap();

            this.CreateMap<RejectReason, SaveRejectReasonCommand>().ReverseMap();
            this.CreateMap<RejectReason, GetRejectReason>().ReverseMap();

            this.CreateMap<PriceGroup, CopyPriceGroupCommand>().ReverseMap();
            this.CreateMap<PriceGroup, GetPriceGroup>().ReverseMap();
            this.CreateMap<PriceGroup, SavePriceGroupCommand>().ReverseMap();

            this.CreateMap<Item, GetItemGroupDetails>()
                .ForMember(d => d.ItemId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.ProductType, opt => opt.MapFrom(s => s.ItemType.Name))
                .ForMember(d => d.VolumeInMl, opt => opt.MapFrom(s => s.Weight))
                .ForMember(d => d.QuantityInPack, opt => opt.MapFrom(s => s.QuantityInPack))
                .ForMember(d => d.PriceGroupDetailsId, opt => opt.MapFrom(s => s.PriceGroupDetails.Where(x => x.IsActive == true && x.IsDelete == false).FirstOrDefault().Id))
                .ForMember(d => d.RetailPrice, opt => opt.MapFrom(s => s.PriceGroupDetails.Where(x => x.IsActive == true && x.IsDelete == false).FirstOrDefault().RetailPrice))
                .ForMember(d => d.TradePrice, opt => opt.MapFrom(s => s.PriceGroupDetails.Where(x => x.IsActive == true && x.IsDelete == false).FirstOrDefault().TradePrice))
                .ForMember(d => d.DistributorPrice, opt => opt.MapFrom(s => s.PriceGroupDetails.Where(x => x.IsActive == true && x.IsDelete == false).FirstOrDefault().DistributorPrice))
                .ForMember(d => d.DistributorPromo, opt => opt.MapFrom(s => s.PriceGroupDetails.Where(x => x.IsActive == true && x.IsDelete == false).FirstOrDefault().DistributorPromo))
                .ForMember(d => d.NetDistributorPrice, opt => opt.MapFrom(s => s.PriceGroupDetails.Where(x => x.IsActive == true && x.IsDelete == false).FirstOrDefault().NetDistributorPrice))
                .ReverseMap();
            this.CreateMap<Vehicle, GetVehicle>().ReverseMap();

            this.CreateMap<Vehicle, SaveVehicleCommand>().ReverseMap();

            this.CreateMap<Order, GetOrder>()
                //.ForMember(d => d.CreatedBy, opt => opt.MapFrom(s => s.CreatedBy.FirstName + " " + s.CreatedBy.LastName))
                .ReverseMap();

            this.CreateMap<OrderItems, GetOrderItems>().ReverseMap();
            this.CreateMap<OrderProcess, GetOrderProcess>().ReverseMap();
            CreateMap<CreateOrderCommand, Order>()
                .ForMember(dest => dest.OrderAttachments, opt => opt.MapFrom(src => src.OrderAttachments))
                .ReverseMap();

            CreateMap<CreateOrderItems, OrderItems>()
                .ReverseMap();

            CreateMap<ImageUploadModel, Attachments>();
            this.CreateMap<Attachments, GetAttachments>()
                .ForMember(d => d.FileSource, opt => opt.MapFrom(s => s.ImageName))
                .ReverseMap();

            this.CreateMap<Attachments, GetDealershipAttachments>()
            .ForMember(d => d.FileSource, opt => opt.MapFrom(s => s.ImageName))
            .ReverseMap();

            this.CreateMap<UserAttendance, GetUserAttendance>()
                   //.ForMember(d => d.ZoneName, opt => opt.MapFrom(s => s.User.UserTerritory.Where(x => x.IsActive).FirstOrDefault().Zone.Name))
                   //.ForMember(d => d.TerritoryName, opt => opt.MapFrom(s => s.User.UserTerritory.Where(x => x.IsActive).FirstOrDefault().Territory.Name))
                   //.ForMember(d => d.DealershipName, opt => opt.MapFrom(s => s.User.UserTerritory.Where(x => x.IsActive).FirstOrDefault().Territory.Dealership.FirstOrDefault().Name))
              .ReverseMap();
            CreateMap<UserAttendance,SaveUserAttendanceCommand>().ReverseMap();

            this.CreateMap<UserTerritory, SaveUserTerritoryCommand>().ReverseMap();

            this.CreateMap<Order, CreateShopOrderCommand>().ReverseMap();
            this.CreateMap<GetTemplates, Templates>().ReverseMap();
            this.CreateMap<SaveTemplatesCommand, Templates>().ReverseMap();
            this.CreateMap<SaveRackCommand, Rack>().ReverseMap();
            this.CreateMap<GetRack, Rack>().ReverseMap();
            this.CreateMap<SaveRowCommand, Row>().ReverseMap();
            this.CreateMap<GetRow, Row>().ReverseMap();
            this.CreateMap<GetSection, Section>().ReverseMap();
            this.CreateMap<SaveSectionCommand, Section>().ReverseMap();
            this.CreateMap<OrderItems, PartialOrderItemsCommand>().ReverseMap();

            this.CreateMap<SalesTarget, GetSalesTarget>().ReverseMap();
            this.CreateMap<SalesTarget, SaveSalesTargetCommand>().ReverseMap();
            this.CreateMap<SalesTarget, TerritoryTargetList>().ReverseMap();
            this.CreateMap<SalesTarget, DSFTargetList>().ReverseMap();

            this.CreateMap<Issuance, GetIssuance>().ReverseMap();
            this.CreateMap<Issuance, GetDropDown>().ReverseMap();
            this.CreateMap<IssuanceDetail, GetIssuanceDetail>().ReverseMap();
            this.CreateMap<Issuance, SaveIssuanceCommand>().ReverseMap();
            this.CreateMap<IssuanceDetail, SaveIssuanceDetailCommand>().ReverseMap();
            this.CreateMap<Account, GetAccountByAccountFlow>().ReverseMap();

            this.CreateMap<Dispatch, GetDispatch>().ReverseMap();
            this.CreateMap<Dispatch, GetDropDown>().ReverseMap();
            this.CreateMap<DispatchDetail, GetDispatchDetail>().ReverseMap();
            this.CreateMap<Dispatch, SaveDispatchCommand>().ReverseMap();
            this.CreateMap<DispatchDetail, SaveDispatchDetailCommand>().ReverseMap();
            this.CreateMap<DispatchOrder, SaveDispatchOrderCommand>().ReverseMap();
            this.CreateMap<DispatchOrder, GetDispatchOrder>().ReverseMap();

            this.CreateMap<CostSheet, GetCostSheet>().ReverseMap();
            this.CreateMap<CostSheetDetail, GetCostSheetDetail>().ReverseMap();
            this.CreateMap<CostSheet, SaveCostSheetCommand>().ReverseMap();
            this.CreateMap<CostSheetDetail, SaveCostSheetDetailCommand>().ReverseMap();
            this.CreateMap<CostSheet, GetDropDown>()
                .ForMember(d => d.Date, opt => opt.MapFrom(s => s.ApprovedDate))
                .ReverseMap();

            this.CreateMap<CancelDispatch, GetCancelDispatch>().ReverseMap();
            this.CreateMap<CancelDispatchDetail, GetCancelDispatchDetail>().ReverseMap();

            this.CreateMap<SaleMaterial, GetSaleMaterial>().ReverseMap();
            this.CreateMap<SaleMaterial, SaveSaleMaterialCommand>().ReverseMap();

            this.CreateMap<SaleMaterialDetail, GetSaleMaterialDetail>().ReverseMap();
            this.CreateMap<SaleMaterialDetail, SaveSaleMaterialDetailCommand>().ReverseMap();
            this.CreateMap<DealershipType, GetDealershipType>().ReverseMap();

            this.CreateMap<EmployeeDesignation, GetEmployeeDesignation>().ReverseMap();
            this.CreateMap<EmployeeDesignation, SaveEmployeeDesignationCommand>().ReverseMap();

            this.CreateMap<EmployeeEducation, GetEmployeeEducation>().ReverseMap();
            this.CreateMap<EmployeeEducation, SaveEmployeeEducationCommand>().ReverseMap();

            this.CreateMap<EmployeeGrade, GetEmployeeGrade>().ReverseMap();
            this.CreateMap<EmployeeGrade, SaveEmployeeGradeCommand>().ReverseMap();

            this.CreateMap<EmployeeShift, GetEmployeeShift>().ReverseMap();
            this.CreateMap<EmployeeShift, SaveEmployeeShiftCommand>().ReverseMap();

            this.CreateMap<EmployeeType, GetEmployeeType>().ReverseMap();
            this.CreateMap<EmployeeType, SaveEmployeeTypeCommand>().ReverseMap();
            this.CreateMap<EmployeeWorkingDays, GetEmployeeWorkingDays>().ReverseMap();

            this.CreateMap<EmployeeBank, GetEmployeeBank>().ReverseMap();
            this.CreateMap<EmployeeBank, SaveEmployeeBankCommand>().ReverseMap();

            this.CreateMap<EmployeeLeaveGroup, GetEmployeeLeaveGroup>().ReverseMap();
            this.CreateMap<EmployeeGroupLeaveType, GetEmployeeGroupLeaveType>().ReverseMap();
            this.CreateMap<EmployeeGroupLeaveTypeDetail, GetEmployeeGroupLeaveTypeDetail>().ReverseMap();
            this.CreateMap<EmployeeLeaveGroup, SaveEmployeeLeaveGroupCommand>().ReverseMap();
            this.CreateMap<EmployeeGroupLeaveType, EmployeeGroupLeaveTypeCommand>().ReverseMap();
            this.CreateMap<EmployeeGroupLeaveTypeDetail, EmployeeGroupLeaveTypeDetailCommand>().ReverseMap();

            this.CreateMap<EmployeeLeaveType, GetEmployeeLeaveType>().ReverseMap();
            this.CreateMap<EmployeeLeaveType, SaveEmployeeLeaveTypeCommand>().ReverseMap();

            this.CreateMap<EmployeeDocumentType, GetEmployeeDocumentType>().ReverseMap();
            this.CreateMap<EmployeeDocumentType, SaveEmployeeDocumentTypeCommand>().ReverseMap();
            this.CreateMap<EmployeeDocument, GetEmployeeDocument>().ReverseMap();

            this.CreateMap<City, GetCity>().ReverseMap();
            this.CreateMap<City, SaveCityCommand>().ReverseMap();

            this.CreateMap<Device, GetDevice>().ReverseMap();
            this.CreateMap<Device, SaveDeviceCommand>().ReverseMap();


            this.CreateMap<DealershipType, GetDistributorType>().ReverseMap();

            this.CreateMap<IGPType, GetIGPType>().ReverseMap();
            this.CreateMap<IGPType, SaveIGPTypeCommand>().ReverseMap();

            this.CreateMap<SaleReturn, GetSaleReturn>().ReverseMap();
            this.CreateMap<SaleReturnDetail, GetSaleReturnDetail>().ReverseMap();

            this.CreateMap<SaleReturn, SaveSaleReturnCommand>().ReverseMap();
            this.CreateMap<SaleReturnDetail, SaveSaleReturnDetailCommand>().ReverseMap();

            this.CreateMap<EmployeeOvertimeRate, GetEmployeeOvertimeRate>().ReverseMap();
            this.CreateMap<EmployeeOvertimeRate, SaveEmployeeOvertimeRateCommand>().ReverseMap();

            this.CreateMap<ShopOrderReturn, GetShopOrderReturn>().ReverseMap();
            this.CreateMap<ShopOrderReturnDetail, GetShopOrderReturnDetail>().ReverseMap();

            this.CreateMap<ShopOrderReturn, SaveShopOrderReturnCommand>().ReverseMap();
            this.CreateMap<ShopOrderReturnDetail, SaveShopOrderReturnDetailCommand>().ReverseMap();


            this.CreateMap<PurchaseReturn, GetPurchaseReturn>().ReverseMap();
            this.CreateMap<PurchaseReturnDetail, GetPurchaseReturnDetail>().ReverseMap();

            this.CreateMap<PurchaseReturn, SavePurchaseReturnCommand>().ReverseMap();
            this.CreateMap<PurchaseReturnDetail, SavePurchaseReturnDetailCommand>().ReverseMap();

            this.CreateMap<WarehouseTransfer, GetWarehouseTransfer>().ReverseMap();
            this.CreateMap<WarehouseTransferDetail, GetWarehouseTransferDetail>().ReverseMap();

            this.CreateMap<WarehouseTransfer, SaveWarehouseTransferCommand>().ReverseMap();
            this.CreateMap<WarehouseTransferDetail, SaveWarehouseTransferDetailCommand>().ReverseMap();
            this.CreateMap<UserProject, GetUserProject>().ReverseMap();
            this.CreateMap<EmployeeDevice, GetEmployeeDevice>().ReverseMap();
            this.CreateMap<VendorType, GetVendorType>().ReverseMap();

            this.CreateMap<SaleMaterialReturn, GetSaleMaterialReturn>().ReverseMap();
            this.CreateMap<SaleMaterialReturn, SaveSaleMaterialReturnCommand>().ReverseMap();

            this.CreateMap<SaleMaterialReturnDetail, GetSaleMaterialReturnDetail>().ReverseMap();
            this.CreateMap<SaleMaterialReturnDetail, SaveSaleMaterialReturnDetailCommand>().ReverseMap();

            this.CreateMap<Order, GetDealershipOrder>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.Title))
           .ReverseMap();

            this.CreateMap<OrderItems, GetDealershipOrderItems>().ReverseMap();
            this.CreateMap<OrderProcess, GetDealershipOrderProcess>()
                .ForMember(dest => dest.FromStatus, opt => opt.MapFrom(src => src.FromStatus.Title))
                .ForMember(dest => dest.ToStatus, opt => opt.MapFrom(src => src.ToStatus.Title))
                .ReverseMap();

            this.CreateMap<HRYear, GetHRYear>().ReverseMap();
            this.CreateMap<HRYear, SaveHRYearCommand>().ReverseMap();

            this.CreateMap<EmployeeLeave, GetEmployeeLeave>().ReverseMap();
            this.CreateMap<EmployeeLeave, SaveEmployeeLeaveCommand>().ReverseMap();
            this.CreateMap<EmployeeLeave, SaveEmployeeLeaveByHrCommand>().ReverseMap();

            this.CreateMap<EmployeeWorkSiteType, GetEmployeeWorkSiteType>().ReverseMap();
            this.CreateMap<EmployeeWorkSiteType, SaveEmployeeWorkSiteTypeCommand>().ReverseMap();
            this.CreateMap<AspNetUsers, GetCreatedBy>();

            this.CreateMap<Interview, GetInterview>().ReverseMap();
            this.CreateMap<Interview, SaveInterviewCommand>().ReverseMap();

            this.CreateMap<InterviewAttendees, GetInterviewAttendees>().ReverseMap();
            this.CreateMap<InterviewHistory, GetInterviewHistory>().ReverseMap();

            this.CreateMap<Attachments, FileCommand>().ReverseMap();

            CreateMap<CreateShopOrderCommand, ShopOrder>().ReverseMap();
            CreateMap<CreateShopOrderByDealershipCommand, ShopOrder>().ReverseMap();
            CreateMap<CreateShopOrderItems, ShopOrderItems>().ReverseMap();

            this.CreateMap<RetailOrder, GetRetailOrder>().ReverseMap();
            this.CreateMap<RetailOrder, CreateRetailOrderCommand>().ReverseMap();

            this.CreateMap<RetailOrderItems, GetRetailOrderItems>().ReverseMap();
            this.CreateMap<RetailOrderProcess, GetRetailOrderProcess>().ReverseMap();
            this.CreateMap<Shop, GetShopBasic>().ReverseMap();

            this.CreateMap<ShopOrder, GetShopOrder>().ReverseMap();
               CreateMap<ShopOrder, GetShopOrder>()
               .ForMember(dest => dest.DealershipId,
               opt => opt.MapFrom(src =>
               src.Shop.Territory.Dealership
               .Where(d => d.IsActive).FirstOrDefault().Id));

            this.CreateMap<ShopOrderItems, GetShopOrderItems>().ReverseMap();

            CreateMap<ShopOrderItems, GetShopOrderItems>()
             .ForMember(dest => dest.DispatchQuantity,
               opt => opt.MapFrom(src =>
                   src.ShopDispatchDetails
                      .Where(d => d.IsActive)
                      .Sum(d => (int?)d.Quantity) ?? 0));

            this.CreateMap<ShopDispatch, SaveShopDispatchCommand>().ReverseMap();
            this.CreateMap<ShopDispatchDetail, SaveShopDispatchDetailCommand>().ReverseMap();

            this.CreateMap<ShopDispatch, GetShopDispatch>().ReverseMap();
            this.CreateMap<ShopDispatchDetail, GetShopDispatchDetail>().ReverseMap();

            this.CreateMap<ItemGroup, SaveItemGroupCommand>().ReverseMap();
            this.CreateMap<ItemGroup, GetItemGroup>().ReverseMap();

            this.CreateMap<Holiday, GetHoliday>().ReverseMap();
            this.CreateMap<Holiday, SaveHolidayCommand>().ReverseMap();

            this.CreateMap<RetailOrderReturn, GetRetailOrderReturn>().ReverseMap();
            this.CreateMap<RetailOrderReturnDetail, GetRetailOrderReturnDetail>().ReverseMap();

            this.CreateMap<RetailOrderReturn, SaveRetailOrderReturnCommand>().ReverseMap();
            this.CreateMap<RetailOrderReturnDetail, SaveRetailOrderReturnDetailCommand>().ReverseMap();
        }
    }
}
