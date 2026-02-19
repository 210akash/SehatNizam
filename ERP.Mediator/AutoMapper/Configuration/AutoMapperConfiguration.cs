namespace ERP.Mediator.AutoMapper.Configuration
{
    using global::AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using ERP.BusinessModels.BaseVM;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Entities.ComplexTypes;
    using ERP.Entities.Models;
    using ERP.Mediator.Mediator.Vendor.Command;
    using ERP.Mediator.Mediator.Company.Command;
    using ERP.Mediator.Mediator.Department.Command;
    using ERP.Mediator.Mediator.Store.Command;
    using ERP.Mediator.Mediator.UOM.Command;
    using ERP.Mediator.Mediator.Category.Command;
    using ERP.Mediator.Mediator.SubCategory.Command;
    using ERP.Mediator.Mediator.ItemType.Command;
    using ERP.Mediator.Mediator.Item.Command;
    using ERP.Mediator.Mediator.Location.Command;
    using ERP.Mediator.Mediator.Project.Command;
    using ERP.Mediator.Mediator.IndentRequest.Command;
    using ERP.Mediator.Mediator.IndentType.Command;
    using ERP.Mediator.Mediator.Priority.Command;
    using ERP.Mediator.Mediator.PurchaseDemand.Command;
    using ERP.Mediator.Mediator.Currency.Command;
    using ERP.Mediator.Mediator.ShipmentMode.Command;
    using ERP.Mediator.Mediator.PaymentMode.Command;
    using ERP.Mediator.Mediator.ComparativeStatement.Command;
    using ERP.Mediator.Mediator.DeliveryTerms.Command;
    using ERP.Mediator.Mediator.GST.Command;
    using ERP.Mediator.Mediator.PurchaseOrder.Command;
    using ERP.Mediator.Mediator.IGP.Command;
    using ERP.Mediator.Mediator.AccountCategory.Command;
    using ERP.Mediator.Mediator.AccountType.Command;
    using ERP.Mediator.Mediator.Account.Command;
    using ERP.Mediator.Mediator.AccountSubCategory.Command;
    using ERP.Mediator.Mediator.AccountHead.Command;
    using ERP.Mediator.Mediator.AccountFlow.Command;
    using ERP.Mediator.Mediator.Transaction.Command;

    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            this.CreateMap<AspNetUsers, AspNetUsersModel>()
                .ForMember(d => d.Code, opt => opt.MapFrom(s => s.Code));
            this.CreateMap<AspNetUsers, AspNetUsersModel>().ReverseMap()
                .ForMember(d => d.Code, opt => opt.MapFrom(s => s.Code));
            this.CreateMap<AspNetRoles, AspNetRolesModel>();
            this.CreateMap<AspNetRoles, AspNetRolesModel>().ReverseMap();
            this.CreateMap<IdentityResult, IdentityResponse>();

            this.CreateMap<spGetUsers, GetUserResponse>();
            this.CreateMap<spGetTotalUserRoles, GetTotalUserRolesResponse>();
            this.CreateMap<AspNetUsers, AspnetUserModelResponse>();

            this.CreateMap<AspNetUsers, GetAllUsers>()
                  .ForMember(d => d.CompanyId, opt => opt.MapFrom(s => s.Department.CompanyId))
                .ReverseMap();
            this.CreateMap<AspNetUsers, GetUser>().ReverseMap();
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


            this.CreateMap<AccountCategory, GetAccountCategory>().ReverseMap();
            this.CreateMap<AccountCategory, SaveAccountCategoryCommand>().ReverseMap();

            this.CreateMap<AccountSubCategory, GetAccountSubCategory>().ReverseMap();
            this.CreateMap<AccountSubCategory, SaveAccountSubCategoryCommand>().ReverseMap();

            this.CreateMap<AccountType, GetAccountType>().ReverseMap();
            this.CreateMap<AccountType, SaveAccountTypeCommand>().ReverseMap();

            this.CreateMap<Account, GetAccount>().ReverseMap();
            this.CreateMap<Account, SaveAccountCommand>().ReverseMap();

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
        }
    }
}
