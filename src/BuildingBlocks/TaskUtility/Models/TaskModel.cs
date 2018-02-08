using System;
using System.Collections.Generic;
using System.Text;

namespace Dy.TaskUtility.Models
{
    /// <summary>
    /// 任务
    /// </summary>
    public class TaskModel
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid TaskID { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

        /// <summary>
        /// 任务key，做rabbitmq传送的key用
        /// </summary>
        public string TaskKey { get; set; }


        /// <summary>
        /// 运行频率设置
        /// </summary>
        public string CronExpressionString { get; set; }

        /// <summary>
        /// 任务运频率中文说明
        /// </summary>
        public string CronRemark { get; set; }

        /// <summary>
        /// 禁用或启动
        /// </summary>
        public bool IsActive { get; set; }


        public Guid UID { get; set; }

        /// <summary>
        /// 添加任务人
        /// </summary>
        public UserModel CreateUser { get; set; }


        /// <summary>
        /// 运行的次数
        /// </summary>
        public int RunCount { get; set; }

        /// <summary>
        /// 最后一次运行的时间
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// 是否等待上次执行完毕再执行
        /// </summary>
        public bool IsDce { get; set; }

        public string TaskUrl { get; set; }

    }
}
