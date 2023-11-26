﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Provider.MaketData.NSE.HttpClientHelper {
    public static class DownloadHelper {
        public static async Task<string> GetDownloadedContent(string absoluteUriToDownload) {

            if (string.IsNullOrEmpty((absoluteUriToDownload ?? string.Empty).Trim())) throw new ArgumentNullException(nameof(absoluteUriToDownload));

            Log.WriteLog(string.Format("Downloadinging, In memory - {0} ", absoluteUriToDownload));

            string contentString = string.Empty;

            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(absoluteUriToDownload);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(absoluteUriToDownload);

                if (response.IsSuccessStatusCode) {
                    
                    //* Get stream of content from downloaded file
                    HttpContent content = response.Content;
                    Stream contentStream = await content.ReadAsStreamAsync();

                    //* Read stremed content as string
                    StreamReader reader = new StreamReader(contentStream);
                    contentString = reader.ReadToEnd();
                }
                else
                {
                    throw new FileNotFoundException();
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
    }
}
