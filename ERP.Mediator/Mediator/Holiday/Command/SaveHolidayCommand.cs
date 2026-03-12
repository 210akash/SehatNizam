using MediatR;
using System;

namespace ERP.Mediator.Mediator.Holiday.Command
{
    public class SaveHolidayCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
