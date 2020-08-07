using Models.Constants;

namespace Api.Utilities
{
    public class CacheBustingUtility
    {
        private readonly GlobalConfigs _globalConfigs;

        public CacheBustingUtility(GlobalConfigs globalConfigs)
        {
            _globalConfigs = globalConfigs;
        }
        
        public long CacheBustingKey()
        {
            return AssemblyInfo.AssemblyVersion.GetHashCode() + _globalConfigs.LastModified.Ticks;
        }
    }
}