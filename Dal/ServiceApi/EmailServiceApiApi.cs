using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dal.Configs;
using DAL.Interfaces;
using Mailjet.Client;
using Microsoft.Extensions.Logging;
using Models.Constants;
using RestSharp;
using RestSharp.Authenticators;

namespace DAL.ServiceApi
{
    public class EmailServiceApi : IEmailServiceApi
    {
        private readonly bool _connected;

        private readonly ILogger<EmailServiceApi> _logger;
        
        private readonly IMailjetClient _mailJetClient;

        private readonly GlobalConfigs _globalConfigs;
        
        private readonly MailGunConfig _mailGunConfig;

        public EmailServiceApi()
        {
            _connected = false;
        }

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mailJetClient"></param>
        /// <param name="globalConfigs"></param>
        /// <param name="mailGunConfig"></param>
        public EmailServiceApi(ILogger<EmailServiceApi> logger, IMailjetClient mailJetClient, GlobalConfigs globalConfigs, MailGunConfig mailGunConfig)
        {
            _connected = true;
            _logger = logger;
            _mailJetClient = mailJetClient;
            _globalConfigs = globalConfigs;
            _mailGunConfig = mailGunConfig;
        }

        /// <summary>
        /// Send the email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="emailSubject"></param>
        /// <param name="emailHtml"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string emailAddress, string emailSubject, string emailHtml)
        {
            if (_connected && !string.IsNullOrWhiteSpace(emailAddress))
            {
                var client = new RestClient
                {
                    BaseUrl = new Uri("https://api.mailgun.net/v3"),
                    Authenticator = new HttpBasicAuthenticator("api", _mailGunConfig.ApiKey)
                };
                var request = new RestRequest();
                request.AddParameter("domain", _mailGunConfig.Domain, ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", ApiConstants.SiteEmail);
                request.AddParameter("to", emailAddress);
                request.AddParameter("subject", emailSubject);
                request.AddParameter("html", emailHtml);
                request.Method = Method.POST;
                var response = await client.ExecuteAsync(request);
                
                _logger.LogTrace(response.Content);

                /*var task = Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(async _ =>
                {
                    var emailList = new JArray
                    {
                        new JObject {{"Email", ApiConstants.SiteEmail}}
                    };

                    // If email test mode is not True, then add recipient
                    if (!_globalConfigs.EmailTestMode)
                    {
                        emailList.Add(new JObject {{"Email", emailAddress}});
                    }

                    var request = new MailjetRequest {Resource = Send.Resource}
                        .Property(Send.FromEmail, "admin@anahita.dev")
                        .Property(Send.FromName, "Anahita.dev")
                        .Property(Send.Subject, emailSubject)
                        .Property(Send.HtmlPart, emailHtml)
                        // CC to ...
                        .Property(Send.Cc, ApiConstants.SiteEmail)
                        .Property(Send.Recipients, emailList);

                    await _mailJetClient.PostAsync(request);
                });

                await task;*/
            }
        }

        /// <summary>
        /// Send the email
        /// </summary>
        /// <param name="emailAddresses"></param>
        /// <param name="emailSubject"></param>
        /// <param name="emailHtml"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(IEnumerable<string> emailAddresses, string emailSubject, string emailHtml)
        {
            var tasks = emailAddresses.Select(x => SendEmailAsync(x, emailSubject, emailHtml)).ToArray();

            await Task.WhenAll(tasks);
        }
    }
}