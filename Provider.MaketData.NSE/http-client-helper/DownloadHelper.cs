using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Mail;
using System.Security.Policy;

namespace Provider.MaketData.NSE.HttpClientHelper {
    public static class DownloadHelper {
        public static async Task<string> GetDownloadedContent(string absoluteUriToDownload) {

            if (string.IsNullOrEmpty((absoluteUriToDownload ?? string.Empty).Trim())) throw new ArgumentNullException(nameof(absoluteUriToDownload));

            Log.WriteLog(string.Format("Downloadinging, In memory - {0} ", absoluteUriToDownload));

            string contentString = string.Empty;

            //CookieContainer cookies = SessionCookiesOfNSEIndia;

            HttpClientHandler clientHandler = new HttpClientHandler {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                UseDefaultCredentials = true,
                PreAuthenticate = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate

            };

            using (HttpClient client = new HttpClient(clientHandler)) {
                client.DefaultRequestHeaders.Accept.Clear();
                RequestHeaders.ForEach(requestHeader => client.DefaultRequestHeaders.Add(requestHeader.Key, requestHeader.Value));
                MediaTypes.ForEach(mediaType => client.DefaultRequestHeaders.Accept.Add(mediaType));

                client.Timeout = TimeSpan.FromSeconds(10);

                using (HttpResponseMessage response = await client.GetAsync(absoluteUriToDownload)) {

                    if (response.IsSuccessStatusCode) {

                        //* Get stream of content from downloaded file
                        HttpContent content = response.Content;
                        Stream contentStream = await content.ReadAsStreamAsync();

                        //* Read stremed content as string
                        StreamReader reader = new StreamReader(contentStream);
                        contentString = reader.ReadToEnd();
                    } else {
                        throw new Exception(response.ToString());
                    }
                }
            }

            Log.WriteLog(string.Format("Downloaded {0} successfully in memory!", absoluteUriToDownload));
            return contentString;

        }

        public static void WriteContentToFile(string contentToWrite, string downloadDirectory, string subDirectory, string destinationFileName) {

            Log.WriteLog(string.Format("Writing {0} at {1}\\{2}", destinationFileName, downloadDirectory, subDirectory));

            string destinationDirectory = Path.Combine(downloadDirectory, subDirectory);
            if (!Directory.Exists(destinationDirectory)) {
                Log.WriteLog(string.Format("Directory {0} does not exists, creating new!", destinationDirectory));
                Directory.CreateDirectory(destinationDirectory);
            }

            string destinationFileFullPath = Path.Combine(destinationDirectory, destinationFileName);
            if (File.Exists(destinationFileFullPath)) {
                Log.WriteLog(string.Format("File {0} already exists, deleting!", destinationFileFullPath));
                File.Delete(destinationFileFullPath);
            }

            File.WriteAllText(destinationFileFullPath, contentToWrite);
        }

        #region Helper methods

        private static List<KeyValuePair<string, string>> RequestHeaders {
            get {
                return new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0"),
                    new KeyValuePair<string, string>("Sec-Fetch-Mode", "navigate"),
                    new KeyValuePair<string, string>("Sec-Fetch-Dest", "document")
                 }.ToList();
            }
        }

        private static List<MediaTypeWithQualityHeaderValue> MediaTypes {
            get {
                return new MediaTypeWithQualityHeaderValue[] { 
                    new MediaTypeWithQualityHeaderValue("application/json"),
                    new MediaTypeWithQualityHeaderValue("text/html"),
                    new MediaTypeWithQualityHeaderValue("application/xhtml+xml"),
                    new MediaTypeWithQualityHeaderValue("application/xml", 0.9),
                    new MediaTypeWithQualityHeaderValue("image/webp"),
                    new MediaTypeWithQualityHeaderValue("image/apng"),
                    new MediaTypeWithQualityHeaderValue("*/*", 0.8),
                    new MediaTypeWithQualityHeaderValue("application/signed-exchange", 0.7)
                }.ToList();
            }
        }

        private static CookieContainer SessionCookiesOfNSEIndia {
            get {

                CookieContainer cookies = new CookieContainer();

                var clientHandler = new HttpClientHandler {
                    CookieContainer = cookies,
                    UseCookies = true,
                    UseDefaultCredentials = true,
                    PreAuthenticate = true,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate

                };

                string uri = @"https://www.nseindia.com/companies-listing/corporate-filings-actions";

                using (HttpClient client = new HttpClient(clientHandler)) {
                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    RequestHeaders.ForEach(requestHeader => client.DefaultRequestHeaders.Add(requestHeader.Key, requestHeader.Value));
                    MediaTypes.ForEach(mediaType => client.DefaultRequestHeaders.Accept.Add(mediaType));

                    client.Timeout = TimeSpan.FromSeconds(5);

                    HttpResponseMessage response = client.GetAsync(uri).Result;

                    cookies = clientHandler.CookieContainer;

                    response.Dispose();
                    client.CancelPendingRequests();
                    client.Dispose();

                }

                return cookies;

            }
        }

        #endregion

    }
}
