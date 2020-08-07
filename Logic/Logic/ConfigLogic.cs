using System;
using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;
using Dal.Extensions;
using Dal.Interfaces;
using Logic.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Constants;
using Models.ViewModels.Config;
using static Models.Constants.ApplicationConstants;

namespace Logic.Logic
{
    public class ConfigLogic : IConfigLogic
    {
        private readonly GlobalConfigs _globalConfigs;
        
        private readonly IS3Service _s3Service;

        private readonly ILogger<ConfigLogic> _logger;

        public ConfigLogic(GlobalConfigs globalConfigs, IS3Service s3Service, ILogger<ConfigLogic> logger)
        {
            _globalConfigs = globalConfigs;
            _s3Service = s3Service;
            _logger = logger;
        }

        public GlobalConfigViewModel ResolveGlobalConfig()
        {
            return _globalConfigs;
        }

        public async Task UpdateGlobalConfig(GlobalConfigViewModel globalConfigViewModel)
        {
            _globalConfigs.Update(globalConfigViewModel);

            var response = await _s3Service.Upload(ConfigFile, globalConfigViewModel.ObjectToByteArray(),
                ImmutableDictionary.Create<string, string>().Add("Description", "Application config file"));

            if (response.Status == HttpStatusCode.BadRequest)
            {
                _logger.LogError("Failed to sync config file with S3");
            }
        }

        public async Task Refresh()
        {
            var response = await _s3Service.Download(ConfigFile);
            
            if (response.Status == HttpStatusCode.OK)
            {
                _logger.LogInformation("Successfully fetched the config from S3");

                var globalConfigViewModel = response.Data.Deserialize<GlobalConfigViewModel>();
                
                _globalConfigs.Update(globalConfigViewModel);
            }
            else
            {
                _logger.LogError("Failed to fetch the config from S3");
            }
        }
    }
}