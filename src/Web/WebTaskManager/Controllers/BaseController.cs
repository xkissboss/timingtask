using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dy.TaskDB;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Dy.TaskCache;

namespace WebTaskManager.Controllers
{
    public abstract class BaseController : Controller
    {
        
        private readonly IServiceProvider provider;
        public BaseController(IServiceProvider provider)
        {
            this.provider = provider;
        }

        private TaskDbContext _dbContext;

        protected TaskDbContext DbContext
        {
            get
            {
                if (_dbContext == null)
                    _dbContext = provider.GetService<TaskDbContext>();
                return _dbContext;
            }
        }


        private ConnectionFactory _factory;

        protected ConnectionFactory ConnectionFactory
        {
            get
            {
                if (_factory == null)
                    _factory = provider.GetService<ConnectionFactory>();
                return _factory;
            }
        }


        private ICache _iCache;

        protected ICache ICache
        {
            get
            {
                if (_iCache == null)
                    _iCache = provider.GetService<ICache>();
                return _iCache;
            }
        }
    }
}
