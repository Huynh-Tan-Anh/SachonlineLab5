
using SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SachOnline.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
		private string connection;
		dbSachOnlineDataContext db;

		public AdminController()
		{
			// Khởi tạo chuỗi kết nối
			connection = "Data Source=LAPTOP-VRO7LLTN\\SQLEXPRESS;Initial Catalog=SachOnline;Integrated Security=True";
			db = new dbSachOnlineDataContext(connection);
		}

		// GET: Admin/Home
		public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        { return View(); }

        [HttpPost] 
        public ActionResult Login(FormCollection f)
        {
            var sTenDN = f["UserName"];
            var sMatKhau = f["Password"];

            ADMIN ad = db.ADMINs.SingleOrDefault(n=> n.Username == sTenDN && n.Password == sMatKhau);
            if (ad == null)
            {
                Session["Admin"] = ad;
                return RedirectToAction("Index","Admin");
            }
            else
            {
                ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng ";
                return View();
            }
        }
    }
}