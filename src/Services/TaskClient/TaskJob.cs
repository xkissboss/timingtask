using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskClient
{
    public class TaskJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string url = dataMap.GetString("url");
            await Console.Error.WriteLineAsync( DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "key: " + key + " url:" + url);

        }
    }
}
