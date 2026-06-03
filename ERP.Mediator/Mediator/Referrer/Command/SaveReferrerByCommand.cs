using MediatR;

namespace ERP.Mediator.Mediator.Referrer.Command
{
    public class SaveReferrerCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Hospital { get; set; }
    }
}
