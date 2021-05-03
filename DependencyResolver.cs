using Microsoft.Extensions.DependencyInjection;
using SecurityProcessTasks.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using SecurityProcessTasks.Repository;
using System.Threading.Tasks;
using Amazon.KeyManagementService;
using Amazon;
using Amazon.KeyManagementService.Model;
using System.IO;

namespace SecurityProcessTasks
{
    class DependencyResolver
    {
        public IServiceProvider ServiceProvider { get; }
        public string CurrentDirectory { get; set; }
        public Action<IServiceCollection> RegisterServices { get; }
        

        public DependencyResolver(Action<IServiceCollection> registerServices = null)
        {
            
            // Set up Dependency Injection
            var serviceCollection = new ServiceCollection();
            RegisterServices = registerServices;
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
       
            
            //var  _securityRepository = ServiceProvider.GetService<ISecurityRepository>();
        
            
            
             //var SecurityTask =  _securityRepository.GetTasks("SecurityUpdate");
            //if (SecurityTask != null)
            //{
              //  testingMatt += SecurityTask.TaskUrl;
            //}
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register env and config services
            
            services.AddTransient<IEnvironmentService, EnvironmentService>();
            services.AddTransient<IConfigurationService, ConfigurationService>
                (provider => new ConfigurationService(provider.GetService<IEnvironmentService>())
                {
                    CurrentDirectory = CurrentDirectory
                });

            // Register DbContext class
            
            services.AddTransient(provider =>
            {
                var configService = provider.GetService<IConfigurationService>();
                var connectionString = configService.GetConfiguration().GetConnectionString(nameof(SecurityContext));
                string pwd = DecodeEnvVar("PASSWORD").Result;
                connectionString = connectionString.Replace("EnvironmentPassword", pwd);
                var optionsBuilder = new DbContextOptionsBuilder<SecurityContext>();
                optionsBuilder.UseSqlServer(connectionString, builder => builder.MigrationsAssembly("NetCoreLambda.EF.Design"));
                return new SecurityContext(optionsBuilder.Options);
            });

            

           // services.AddDbContext<SecurityContext>(options => options.UseSqlServer("data source=s11.winhost.com;initial catalog=DB_81035_kwikkards;persist security info=True;user id=DB_81035_kwikkards_user;password=2toogolleH;MultipleActiveResultSets=True;"));

            //testingMatt = "Testing Matt here there everywhere";

            // Register other services
            RegisterServices?.Invoke(services);
        }


        /// <summary>
        /// Used to Decrypt any records that are Encrypted in AWS
        /// </summary>
        /// <param name="envVarName">The Enviroment Variable Name</param>
        /// <returns></returns>
        private static async Task<string> DecodeEnvVar(string envVarName)
        {
            // Retrieve env var text
            var encryptedBase64Text = Environment.GetEnvironmentVariable(envVarName);
            // Convert base64-encoded text to bytes
            var encryptedBytes = Convert.FromBase64String(encryptedBase64Text);
            // Set up encryption context
            var encryptionContext = new Dictionary<string, string>();
            encryptionContext.Add("LambdaFunctionName",
                    Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME"));
            // Construct client
            using (var client = new AmazonKeyManagementServiceClient(RegionEndpoint.GetBySystemName("us-east-2")))
            {
                // Construct request
                var decryptRequest = new DecryptRequest
                {
                    CiphertextBlob = new MemoryStream(encryptedBytes),
                    EncryptionContext = encryptionContext,
                };
                // Call KMS to decrypt data
                var response = await client.DecryptAsync(decryptRequest);
                using (var plaintextStream = response.Plaintext)
                {
                    // Get decrypted bytes
                    var plaintextBytes = plaintextStream.ToArray();
                    // Convert decrypted bytes to ASCII text
                    var plaintext = Encoding.UTF8.GetString(plaintextBytes);
                    return plaintext;
                }
            }
        }


    }
}
