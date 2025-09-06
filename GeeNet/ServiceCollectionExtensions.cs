using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;


namespace GeeNet
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGeeNet(this IServiceCollection services, Action<GeeNetOptions> options)
        {
            services.Configure(options);

            services.PostConfigure<GeeNetOptions>(options =>
            {
                var serviceAccountFile = options.GoogleServiceAccount;
                var jsonText = File.ReadAllText(serviceAccountFile);
                using var doc = JsonDocument.Parse(jsonText);
                var root = doc.RootElement;

                string projectId = root.GetProperty("project_id").GetString() ?? throw new Exception("project_id not found in service account json file");
                options.ProjectId = projectId;

            });

            services.AddHttpClient<GeeNetClient>(client =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var geeNetOptions = serviceProvider.GetRequiredService<IOptions<GeeNetOptions>>().Value;

                client.BaseAddress = new Uri("https://earthengine.googleapis.com/" + geeNetOptions.ApiVersion + "/");
            });
            return services;
        }
    }
}
