using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Doctor.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Doctor.Handler
{
    public class SaveDoctorProfileHandler : IRequestHandler<SaveDoctorProfileCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public SaveDoctorProfileHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SaveDoctorProfileCommand request, CancellationToken cancellationToken)
        {
            if (request.DoctorId == Guid.Empty)
            {
                return 400;
            }

            Entities.Models.DoctorProfile doctorProfile;

            if (request.Id > 0)
            {
                doctorProfile = await unitOfWork.Repository<Entities.Models.DoctorProfile>().FindAsync(y => y.Id == request.Id);
                if (doctorProfile == null)
                {
                    return 404;
                }

                mapper.Map(request, doctorProfile);
                doctorProfile.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
                doctorProfile.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.DoctorProfile>().Update(doctorProfile);
            }
            else
            {
                var exists = await unitOfWork.Repository<Entities.Models.DoctorProfile>()
                    .GetExistsAsync(x => x.DoctorId == request.DoctorId
                        && x.IsActive
                        && !x.IsDelete);

                if (exists)
                {
                    return 409;
                }

                doctorProfile = mapper.Map<Entities.Models.DoctorProfile>(request);
                doctorProfile.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                doctorProfile.IsActive = true;

                await unitOfWork.Repository<Entities.Models.DoctorProfile>().AddAsync(doctorProfile);
            }

            await unitOfWork.SaveChangesAsync();
            return 200;
        }
    }
}
