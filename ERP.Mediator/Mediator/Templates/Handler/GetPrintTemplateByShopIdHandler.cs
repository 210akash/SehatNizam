using AutoMapper;

using ERP.Entities.Models;
using ERP.Mediator.Mediator.Templates.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Templates.Handler
{
    public class GetPrintTemplateByShopIdHandler : IRequestHandler<GetPrintTemplateByShopIdQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPrintTemplateByShopIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<string> Handle(GetPrintTemplateByShopIdQuery request, CancellationToken cancellationToken)
        {
            var template = unitOfWork.Repository<Entities.Models.Templates>().FindAsync(y => y.Id == request.TemplateId).Result.Content;

            //Expression<Func<Entities.Models.Shop, bool>> predicate = x => x.IsActive == true && x.Id == request.ShopId;


            //Expression<Func<Entities.Models.Shop, object>>[] includes = {
            //        x => x.PaymentBooking.Where(x=>x.IsActive == true && x.IsDelete == false)
            //        };

            //List<string> thenInclude = new List<string>();
            //thenInclude.Add("PaymentBooking.Account");


            //var lObjShopEntity = unitOfWork.Repository<Entities.Models.Shop>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, thenInclude, includes);



            //var productRows = new StringBuilder();
            //if (lObjShopEntity.Item1 != null)
            //{
            //    var shop = lObjShopEntity.Item1.ToList().FirstOrDefault();
            //    template = template.Replace("{{shopName}}", shop.Name);
            //    template = template.Replace("{{ownerName}}", shop.OwnerName);
            //    template = template.Replace("{{address}}", shop.Address);
            //    template = template.Replace("{{phone}}", shop.PhoneNo);


            //    var totalDebit = shop.PaymentBooking.Sum(x => x.Debit);
            //    var totalCredit = shop.PaymentBooking.Sum(x => x.Credit);

            //    template = template.Replace("{{debit}}", ((int)totalDebit).ToString());
            //    template = template.Replace("{{credit}}", ((int)totalCredit).ToString());
            //}


            //template = template.Replace("{{ledger_rows}}", productRows.ToString());

            return template;
        }
    }
}
