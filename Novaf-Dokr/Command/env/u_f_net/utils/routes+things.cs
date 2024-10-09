using RestSharp.Authenticators;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading;
using System.IO;

namespace Novaf_Dokr.Command.env.u_f_net.utils
{
    public class routes_things
    {
        public static void HandlReqRequest(string[] parts)
        {
            switch (parts[2].ToLower())
            {
                case "help":
                    u_f_net.fri3ndly_network.ShowHelp("req");
                    break;
                case "list":
                    foreach (var xyz in PossibleRequests.Get())
                    {
                        PossibleRequests.Write(xyz);
                    }
                    break;
                case "get":
                case "post":
                case "put":
                case "delete":
                    ProcessHttpRequest(parts);
                    break;
                default:
                    Console.WriteLine("Unknown command. Use 'help' for available options.");
                    break;
            }
        }

        private static void ProcessHttpRequest(string[] parts)
        {
            if (parts.Length < 4)
            {
                Console.WriteLine("Invalid command. Usage: @fnet req [method] [url] [options]");
                return;
            }

            string method = parts[2].ToLower();
            string url = parts[3];
            bool showContent = parts.Contains("--show-content");
            int threads = 1;
            int retries = 0;

            for (int i = 4; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("--threads"))
                {
                    if (i + 1 < parts.Length && int.TryParse(parts[i + 1], out int threadCount))
                    {
                        threads = threadCount;
                        i++;
                    }
                }
                else if (parts[i].StartsWith("--retry-on-fail"))
                {
                    if (i + 1 < parts.Length && int.TryParse(parts[i + 1], out int retryCount))
                    {
                        retries = retryCount;
                        i++;
                    }
                }
            }

            var tasks = new List<Task>();
            for (int i = 0; i < threads; i++)
            {
                tasks.Add(Task.Run(() => ExecuteRequest(method, url, showContent, retries)));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static void ExecuteRequest(string method, string url, bool showContent, int retries)
        {
            for (int attempt = 0; attempt <= retries; attempt++)
            {
                try
                {
                    var result = ReqProcedure.ExecuteRequest(method, url);
                    ReqProcedure.WriteResult(result, showContent);
                    break;
                }
                catch (Exception ex)
                {
                    if (attempt == retries)
                    {
                        Console.WriteLine($"Request failed after {retries + 1} attempts: {ex.Message}");
                    }
                    else
                    {
                        Thread.Sleep(1000); // Wait 1 second before retrying
                    }
                }
            }
        }

        public class ReqProcedure
        {
            private static readonly HttpClient _httpClient = new HttpClient();

            public static RequestResult ExecuteRequest(string method, string url)
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "http://" + url;
                }

                try
                {
                    HttpResponseMessage response;
                    switch (method)
                    {
                        case "get":
                            response = _httpClient.GetAsync(url).Result;
                            break;
                        case "post":
                            response = _httpClient.PostAsync(url, null).Result;
                            break;
                        case "put":
                            response = _httpClient.PutAsync(url, null).Result;
                            break;
                        case "delete":
                            response = _httpClient.DeleteAsync(url).Result;
                            break;
                        default:
                            throw new ArgumentException("Invalid HTTP method");
                    }

                    return new RequestResult
                    {
                        Url = url,
                        RequestCode = (int)response.StatusCode,
                        Content = response.Content.ReadAsStringAsync().Result
                    };
                }
                catch (Exception ex)
                {
                    return new RequestResult
                    {
                        Url = url,
                        RequestCode = 999,
                        Content = $"ERROR: {ex.Message}"
                    };
                }
            }

            public static void WriteResult(RequestResult result, bool showContent)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("* ");

                if (result.RequestCode == 200)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.WriteLine($"{result.Url} --> {result.RequestCode}");

                if (showContent)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("* ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{result.Content} \n");
                }
            }

            public class RequestResult
            {
                public int RequestCode { get; set; }
                public string Content { get; set; }
                public string Url { get; set; }
            }
        }

        public class PossibleRequests
        {
            public static List<Req> Get()
            {
                List<Req> list = new List<Req>();

                foreach (Node nde in u_f_net.fri3ndly_network.nodes)
                {
                    PossibleRequests.Req neqReq = new PossibleRequests.Req
                    {
                        __path = "/",
                        Port = nde.Port,
                        IPAddress = nde.IPAddress
                    };

                    list.Add(neqReq);
                }

                return list;
            }

            public static void Write(Req RequestNode)
            {
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write("* ");

                string ReqUestT = Path.Join($"http://{RequestNode.IPAddress}:{RequestNode.Port}", RequestNode.__path);
                int statusCode = InsureGetRequest(ReqUestT);

                if (statusCode == 200)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{ReqUestT} --> 200\n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{ReqUestT} --> {statusCode}\n");
                }
            }

            public class Req
            {
                public string __path { get; set; }
                public string IPAddress { get; set; }
                public int Port { get; set; }
            }

            private static readonly HttpClient _httpClient = new HttpClient();

            public static int InsureGetRequest(string url)
            {
                try
                {
                    var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                    return (int)response.StatusCode;
                }
                catch (HttpRequestException)
                {
                    return -1;
                }
            }
        }
    }
}