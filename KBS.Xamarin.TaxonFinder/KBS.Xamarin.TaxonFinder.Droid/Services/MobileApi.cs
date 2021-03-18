using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using KBS.App.TaxonFinder.Droid.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(MobileApi))]
namespace KBS.App.TaxonFinder.Droid.Services
{
    public class MobileApi : IMobileApi
    {

        private HttpClient _client = new HttpClient() { MaxResponseContentBufferSize = 256000 };

        private static readonly string _serviceUrl = "https://www.idoweb.bodentierhochvier.de/api/";
        private static readonly string _loginUrl = $@"{_serviceUrl}ApplicationUser/Login/Mobile";
        private static readonly string _registerUrl = $@"{_serviceUrl}ApplicationUser/Register/Mobile";
        private static readonly string _adviceServiceUrl_mobile = $@"{_serviceUrl}Advice/SaveAdvice/Mobile";
        private static readonly string _changesServiceUrl = $@"{_serviceUrl}Advice/SyncAdvices";
        private static readonly string _mailServiceUrl = $@"{_serviceUrl}values/Mail/SendFeedback";
        private static RecordDatabase _database;

        public MobileApi()
        {

        }

        public static RecordDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new RecordDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("RecordSQLite.db3"));
                }
                return _database;
            }
        }

        public async Task<string> Register(string userName, string password)
        {
            LoginModel contentJson = new LoginModel { username = userName, password = password, deviceId = DependencyService.Get<IDeviceId>().GetDeviceId() };
            StringContent content = new StringContent(JsonConvert.SerializeObject(contentJson), Encoding.UTF8, "application/json");
            try
            {
                /**TODO: replace SSL Validation **/
                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    MaxRequestContentBufferSize = 256000
                };
                _client = new HttpClient(handler);
                var response = await _client.PostAsync(_loginUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var response_content = await response.Content.ReadAsStringAsync();
                    if (response_content.Contains('"'))
                    {
                        response_content = JsonConvert.DeserializeObject(response_content) as string;
                    }
                    return response_content;
                }
                throw new Exception("Anmeldung fehlgeschlagen.");
            }
            catch (Exception e)
            {
                var exc = e.InnerException;
                throw new Exception("Anmeldung fehlgeschlagen.");
            }
        }

        public async Task<string> AddNewUser(string givenname, string surname, string mail, string password, string comment, string source)
        {
            try
            {
                RegisterModel contentJson = new RegisterModel { givenname = givenname, surname = surname, mail = mail, password = password, comment = comment, source = source };
                /**TODO: replace SSL Validation **/
                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    MaxRequestContentBufferSize = 256000
                };

                _client = new HttpClient(handler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(contentJson), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(_registerUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var response_content = await response.Content.ReadAsStringAsync();
                    return response_content;
                }
                return "Registrierung fehlgeschlagen.";
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<string> GetChangesByDevice(AuthorizationJson auth)
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                MaxRequestContentBufferSize = 256000,
            };
            var httpClient = new HttpClient(handler);
            var stringContent = new StringContent(JsonConvert.SerializeObject(auth), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(_changesServiceUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            throw new Exception("Übermittlung fehlgeschlagen.");
        }

        public async Task<string> SaveAdvicesByDevice(AdviceJsonItem[] adviceJsonItem)
        {
            var userName = Database.GetUserName();
            adviceJsonItem[0].UserName = userName;

            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                MaxRequestContentBufferSize = 256000,
            };
            _client = new HttpClient(handler);
            var stringContent = new StringContent(JsonConvert.SerializeObject(adviceJsonItem[0]), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_adviceServiceUrl_mobile, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            throw new Exception("Übermittlung fehlgeschlagen.");

        }
        public async Task<string> SendFeedback(string text, string mail)
        {
            if (mail == null) { mail = ""; }
            Uri feedbackUri = new Uri(string.Format("{0}?text={1}&adress={2}", _mailServiceUrl, Uri.EscapeDataString(text), Uri.EscapeDataString(mail)));

            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                MaxRequestContentBufferSize = 256000,
            };
            _client = new HttpClient(handler);
            var response = await _client.GetAsync(feedbackUri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }

            throw new Exception("Übermittlung fehlgeschlagen.");
        }
    }


    public class RegisterModel
    {
        public string givenname { get; set; }
        public string surname { get; set; }
        public string mail { get; set; }
        public string password { get; set; }
        public string comment { get; set; }
        public string source { get; set; }
    }
    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string deviceId { get; set; }
    }

    public class TokenResponse
    {
        public string token { get; set; }
    }

}