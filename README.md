# 2024.GirlzTechIn
## 參考網址
[在 LINE Developers 上建立 LINE Bot](https://datasciocean.tech/others/create-line-bot/)

[取得 LINE 的 Channel Access Token](https://hackmd.io/@rolence/SyGjWeEF2)

[結合⽣成式 AI 打造有趣 LINE Bot 應⽤](https://s.itho.me/ccms_slides/2023/7/28/c4764d62-8341-4f21-bb06-17146db20c94.pdf)

[介紹與安裝LINE Bot SDK](https://site.cc-work.com/WebUPD/school/ai-linebot/8_%E4%BD%BF%E7%94%A8%20LINE%20Bot%20SDK_v3.2.pdf)

[OpenAI API 注意事項](https://site.cc-work.com/WebUPD/school/ai-linebot/11_%E8%B7%9F%20OpenAI%20API%20%E4%BA%92%E5%8B%95_v1.2_20240506.pdf)

[LINE Messaging API SDKs](https://developers.line.biz/en/docs/messaging-api/line-bot-sdk/)

-----
[LINE Developer](https://developers.line.biz/)

[OpenAI API](https://platform.openai.com/)

[Send Flex Messages](https://developers.line.biz/en/docs/messaging-api/using-flex-messages/)

[FLEX MESSAGE SIMULATOR](https://developers.line.biz/flex-simulator/)

## Create project
Create project
``` bash
dotnet new webapi  --use-program-main -controllers --name Girlz
```

Install package
``` bash
dotnet add package linebotsdk
```

Install and create template
``` bash
dotnet new --install isRock.Template.LineWebHook  
dotnet new linewebhook
```

## Github Copilot Prompt
> C# 的內容提示不完善。

``` text
我想要做一個基礎 LINE 聊天機器人，我說甚麼，他就回復甚麼，使用 C# 撰寫。
```

``` text
找不到 Startup.cs
```

## Ngrok config

1. 下載 ngrok
``` bash
choco install ngrok -y
```

2. 設定 ngrok
``` bash
ngrok config add-authtoken _Token_
```

3. 啟動 ngrok
``` bash
ngrok http http://localhost:5258
```

## Security
[在 ASP.NET Core 的開發中安全儲存應用程式秘密](https://learn.microsoft.com/zh-tw/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows)