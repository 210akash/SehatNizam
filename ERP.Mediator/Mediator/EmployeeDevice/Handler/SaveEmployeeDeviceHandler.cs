using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.City.Command;
using ERP.Mediator.Mediator.EmployeeDevice.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.City.Handler
{
    public class SaveEmployeeDeviceHandler : IRequestHandler<SaveEmployeeDeviceCommand, Tuple<long, string>>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeDeviceHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<Tuple<long,string>> IRequestHandler<SaveEmployeeDeviceCommand, Tuple<long, string>>.Handle(SaveEmployeeDeviceCommand request, CancellationToken cancellationToken)
        {
            var deviceList = await unitOfWork.Repository<Entities.Models.EmployeeDevice>()
                .GetPagingWhereAsNoTrackingAsync(y => y.EmployeeId == request.EmployeeId && y.IsActive == true,
                null, null, null, null, null).Item1.ToListAsync();

            // Extract EnrollmentNos from request into a list
            var enrollmentNos = request.EmployeeDevices
                .Where(x => !string.IsNullOrWhiteSpace(x.EnrollmentNo))
                .Select(x => x.EnrollmentNo)
                .ToList();

            foreach (var item in request.EmployeeDevices)
            {
                // Now query using Contains (translatable to SQL)
                var isEnrollNoExists = await unitOfWork.Repository<Entities.Models.EmployeeDevice>()
                    .GetFirstAsync(e =>
                        e.DeviceId == item.DeviceId &&
                        e.EmployeeId != request.EmployeeId &&
                        e.IsActive == true &&
                        e.Id != item.Id &&
                        enrollmentNos.Contains(e.EnrollmentNo)
                    );

                if (isEnrollNoExists != null)
                {
                    var _device = await unitOfWork.Repository<Entities.Models.Device>().GetFirstAsync(y => y.Id == item.DeviceId);
                    var _employee = await unitOfWork.Repository<AspNetUsers>().GetFirstAsync(y => y.Id == isEnrollNoExists.EmployeeId);
                    return new Tuple<long, string>(409,"Already Register against user " + _employee.FirstName + " " + _employee.LastName + " and device " + _device.Name);
                }
            }

            List<long> previousdevicesIds = deviceList
                .Select(y => y.Id)
                .ToList();

            List<long> currentCategoryDeviceIds = request.EmployeeDevices.Select(y => y.Id).ToList();
            List<long> deletedCategoryDeviceIds = previousdevicesIds.Except(currentCategoryDeviceIds).ToList();

            foreach (var deletedCategoryDeviceId in deletedCategoryDeviceIds)
            {
                Entities.Models.EmployeeDevice _device = deviceList.Where(y => y.Id == deletedCategoryDeviceId).FirstOrDefault();

                if (_device != null)
                {
                    _device.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _device.DeleteDate = DateTime.Now;
                    _device.IsActive = false;
                    _device.IsDelete = true;
                    unitOfWork.Repository<Entities.Models.EmployeeDevice>().Update(_device);
                }
            }

            foreach (var Device in request.EmployeeDevices)
            {
                if (Device.Id != 0)
                {
                    var update = await unitOfWork.Repository<Entities.Models.EmployeeDevice>().GetFirstAsync(x => x.Id == Device.Id);
                    update.EnrollmentNo = Device.EnrollmentNo;
                    update.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    update.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeDevice>().Update(update);
                }
                else
                {
                    var update = await unitOfWork.Repository<Entities.Models.EmployeeDevice>().GetFirstAsync(x => x.DeviceId == Device.DeviceId && x.EmployeeId == request.EmployeeId);
                    if(update != null)
                    {
                        update.EnrollmentNo = Device.EnrollmentNo;
                        update.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        update.ModifiedDate = DateTime.Now;
                        update.IsActive = true;
                        update.IsDelete = false;
                        update.DeleteDate = null;
                        unitOfWork.Repository<Entities.Models.EmployeeDevice>().Update(update);
                    }
                    else
                    {
                        Entities.Models.EmployeeDevice _device = new()
                        {
                            EmployeeId = request.EmployeeId,
                            EnrollmentNo = Device.EnrollmentNo,
                            DeviceId = Device.DeviceId,
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now
                        };
                        unitOfWork.Repository<Entities.Models.EmployeeDevice>().Add(_device);
                    }
                }
            }

            long check = SaveChanges();
            if (check > 0)
                return new Tuple<long, string>(200, "Save Data Successfully");
            else
                return new Tuple<long, string>(500, "There is some error");
        }
    }
}