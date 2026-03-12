using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeaveGroup.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Handler
{
    public class SaveEmployeeLeaveGroupHandler : IRequestHandler<SaveEmployeeLeaveGroupCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeLeaveGroupHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeLeaveGroupCommand, long>.Handle(SaveEmployeeLeaveGroupCommand request, CancellationToken cancellationToken)
        {
            var employeeLeaveGroup = await unitOfWork.Repository<Entities.Models.EmployeeLeaveGroup>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeLeaveGroup>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (employeeLeaveGroup == null)
                {
                    var _employeeLeaveGroup = mapper.Map<Entities.Models.EmployeeLeaveGroup>(request);
                    _employeeLeaveGroup.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _employeeLeaveGroup.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeLeaveGroup>().Add(_employeeLeaveGroup);
                    SaveChanges();

                    //foreach (var leaveGroupType in request.LeaveGroupTypes)
                    //{
                    //    var employeeLeaveGroupType = new Entities.Models.EmployeeLeaveGroupType
                    //    {
                    //        NoOfLeaves = (long)leaveGroupType.NoOfLeaves,
                    //        EmployeeLeaveTypeId = leaveGroupType.EmployeeLeaveTypeId,
                    //        EmployeeLeaveGroupId = _employeeLeaveGroup.Id,
                    //        CreatedById = sessionProvider.Session.LoggedInUserId,
                    //        CreatedDate = DateTime.Now
                    //    };
                    //    unitOfWork.Repository<Entities.Models.EmployeeLeaveGroupType>().Add(employeeLeaveGroupType);
                    //}
                }
                else
                {
                    var _employeeLeaveGroup = mapper.Map<Entities.Models.EmployeeLeaveGroup>(request);
                    _employeeLeaveGroup.CreatedById = employeeLeaveGroup.CreatedById;
                    _employeeLeaveGroup.CreatedDate = employeeLeaveGroup.CreatedDate;
                    _employeeLeaveGroup.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _employeeLeaveGroup.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeLeaveGroup>().Update(_employeeLeaveGroup);

                    //var existingLeaveGroupTypes = await unitOfWork.Repository<Entities.Models.EmployeeLeaveGroupType>()
                    //    .GetAsync(x => x.EmployeeLeaveGroupId == _employeeLeaveGroup.Id);

                    //foreach (var leaveGroupType in request.LeaveGroupTypes)
                    //{
                    //    var existingLeaveGroupType = existingLeaveGroupTypes
                    //        .FirstOrDefault(x => x.EmployeeLeaveTypeId == leaveGroupType.EmployeeLeaveTypeId);

                    //    if(existingLeaveGroupType != null)
                    //    {
                    //        existingLeaveGroupType.NoOfLeaves = (long)leaveGroupType.NoOfLeaves;
                    //        existingLeaveGroupType.ModifiedDate = DateTime.Now;
                    //        existingLeaveGroupType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    //        unitOfWork.Repository<Entities.Models.EmployeeLeaveGroupType>().Update(existingLeaveGroupType);
                    //    }
                    //    else
                    //    {
                    //        var employeeLeaveGroupType = new Entities.Models.EmployeeLeaveGroupType
                    //        {
                    //            NoOfLeaves = (long)leaveGroupType.NoOfLeaves,
                    //            EmployeeLeaveTypeId = leaveGroupType.EmployeeLeaveTypeId,
                    //            EmployeeLeaveGroupId = _employeeLeaveGroup.Id,
                    //            CreatedById = sessionProvider.Session.LoggedInUserId,
                    //            CreatedDate = DateTime.Now
                    //        };
                    //        unitOfWork.Repository<Entities.Models.EmployeeLeaveGroupType>().Add(employeeLeaveGroupType);
                    //    }
                    //}

                    SaveChanges();
                }
                return 200;

            }
            else
            {
                return 409;
            }

        }
    }
}