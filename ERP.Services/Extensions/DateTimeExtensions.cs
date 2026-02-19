using ERP.Entities.Models;
using ERP.Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertToTimeZone(this DateTime dateTime, Guid userId, IUnitOfWork context)
        {
            var user = context.Repository<AspNetUsers>().Set.Where(u => u.Id == userId).FirstOrDefault();
            if (user != null && Convert.ToInt32(user.TimeZone) > 0)
            {
            }
            return dateTime;
        }

    }
}
