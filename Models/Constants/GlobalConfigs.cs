using System;
using Models.ViewModels.Config;

namespace Models.Constants
{
    public class GlobalConfigs : GlobalConfigViewModel
    {
        public DateTimeOffset LastModified { get; set; }

        public void Update(GlobalConfigViewModel viewModel)
        {
            Theme = viewModel.Theme;
            EmailTestMode = viewModel.EmailTestMode;
            LastModified = DateTimeOffset.Now;
        }
    }
}