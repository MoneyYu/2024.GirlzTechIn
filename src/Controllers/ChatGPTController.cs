using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using isRock.LineBot;

namespace Girlz.Controllers
{
    public class ChatGptController : LineWebHookControllerBase
    {
        [Route("api/ChatGPT")]
        [HttpPost]
        public IActionResult Post()
        {
            const string AdminUserId = "U81025e615f2b5734e31d2715303acb5a";

            try
            {
                //設定ChannelAccessToken
                this.ChannelAccessToken =
                    "jyXxVqIcKeZU3qyONPyCX/w8kUgtz3wVJn6jhVwB3Zc1cn97fdSR53giZ138HSZJXymbJIw5VOiR3xCEZxNNfIOo1C86OV1I3hisqsq1+R+/+RJTZEvChbcmt93YuISkIMGxl7JvPS93mTF+7H/xPQdB04t89/1O/w1cDnyilFU=";

                // create bot instance
                var bot = new Bot(this.ChannelAccessToken);
                // show loading animation
                var ret = bot.DisplayLoadingAnimation(AdminUserId, 15);

                //配合Line Verify
                if (ReceivedMessage.events == null || ReceivedMessage.events.Count() <= 0 ||
                    ReceivedMessage.events.FirstOrDefault().replyToken == "00000000000000000000000000000000")
                    return Ok();

                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                var responseMsg = "";

                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                {
                    responseMsg = ChatGpt.GetResponseFromGpt(LineEvent.message.text);
                }
                else if (LineEvent.type.ToLower() == "message")
                    responseMsg = $"收到 event : {LineEvent.type} type: {LineEvent.message.type} ";
                else
                    responseMsg = $"收到 event : {LineEvent.type} ";

                //回覆訊息
                this.ReplyMessage(LineEvent.replyToken, responseMsg);
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }
    }

    public class ChatGpt
    {
        const string AzureOpenAiEndpoint = "https://demo-yu-aoai.openai.azure.com/"; //👉replace it with your Azure OpenAI Endpoint
        const string AzureOpenAiModelName = "gpt-4o"; //👉repleace it with your Azure OpenAI Model Deploy Name
        const string AzureOpenAiApiKey = "d6094cb6a99e47838f0185797f4b28e7"; //👉repleace it with your Azure OpenAI API Key
        const string AzureOpenAiVersion = "2024-02-15-preview"; //👉replace  it with your Azure OpenAI API Version

        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum Role
        {
            assistant,
            user,
            system
        }

        public static string CallAzureOpenAiChatApi(
            string endpoint, string deploymentName, string apiKey, string apiVersion, object requestData)
        {
            var client = new HttpClient();

            // 設定 API 網址
            var apiUrl = $"{endpoint}/openai/deployments/{deploymentName}/chat/completions?api-version={apiVersion}";

            // 設定 HTTP request headers
            client.DefaultRequestHeaders.Add("api-key", apiKey); //👉Azure OpenAI key
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header
            // 將 requestData 物件序列化成 JSON 字串
            string jsonRequestData = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            // 建立 HTTP request 內容
            var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
            // 傳送 HTTP POST request
            var response = client.PostAsync(apiUrl, content).Result;
            // 取得 HTTP response 內容
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var obj = JsonConvert.DeserializeObject<dynamic>(responseContent);
            return obj.choices[0].message.content.Value;
        }

        public static string CallOpenAiChatApi(string apiKey, object requestData)
        {
            var client = new HttpClient();

            // 設定 API 網址
            var apiUrl = $"https://api.openai.com/v1/chat/completions";

            // 設定 HTTP request headers
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}"); //👉OpenAI key
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header
            // 將 requestData 物件序列化成 JSON 字串
            string jsonRequestData = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            // 建立 HTTP request 內容
            var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
            // 傳送 HTTP POST request
            var response = client.PostAsync(apiUrl, content).Result;
            // 取得 HTTP response 內容
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
            return obj.choices[0].message.content.Value;
        }


        public static string GetResponseFromGpt(string message)
        {
            string openAiApiKey = "";

            //return ChatGPT.CallOpenAiChatApi(
            //    openAiApiKey,
            //    //ref: https://learn.microsoft.com/en-us/azure/cognitive-services/openai/reference#chat-completions
            //    new
            //    {
            //        model = "gpt-4o",
            //        messages = new[]
            //        {
            //                        new {
            //                            role = ChatGPT.role.system ,
            //                            content = @"
            //                                假設你是一個專業的客戶服務人員，對於客戶非常有禮貌、也能夠安撫客戶的抱怨情緒、
            //                                盡量讓客戶感到被尊重、竭盡所能的回覆客戶的疑問。

            //                                請檢視底下的客戶訊息，以最親切有禮的方式回應。

            //                                但回應時，請注意以下幾點:
            //                                * 不要說 '感謝你的來信' 之類的話，因為客戶是從對談視窗輸入訊息的，不是寫信來的
            //                                * 不能過度承諾
            //                                * 要同理客戶的情緒
            //                                * 要能夠盡量解決客戶的問題
            //                                * 不要以回覆信件的格式書寫，請直接提供對談機器人可以直接給客戶的回覆
            //                                ----------------------
            //"
            //                        },
            //                        new {
            //                             role = ChatGPT.role.user,
            //                             content = message
            //                        },
            //        }
            //    });


            return ChatGpt.CallAzureOpenAiChatApi(
               AzureOpenAiEndpoint, AzureOpenAiModelName, AzureOpenAiApiKey, AzureOpenAiVersion,
                //ref: https://learn.microsoft.com/en-us/azure/cognitive-services/openai/reference#chat-completions
                new
                {
                    //model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                                    new {
                                        role = ChatGpt.Role.system ,
                                        content = @"
                                            假設你是一個專業的電影鑑賞家，對於使用者非常有禮貌、也能夠安撫使用者的情緒、
                                            盡量讓使用者感到被尊重、竭盡所能的回覆使用者的疑問。

                                            請檢視底下的使用者訊息，以最親切有禮的方式回應。

                                            但回應時，請注意以下幾點:
                                            * 不要說 '感謝你的來信' 之類的話，因為客戶是從對談視窗輸入訊息的，不是寫信來的
                                            * 不能過度承諾
                                            * 要同理客戶的情緒
                                            * 要能夠盡量解決客戶的問題
                                            * 不要以回覆信件的格式書寫，請直接提供對談機器人可以直接給客戶的回覆
                                            * 若使用者問了跟電影沒關的問題，直接說你無法提供除了電影以外的相關資訊
                                            * 所有回復的內容都要使用繁體中文
                                            ----------------------
            "
                                    },
                                    new {
                                         role = ChatGpt.Role.user,
                                         content = message
                                    },
                    }
                });
        }
    }
}