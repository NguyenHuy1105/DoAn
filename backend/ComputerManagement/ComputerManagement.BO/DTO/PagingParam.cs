using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO
{
    public class PagingParam
    {
        public string? KeySearch { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string FieldSort { get; set; } = "UpdateAt";
        public bool SortAsc { get; set; } = false;

        public Dictionary<string, string>? Filters { get; set; }
    }
}
