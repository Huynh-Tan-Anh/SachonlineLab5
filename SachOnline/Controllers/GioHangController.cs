using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SachOnline.Models;


namespace SachOnline.Controllers
{
    public class GioHangController : Controller
    {
		private string connection;
		private dbSachOnlineDataContext db;

		public GioHangController()
		{
			// Khởi tạo chuỗi kết nối
			connection = "Data Source=LAPTOP-VRO7LLTN\\SQLEXPRESS;Initial Catalog=SachOnline;Integrated Security=True";
			db = new dbSachOnlineDataContext(connection);
		}

		// GET: GioHang
		public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                //Khởi tạo Giỏ hàng (giỏ hàng chưa tồn tại) 
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;

        }
        public ActionResult ThemGioHang(int ms, string url) 
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.Find(n=>n.iSachID==ms);
            if (sp == null)
            {
                sp = new GioHang(ms);
                lstGioHang.Add(sp);
            }
            else
            {
                sp.iSoLuong++;
            }
            return Redirect(url);
        }

		public ActionResult XoaSPKhoiGioHang(int iMaSach)
		{
			List<GioHang> lstGioHang = LayGioHang();
			GioHang sp = lstGioHang.SingleOrDefault(n => n.iSachID == iMaSach);
			if (sp != null)
			{				
				lstGioHang.RemoveAll(n => n.iSachID == iMaSach);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("SachOnline", "SachOnline");
                }
			}

            return RedirectToAction("GioHang");
		}

		public ActionResult CapNhapGioHang(int ms, FormCollection f)
		{
			List<GioHang> lstGioHang = LayGioHang();
			GioHang sp = lstGioHang.SingleOrDefault(n => n.iSachID == ms);
			if (sp != null)
			{
				string txtSoLuong = f["txtSoLuong"];
				if (!string.IsNullOrEmpty(txtSoLuong) && int.TryParse(txtSoLuong, out int soLuong))
				{
					sp.iSoLuong = soLuong;
				}
			}
			return RedirectToAction("GioHang");
		}

		public ActionResult XoaGioHang()
        {
			List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "SachOnline");
		}

		private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGIoHang = Session["GioHang"] as List <GioHang>;
            if(lstGIoHang != null)
            {
                iTongSoLuong = lstGIoHang.Sum(n=>n.iSoLuong);
            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dTongTien);
            }    
            return dTongTien;
        }

        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "SachOnline");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        public ActionResult GioHangPartial()
        {
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            return PartialView();

        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {				
				return RedirectToAction("DangNhap", "SachOnline");
			}
			if (Session["GioHang"] == null )
            {
                return RedirectToAction("Index", "SachOnline");
            }   

            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            return View(lstGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {

			DONDATHANG ddh = new DONDATHANG();
			KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            
			List<GioHang> lstGioHang = LayGioHang();

            ddh.KhachHangID = kh.KhachHangID;
            ddh.NgayDat = DateTime.Now;

            var NgayGiao = String.Format("{0:MM/dd/yyyy}", f["NgayGiao"]);

            ddh.NgayGiao = DateTime.Parse(NgayGiao);
            ddh.TinhTrangGiaoHang = 1;
            ddh.DaThanhToan = false;

            db.DONDATHANGs.InsertOnSubmit(ddh);
            db.SubmitChanges();

			// Thêm chi tiết đơn hàng
			foreach (var item in lstGioHang)
            {
                CHITIETDATHANG ctdh = new CHITIETDATHANG();

                ctdh.DonDatHangID = ddh.DonDatHangID;
                ctdh.SachID = item.iSachID;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (int)item.dGiaTien;

                db.CHITIETDATHANGs.InsertOnSubmit(ctdh);
            }

            db.SubmitChanges();
            Session["GioHang"] = null;

            return RedirectToAction("XacNhanDonHang", "GioHang");
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }
}