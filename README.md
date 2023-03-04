<div align="center">

<img width="100" src="Resources/konata_icon_512_round64.png">

## Konata.Core

[![Core](https://img.shields.io/badge/Konata-Core-blue)](#)
[![C#](https://img.shields.io/badge/.NET-Standard%202.1-blue)](#)
[![NuGet](https://img.shields.io/nuget/v/Konata.Core)](https://www.nuget.org/packages/Konata.Core)  
[![NuGet](https://img.shields.io/nuget/dt/Konata.Core)](https://www.nuget.org/packages/Konata.Core)
[![License](https://img.shields.io/static/v1?label=LICENSE&message=GNU%20GPLv3&color=lightrey)](./blob/main/LICENSE)
[![Build](https://github.com/KonataDev/Konata.Core/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/KonataDev/Konata.Core/actions/workflows/build.yml)
[![Telegram](https://img.shields.io/endpoint?color=blue&url=https://telegram-badge-4mbpu8e0fit4.runkit.sh/?url=https://t.me/konachan_wifi)](https://t.me/konachan_wifi)

QQ(Android) protocol core implemented with pure C#  
based on **.NET Standard 2.1**, event driven.
</div>

## Docs

[简体中文](/README_CN.md) / English

- Go to [API references](https://github.com/KonataDev/Konata.Core/wiki) page
- Quick start with [Kagami](https://github.com/KonataDev/Kagami)

<details>
<summary>Example code snippets</summary>

```C#
// Create a bot instance
var bot = BotFather.Create(config, device, keystore);
{
    // Handle the captcha
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

    // Print the log
    bot.OnLog += (_, e) 
        => Console.WriteLine(e.EventMessage);

    // Handle group messages
    bot.OnGroupMessage += (_, e) 
        => Console.WriteLine(e.Message); 
    
    // Handle friend messages
    bot.OnFriendMessage += (_, e) 
        => Console.WriteLine(e.Message);
    
    // ... More handlers
}

// Do login
if(!await bot.Login())
{
    Console.WriteLine("Login failed");
    return;
}

Console.WriteLine("We got online!");
```

</details>

## Features List
| Messages    | Support           | Operations     | Support          | Events              | Support          |
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

[^1]: Not supported to recall messages sent from the bot.  
[^2]: Not supported temp messages.
[^3]: Not supported to forward messages between group and friend.  

## Special Thanks
Special thanks to **JetBrains** offers free open-source licenses for us!  
  
[<img src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" width="200"/>](https://www.jetbrains.com/?from=konata)

## License
Licensed in GNU GPLv3 with ❤.
