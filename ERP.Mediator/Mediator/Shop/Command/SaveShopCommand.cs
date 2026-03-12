using ERP.BusinessModels.ParameterVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Shop.Command
{
    public class SaveShopCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string PinLocation { get; set; }
        public long TerritoryId { get; set; }
        //public long? SchedulerId { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public long ShopTypeId { get; set; }
        public List<ImageUploadModel> ShopImages { get; set; }

        public string SecondaryPhoneNo { get; set; }
        public int? PepsiFridge { get; set; }
        public int? CokeFridge { get; set; }
        public int? NestleFridge { get; set; }
        public int? NesfrutaFridge { get; set; }
        public int? OthersFridge { get; set; }
        public string Landmark { get; set; }
    }
}
