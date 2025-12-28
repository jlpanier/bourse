using Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;

namespace WsBoursorama
{
    public class WsBoursorama
    {
        public static BoursoramaResponse WebSite(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicy) =>
            {
                bool policysecure = false;
                switch (sslPolicy)
                {
                    case SslPolicyErrors.None:
                        policysecure = true;
                        break;
                    case SslPolicyErrors.RemoteCertificateNotAvailable:
                        break;
                    case SslPolicyErrors.RemoteCertificateNameMismatch:
                        break;
                    case SslPolicyErrors.RemoteCertificateChainErrors:
                        policysecure = (((HttpWebRequest)sender).RequestUri.Authority.Equals("boursorama.com"));
                        break;
                }
                return policysecure;
            };

            var request = HttpWebRequest.CreateHttp(new Uri(url));
            request.Method = "GET";
            request.Credentials = CredentialCache.DefaultCredentials;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var dataStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    var result = new BoursoramaResponse();
                    string responses = reader.ReadToEnd();

                    int index = responses.IndexOf("</head>");
                    string body = responses.Substring(index + "</head>".Length);
                    index = body.IndexOf("data-ist-last>");
                    string text1 = body.Substring(index + "data-ist-last>".Length);
                    index = text1.IndexOf("</span>");
                    string text2 = text1.Substring(0, index);
                    if (double.TryParse(text2.Replace(".",","), out double amount))
                    {
                        result.Amount = amount;
                    }
                    index = text1.IndexOf("c-median-gauge__tooltip");
                    string text3 = text1.Substring(index+ "c-median-gauge__tooltip".Length +5);
                    index = text3.IndexOf("</div>");
                    if (double.TryParse(text3.Substring(0, index - 2).Trim().Replace("/100", ""), out double val))
                    {
                        result.Risk = val;
                    }
                    else
                    {
                        result.Risk = 0;
                    }

                    index = text3.IndexOf("c-median-gauge__tooltip");
                    string text4 = text3.Substring(index + "c-median-gauge__tooltip".Length + 5);
                    index = text4.IndexOf("</div>");
                    string text5 = text4.Substring(0, index - 2).Trim();
                    if (double.TryParse(text5.Replace(".", ","), out double consensus))
                    {
                        result.Consensus = consensus;
                    }
                    index = text4.IndexOf("c-table__cell c-table__cell--dotted c-table__cell--inherit-height c-table__cell--align-top / u-text-left u-text-normal-whitespace");
                    string text6 = text4.Substring(index + "c-table__cell c-table__cell--dotted c-table__cell--inherit-height c-table__cell--align-top / u-text-left u-text-normal-whitespace".Length + 2);
                    index = text6.IndexOf("Rendement");
                    string text7 = text6.Substring(index);
                    index = text7.IndexOf("u-ellipsis");
                    string text8 = text7.Substring(index + "u-ellipsis".Length + 2);
                    index = text8.IndexOf("</td>");
                    string text9 = text8.Substring(0,index).Trim().Replace("%", "");
                    if (double.TryParse(text9.Replace(".", ","), out double rendement))
                    {
                        result.Rendement = rendement;
                    }


                    dataStream.Close();
                    response.Close();

                    return result;
                }
            }

        }


    }
}