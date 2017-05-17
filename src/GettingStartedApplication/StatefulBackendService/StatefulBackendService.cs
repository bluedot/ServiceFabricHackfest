// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace StatefulBackendService
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Microsoft.ServiceFabric.Data.Collections;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.Owin.Hosting;

    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class StatefulBackendService : StatefulService
    {
        internal const string messageQueue = "putMessageQueue";
        public StatefulBackendService(StatefulServiceContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
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
                            }))
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            IReliableQueue<string> messageQueue = await this.StateManager.GetOrAddAsync<IReliableQueue<string>>("messageQueue");
            const string baseAddress = "http://localhost:8864";
            using (Microsoft.Owin.Hosting.WebApp.Start<Startup>(baseAddress))
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage res = client.GetAsync(baseAddress + "api/values/5").Result;
                res.EnsureSuccessStatusCode();
                var results = res.Content.ReadAsStringAsync().Result;
                using (ITransaction tx = this.StateManager.CreateTransaction())
                {

                    await messageQueue.EnqueueAsync(tx, results);
                    await tx.CommitAsync();

                }
            }

              
             

        }
    }
}