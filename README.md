## Konata.Core
[![Core](https://img.shields.io/badge/Konata-Core-blue)](#)
[![C#](https://img.shields.io/badge/C%23-9.0-green)](#)
[![NuGet](https://img.shields.io/badge/NuGet-1.0.3alpha.1-orange)](https://www.nuget.org/packages/Konata.Core/)
[![License](https://img.shields.io/static/v1?label=LICENSE&message=GNU%20GPLv3&color=lightrey)](./blob/main/LICENSE)

QQ(Android) protocol core implementation in C# 9   
based on **.net standard 2.1**, event driven.

## Documentation
 Stay tuned...

## Example
```C#
// Create a bot instance
var bot = new Bot(config, device, keystore);
{
    // Print the log
    bot.OnLog += (s, e) =>
    {
        Console.WriteLine(e.EventMessage); 
    };

    // Handle the captcha
    bot.OnCaptcha += (s, e) =>
    {
        if(e.Type == CaptchaType.Slider)
        {
            Console.WriteLine(e.SliderUrl); 
            ((Bot)s).SubmitSliderTicket(Console.ReadLine());
        }
        else if(e.Type == CaptchaType.CheckSms)
        {
            Console.WriteLine(e.Phone); 
            ((Bot)s).SubmitSMSCode(Console.ReadLine());
        }
    };

    // Handle messages from group
    bot.OnGroupMessage += (s, e) =>
    {
         Console.WriteLine($"{e.Message.ToString()}"); 
    }
    
    // Handle messages from friend
    bot.OnPrivateMessage += (s, e) =>
    {
         Console.WriteLine($"{e.Message.ToString()}"); 
    }
    
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
- [ ] Internal audio codec
- [ ] Internal audio resampling
- [ ] Record chain
- [ ] Expose more APIs
- [ ] [More Plans](../../projects/1)...

## LICENSE
Licensed in GNU GPLv3 with ❤.