using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;

namespace Girlz.Controllers
{
    public class LineBotController : LineWebHookControllerBase
    {
        private readonly string _adminUserId;
        private readonly IConfiguration _config;
        public LineBotController(IConfiguration config)
        {
            _adminUserId = config.GetValue<string>("AppSettings:LineBot:AdminUserId");
            this.ChannelAccessToken = config.GetValue<string>("AppSettings:LineBot:AccessToken");
            _config = config;
        }

        [Route("api/LineBot")]
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
                    ReceivedMessage.events.FirstOrDefault().replyToken == "00000000000000000000000000000000") return Ok();

                // 取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                var responseMsg = "";

                // 準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                {
                    var oriMsg = LineEvent.message.text;
                    if (oriMsg.ToLower() == "location" || oriMsg.ToLower() == "map")
                    {
                        var locationMsg = new LocationMessage("台灣微軟", "台北市信義區忠孝東路五段68號", 25.0405044, 121.5668964);
                        this.ReplyMessage(LineEvent.replyToken, locationMsg);
                    }
                    else if (oriMsg.ToLower() == "sticker")
                    {
                        var stickerMsg = new StickerMessage(1, 1);
                        this.ReplyMessage(LineEvent.replyToken, stickerMsg);
                    }
                    else if (oriMsg.ToLower() == "image")
                    {
                        this.ReplyMessage(LineEvent.replyToken, new Uri("https://i.imgur.com/s29ZjUL.jpeg"));
                    }
                    else if (oriMsg.ToLower() == "audio")
                    {
                        var audioMsg = new AudioMessage(new Uri("https://mttmedia.yu.money/common/demo.mp3"), 90000);
                        this.ReplyMessage(LineEvent.replyToken, audioMsg);
                    }
                    else if (oriMsg.ToLower() == "video")
                    {
                        var videoMsg = new VideoMessage(new Uri("https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"), new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Big_buck_bunny_poster_big.jpg/800px-Big_buck_bunny_poster_big.jpg"));
                        this.ReplyMessage(LineEvent.replyToken, videoMsg);
                    }
                    else if (oriMsg.ToLower() == "flex")
                    {
                        var flexMsg = @"
                                [
                                {
                                ""type"": ""flex"",
                                ""altText"": ""This is a Flex Message"",
                                ""contents"": $flex$
                                }
                                ]";

                        #region flexContent
                        //替換Flex Contents
                        var MessageJSON = flexMsg.Replace("$flex$", @"{
  ""type"": ""bubble"",
  ""size"": ""mega"",
  ""header"": {
    ""type"": ""box"",
    ""layout"": ""vertical"",
    ""contents"": [
      {
        ""type"": ""box"",
        ""layout"": ""vertical"",
        ""contents"": [
          {
            ""type"": ""text"",
            ""text"": ""FROM"",
            ""color"": ""#ffffff66"",
            ""size"": ""sm""
          },
          {
            ""type"": ""text"",
            ""text"": ""Akihabara"",
            ""color"": ""#ffffff"",
            ""size"": ""xl"",
            ""flex"": 4,
            ""weight"": ""bold""
          }
        ]
      },
      {
        ""type"": ""box"",
        ""layout"": ""vertical"",
        ""contents"": [
          {
            ""type"": ""text"",
            ""text"": ""TO"",
            ""color"": ""#ffffff66"",
            ""size"": ""sm""
          },
          {
            ""type"": ""text"",
            ""text"": ""Shinjuku"",
            ""color"": ""#ffffff"",
            ""size"": ""xl"",
            ""flex"": 4,
            ""weight"": ""bold""
          }
        ]
      }
    ],
    ""paddingAll"": ""20px"",
    ""backgroundColor"": ""#0367D3"",
    ""spacing"": ""md"",
    ""height"": ""154px"",
    ""paddingTop"": ""22px""
  },
  ""body"": {
    ""type"": ""box"",
    ""layout"": ""vertical"",
    ""contents"": [
      {
        ""type"": ""text"",
        ""text"": ""Total: 1 hour"",
        ""color"": ""#b7b7b7"",
        ""size"": ""xs""
      },
      {
        ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {
            ""type"": ""text"",
            ""text"": ""20:30"",
            ""size"": ""sm"",
            ""gravity"": ""center""
          },
          {
            ""type"": ""box"",
            ""layout"": ""vertical"",
            ""contents"": [
              {
                ""type"": ""filler""
              },
              {
                ""type"": ""box"",
                ""layout"": ""vertical"",
                ""contents"": [],
                ""cornerRadius"": ""30px"",
                ""height"": ""12px"",
                ""width"": ""12px"",
                ""borderColor"": ""#EF454D"",
                ""borderWidth"": ""2px""
              },
              {
                ""type"": ""filler""
              }
            ],
            ""flex"": 0
          },
          {
            ""type"": ""text"",
            ""text"": ""Akihabara"",
            ""gravity"": ""center"",
            ""flex"": 4,
            ""size"": ""sm""
          }
        ],
        ""spacing"": ""lg"",
        ""cornerRadius"": ""30px"",
        ""margin"": ""xl""
      },
      {
        ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {
            ""type"": ""box"",
            ""layout"": ""baseline"",
            ""contents"": [
              {
                ""type"": ""filler""
              }
            ],
            ""flex"": 1
          },
          {
            ""type"": ""box"",
            ""layout"": ""vertical"",
            ""contents"": [
              {
                ""type"": ""box"",
                ""layout"": ""horizontal"",
                ""contents"": [
                  {
                    ""type"": ""filler""
                  },
                  {
                    ""type"": ""box"",
                    ""layout"": ""vertical"",
                    ""contents"": [],
                    ""width"": ""2px"",
                    ""backgroundColor"": ""#B7B7B7""
                  },
                  {
                    ""type"": ""filler""
                  }
                ],
                ""flex"": 1
              }
            ],
            ""width"": ""12px""
          },
          {
            ""type"": ""text"",
            ""text"": ""Walk 4min"",
            ""gravity"": ""center"",
            ""flex"": 4,
            ""size"": ""xs"",
            ""color"": ""#8c8c8c""
          }
        ],
        ""spacing"": ""lg"",
        ""height"": ""64px""
      },
      {
        ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {
            ""type"": ""box"",
            ""layout"": ""horizontal"",
            ""contents"": [
              {
                ""type"": ""text"",
                ""text"": ""20:34"",
                ""gravity"": ""center"",
                ""size"": ""sm""
              }
            ],
            ""flex"": 1
          },
          {
            ""type"": ""box"",
            ""layout"": ""vertical"",
            ""contents"": [
              {
                ""type"": ""filler""
              },
              {
                ""type"": ""box"",
                ""layout"": ""vertical"",
                ""contents"": [],
                ""cornerRadius"": ""30px"",
                ""width"": ""12px"",
                ""height"": ""12px"",
                ""borderWidth"": ""2px"",
                ""borderColor"": ""#6486E3""
              },
              {
                ""type"": ""filler""
              }
            ],
            ""flex"": 0
          },
          {
            ""type"": ""text"",
            ""text"": ""Ochanomizu"",
            ""gravity"": ""center"",
            ""flex"": 4,
            ""size"": ""sm""
          }
        ],
        ""spacing"": ""lg"",
        ""cornerRadius"": ""30px""
      },
      {
        ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {
            ""type"": ""box"",
            ""layout"": ""baseline"",
            ""contents"": [
              {
                ""type"": ""filler""
              }
            ],
            ""flex"": 1
          },
          {
            ""type"": ""box"",
            ""layout"": ""vertical"",
            ""contents"": [
              {
                ""type"": ""box"",
                ""layout"": ""horizontal"",
                ""contents"": [
                  {
                    ""type"": ""filler""
                  },
                  {
                    ""type"": ""box"",
                    ""layout"": ""vertical"",
                    ""contents"": [],
                    ""width"": ""2px"",
                    ""backgroundColor"": ""#6486E3""
                  },
                  {
                    ""type"": ""filler""
                  }
                ],
                ""flex"": 1
              }
            ],
            ""width"": ""12px""
          },
          {
            ""type"": ""text"",
            ""text"": ""Metro 1hr"",
            ""gravity"": ""center"",
            ""flex"": 4,
            ""size"": ""xs"",
            ""color"": ""#8c8c8c""
          }
        ],
        ""spacing"": ""lg"",
        ""height"": ""64px""
      },
      {
        ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {
            ""type"": ""text"",
            ""text"": ""20:40"",
            ""gravity"": ""center"",
            ""size"": ""sm""
          },
          {
            ""type"": ""box"",
            ""layout"": ""vertical"",
            ""contents"": [
              {
                ""type"": ""filler""
              },
              {
                ""type"": ""box"",
                ""layout"": ""vertical"",
                ""contents"": [],
                ""cornerRadius"": ""30px"",
                ""width"": ""12px"",
                ""height"": ""12px"",
                ""borderColor"": ""#6486E3"",
                ""borderWidth"": ""2px""
              },
              {
                ""type"": ""filler""
              }
            ],
            ""flex"": 0
          },
          {
            ""type"": ""text"",
            ""text"": ""Shinjuku"",
            ""gravity"": ""center"",
            ""flex"": 4,
            ""size"": ""sm""
          }
        ],
        ""spacing"": ""lg"",
        ""cornerRadius"": ""30px""
      }
    ]
  }
}");
                        #endregion
                        // 發送訊息
                        bot.PushMessageWithJSON(_adminUserId, MessageJSON);

                    }
                    else
                    {
                        responseMsg = $"你說了: {LineEvent.message.text}";
                        this.ReplyMessage(LineEvent.replyToken, responseMsg);
                    }
                }
                else
                {
                    if (LineEvent.type.ToLower() == "message")
                        responseMsg = $"收到 event : {LineEvent.type} type: {LineEvent.message.type} ";
                    else
                        responseMsg = $"收到 event : {LineEvent.type} ";

                    // 回覆訊息
                    this.ReplyMessage(LineEvent.replyToken, responseMsg);
                }

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
}