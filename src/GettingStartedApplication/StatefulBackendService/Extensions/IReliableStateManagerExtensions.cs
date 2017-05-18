using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using StatelessBackendService.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatefulBackendService
{
    public static class IReliableStateManagerExtensions
    {

        public async static Task<IReliableQueue<Message>> GetMessageQueue(this IReliableStateManager stateManager)
        {
            return await stateManager.GetOrAddAsync<IReliableQueue<Message>>(StatefulBackendService.messageQueue);
        }
    }
}
