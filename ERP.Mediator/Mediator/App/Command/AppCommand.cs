using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.App.Command
{
    public class MarkAttendanceCommand
    {
        public string UserId { get; set; }
        public bool IsPresent { get; set; }
        public string Reason { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public DateTime AppDateTime { get; set; }
        public long? DealershipId { get; set; }
    }
    public class MarkAttendanceCheckOutCommand
    {
        public string UserId { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public DateTime AppDateTime { get; set; }
    }
    public class GetUserDetailsByUserIdCommand
    {
        public string UserId { get; set; }
        public DateTime AppDateTime { get; set; }
        [Required(ErrorMessage = "DeviceId is required")]
        public string DeviceId { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
    public class SaveShopTaggingCommand
    {
        public string UserId { get; set; }
        public string ShopName { get; set; }
        public string PhoneNo { get; set; }
        public string SecondaryPhoneNo { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public decimal? lat { get; set; }
        public decimal? lng { get; set; }
        public string ImageFileSource { get; set; }
        public string ImageExtension { get; set; } = ".jpg";

        public long ShopTypeId { get; set; }
        public int? PepsiFridge { get; set; }
        public int? CokeFridge { get; set; }
        public int? NestleFridge { get; set; }
        public int? NesfrutaFridge { get; set; }
        public int? OthersFridge { get; set; }
        public string Landmark { get; set; }

        public DateTime AppDateTime { get; set; }
    }
    public class SaveShopTaggingByDistCommand
    {
        public string UserId { get; set; }
        public long DistributorId { get; set; }
        public string ShopName { get; set; }
        public string PhoneNo { get; set; }
        public string SecondaryPhoneNo { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public decimal? lat { get; set; }
        public decimal? lng { get; set; }
        public string ImageFileSource { get; set; }
        public string ImageExtension { get; set; } = ".jpg";

        public long ShopTypeId { get; set; }
        public int? PepsiFridge { get; set; }
        public int? CokeFridge { get; set; }
        public int? NestleFridge { get; set; }
        public int? NesfrutaFridge { get; set; }
        public int? OthersFridge { get; set; }
        public string Landmark { get; set; }

        public DateTime AppDateTime { get; set; }
    }

    public class SaveShopTaggingByTerritoryCommand
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long TerritoryId { get; set; }
        public string ShopName { get; set; }
        public string PhoneNo { get; set; }
        public string SecondaryPhoneNo { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public decimal? lat { get; set; }
        public decimal? lng { get; set; }
        public List<string> ImageFileSource { get; set; }
        public string ImageExtension { get; set; } = ".jpg";

        public long ShopTypeId { get; set; }
        public int? PepsiFridge { get; set; }
        public int? CokeFridge { get; set; }
        public int? NestleFridge { get; set; }
        public int? NesfrutaFridge { get; set; }
        public int? OthersFridge { get; set; }
        public string Landmark { get; set; }

        public DateTime AppDateTime { get; set; }
    }

    public class UserAppDateCommand
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
        public DateTime AppDateTime { get; set; }
    }

    public class UserShopTagHistoryByUserCommand
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
        public DateTime AppDateTime { get; set; }
        public UserPagingData PagingData { get; set; }
    }

    public class GetUserDetailsByDeviceIdCommand
    {
        [Required(ErrorMessage = "DeviceId is required")]
        public string DeviceId { get; set; }
        public DateTime AppDateTime { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
    public class UpdateUVShopVerificationStatusCommand
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "ShopId is required")]
        public long ShopId { get; set; }
        [Required(ErrorMessage = "IsVerified is required")]
        public bool IsVerified { get; set; }
        public DateTime AppDateTime { get; set; }
    }
    public class GetTodayDSFShopByUserLocationCommand
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public DateTime AppDateTime { get; set; }
    }
    public class GetMarkShopVisitsByIdCommand
    {
        public long MarkShopVisitsId { get; set; }
        public DateTime AppDateTime { get; set; }
    }

    public class SupervisorDashboardVM
    {
        public long TodayTerritoryOrder { get; set; } = 0;
        public long TodayRouteShopVisited { get; set; } = 0;
        public long TotalRouteShopVisit { get; set; } = 0;
        public long TotalTeamMember { get; set; } = 0;
        public long OfflineTeamMember { get; set; } = 0;
        public long PresentTeamMember { get; set; } = 0;
        public long AbsentTeamMember { get; set; } = 0;
        public long TotalPendingShopTaggingRequest { get; set; } = 0;
    }

    public class DealershipProductVM
    {
        public long DealershipId { get; set; }
        public string DealershipName { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string PinLocation { get; set; }
        public List<ProductResult> Products { get; set; }
    }

    public class ProductResult
    {
        public long ProductId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal DistributorPrice { get; set; }
        public decimal QuantityInPack { get; set; }
        public string ImageName { get; set; }
        public decimal VolumeInMl { get; set; }
        public int OrderQuantity { get; set; } = 0;
    }
    public class SaveDealershipOrderCommand
    {
        public long? OrderId { get; set; } = 0;
        public long? DealershipId { get; set; }
        public string Address { get; set; }
        public string UserId { get; set; }
        public DateTime AppDateTime { get; set; }
        public string ImageFileSource { get; set; }
        public string ImageExtension { get; set; } = ".jpg";
        public List<SaveDealershipOrderItemCommand> OrderItemCommandList { get; set; }
    }
    public class SaveDealershipOrderItemCommand
    {
        public long ProductId { get; set; }
        public decimal DistributorPrice { get; set; }
        public int OrderQuantity { get; set; } = 0;
    }

    public class SaveShopOrderCommand
    {
        public long? OrderId { get; set; } = 0;
        public long? ShopId { get; set; }
        public string DSFId { get; set; }
        public string UserId { get; set; }
        public DateTime AppDateTime { get; set; }
        public List<SaveShopOrderItemCommand> OrderItemCommandList { get; set; }
    }
    public class SaveShopOrderItemCommand
    {
        public long ProductId { get; set; }
        public decimal TradePrice { get; set; }
        public int OrderQuantity { get; set; } = 0;
    }

    public class UpdateOrderStatusCommand
    {
        public string UserId { get; set; }
        public DateTime AppDateTime { get; set; }
        public long OrderId { get; set; }
        public long FromStatusId { get; set; }
        public long ToStatusId { get; set; }
        public string Comments { get; set; }
    }
    public class DispatchShopOrderCommand
    {
        public long OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime AppDateTime { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string VehicleNo { get; set; }
        public string DriverName { get; set; }
        public string DriverPhoneNo { get; set; }
        public string Comments { get; set; }
    }
    public class ReceiveShopOrderCommand
    {
        public long OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime AppDateTime { get; set; }
        public string DeliveryChallanCode { get; set; }
        public string Comments { get; set; }
    }
    public class MarkLoginLogoutState
    {
        public string DeviceId { get; set; }
        public DateTime AppDateTime { get; set; }
        public bool IsLogin { get; set; }
    }
    public class SaveDistOrderCommand
    {
        public long? OrderId { get; set; } = 0;
        public long? ShopId { get; set; }
        public string UserId { get; set; }
        public DateTime AppDateTime { get; set; }
        public List<SaveDistOrderItemCommand> OrderItemCommandList { get; set; }
    }
    public class SaveDistOrderItemCommand
    {
        public long ProductId { get; set; }
        public decimal DistributorPrice { get; set; }
        public int OrderQuantity { get; set; } = 0;
    }
    public class ReceiveOrderByDOIdCommand
    {
        public long DOId { get; set; }
        public string UserId { get; set; }
        public DateTime AppDateTime { get; set; }
        public string DeliveryChallanCode { get; set; }
        public string Comments { get; set; }
    }
    public class DeleteAccountCommand
    {
        public string UserId { get; set; }
    }

    public class UserPagingData
    {
        public int CurrentPage { get; set; }
        public int Take { get; set; }
        public int Skip
        {
            get { return Take * (CurrentPage); }   // get method
            set { Skip = value; }  // set method }
        }
        public bool IsPagingEnabled { get; set; } = true;
    }
}

