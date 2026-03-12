using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PriceGroup.Query;
using ERP.Mediator.Mediator.Region.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.PriceGroup.Handler
{
    public class GetItemGroupDetailsByGroupIdHandler : IRequestHandler<GetItemGroupDetailsByGroupIdQuery, List<GetItemGroupDetails>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetItemGroupDetailsByGroupIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetItemGroupDetails>> Handle(GetItemGroupDetailsByGroupIdQuery request, CancellationToken cancellationToken)
        {
            var mappedProducts = new List<GetItemGroupDetails>();
            var priceGroupDetails = await unitOfWork.Repository<ERP.Entities.Models.PriceGroupDetails>().GetAsync(x => x.PriceGroupId == request.Id, null, null, "Item");

            if (priceGroupDetails.Count() > 0)
            {
                var ids = priceGroupDetails.Select(x => x.ItemId).ToList();

                var customOrder = new Dictionary<char, int>
                {
                    { 'C', 1 },
                    { 'L', 2 },
                    { 'O', 3 },
                    { 'K', 4 }
                };

                // Get products that match the ids
                var products = (await unitOfWork.Repository<ERP.Entities.Models.Item>().GetAsync(
                          x => ids.Contains(x.Id),
                          null,
                          null,
                          "ItemType,PriceGroupDetails"
                      ))
                      .OrderByDescending(x => x.Weight)
                      .ThenBy(x => customOrder.ContainsKey(char.ToUpper(x.Name[0]))
                                   ? customOrder[char.ToUpper(x.Name[0])]
                                   : int.MaxValue)
                      .ToList();

                // Map pricing details and other information
                mappedProducts = products.Select(prd => new GetItemGroupDetails
                {
                    ItemId = prd.Id,
                    ProductName = prd.Name,
                    ProductType = prd.ItemType.Name,
                    VolumeInMl = prd.Weight,
                    QuantityInPack = prd.QuantityInPack,
                    ImageName = prd.Image,
                    // Map PriceGroupDetails if available
                    PriceGroupDetailsId = priceGroupDetails.FirstOrDefault(pgd => pgd.ItemId == prd.Id)?.Id,
                    RetailPrice = priceGroupDetails.FirstOrDefault(pgd => pgd.ItemId == prd.Id)?.RetailPrice,
                    TradePrice = priceGroupDetails.FirstOrDefault(pgd => pgd.ItemId == prd.Id)?.TradePrice,
                    DistributorPrice = priceGroupDetails.FirstOrDefault(pgd => pgd.ItemId == prd.Id)?.DistributorPrice,
                    DistributorPromo = priceGroupDetails.FirstOrDefault(pgd => pgd.ItemId == prd.Id)?.DistributorPromo,
                    NetDistributorPrice = priceGroupDetails.FirstOrDefault(pgd => pgd.ItemId == prd.Id)?.NetDistributorPrice
                }).ToList();
            }
            else
            {



                var items = (await unitOfWork.Repository<ERP.Entities.Models.Item>().GetAsync(
                 x => x.CompanyId == 2,
                 null,
                 null,
                 "ItemType.SubCategory.Category.CategoryStores,PriceGroupDetails"
                  )).Where(x => x.ItemType.SubCategory.Category.CategoryStores
                 .Any(cs => cs.StoreId == 3)).OrderBy(x => x.Name).ToList();


                var map = mapper.Map<List<GetItemGroupDetails>>(items);
                // Map pricing details from the fetched items if necessary
                // This assumes `PriceGroupDetails` is already part of the `items`
                mappedProducts = items.Select(prd => new GetItemGroupDetails
                {
                    ItemId = prd.Id,
                    ProductName = prd.Name,
                    ProductType = prd.ItemType.Name,
                    VolumeInMl = prd.Weight,
                    QuantityInPack = prd.QuantityInPack,
                    ImageName = prd.Image,
                    // Assuming PriceGroupDetails already available within prd
                    PriceGroupDetailsId = 0,
                    RetailPrice = 0,
                    TradePrice = 0,
                    DistributorPrice = 0,
                    DistributorPromo = 0,
                    NetDistributorPrice = 0
                }).ToList();
            }





            //var query = (from prd in unitOfWork.Repository<ERP.Entities.Models.Item>().GetAll()
            //                 //join att in unitOfWork.Repository<Attachments>().GetAll() on prd.Id equals att.ProductId
            //             join pgdTemp in unitOfWork.Repository<PriceGroupDetails>().GetAll()
            //                 .Where(pgd => pgd.IsActive && !pgd.IsDelete && pgd.PriceGroupId == request.Id)
            //                 on prd.Id equals pgdTemp.ItemId into priceGroupDetails
            //             from pgd in priceGroupDetails.DefaultIfEmpty() // Left join
            //             where prd.IsActive && !prd.IsDelete
            //             //&& att.IsActive && !att.IsDelete
            //             orderby prd.Id ascending
            //             select new GetProductGroupDetails
            //             {
            //                 ItemId = prd.Id,
            //                 ProductName = prd.Name,
            //                 ProductType = prd.ItemType.Name,
            //                 VolumeInMl = prd.Weight,
            //                 QuantityInPack = prd.QuantityInPack,
            //                 //ImageName = att.ImageName,
            //                 PriceGroupDetailsId = pgd != null ? pgd.Id : (long?)null,
            //                 RetailPrice = pgd != null ? pgd.RetailPrice : (decimal?)null,
            //                 TradePrice = pgd != null ? pgd.TradePrice : (decimal?)null,
            //                 DistributorPrice = pgd != null ? pgd.DistributorPrice : (decimal?)null,
            //                 DistributorPromo = pgd != null ? pgd.DistributorPromo : (decimal?)null,
            //                 NetDistributorPrice = pgd != null ? pgd.NetDistributorPrice : (decimal?)null
            //             }).ToList();





            //var query = (from prd in unitOfWork.Repository<ERP.Entities.Models.Product>().GetAll()
            //             join att in unitOfWork.Repository<Attachments>().GetAll() on prd.Id equals att.ProductId
            //             join pgd in unitOfWork.Repository<PriceGroupDetails>().GetAll() on prd.Id equals pgd.ProductId into priceGroupDetails
            //             from pgd in priceGroupDetails.DefaultIfEmpty() // Left join on PriceGroupDetails
            //             where prd.IsActive == true && prd.IsDelete == false
            //                   && att.IsActive == true && att.IsDelete == false
            //                   && (pgd == null || (pgd.IsActive == true && pgd.IsDelete == false && pgd.PriceGroupId == request.Id))
            //             orderby prd.Id ascending
            //             select new GetProductGroupDetails
            //             {
            //                 ProductId = prd.Id,
            //                 ProductName = prd.Name,
            //                 ProductType = prd.Type,
            //                 VolumeInMl = prd.VolumeInMl,
            //                 QuantityInPack = prd.QuantityInPack,
            //                 ImageName = att.ImageName,
            //                 PriceGroupDetailsId = pgd != null ? pgd.Id : (long?)null,
            //                 RetailPrice = pgd != null ? pgd.RetailPrice : (decimal?)null,
            //                 TradePrice = pgd != null ? pgd.TradePrice : (decimal?)null,
            //                 DistributorPrice = pgd != null ? pgd.DistributorPrice : (decimal?)null
            //             }).ToList();

            return mappedProducts;
        }
    }
}
