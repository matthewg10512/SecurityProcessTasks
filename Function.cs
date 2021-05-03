using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using SecurityProcessTasks.DbContexts;
using SecurityProcessTasks.Entities;
using SecurityProcessTasks.Repository;
//dotnet lambda deploy-function SecurityProcessTasks
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SecurityProcessTasks
{


    public class Jobs
    {
        public string jobName { get; set; }
    }


    public class Function
    {

        static string details="";
        public ISecurityRepository _securityRepository { get; }


        /*
        public static Func<IServiceProvider> ConfigureServices = () => {
            var serviceCollection = new ServiceCollection();
            Console.WriteLine("constructor hit");
            serviceCollection.AddTransient(provider =>
            {
                var configService = provider.GetService<IConfigurationService>();
                //var connectionString = configService.GetConfiguration().GetConnectionString(nameof(SecurityContext));
                var connectionString = "data source=s11.winhost.com;initial catalog=DB_81035_kwikkards;persist security info=True;user id=DB_81035_kwikkards_user;password=2toogolleH;MultipleActiveResultSets=True;";
                var optionsBuilder = new DbContextOptionsBuilder<SecurityContext>();
                optionsBuilder.UseSqlServer(connectionString, builder => builder.MigrationsAssembly("NetCoreLambda.EF.Design"));
                return new SecurityContext(optionsBuilder.Options);
            });
            Console.WriteLine("constructor hitting");
            return serviceCollection.BuildServiceProvider();
        };
        */

        public IServiceProvider services;
        public Function()
        {
            var resolver = new DependencyResolver(ConfigureServices);

            _securityRepository = resolver.ServiceProvider.GetService<ISecurityRepository>();
        }

       



        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        
        public async Task<string> FunctionHandler(Jobs job, ILambdaContext context)
        {
            string value = details;
            
            try
            {
                LambdaLogger.Log("jobName: " + job.jobName);
                var task =  _securityRepository.GetTasks(job.jobName);
                if (task != null)
                {
                    LambdaLogger.Log("Running Task: " + task.TaskName );
                    RunAsyncJob(task.TaskUrl);
                    task.LastTaskRun = DateTime.Now;
                    _securityRepository.UpdateTasks(task);
                }
            }
            catch (Exception ex)
            {
                value += ex.Message;
            }

            
            return value;
        }


      
   

        /// <summary>
        /// Makes the call to the job based on the URL for the task
        /// </summary>
        /// <param name="url">URL used to call the API</param>
        private void RunAsyncJob(string url)
        {
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

        }


        // Register services with DI system
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ISecurityRepository, SecurityRepository>();
        }

      
        
     
    }
}
