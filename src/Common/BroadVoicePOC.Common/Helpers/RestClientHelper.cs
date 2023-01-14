using BroadVoicePOC.Common.Constants.Enums;
using BroadVoicePOC.Common.Extensions;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace BroadVoicePOC.Common.Helpers
{
    public class RestClientHelperConfig
    {
        public int? Timeout { get; set; }
        public IEnumerable<string> RedactFields;
        public int? JsonMaxLength { get; set; }
        public bool ShouldLogRESTCall { get; set; }
        public HTTPLoggingCallType LoggingCallType { get; set; }
        public RestClientHelperConfig() { }
        public RestClientHelperConfig(IConfiguration config)
        {
            int timeout = int.Parse(config["REST_Timeout"]);
            Timeout = timeout > -1 ? timeout : (int?)null;
            RedactFields = config["HTTPLoggingRedactFields"]?.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            JsonMaxLength = int.Parse(config["HTTPLoggingTruncateJSONLength"]);
            HTTPLogging httpLogging = (HTTPLogging)Enum.Parse(typeof(HTTPLogging), config["HTTPLogging"]);
            HTTPLoggingCallType logCallTypes = (HTTPLoggingCallType)Enum.Parse(typeof(HTTPLoggingCallType), config["HTTPLoggingCallType"]);
            ShouldLogRESTCall = httpLogging != HTTPLogging.None && (logCallTypes == HTTPLoggingCallType.Both || logCallTypes == HTTPLoggingCallType.External);
        }
    }

    public class RestClientHelper : IDisposable
    {
        private RestClientHelperConfig _config;
        private const string _redacted = "[Redacted]";

        public RestClientHelper()
        {
            _config = new RestClientHelperConfig()
            {
                Timeout = null,
                RedactFields = null,
                JsonMaxLength = null,
            };
        }

        public RestClientHelper(IConfiguration config) { _config = new RestClientHelperConfig(config); }

        public RestClientHelper(RestClientHelperConfig config) { _config = config; }

        public void Dispose() { }

        private string SerializeRequest(RestSharp.RestClient client, RestRequest request)
        {
            string headers = String.Join(Environment.NewLine, request.Parameters
                .Where(x => x.Type == ParameterType.HttpHeader)
                .Select(x =>
                {
                    bool redact = _config.RedactFields != null && _config.RedactFields.Any(f => x.Name.ToLower().Like(f.ToLower()));
                    return String.Format("{0}: {1}", x.Name, (redact ? _redacted : x.Value));
                }));
            string requestLog = $"Method: {request.Method.ToString()}{Environment.NewLine}{headers}";
            Parameter bodyParam = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.RequestBody);
            if (bodyParam != null && bodyParam.Value != null)
            {
                string body = JsonSerializer.Serialize(bodyParam.Value);
                body = JSONHelper.TruncateAndRedactJSON(body, _config.JsonMaxLength, _config.RedactFields);
                requestLog += $"{Environment.NewLine}Body: {body}";
            }
            return requestLog;
        }

        private string SerializeResponse(RestSharp.RestClient client, RestResponse response)
        {
            string headers = String.Join(Environment.NewLine, response.Headers
                .Select(x =>
                {
                    bool redact = _config.RedactFields != null && _config.RedactFields.Any(f => x.Name.ToLower().Like(f.ToLower()));
                    return String.Format("{0}: {1}", x.Name, (redact ? _redacted : x.Value));
                }));
            string responseLog = $"StatusCode: {response.StatusCode.ToString()}{Environment.NewLine}{headers}";
            if (!String.IsNullOrEmpty(response.Content))
            {
                string body = JSONHelper.TruncateAndRedactJSON(response.Content, _config.JsonMaxLength, _config.RedactFields);
                responseLog += $"{Environment.NewLine}Body: {body}";
            }
            return responseLog;
        }

        public T OAuth2<T>(
            string url,
            string clientId,
            string username,
            string password
        )
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", username);
            request.AddParameter("password", password);
            request.AddParameter("client_id", clientId);

            var response = client.Execute(request);
            if (response == null)
            {
                throw new Exception("HttpResponse is null");
            }
            if (!IsSuccessStatusCode(response.StatusCode))
            {
                throw new Exception($"Error code: {response.StatusCode}.\nContent: {response.Content}.\nError Message: {response.ErrorMessage}.\nError exception: {response.ErrorException}");
            }
            var returnObj = JsonSerializer.Deserialize<T>(response.Content);
            return returnObj;
        }
        public T InvokeWebAPI<T>(
            string url,
            string resource,
            Method method,
            string basicAuthUsername,
            string basicAuthPassword,
            Dictionary<string, string> headers,
            Dictionary<string, string> parameters,
            string contentType,
            object body,
            byte[] file,
            string fileName,
            string fileFormName,
            bool useProxy,
            string proxyHost,
            int proxyPort,
            string proxyDomain,
            string proxyUser,
            string proxyPassword,
            out List<HeaderParameter> ResponseHeaders,
            bool downloadData = false,
            string authToken = null,
            Action<RestRequest> TransformRequest = null,
            Action<string, string, string, double> LogRequest = null
        )
        {
            // invoke backend
            var client = new RestClient(new RestClientOptions()
            {
                BaseUrl = new Uri(url),
                //RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true, // ignore SSL errors
                Proxy = useProxy ? new WebProxy(proxyHost, proxyPort) { Credentials = new NetworkCredential(proxyUser, proxyPassword, proxyDomain) } : null
            });

            if (!String.IsNullOrEmpty(basicAuthUsername) && !String.IsNullOrEmpty(basicAuthPassword))
            {
                client.Authenticator = new HttpBasicAuthenticator(basicAuthUsername, basicAuthPassword);
            }

            if (!String.IsNullOrEmpty(authToken))
            {
                client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", authToken));
            }

            var request = new RestRequest(resource, method);
            if (_config.Timeout.HasValue)
                request.Timeout = _config.Timeout.Value;

            if (!String.IsNullOrEmpty(contentType))
            {
                request.AddHeader("Content-Type", contentType);
            }
            request.AddHeader("Accept", "application/json");

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    if (header.Key != "Accept" && header.Key != "Content-Type") // these are handled separately
                    {
                        request.AddHeader(header.Key, header.Value);
                    }
                }
            }

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    request.AddParameter(parameter.Key, parameter.Value);
                }
            }

            if (body != null)
            {
                if (contentType == "text/plain")
                    request.AddParameter("text/xml", body, ParameterType.RequestBody);
                else
                    request.AddJsonBody(body);
            }

            // TODO: This code should not be in this class, or should be reviewed to be more generic.
            if (file != null)
            {
                //request.AddParameter("application/octet-stream", bodyArray, ParameterType.RequestBody);
                request.AddFile(fileFormName, file, fileName);
            }

            TransformRequest?.Invoke(request);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var response = client.Execute(request);
            stopwatch.Stop();
            if (response == null)
            {
                if (_config.ShouldLogRESTCall)
                    LogRequest?.Invoke(client.BuildUri(request).ToString(), SerializeRequest(client, request), "HttpResponse is null", stopwatch.Elapsed.TotalMilliseconds);
                throw new Exception("HttpResponse is null");
            }

            if (_config.ShouldLogRESTCall)
                LogRequest?.Invoke(client.BuildUri(request).ToString(), SerializeRequest(client, request), SerializeResponse(client, response), stopwatch.Elapsed.TotalMilliseconds);

            if (!IsSuccessStatusCode(response.StatusCode))
            {
                throw new Exceptions.HttpException(response.StatusCode, $"Error code: {response.StatusCode}.\nContent: {response.Content}.\nError Message: {response.ErrorMessage}.\nError exception: {response.ErrorException}");
            }

            ResponseHeaders = response.Headers.ToList();

            if (typeof(T) == typeof(byte[]) && downloadData)
            {
                return (T)Convert.ChangeType(client.DownloadData(request), typeof(T));
            }

            var returnObj = JsonSerializer.Deserialize<T>(response.Content);

            return returnObj;
        }

        public T SendBinary<T>(
            string url,
            string resource,
            Method method,
            string basicAuthUsername,
            string basicAuthPassword,
            string title,
            string fileName,
            string mimeType,
            byte[] file,
            Action<RestRequest> TransformRequest = null,
            Action<string, string, string, double> LogRequest = null
        )
        {
            List<HeaderParameter> ignore;
            return InvokeWebAPI<T>(url, resource, method, basicAuthUsername, basicAuthPassword, null, null, mimeType, null, file, fileName, title, false, null, 0, null, null, null, out ignore, false, null, TransformRequest, LogRequest);
        }

        public T InvokeWebAPI<T>(
            string url,
            string resource,
            Method method,
            string basicAuthUsername,
            string basicAuthPassword,
            Dictionary<string, string> headers,
            Dictionary<string, string> parameters,
            string contentType,
            object body,
            out List<HeaderParameter> responseHeaders,
            //Action<RestRequest> TransformRequest = null,
            bool downloadData = false,
            byte[] file = null,
            string fileName = null,
            string fileFormName = null
        )
        {
            return InvokeWebAPI<T>(url, resource, method, basicAuthUsername, basicAuthPassword, headers, parameters, contentType, body, file, fileName, fileFormName, false, null, 0, null, null, null, out responseHeaders, downloadData);
        }

        public T InvokeWebAPI<T>(
            string url,
            string resource,
            Method method,
            string basicAuthUsername,
            string basicAuthPassword,
            Dictionary<string, string> headers,
            Dictionary<string, string> parameters,
            string contentType,
            object body,
            Action<RestRequest> TransformRequest = null
        )
        {
            List<HeaderParameter> ignore;
            return InvokeWebAPI<T>(url, resource, method, basicAuthUsername, basicAuthPassword, headers, parameters, contentType, body, null, null, null, false, null, 0, null, null, null, out ignore, false, null, TransformRequest);
        }

        public T InvokeWebAPI<T>(
            string url,
            string resource,
            Method method,
            string basicAuthUsername,
            string basicAuthPassword,
            object body,
            Action<RestRequest> TransformRequest = null
        )
        {
            List<HeaderParameter> ignore;
            return InvokeWebAPI<T>(url, resource, method, basicAuthUsername, basicAuthPassword, null, null, null, body, null, null, null, false, null, 0, null, null, null, out ignore, false, null, TransformRequest);
        }

        public T InvokeWebAPIWithToken<T>(
            string url,
            string resource,
            Method method,
            string token,
            Action<string, string, string, double> LogRequest = null
        )
        {
            List<HeaderParameter> ignore;
            return InvokeWebAPI<T>(url, resource, method, null, null, null, null, null, null, null, null, null, false, null, 0, null, null, null, out ignore, false, token, null, LogRequest);
        }

        public T InvokeWebAPIWithToken<T>(
            string url,
            string resource,
            Method method,
            string token,
            object body,
            Action<string, string, string, double> LogRequest = null
        )
        {
            List<HeaderParameter> ignore;
            return InvokeWebAPI<T>(url, resource, method, null, null, null, null, null, body, null, null, null, false, null, 0, null, null, null, out ignore, false, token, null, LogRequest);
        }

        public T InvokeWebAPI<T>(
            string url,
            string resource,
            Method method,
            string basicAuthUsername,
            string basicAuthPassword,
            object body,
            Action<RestRequest> TransformRequest = null,
            Action<string, string, string, double> LogRequest = null
        )
        {
            List<HeaderParameter> ignore;
            return InvokeWebAPI<T>(url, resource, method, basicAuthUsername, basicAuthPassword, null, null, null, body, null, null, null, false, null, 0, null, null, null, out ignore, false, null, TransformRequest, LogRequest);
        }

        public T InvokeWebAPI<T>(
            string url,
            string resource,
            Method method,
            string basicAuthUsername,
            string basicAuthPassword,
            Dictionary<string, string> headers,
            Dictionary<string, string> parameters,
            object body,
            Action<RestRequest> TransformRequest = null
        )
        {
            List<HeaderParameter> ignore;
            return InvokeWebAPI<T>(url, resource, method, basicAuthUsername, basicAuthPassword, headers, parameters, null, body, null, null, null, false, null, 0, null, null, null, out ignore, false, null, TransformRequest);
        }

        public T InvokeWebAPI<T>(
            string url,
            string resource,
            Method method,
            string basicAuthUsername,
            string basicAuthPassword,
            Action<string, string, string, double> LogRequest = null
        )
        {
            List<HeaderParameter> ignore;
            return InvokeWebAPI<T>(url, resource, method, basicAuthUsername, basicAuthPassword, null, null, null, null, null, null, null, false, null, 0, null, null, null, out ignore, false, null, null, LogRequest);
        }

        public T InvokeWebAPI<T>(
            string url,
            string resource,
            Method method,
            string token,
            Dictionary<string, string> headers,
            Dictionary<string, string> parameters,
            string contentType,
            object body,
            out List<HeaderParameter> responseHeaders,
            bool downloadData = false,
            byte[] file = null,
            string fileName = null,
            string fileFormName = null
        )
        {
            return InvokeWebAPI<T>(url, resource, method, null, null, headers, parameters, contentType, body, file, fileName, fileFormName, false, null, 0, null, null, null, out responseHeaders, downloadData, token);
        }

        private static bool IsSuccessStatusCode(HttpStatusCode statusCode)
        {
            return (int)statusCode >= 200 && (int)statusCode <= 299;
        }
    }
}
