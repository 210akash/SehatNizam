using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ERP.Entities.Models
{
    public class PagingData
    {
        //public PagingData(int? pageid, int? PageSize)
        //{
        //    IsPagingEnabled = true;
        //    Take = PageSize == null ? 20 : (int)PageSize;
        //    CurrentPage = pageid == null ? 0 : (int)pageid;
        //    Skip = (Take * (CurrentPage));
        //}

        public int CurrentPage { get; set; }
        public int Take { get; set; }
        public int Skip
        {
            get { return Take * (CurrentPage); }   // get method
            set { Skip = value; }  // set method }
        }
        public bool IsPagingEnabled  { get; set; } = true;
    }
}
