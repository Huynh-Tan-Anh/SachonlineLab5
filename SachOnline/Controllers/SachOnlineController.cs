using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using SachOnline.Models;
using PagedList;
using PagedList.Mvc;

namespace SachOnline.Controllers
{
    public class SachOnlineController : Controller
    {
        private string connection;
        private dbSachOnlineDataContext data;

        private List<SACH> LaySachMoi(int count)
        {
            return data.SACHes.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        public SachOnlineController()
        {
            // Khởi tạo chuỗi kết nối
            connection = "Data Source=LAPTOP-VRO7LLTN\\SQLEXPRESS;Initial Catalog=SachOnline;Integrated Security=True";
            data = new dbSachOnlineDataContext(connection);
        }

        // GET: SachOnline
        public ActionResult Index()
        {
            var listSachMoi = LaySachMoi(6);
            return View(listSachMoi);
        }
        public ActionResult ChuDePartial()
        {
            var listChuDe = from cd in data.CHUDEs select cd;
            return PartialView(listChuDe);
        }
        public ActionResult NhaXuatBanPartial()
        {
            var listNhaXuatBan = from cd in data.NHAXUATBANs select cd;
            return PartialView(listNhaXuatBan);
        }
        public ActionResult SachBanNhieuPartial()
        {
			// Truy vấn cơ sở dữ liệu để lấy danh sách sách bán nhiều nhất.
			var listSachBanNhieu = data.SACHes.OrderByDescending(a => a.SoLuong).Take(6).ToList();

			// Trả về PartialView chứa danh sách sách bán nhiều nhất.
			return PartialView("SachBanNhieuPartial", listSachBanNhieu);
		}

		private string LayTenChuDe(int id)
		{
			var chuDe = data.CHUDEs.SingleOrDefault(cd => cd.ChuDeID == id);

			if (chuDe != null)
			{
				return chuDe.TenChuDe;
			}

			return "Không tìm thấy chủ đề";
		}
		private string LayTenNhaXuatBan(int id)
		{
			var nhaxuatban = data.NHAXUATBANs.SingleOrDefault(cd => cd.NhaXuatBanID == id);

			if (nhaxuatban != null)
			{
				return nhaxuatban.TenNhaXuatBan;
			}

			return "Không tìm thấy nhà xuất bản";
		}

		public ActionResult SachTheoChuDe(int id, int? page)
		{
			int pageSize = 3;
			int pageNumber = page ?? 1;

			var sach = data.SACHes.Where(s => s.ChuDeID == id)
								  .OrderByDescending(s => s.NgayCapNhat)
								  .ToPagedList(pageNumber, pageSize);

			ViewBag.Id = id;
			ViewBag.ten = LayTenChuDe(id);
			return View(sach);
		}

		public ActionResult SachTheoNhaXuatBan(int id, int? page)
        {
			int pageSize = 3;
			int pageNumber = page ?? 1;

			var sach = data.SACHes.Where(s => s.NhaXuatBanID == id)
								  .OrderByDescending(s => s.NgayCapNhat)
								  .ToPagedList(pageNumber, pageSize);

			ViewBag.Id = id;
			ViewBag.ten = LayTenNhaXuatBan(id);
			return View(sach);
		}

        public ActionResult ChiTietSach(int id)
        {
            var sach = from s in data.SACHes
                       where s.SachID == id
                       select s;

            return View(sach.Single());
        }

		[HttpGet]
		public ActionResult TimKiem(string tuKhoa, int? page)
		{
			ViewBag.tuKhoa = tuKhoa;
			int pageSize = 20;
			int pageNumber = (page ?? 1);

			var sachTimKiem = data.SACHes
				.Where(s => s.TenSach.Contains(tuKhoa))
				.OrderByDescending(s => s.NgayCapNhat)
				.ToPagedList(pageNumber, pageSize);

			return View(sachTimKiem);
		}

		public ActionResult AddToCart(int id)
        {
            List<int> cart = Session["Cart"] as List<int>;
            if (cart == null)
            {
                cart = new List<int>();
            }
            cart.Add(id);
            Session["Cart"] = cart;
            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";
            return RedirectToAction("Index", "SachOnline");
        }

        [HttpGet] // Đánh dấu action chỉ được xử lý dưới phương thức GET
        public ActionResult DangKy()
        {
            return View(); // Trả về view cho action "DangKy"
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KHACHHANG kh)
        {
            var sTenKhachHang = collection["TenKhachHang"];
            var sDiaChi = collection["DiaChi"];
            var sTenDN = collection["TenDN"];
            var sMatkhau = collection["Matkhau"];
            var sMatkhauNhapLai = collection["MatKhauNL"];
            var sDiachi = collection["DiaChi"];
            var sEmail = collection["Email"];
            var sSoDienThoai = collection["SoDienThoai"];
            if (String.IsNullOrEmpty(sTenKhachHang))
            {
                ViewData["erro1"] = "Họ tên không được rỗng";
            }
            else if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["err2"] = "Tên đăng nhập không được rỗng";
            }

            else if (String.IsNullOrEmpty(sMatkhau))
            {
                ViewData["err3"] = "Phải nhập mật khẩu";
            }

            else if (String.IsNullOrEmpty(sMatkhauNhapLai))
            {
                ViewData["err4"] = "Phải nhập lại mật khẩu";
            }

            else if (sMatkhau != sMatkhauNhapLai)
            {
                ViewData["err4"] = "MK nhập lại không khớp";
            }



            else if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err5"] = "Email không được rỗng";
            }



            else if (String.IsNullOrEmpty(sSoDienThoai))
            {
                ViewData["err6"] = "Số điện thoại không được rỗng";
            }

            else if (data.KHACHHANGs.SingleOrDefault(n => n.TenDN == sTenDN) != null)
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại";
            }

            else if (data.KHACHHANGs.SingleOrDefault(n => n.Email == sEmail) != null)
            {
                ViewBag.ThongBao = "Email này đã được sử dụng";
            }

            else
            {
                //Gần giá trị cho đối tượng được tạo mới (kh)
                kh.TenKhachHang = sTenKhachHang;
                kh.TenDN = sTenDN;
                kh.MatKhau = sMatkhau;
                kh.Email = sEmail;
                kh.DiaChi = sDiachi;
                kh.SoDienThoai = sSoDienThoai;
                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("DangNhap");
            }
            if (ModelState.IsValid)
            {
                // Nếu đăng ký thành công, bạn có thể chuyển hướng đến trang khác hoặc hiển thị thông báo thành công.
                // Ví dụ:
                TempData["SuccessMessage"] = "Đăng ký thành công!";
                return RedirectToAction("Index", "SachOnline");
            }
            return RedirectToAction("DangKy", new { collection = collection });

        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var sTenDN = collection["TenDN"];
            var sMatkhau = collection["Matkhau"];
            if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["Err1"] = "Vui lòng nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(sMatkhau))
            {
                ViewData["Err2"] = "Vui lòng nhập mật khẩu";
            }
            else
            {
                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.TenDN == sTenDN && n.MatKhau == sMatkhau);
                if (kh != null)
                {
                    ViewBag.ThongBao = "Đăng nhập thành công vào hệ thống";
                    Session["TaiKhoan"] = kh;
					
                        return RedirectToAction("DatHang", "GioHang"); 
                   
					
				}
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không hợp lệ";
                }
                
            }
			return View();

        }




    }
}