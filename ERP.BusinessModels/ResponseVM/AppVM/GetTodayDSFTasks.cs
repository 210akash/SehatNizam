using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM.AppVM
{
    public class GetTodayDSFTasks
    {
        public Guid DSFId { get; set; }
        public string SalesmenName { get; set; }
        public string SalesmenPhoneNo { get; set; }
        public string SalesmenCNIC { get; set; }
        public TimeSpan? SalesmenShiftTimeStart { get; set; }
        public TimeSpan? SalesmenShiftTimeEnd { get; set; }

        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public long TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public long RouteId { get; set; }
        public long ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopPhone { get; set; }
        public string ShopAddress { get; set; }
        public string ShopLocation { get; set; }
        public string SchedulerName { get; set; }

        public string CNIC { get; set; }
        public string RouteName { get; set; }
        public string VisitDay { get; set; }

        public long? VisitPlannerId { get; set; }
        public DateTime? PlannedDate { get; set; }
        public string Comments { get; set; }
        public bool? IsVisit { get; set; }
        public DateTime? VisitDate { get; set; }
        public long? VisitStatusId { get; set; }
        public string VisitStatus { get; set; }
        public bool IsShiftStarted { get; set; }


        public VisitPlannersHistoryVM VisitPlannersHistoryVM { get; set; }
        public List<VisitPlannersAttachmentVM> VisitPlannersAttachments { get; set; }
    }

    public class VisitPlannersHistoryVM
    {
        public DateTime? PlannedDate { get; set; }
        public string Comments { get; set; }
        public bool? IsVisit { get; set; }
        public DateTime? VisitDate { get; set; }

        public List<VisitPlannersAttachmentVM> VisitPlannersAttachments { get; set; }
    }
    public class VisitPlannersAttachmentVM
    {
        public long ImageId { get; set; }
        public String ImageName { get; set; }
    }
}
