using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting;
using StatelessBackendService.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatelessBackendService.Interfaces
{
    public interface IQueueAdapter: IService
    {
        Task<bool> Push(Message message);
        Task<Message> Pop();
    }
}
