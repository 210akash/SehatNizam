using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Transaction.Handler
{
    public class SaveTransactionHandler : IRequestHandler<SaveTransactionCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;


        public SaveTransactionHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
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

        async Task<long> IRequestHandler<SaveTransactionCommand, long>.Handle(SaveTransactionCommand request, CancellationToken cancellationToken)
        {
            var Transaction = await unitOfWork.Repository<Entities.Models.Transaction>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (Transaction == null)
            {
                var VoucherType = await unitOfWork.Repository<Entities.Models.VoucherType>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.Id == request.VoucherTypeId, null, null);

                string _TransactionCode = "";
                if (await unitOfWork.Repository<Entities.Models.Transaction>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.VoucherTypeId == request.VoucherTypeId))
                {
                    Func<IQueryable<Entities.Models.Transaction>, IOrderedQueryable<Entities.Models.Transaction>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var TransactionCode = await unitOfWork.Repository<Entities.Models.Transaction>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.VoucherTypeId == request.VoucherTypeId, OrderByDesc, null);
                    int No = Convert.ToInt32(TransactionCode.Code.Replace(VoucherType.Code, "")) + 1;
                    _TransactionCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _TransactionCode = "0000001";
                request.Code = VoucherType.Code +  _TransactionCode;

                foreach (var _Documents in request.TransactionDocuments)
                {
                    BlobImageUploadModel blobModel = new()
                    {
                        File = _Documents.Path,
                        FileName = _Documents.FileName,
                        FolderName = "assets/Files/"
                    };

                    _Documents.Path = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, _Documents.Extension);
                }

                var _Transaction = mapper.Map<Entities.Models.Transaction>(request);
                _Transaction.CompanyId = sessionProvider.Session.CompanyId;
                _Transaction.CreatedById = sessionProvider.Session.LoggedInUserId;
                _Transaction.CreatedDate = DateTime.Now;
                _Transaction.StatusId = 1;

                _Transaction.TransactionDetails.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                });

                _Transaction.TransactionDocuments.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                });

                unitOfWork.Repository<Entities.Models.Transaction>().Add(_Transaction);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate =  masterupdate.TransactionDetails;
                var detaildocument =  masterupdate.TransactionDocuments;
                masterupdate.TransactionDetails = null;
                masterupdate.TransactionDocuments = null;
                var _Transaction = mapper.Map<Entities.Models.Transaction>(masterupdate);
                _Transaction.Code = Transaction.Code;
                _Transaction.StatusId = Transaction.StatusId;
                _Transaction.CreatedById = Transaction.CreatedById;
                _Transaction.CreatedDate = Transaction.CreatedDate;
                _Transaction.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _Transaction.ModifiedDate = DateTime.Now;
                _Transaction.CompanyId = Transaction.CompanyId;
                unitOfWork.Repository<Entities.Models.Transaction>().Update(_Transaction);

                var CategoryStoreList = await unitOfWork.Repository<TransactionDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.TransactionId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousCategoryStoreIds = CategoryStoreList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                // Handle deletions
                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    TransactionDetail _TransactionDetail = CategoryStoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_TransactionDetail != null)
                    {
                        _TransactionDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _TransactionDetail.DeleteDate = DateTime.Now;
                        _TransactionDetail.IsActive = false; // Soft delete
                        _TransactionDetail.IsDelete = true; // Soft delete
                    }
                }

                // Handle additions
                foreach (var TransactionD in detailupdate)
                {
                    if (TransactionD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<TransactionDetail>()
                                .GetFirstAsync(x => x.Id == TransactionD.Id);

                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        updatedetail.TransactionId = masterupdate.Id;
                        updatedetail.AccountId = TransactionD.AccountId;
                        updatedetail.DepartmentId = TransactionD.DepartmentId;
                        updatedetail.ProjectId = TransactionD.ProjectId;
                        updatedetail.Quantity = TransactionD.Quantity;
                        updatedetail.DebitAmount = TransactionD.DebitAmount;
                        updatedetail.CreditAmount = TransactionD.CreditAmount;
                        unitOfWork.Repository<TransactionDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _TransactionDetail = mapper.Map<TransactionDetail>(TransactionD);
                        _TransactionDetail.TransactionId = request.Id;
                        _TransactionDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _TransactionDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<TransactionDetail>().Add(_TransactionDetail);
                    }
                }


                // Documents deletion

                var docList = await unitOfWork.Repository<TransactionDocument>()
                   .GetPagingWhereAsNoTrackingAsync(y => y.TransactionId == request.Id && y.IsActive == true,
                   null, null, null, null, null).Item1.ToListAsync();

                List<long> previousdocIds = docList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentdocIds = detaildocument.Select(y => y.Id).ToList();
                List<long> deleteddocIds = previousdocIds.Except(currentdocIds).ToList();

                // Handle deletions
                foreach (var deleteddocId in deleteddocIds)
                {
                    TransactionDocument _TransactionDocument = docList.Where(y => y.Id == deleteddocId).FirstOrDefault();

                    if (_TransactionDocument != null)
                    {
                        _TransactionDocument.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _TransactionDocument.DeleteDate = DateTime.Now;
                        _TransactionDocument.IsActive = false; // Soft delete
                        _TransactionDocument.IsDelete = true; // Soft delete
                    }
                }

                // Handle additions
                foreach (var TransactionDoc in detaildocument)
                {
                    if (TransactionDoc.Id != 0)
                    {
                     
                    }
                    else
                    {
                        var _Transactiondoc = mapper.Map<TransactionDocument>(TransactionDoc);
                        BlobImageUploadModel blobModel = new()
                        {
                            File = TransactionDoc.Path,
                            FileName = TransactionDoc.FileName,
                            FolderName = "assets/Files/"
                        };

                        _Transactiondoc.Path = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, TransactionDoc.Extension);
                        _Transactiondoc.TransactionId = request.Id;
                        _Transactiondoc.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _Transactiondoc.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<TransactionDocument>().Add(_Transactiondoc);
                    }
                }
               
                SaveChanges();
            }
                return 200;
        }
    }
}