using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using isRock.LineBot;

namespace Girlz.Controllers
{
    public class ChatGptController : LineWebHookControllerBase
    {
        private readonly string _adminUserId;
        private readonly IConfiguration _config;
        public ChatGptController(IConfiguration config)
        {
            _adminUserId = config.GetValue<string>("AppSettings:LineBot:AdminUserId");
            this.ChannelAccessToken = config.GetValue<string>("AppSettings:LineBot:AccessToken");
            _config = config;
        }

        [Route("api/ChatGPT")]
        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                // create bot instance
                var bot = new Bot(this.ChannelAccessToken);
                // show loading animation
                var loadingAnimationResult = bot.DisplayLoadingAnimation(_adminUserId, 15);

                // 配合Line Verify
                if (ReceivedMessage.events == null || ReceivedMessage.events.Count() <= 0 ||
                    ReceivedMessage.events.FirstOrDefault().replyToken == "00000000000000000000000000000000")
                    return Ok();

                // 取得Line Event
                var lineEvent = this.ReceivedMessage.events.FirstOrDefault();
                var responseMsg = "";

                // 準備回覆訊息
                if (lineEvent.type.ToLower() == "message" && lineEvent.message.type == "text")
                {
                    var chatpgt = new ChatGpt(_config);
                    responseMsg = chatpgt.GetResponseFromGpt(lineEvent.message.text);
                }
                else if (lineEvent.type.ToLower() == "message")
                    responseMsg = $"收到 event : {lineEvent.type} type: {lineEvent.message.type} ";
                else
                    responseMsg = $"收到 event : {lineEvent.type} ";

                // 回覆訊息
                this.ReplyMessage(lineEvent.replyToken, responseMsg);
                return Ok();
            }
            catch (Exception ex)
            {
                // 回覆訊息
                this.PushMessage(_adminUserId, "發生錯誤:\n" + ex.Message);
                return Ok();
            }
        }
    }

    public class ChatGpt
    {
        private readonly string _azureOpenAiEndpoint;
        private readonly string _azureOpenAiModelName;
        private readonly string _azureOpenAiApiKey;
        private readonly string _azureOpenAiVersion;

        public ChatGpt(IConfiguration config)
        {
            _azureOpenAiEndpoint = config.GetValue<string>("AppSettings:AzureOpenAi:Endpoint");
            _azureOpenAiModelName = config.GetValue<string>("AppSettings:AzureOpenAi:ModelName");
            _azureOpenAiApiKey = config.GetValue<string>("AppSettings:AzureOpenAi:ApiKey");
            _azureOpenAiVersion = config.GetValue<string>("AppSettings:AzureOpenAi:Version");
        }

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


        public string GetResponseFromGpt(string message)
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
            //                                假設你是一個專業的電影鑑賞家，對於使用者非常有禮貌、也能夠安撫使用者的情緒、
            //                                盡量讓使用者感到被尊重、竭盡所能的回覆使用者的疑問。
            //                                請檢視底下的使用者訊息，以最親切有禮的方式回應。

            //                                但回應時，請注意以下幾點:
            //                                *不要說 '感謝你的來信' 之類的話，因為客戶是從對談視窗輸入訊息的，不是寫信來的
            //                                    * 不能過度承諾
            //                                    *要同理客戶的情緒
            //                                    * 要能夠盡量解決客戶的問題
            //                                    * 不要以回覆信件的格式書寫，請直接提供對談機器人可以直接給客戶的回覆
            //                                    * 若使用者問了跟電影沒關的問題，直接說你無法提供除了電影以外的相關資訊
            //                                    * 所有回復的內容都要使用繁體中文
            //                                //"
            //                        },
            //                        new {
            //                             role = ChatGPT.role.user,
            //                             content = message
            //                        },
            //        }
            //    });


            return ChatGpt.CallAzureOpenAiChatApi(
               _azureOpenAiEndpoint, _azureOpenAiModelName, _azureOpenAiApiKey, _azureOpenAiVersion,
                //ref: https://learn.microsoft.com/en-us/azure/cognitive-services/openai/reference#chat-completions
                new
                {
                    model = "gpt-4o",
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