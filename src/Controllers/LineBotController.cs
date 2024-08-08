using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;

namespace Girlz.Controllers
{
    public class LineWebHookController : LineWebHookControllerBase
    {
        [Route("api/LineBot")]
        [HttpPost]
        public IActionResult Post()
        {
            var AdminUserId = "U81025e615f2b5734e31d2715303acb5a";

            try
            {
                //設定ChannelAccessToken
                this.ChannelAccessToken = "jyXxVqIcKeZU3qyONPyCX/w8kUgtz3wVJn6jhVwB3Zc1cn97fdSR53giZ138HSZJXymbJIw5VOiR3xCEZxNNfIOo1C86OV1I3hisqsq1+R+/+RJTZEvChbcmt93YuISkIMGxl7JvPS93mTF+7H/xPQdB04t89/1O/w1cDnyilFU=";

                // create bot instance
                var bot = new Bot(this.ChannelAccessToken);
                // show loading animation
                var ret = bot.DisplayLoadingAnimation(AdminUserId, 15);

                //配合Line Verify
                if (ReceivedMessage.events == null || ReceivedMessage.events.Count() <= 0 ||
                    ReceivedMessage.events.FirstOrDefault().replyToken == "00000000000000000000000000000000") return Ok();

                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                var responseMsg = "";

                //準備回覆訊息
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

                    //回覆訊息
                    this.ReplyMessage(LineEvent.replyToken, responseMsg);
                }
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
}