// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace StatefulBackendService
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using StatelessBackendService.Interfaces.Models;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Data.Collections;
    using System.Threading.Tasks;
    using StatelessBackendService.Interfaces;


    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class StatefulBackendService : StatefulService, IQueueAdapter
    {
        internal const string messageQueue = "putMessageQueue";
        public StatefulBackendService(StatefulServiceContext context)
            : base(context)
        {
        }

        public async Task<Message> Pop()
        {
            ConditionalValue<Message> message = new ConditionalValue<Message>();
            IReliableQueue<Message> messageQueue = await this.StateManager.GetOrAddAsync<IReliableQueue<Message>>(StatefulBackendService.messageQueue);
            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                message = await messageQueue.TryDequeueAsync(tx);
                await tx.CommitAsync();
            }
            return message.Value;
        }

        public async Task<bool> Push(Message message)
        {
            IReliableQueue<Message> messageQueue = await this.StateManager.GetOrAddAsync<IReliableQueue<Message>>(StatefulBackendService.messageQueue);
            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                await messageQueue.EnqueueAsync(tx, message);
                await tx.CommitAsync();                
            }
            return true;
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                new ServiceReplicaListener(context =>
                    this.CreateServiceRemotingListener(context), "rdp", false),
                new ServiceReplicaListener(
                    serviceContext =>
                        new KestrelCommunicationListener(
                            serviceContext,
                            (url, listener) =>
                            {
                                ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                                return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<IReliableStateManager>(this.StateManager)
                                            .AddSingleton<StatefulServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                    .UseStartup<Startup>()
                                    .UseUrls(url)
                                    .Build();
                            }), "kestrel", false)
            };
        }

        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    IReliableQueue<string> messageQueue = await this.StateManager.GetOrAddAsync<IReliableQueue<string>>("messageQueue");
        //    const string baseAddress = "http://localhost:8864";

        //    using (Microsoft.Owin.Hosting.WebApp.Start<Startup>(baseAddress))
        //    {
        //        HttpClient client = new HttpClient();
        //        HttpResponseMessage res = client.GetAsync(baseAddress + "api/values/5").Result;
        //        res.EnsureSuccessStatusCode();
        //        var results = res.Content.ReadAsStringAsync().Result;
        //        using (ITransaction tx = this.StateManager.CreateTransaction())
        //        {

        //            await messageQueue.EnqueueAsync(tx, results);
        //            await tx.CommitAsync();

        //        }
        //    }




        //}
    }
}