using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetDistributorsSaleReport
    {
        public string MonthName { get; set; }
        public int MonthNumber { get; set; }
        public decimal Sale { get; set; }
        public int Quantity { get; set; }
    }

    public class GetTop50DistributorsSale
    {
        public long DistributorId { get; set; }
        public string DistributorName { get; set; }
        public string TerritoryName { get; set; }
        public decimal Sale { get; set; }
        public int Quantity { get; set; }
        public int OrderCount { get; set; }
    }

    public class GetTopSellingSKUs
    {
        public string Item { get; set; }
        public string ItemImage { get; set; }
        public int SoldPets { get; set; }
        public int? TotalAmount { get; set; }
    }

    public class GetOrdersByDistributorByMonth
    {
        public long OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string DistributorName { get; set; }
        public string DistributorPhone { get; set; }
        public string DistributorAddress { get; set; }
        public string TerritoryName { get; set; }
        public long Quantity { get; set; }
        public long Sale { get; set; }
    }

    public class GetRegionWiseSale
    {
        public long RegionId { get; set; }
        public string RegionName { get; set; }
        public long SoldPets { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class GetZoneWiseSale
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public long SoldPets { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ItemMonthlySalesVM
    {
        public string ItemName { get; set; }
        public decimal Jan { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar { get; set; }
        public decimal Apr { get; set; }
        public decimal May { get; set; }
        public decimal Jun { get; set; }
        public decimal Jul { get; set; }
        public decimal Aug { get; set; }
        public decimal Sep { get; set; }
        public decimal Oct { get; set; }
        public decimal Nov { get; set; }
        public decimal Dec { get; set; }
        public decimal Total { get; set; }
    }


}
