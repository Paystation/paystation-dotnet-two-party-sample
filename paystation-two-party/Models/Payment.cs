using System;
using System.Xml;
using System.Net;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace paystation_two_party.Models
{
    /// <summary>
    /// Enables payments to PayStation and provides payment response data.
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// The payment amount in cents. Use the following cent values when using a test account:
        /// 
        /// 00 - Approved (Response Code 0)
        /// 51 - Insufficient Funds (Response Code 5)
        /// 12 - Invalid Transaction (Response Code 8)
        /// 54 - Expired Card (Response Code 4)
        /// 91 - Error communicating with Bank (Response Code 6)
        /// 
        /// For example, $8.00 = 800 would receive a Response Code of 0.
        /// </summary>
        public int Amount { get; set; } = 100;

        /// <summary>
        /// The credit card number. For test payments, use "5123456789012346" for Mastercard or
        /// "4987654321098769" for Visa.
        /// </summary>
        public string CardNumber { get; set; } = "5123456789012346";

        /// <summary>
        /// The credit card expiry date. Must be in YYMM format. Use "1305" for test payments.
        /// </summary>
        public string CardExpiry { get; set; } = "2105";

        /// <summary>
        /// Paystation ID - Populated from appsettings.json
        /// </summary>
        public string PaystationId { get; set; }

        /// <summary>
        /// Gateway ID - Populated from appsettings.json
        /// </summary>
        public string GatewayId { get; set; }

        /// <summary>
        /// Client session id. Used to identify transactions.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// PayStation payment URL with querystring data generated from
        /// given payment data.
        /// </summary>
        public string Url
        {
            get
            {
                return $@"?paystation=_empty
                        &pstn_pi={PaystationId}&pstn_gi={GatewayId}
                        &pstn_cn={CardNumber}&pstn_ex={CardExpiry}
                        &pstn_am={Amount}&pstn_ms={GetMerchantSessionID()}
                        &pstn_2p=t&pstn_nr=t&pstn_tm=t";
            }
        }

        /// <summary>
        /// Generates the Merchant Session ID for this transaction.
        /// a string representing the current date and time to the nearest millisecond
        /// and lastly the session ID.
        /// </summary>
        /// <returns>The Merchant Session ID</returns>
        private string GetMerchantSessionID()
        {
            DateTime now = DateTime.Now;
            String dateTime = now.Year.ToString() + now.Month.ToString() + now.Day.ToString() + now.Hour.ToString()
                + now.Minute.ToString() + now.Second.ToString() + now.Millisecond.ToString();
            return dateTime + SessionId;
        }

        /// <summary>
        /// Sends a request to the PayStation payment URL, reads the XML response and returns the response data.
        /// </summary>
        /// <returns>Response data from the XML response</returns>
        public PaymentResponse GetResponse()
        {
            XmlDocument xmlDocument = new XmlDocument();
            PaymentResponse response = new PaymentResponse();

            try
            {
                // Create the request obj
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.paystation.co.nz/direct/paystation.dll");

                // Set values for the request back
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = Url.Length;

                // Write the request
                StreamWriter stOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
                stOut.Write(Url);
                stOut.Close();

                // Do the request to get the response
                StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
                string strResponse = stIn.ReadToEnd();
                stIn.Close();

                xmlDocument.Load(new StringReader(strResponse));

                response.ResponseXml = xmlDocument.OuterXml.Replace("><", ">\n<");
                response.Code = xmlDocument.GetElementsByTagName("PaystationErrorCode")[0].InnerXml;
                response.Message = xmlDocument.GetElementsByTagName("PaystationErrorMessage")[0].InnerXml;
            }
            catch (System.Net.WebException)
            {
                response.Message = "Unable to connect to PayStation.";
                response.IsError = true;
            }
            catch (System.Xml.XmlException)
            {
                response.Message = "The XML file is not well formed.";
                response.IsError = true;
            }

            return response;
        }
    }
}