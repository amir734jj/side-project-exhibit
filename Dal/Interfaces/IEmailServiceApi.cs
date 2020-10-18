using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dal.Interfaces
{
    public interface IEmailServiceApi
    {
        Task SendEmailAsync(string emailAddress, string emailSubject, string emailHtml);

        Task SendEmailAsync(IEnumerable<string> emailAddresses, string emailSubject, string emailHtml);
    }
}