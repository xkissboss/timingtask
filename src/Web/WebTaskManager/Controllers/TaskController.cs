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
    public class TaskController : BaseController
    {
        public TaskController(IServiceProvider provider) : base(provider)
        {
        }

        // GET: Task
        public ActionResult Index()
        {
            return View();
        }

        // GET: Task/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Task/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Task/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind("TaskName,TaskDesc,CronExpressionString,CronRemark,UID,TaskKey,IsDce,TaskUrl")]TaskModel task)
        {
            task.IsActive = true;
            await DbContext.Task.AddAsync(task);
            await DbContext.SaveChangesAsync();
            ICache.SetList<TaskModel>("task_list", delegate ()
            {
                return DbContext.Task.ToArray();
            });
            RabbitMQHelper.PublishMessage(ConnectionFactory, GlobalVariable.EXCHANGE, task.TaskKey, task.TaskID.ToString());
            return APIReturn.BuildSuccess("添加成功");
        }

        // GET: Task/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Task/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string taskId, [Bind("taskId,TaskName,TaskDesc,CronExpressionString,CronRemark,IsDce,TaskUrl,IsActive")]TaskModel task)
        {
            TaskModel taskModel = await DbContext.Task.FindAsync(Guid.Parse(taskId));
            if (taskModel == null)
                return APIReturn.BuildFail("记录不存在");
            taskModel.TaskName = task.TaskName;
            taskModel.TaskDesc = task.TaskDesc;
            taskModel.CronExpressionString = task.CronExpressionString;
            taskModel.CronRemark = task.CronRemark;
            taskModel.IsDce = task.IsDce;
            taskModel.TaskUrl = task.TaskUrl;
            taskModel.IsActive = task.IsActive;
            DbContext.Task.Update(taskModel);
            await DbContext.SaveChangesAsync();
            ICache.SetList<TaskModel>("task_list", delegate ()
            {
                return DbContext.Task.ToArray();
            });
            RabbitMQHelper.PublishMessage(ConnectionFactory, GlobalVariable.EXCHANGE, taskModel.TaskKey, taskModel.TaskID.ToString());
            return APIReturn.BuildSuccess("修改成功");
        }

        // GET: Task/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Task/Delete/5
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