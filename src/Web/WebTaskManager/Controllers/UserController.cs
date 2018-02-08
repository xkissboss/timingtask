using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dy.TaskUtility.Models;
using WebTaskManager.Utils;

namespace WebTaskManager.Controllers
{
    public class UserController : BaseController
    {
        public UserController(IServiceProvider provider) : base(provider)
        {
        }

        // GET: User
        public ActionResult Index()
        {
            List<UserModel> userList = DbContext.User.ToList();
            ViewBag.UserList = userList;
            return View();
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind("Name,Email")] UserModel user)
        {
            if (!ModelState.IsValid)
                return APIReturn.BuildFail("数据验证不通过");
            user.CreateTime = DateTime.Now;
            user.IsActive = true;
            await DbContext.User.AddAsync(user);
            DbContext.SaveChanges();
            return APIReturn.BuildSuccess("添加成功");
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}