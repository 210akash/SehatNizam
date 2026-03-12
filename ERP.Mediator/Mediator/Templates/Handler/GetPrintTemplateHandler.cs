using AutoMapper;
using MediatR;
using ERP.Mediator.Mediator.Templates.Query;
using ERP.Repositories.UnitOfWork;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using ERP.BusinessModels.Enums;

namespace ERP.Mediator.Mediator.Templates.Handler
{
    public class GetPrintTemplateHandler : IRequestHandler<GetPrintTemplateQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPrintTemplateHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<string> Handle(GetPrintTemplateQuery request, CancellationToken cancellationToken)
        {
            var template = unitOfWork.Repository<Entities.Models.Templates>().FindAsync(y => y.Id == request.TemplateId).Result.Content;

            var dispatch = await unitOfWork.Repository<Entities.Models.Dispatch>().GetFirstAsNoTrackingAsync(y => y.Id == request.DispatchId, null, null,
                "Vehicle,DispatchOrder,DispatchOrder.DispatchDetail,DispatchOrder.DispatchDetail.OrderItem,DispatchOrder.DispatchDetail.OrderItem.Item," +
                "DispatchOrder.DispatchDetail.OrderItem.Item.ItemType");

            var order = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(y => y.Id == request.OrderId, null, null,
                "Dealership,DispatchOrder,Dealership.Territory");
            
            var productRows = new StringBuilder();
            decimal totalPrice = 0;

            if (order.DealershipId != null)
            {
                foreach (var detail in dispatch.DispatchOrder.Where(x => x.OrderId == request.OrderId && x.DispatchId == request.DispatchId).FirstOrDefault().DispatchDetail)
                {
                    decimal itemTotalPrice = (decimal)(detail.OrderItem.DistributorPrice * detail.Quantity);
                    totalPrice += itemTotalPrice;

                    string ProductType = detail.OrderItem.Item.ItemType.Name.ToLower() == "bottle" ? "Pets" :
                        detail.OrderItem.Item.ItemType.Name.ToLower() == "can" ? "Tray" : detail.OrderItem.Item.ItemType.Name;

                    productRows.AppendLine($@"
                    <tr>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{detail.OrderItem.Item.Name}-{detail.OrderItem.Item.Volume} ML {ProductType}</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{detail.Quantity}</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{detail.OrderItem.DistributorPrice.ToString("#,0.##")} Rs.</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{itemTotalPrice.ToString("#,0.##")} Rs.</td>
                    </tr>");
                }

                // Add a row for the total price at the end of the table
                productRows.AppendLine($@"
                <tr>
                    <td colspan='3' style='padding: 10px; border: 1px solid #ddd; text-align: right;'><strong>Total Price</strong></td>
                    <td style='padding: 10px; border: 1px solid #ddd;'><strong>{totalPrice.ToString("#,0.##")} Rs.</strong></td>
                </tr>");

                template = template.Replace("{{name}}", order.Dealership.Name + " (Distributor)");
                template = template.Replace("{{phone}}", order.Dealership.PhoneNo);

                template = template.Replace("{{delivery_note}}", order.DispatchOrder.Where(x => x.OrderId == request.OrderId && x.DispatchId == request.DispatchId).FirstOrDefault().DCCode);
                template = template.Replace("{{do_date}}", order.DispatchOrder.Where(x => x.OrderId == request.OrderId && x.DispatchId == request.DispatchId).FirstOrDefault().CreatedDate.Value.ToString("dd-MMMM-yyyy"));
                template = template.Replace("{{entity_name}}", order.Dealership.Name);
                template = template.Replace("{{shipping_address}}", order.DealershipAddress);
                template = template.Replace("{{vehicle_number}}", dispatch.Vehicle.RegistrationNumber);
                template = template.Replace("{{driver_name}}", dispatch.Vehicle.DriverName);
                template = template.Replace("{{driver_number}}", dispatch.Vehicle.DriverPhoneNo);
                template = template.Replace("{{sale_order_reference}}", order.Id.ToString());
                template = template.Replace("{{sale_order_date}}", order.CreatedDate.Value.Date.ToString("dd-MMMM-yyyy"));
                template = template.Replace("{{town_territory}}", order.Dealership.Territory.Name);
                template = template.Replace("{{contact_no}}", order.Dealership.PhoneNo);
                template = template.Replace("{{transporter_name}}", dispatch.Vehicle.LogisticPartner);
                template = template.Replace("{{builty_number}}", dispatch.BiltyNo.ToString());
                template = template.Replace("{{freight_amount}}", dispatch.FreightCharges.ToString());
                template = template.Replace("{{remarks}}", dispatch.Remarks);
            }
            else if (order.ShopId != null)
            {
                foreach (var detail in order.OrderItems)
                {
                    decimal itemTotalPrice = (decimal)(detail.CustomTradePrice * detail.ShippedQuantity);

                    totalPrice += itemTotalPrice;
                    productRows.AppendLine($@"
                    <tr>
                    //<td style='padding: 10px; border: 1px solid #ddd;'>{detail.Item.Name} - {detail.Item.Weight + "ML"} - {detail.Item.QuantityInPack + "Pcs"}</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{detail.ShippedQuantity}</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{detail.CustomTradePrice} Rs.</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{detail.CustomTradePrice * detail.ShippedQuantity} Rs.</td>
                    </tr>");
                }

                // Add a row for the total price at the end of the table
                productRows.AppendLine($@"
                <tr>
                    <td colspan='3' style='padding: 10px; border: 1px solid #ddd; text-align: right;'><strong>Total Price</strong></td>
                    <td style='padding: 10px; border: 1px solid #ddd;'><strong>{totalPrice.ToString("#,0.##")} Rs.</strong></td>
                </tr>");

                template = template.Replace("{{name}}", "<strong>Shop : </strong>" + order.Shop.Name);
                template = template.Replace("{{phone}}", "<strong>Phone : </strong>" + order.Shop.PhoneNo);
            }


            template = template.Replace("{{delivery_challan_no}}", order.DispatchOrder.Where(x => x.OrderId == request.OrderId && x.DispatchId == request.DispatchId).FirstOrDefault().DCCode);
            template = template.Replace("{{product_rows}}", productRows.ToString());

            if (request.TemplateId == 4 || request.TemplateId == 5)
            {
                //var fairPriceShopRemarks = order.OrderProcess.Where(x => x.FromStatusId == null && x.ToStatusId == (long)OrderStatusEnum.Create && !string.IsNullOrEmpty(x.Comments) && !string.IsNullOrEmpty(x.Reference) && !string.IsNullOrEmpty(x.Department)).FirstOrDefault();
                //template = template.Replace("{{comments}}", fairPriceShopRemarks == null ? string.Empty : "<strong>Remarks : </strong>" + fairPriceShopRemarks.Comments);
                //template = template.Replace("{{department}}", fairPriceShopRemarks == null ? string.Empty : "<strong>Department : </strong>" + fairPriceShopRemarks.Department);
                //template = template.Replace("{{reference}}", fairPriceShopRemarks == null ? string.Empty : "<strong>Customer : </strong>" + fairPriceShopRemarks.Reference);
            }

            return template;
        }
    }
}
