using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SachOnline.Models
{
    public class GioHang
    {
        dbSachOnlineDataContext db;
        public int iSachID { get; set; }
        public string sTenSach { get; set; }
        public string sAnhSP { get; set; }
        public double dGiaTien { get; set; }
        public int iSoLuong { get; set; }
        public double dTongTien
        {
            get { return iSoLuong * dGiaTien; }

        }

		public GioHang(int ms)
		{
			iSachID = ms;
			db = new dbSachOnlineDataContext("Data Source=LAPTOP-VRO7LLTN\\SQLEXPRESS;Initial Catalog=SachOnline;Integrated Security=True");

			SACH s = db.SACHes.Single(n => n.SachID == iSachID);
			sTenSach = s.TenSach;
			sAnhSP = s.anhSP;
			dGiaTien = double.Parse(s.GiaBan.ToString());
			iSoLuong = 1;
		}

	}
}