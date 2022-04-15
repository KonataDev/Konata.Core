<div align="center">

<img width="100" src="Resources/konata_icon_512_round64.png">

## Konata.Core

[![Core](https://img.shields.io/badge/Konata-Core-blue)](#)
[![C#](https://img.shields.io/badge/.NET-Standard%202.1-blue)](#)
[![NuGet](https://img.shields.io/nuget/v/Konata.Core)](https://www.nuget.org/packages/Konata.Core)  
[![NuGet](https://img.shields.io/nuget/dt/Konata.Core)](https://www.nuget.org/packages/Konata.Core)
[![License](https://img.shields.io/static/v1?label=LICENSE&message=GNU%20GPLv3&color=lightrey)](./blob/main/LICENSE)
[![Build](https://github.com/KonataDev/Konata.Core/actions/workflows/build.yml/badge.svg?branch=master)](./actions/workflows/build.yml)

纯 C# 实现的 QQ(Android) 协议核心

基于 **.NET Standard 2.1**，事件驱动

</div>

## 文档

简体中文 / [English](/README.md)

- [API 参考](https://github.com/KonataDev/Konata.Core/wiki) 页面
- 查看示例 bot：[Kagami](https://github.com/KonataDev/Kagami)

<details>
<summary>示例代码片段</summary>

```C#
// 创建一个 bot 实例
var bot = BotFather.Create(config, device, keystore);
{
    // 处理验证码
    bot.OnCaptcha += (bot, e) =>
    {
        if(e.Type == CaptchaType.Slider)
        {
            Console.WriteLine(e.SliderUrl); 
            bot.SubmitSliderTicket(Console.ReadLine());
        }
        else if(e.Type == CaptchaType.Sms)
        {
            Console.WriteLine(e.Phone); 
            bot.SubmitSmsCode(Console.ReadLine());
        }
    };

    // 输入日志
    bot.OnLog += (_, e) 
        => Console.WriteLine(e.EventMessage);

    // 处理群消息
    bot.OnGroupMessage += (_, e) 
        => Console.WriteLine(e.Message); 
    
    // Handle friend messages
    bot.OnFriendMessage += (_, e) 
        => Console.WriteLine(e.Message);
    
    // ... 其他处理器
}

// 登录 bot
if(!await bot.Login())
{
    Console.WriteLine("Login failed");
    return;
}

Console.WriteLine("We got online!");
```

</details>

## 现已支持
| 消息    | 状态           | 操作     | 状态          | 事件              | 状态          |
|:------------|:------------------|:---------------|:-----------------|:--------------------|:-----------------|
| Images      | 🟢                | Poke           | 🟢               | Captcha             | 🟢               |
| Text / At   | 🟢                | Recall         | 🟡[^1]           | BotOnline           | 🟢               |
| Records     | 🟢                | Leave Group    | 🟢               | BotOffline          | 🟢               |
| QFace       | 🟢                | Special Title  | 🟢               | Message             | 🟡[^2]           |
| Json        | 🟢                | Kick Member    | 🟢               | Poke                | 🟢               |
| Xml         | 🟢                | Mute Member    | 🟢               | MessageRecall       | 🟢               |
| Forward     | 🟡[^3]            | Set Admin      | 🟢               | GroupMemberDecrease | 🟢               |
| Video       | 🔴                | Friend Request | 🟢               | GroupMemberIncrease | 🟢               |
| Flash Image | 🟢                | Group Request  | 🟢               | GroupPromoteAdmin   | 🟢               |
| Reply       | 🟢                | Voice Call     | 🔴               | GroupInvite         | 🟢               |
| File        | 🔴                | Csrf Token     | 🟢               | GroupRequestJoin    | 🟢               |
|             |                   | Cookies        | 🔴               | FriendRequest       | 🟢               |
|             |                   |                |                  | FriendTyping        | 🟢               |
|             |                   |                |                  | FriendVoiceCall     | 🔴               |


[^1]: 不支持撤回 bot 发送的消息。

[^2]: 不支持临时消息。

[^3]: 不支持在群和好友之间转发消息。 

## 特别致谢

非常感谢 **JetBrains** 给我们提供了免费的开源许可证。
  
[<img src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" width="200"/>](https://www.jetbrains.com/?from=konata)

## 开源许可

Licensed in GNU GPLv3 with ❤.
