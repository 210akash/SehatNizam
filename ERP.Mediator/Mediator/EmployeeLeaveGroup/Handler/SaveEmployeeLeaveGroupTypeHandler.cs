using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dispatch.Command;
using ERP.Mediator.Mediator.EmployeeLeaveGroup.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Handler
{
    public class SaveEmployeeGroupLeaveTypeHandler : IRequestHandler<SaveEmployeeGroupLeaveTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeGroupLeaveTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeGroupLeaveTypeCommand, long>.Handle(SaveEmployeeGroupLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            foreach (var leaveGroupType in request.EmployeeGroupLeaveType)
            {
                EmployeeGroupLeaveType _GetEmployeeGroupLeaveType =
                   await unitOfWork.Repository<EmployeeGroupLeaveType>().GetFirstAsNoTrackingAsync(y => y.Id == leaveGroupType.Id);

                if (_GetEmployeeGroupLeaveType == null)
                {
                    var _EmployeeGroupLeaveType = mapper.Map<EmployeeGroupLeaveType>(leaveGroupType);
                    _EmployeeGroupLeaveType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _EmployeeGroupLeaveType.EmployeeLeaveGroupId = request.EmployeeLeaveGroupId;
                    _EmployeeGroupLeaveType.EmployeeGroupLeaveTypeDetail.ForEach(x =>
                    {
                        x.CreatedDate = DateTime.Now;
                        x.CreatedById = sessionProvider.Session.LoggedInUserId;
                    });
                    unitOfWork.Repository<EmployeeGroupLeaveType>().Add(_EmployeeGroupLeaveType);
                }
                else
                {
                    var masterupdate = leaveGroupType;
                    var detailupdate = masterupdate.EmployeeGroupLeaveTypeDetail;
                    masterupdate.EmployeeGroupLeaveTypeDetail = null;
                    var _EmployeeGroupLeaveTypeUpdate = mapper.Map<EmployeeGroupLeaveType>(masterupdate);
                    _EmployeeGroupLeaveTypeUpdate.CreatedById = _GetEmployeeGroupLeaveType.CreatedById;
                    _EmployeeGroupLeaveTypeUpdate.CreatedDate = _GetEmployeeGroupLeaveType.CreatedDate;
                    _EmployeeGroupLeaveTypeUpdate.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _EmployeeGroupLeaveTypeUpdate.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<EmployeeGroupLeaveType>().Update(_EmployeeGroupLeaveTypeUpdate);

                    var StoreList = unitOfWork.Repository<EmployeeGroupLeaveTypeDetail>()
                     .GetPagingWhereAsNoTrackingAsync(y => y.EmployeeGroupLeaveTypeId == leaveGroupType.Id && y.IsActive == true,
                     null, null, null, null, null).Item1;

                    List<long> previousCategoryStoreIds = StoreList.Select(y => y.Id).ToList();
                    List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                    List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                    // Handle deletions
                    foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                    {
                        EmployeeGroupLeaveTypeDetail _EmployeeGroupLeaveTypeDetail = StoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                        if (_EmployeeGroupLeaveTypeDetail != null)
                        {
                            _EmployeeGroupLeaveTypeDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            _EmployeeGroupLeaveTypeDetail.DeleteDate = DateTime.Now;
                            _EmployeeGroupLeaveTypeDetail.IsActive = false; // Soft delete
                            _EmployeeGroupLeaveTypeDetail.IsDelete = true; // Soft delete
                            unitOfWork.Repository<EmployeeGroupLeaveTypeDetail>().Update(_EmployeeGroupLeaveTypeDetail);
                        }
                    }

                    // Add Update
                    foreach (var _employeeGroupLeaveTypeDetailCom in detailupdate)
                    {
                        if (_employeeGroupLeaveTypeDetailCom.Id != 0)
                        {
                            var _employeeGroupLeaveTypeDetail = mapper.Map<EmployeeGroupLeaveTypeDetail>(_employeeGroupLeaveTypeDetailCom);
                            _employeeGroupLeaveTypeDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            _employeeGroupLeaveTypeDetail.ModifiedDate = DateTime.Now;
                            unitOfWork.Repository<EmployeeGroupLeaveTypeDetail>().Update(_employeeGroupLeaveTypeDetail);
                        }
                        else
                        {
                            var _employeeGroupLeaveTypeDetail = mapper.Map<EmployeeGroupLeaveTypeDetail>(_employeeGroupLeaveTypeDetailCom);
                            _employeeGroupLeaveTypeDetail.EmployeeGroupLeaveTypeId = leaveGroupType.Id;
                            _employeeGroupLeaveTypeDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                            _employeeGroupLeaveTypeDetail.CreatedDate = DateTime.Now;
                            unitOfWork.Repository<EmployeeGroupLeaveTypeDetail>().Add(_employeeGroupLeaveTypeDetail);
                        }
                    }
                }
            }

            if (SaveChanges() > 0)
                return 200;
            else
                return 405;
        }

        //async Task<long> IRequestHandler<SaveEmployeeGroupLeaveTypeCommand, long>.Handle(SaveEmployeeGroupLeaveTypeCommand request, CancellationToken cancellationToken)
        //{
        //    foreach (var leaveGroupType in request.EmployeeGroupLeaveType)
        //    {
        //        var existingLeaveGroupType = await unitOfWork.Repository<Entities.Models.EmployeeGroupLeaveType>().GetFirstAsNoTrackingAsync(x => x.Id == leaveGroupType.Id);
        //        if (existingLeaveGroupType != null)
        //        {
        //            // existingLeaveGroupType.NoOfLeaves = (long)leaveGroupType.NoOfLeaves;
        //            existingLeaveGroupType.ModifiedDate = DateTime.Now;
        //            existingLeaveGroupType.ModifiedById = sessionProvider.Session.LoggedInUserId;
        //            unitOfWork.Repository<Entities.Models.EmployeeGroupLeaveType>().Update(existingLeaveGroupType);
        //        }
        //        else
        //        {
        //            //var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeGroupLeaveType>().GetAsync(x => x.EmployeeLeaveGroupId == request.EmployeeLeaveGroupId && x.EmployeeLeaveTypeId == existingLeaveGroupType.EmployeeLeaveTypeId && x.IsActive == true && x.IsDelete == false);

        //            var EmployeeGroupLeaveType = new Entities.Models.EmployeeGroupLeaveType
        //            {
        //                //NoOfLeaves = (long)leaveGroupType.NoOfLeaves,
        //                //EmployeeLeaveTypeId = leaveGroupType.EmployeeLeaveTypeId,
        //                EmployeeLeaveGroupId = request.EmployeeLeaveGroupId,
        //                CreatedById = sessionProvider.Session.LoggedInUserId,
        //                CreatedDate = DateTime.Now
        //            };
        //            unitOfWork.Repository<Entities.Models.EmployeeGroupLeaveType>().Add(EmployeeGroupLeaveType);
        //        }
        //    }
        //    SaveChanges();

        //    return 409;
        //}
    }
}