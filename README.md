<div align="center">

<img width="100" src="Resources/konata_icon_512_round64.png">

## Konata.Core

[![Core](https://img.shields.io/badge/Konata-Core-blue)](#)
[![C#](https://img.shields.io/badge/.NET-Standard%202.1-blue)](#)
[![NuGet](https://img.shields.io/nuget/v/Konata.Core)](https://www.nuget.org/packages/Konata.Core)  
[![NuGet](https://img.shields.io/nuget/dt/Konata.Core)](https://www.nuget.org/packages/Konata.Core)
[![License](https://img.shields.io/static/v1?label=LICENSE&message=GNU%20GPLv3&color=lightrey)](./blob/main/LICENSE)
[![Build](https://github.com/KonataDev/Konata.Core/actions/workflows/build.yml/badge.svg?branch=master)](./actions/workflows/build.yml)

QQ(Android) protocol core implemented with pure C#  
based on **.net standard 2.1**, event driven.
</div>

## Documentation
 Stay tuned...

## Example
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
        else if(e.Type == CaptchaType.SMS)
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

## TODO list
- [x] Internal audio codec
- [x] Internal audio resampling
- [x] Record chain
- [ ] Expose more APIs
- [x] Refactor sso services
- [ ] Refactor packets
- [ ] Robust login logic
- [x] Ecdh xchg
- [x] Fix task stuck
- [ ] [More Plans](../../projects/1)...

## LICENSE
Licensed in GNU GPLv3 with ❤.
