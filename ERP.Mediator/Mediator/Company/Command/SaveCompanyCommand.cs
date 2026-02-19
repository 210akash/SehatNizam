using MediatR;

namespace ERP.Mediator.Mediator.Company.Command
{
    public class SaveCompanyCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string NTN { get; set; }
        public string Phone { get; set; }
        public string Color { get; set; }

        public FileCommand FileCommand { get; set; }
    }

    public class FileCommand
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
    }
}
