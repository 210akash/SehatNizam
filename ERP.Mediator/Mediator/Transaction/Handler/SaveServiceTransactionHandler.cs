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
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Transaction.Handler
{
    public class SaveServiceTransactionHandler : IRequestHandler<SaveServiceTransactionCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;


        public SaveServiceTransactionHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
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

        async Task<long> IRequestHandler<SaveServiceTransactionCommand, long>.Handle(SaveServiceTransactionCommand request, CancellationToken cancellationToken)
        {
            var Transaction = await unitOfWork.Repository<Entities.Models.Transaction>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            GetTransactionCodeQuery getTransactionCodeQuery = new GetTransactionCodeQuery(request.VoucherTypeId, request.Date);
            var _newCode = await GetCode(getTransactionCodeQuery, Transaction);
            request.Code = _newCode;

            if (Transaction == null)
            {
                var _Transaction = mapper.Map<Entities.Models.Transaction>(request);
                _Transaction.CompanyId = sessionProvider.Session.CompanyId;
                _Transaction.CreatedById = sessionProvider.Session.LoggedInUserId;
                _Transaction.CreatedDate = DateTime.Now;
                _Transaction.ProcessedById = sessionProvider.Session.LoggedInUserId;
                _Transaction.ProcessedDate = DateTime.Now;
                _Transaction.ApprovedById = new Guid("408C1D72-07FD-4E9A-A54C-D1AD4112F875");
                _Transaction.ApprovedDate = DateTime.Now;
                _Transaction.StatusId = _Transaction.StatusId;

                _Transaction.TransactionDetails.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                });

                unitOfWork.Repository<Entities.Models.Transaction>().Add(_Transaction);
                try
                {
                    SaveChanges();

                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            else
            {
                var masterupdate = request;
                var detailupdate =  masterupdate.TransactionDetails;
                var detaildocument =  masterupdate.TransactionDocuments;
                masterupdate.TransactionDetails = null;
                masterupdate.TransactionDocuments = null;
                var _Transaction = mapper.Map<Entities.Models.Transaction>(masterupdate);
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
                        unitOfWork.Repository<TransactionDetail>().Update(_TransactionDetail);
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
                        updatedetail.AccountGroupId = TransactionD.AccountGroupId;
                        updatedetail.IsGroup = TransactionD.IsGroup;
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
                        unitOfWork.Repository<TransactionDocument>().Update(_TransactionDocument);
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


        public async Task<string> GetCode(GetTransactionCodeQuery request, Entities.Models.Transaction transaction)
        {
            var VoucherType = await unitOfWork.Repository<Entities.Models.VoucherType>()
                .GetOneAsync(y => y.IsActive == true
                    && y.CompanyId == sessionProvider.Session.CompanyId
                    && y.Id == request.VoucherTypeId, null, null);

            string _TransactionCode = "";
            string yearMonth = request.VoucherDate.Year.ToString("") + request.VoucherDate.Month.ToString("D2"); // YYYYMM Format

            if(transaction == null)
            {
                // Fetch last transaction for the same Voucher Type & Year-Month
                Func<IQueryable<Entities.Models.Transaction>, IOrderedQueryable<Entities.Models.Transaction>> OrderByDesc =
                    query => query.OrderByDescending(x => x.Code);

                var lastTransaction = await unitOfWork.Repository<Entities.Models.Transaction>()
                    .GetFirstAsNoTrackingAsync(y => y.IsActive == true
                        && y.CompanyId == sessionProvider.Session.CompanyId
                        && y.VoucherTypeId == request.VoucherTypeId
                        && y.Code.StartsWith(VoucherType.Code + yearMonth),
                        OrderByDesc, null);

                // Extract last 4-digit serial and increment
                int newSerial = 1;
                if (lastTransaction != null)
                {
                    string lastSerialStr = lastTransaction.Code.Substring((VoucherType.Code + yearMonth).Length);
                    if (int.TryParse(lastSerialStr, out int lastSerial))
                    {
                        newSerial = lastSerial + 1;
                    }
                }

                // Format the new transaction code
                _TransactionCode = newSerial.ToString().PadLeft(4, '0'); // 4-digit serial
            }
            else
            {
                int oldSerial = 1;
                string lastSerialStr = transaction.Code.Substring((VoucherType.Code + yearMonth).Length);
                if (int.TryParse(lastSerialStr, out int lastSerial))
                {
                    oldSerial = lastSerial;
                }

                _TransactionCode = oldSerial.ToString().PadLeft(4, '0'); // 4-digit serial
            }

            return $"{VoucherType.Code}{yearMonth}{_TransactionCode}";
        }

    }
}