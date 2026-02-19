using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Company.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Implementation;
using ERP.Services.Interfaces;
using MediatR;

namespace ERP.Mediator.Mediator.Company.Handler
{
    public class SaveCompanyHandler : IRequestHandler<SaveCompanyCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveCompanyHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.blobService = blobService;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveCompanyCommand, long>.Handle(SaveCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await unitOfWork.Repository<Entities.Models.Company>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Company>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (company == null)
                {
                    var _company = mapper.Map<Entities.Models.Company>(request);
                    _company.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _company.CreatedDate = DateTime.Now;

                    if (request.FileCommand != null)
                    {
                        BlobImageUploadModel blobModel = new();
                        blobModel.File = request.FileCommand.FilePath;
                        blobModel.FileName = request.FileCommand.FileName;
                        blobModel.FolderName = "assets\\Files";
                        _company.Logo = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, request.FileCommand.Extension);
                    }

                    unitOfWork.Repository<Entities.Models.Company>().Add(_company);
                }
                else
                {
                    var _company = mapper.Map<Entities.Models.Company>(request);
                    _company.CreatedById = company.CreatedById;
                    _company.CreatedDate = company.CreatedDate;
                    _company.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _company.ModifiedDate = DateTime.Now;

                    if (request.FileCommand != null)
                    {
                        BlobImageUploadModel blobModel = new();
                        blobModel.File = request.FileCommand.FilePath;
                        blobModel.FileName = request.FileCommand.FileName;
                        blobModel.FolderName = "assets\\Files";
                        _company.Logo = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, request.FileCommand.Extension);
                    }

                    unitOfWork.Repository<Entities.Models.Company>().Update(_company);
                }
                SaveChanges();
                return 200;

            }
            else
            {
                return 409;
            }

        }
    }
}