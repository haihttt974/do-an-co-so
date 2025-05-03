using System.Collections.Generic;

namespace doan3.ViewModel
{
    public class UserLichHocViewModel
    {
        public List<KhoaHocLichHocViewModel> KhoaHocLichHocs { get; set; }
        public string FilterStatus { get; set; }
    }
}