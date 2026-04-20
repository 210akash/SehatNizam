using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Roster.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Roster.Handler
{
    public class SaveRosterByManagerHandler : IRequestHandler<SaveRosterByManagerCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRosterByManagerHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveRosterByManagerCommand, long>.Handle(SaveRosterByManagerCommand request, CancellationToken cancellationToken)
        {
            var departmentId = sessionProvider.Session.DepartmentId;
            var saveDateTime = DateTime.Now;
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Roster>().GetAsync(x => x.DepartmentId == departmentId && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.Year == request.Year && x.Month == request.Month);
            if (checkDuplicate.Count() == 0)
            {
                var Roster = await unitOfWork.Repository<Entities.Models.Roster>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
                if (Roster == null)
                {
                    var _Roster = mapper.Map<Entities.Models.Roster>(request);
                    _Roster.DepartmentId = departmentId;
                    _Roster.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Roster.CreatedDate = saveDateTime;
                    _Roster.StatusId = 1;

                    _Roster.RosterDetail.ForEach(y =>
                    {
                        y.CreatedDate = saveDateTime;
                        y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                    });

                    unitOfWork.Repository<Entities.Models.Roster>().Add(_Roster);
                    SaveChanges();
                }
                else
                {
                    // 🔒 BLOCK IF APPROVED
                    if (Roster.ApprovedDate != null)
                        throw new Exception("Roster already approved. Cannot modify.");

                    var masterupdate = request;
                    var detailupdate = masterupdate.RosterDetail;
                    masterupdate.RosterDetail = null;
                    var _Roster = mapper.Map<Entities.Models.Roster>(masterupdate);
                    _Roster.StatusId = Roster.StatusId;
                    _Roster.CreatedById = Roster.CreatedById;
                    _Roster.DepartmentId = Roster.DepartmentId;
                    _Roster.CreatedDate = Roster.CreatedDate;
                    _Roster.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Roster.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Roster>().Update(_Roster);

                    var RosterDetailList = await unitOfWork.Repository<RosterDetail>()
                        .GetPagingWhereAsNoTrackingAsync(y => y.RosterId == request.Id && y.IsActive == true,
                        null, null, null, null, null).Item1.ToListAsync();

                    List<long> previousRosterDetailIds = RosterDetailList
                        .Select(y => y.Id)
                        .ToList();

                    List<long> currentRosterDetailIds = detailupdate.Select(y => y.Id).ToList();
                    List<long> deletedRosterDetailIds = previousRosterDetailIds.Except(currentRosterDetailIds).ToList();

                    // Handle deletions
                    foreach (var deletedRosterDetailId in deletedRosterDetailIds)
                    {
                        RosterDetail _RosterDetail = RosterDetailList.Where(y => y.Id == deletedRosterDetailId).FirstOrDefault();

                        if (_RosterDetail != null)
                        {
                            _RosterDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            _RosterDetail.DeleteDate = DateTime.Now;
                            _RosterDetail.IsActive = false; // Soft delete
                            _RosterDetail.IsDelete = true; // Soft delete
                            unitOfWork.Repository<RosterDetail>().Update(_RosterDetail);
                        }
                    }

                    // Handle additions
                    foreach (var RosterD in detailupdate)
                    {
                        if (RosterD.Id != 0)
                        {
                            var updatedetail = await unitOfWork.Repository<RosterDetail>()
                                    .GetFirstAsync(x => x.Id == RosterD.Id);

                            updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            updatedetail.ModifiedDate = DateTime.Now;
                            updatedetail.IsOffDay = RosterD.IsOffDay;
                            updatedetail.EmployeeShiftId = RosterD.EmployeeShiftId;
                            unitOfWork.Repository<RosterDetail>().Update(updatedetail);
                        }
                        else
                        {
                            var _RosterDetail = mapper.Map<RosterDetail>(RosterD);
                            _RosterDetail.RosterId = request.Id;
                            _RosterDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                            _RosterDetail.CreatedDate = DateTime.Now;
                            unitOfWork.Repository<RosterDetail>().Add(_RosterDetail);
                        }
                    }
                    SaveChanges();
                } 
            }
            else
            {
                return 409;
            }
            return 404;
        }
    }
}