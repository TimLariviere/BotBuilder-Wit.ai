# Wit.ai Bot Framework Integration

## Overview
This is an integration between Wit.ai and Microsoft Bot Framework. Wit.ai allows users to create full fledged dialogs that can contain placeholders for actions, with expected inputs and outputs. This integration will facilitate the implementation of the actions, and automatically passes the message responses, leveraging the Wit.ai context variables, from Wit.ai bot engine directly to the user.

This is a fork of q3blend's [BotBuilder-Wit.ai repository](https://github.com/q3blend/BotBuilder-Wit.ai).
If you want support for the POST /converse endpoint, use q3blend's lib.
If you want support for the GET /message endpoint, use this lib.

## Getting Started

* Go to Wit.ai to an existing application, or create a new one.
* Go to Settings tab, and copy the Server Access Token.
* Open Web.Config file and edit Wit.ApiKey (default)
```xml
    <add key="Wit.ApiKey" value="Access Token" />
```
* If you have multi-langue wit.ai bot, you can add several key in your web.config. Each key must have this pattern Wit.ApiKey_langue, for exemple:
```xml
    <add key="Wit.ApiKey_fr" value="Access Token" />
```
* Define wit.ai configuration to indicate what key use with a language
```csharp
WitLocator.Instance.Register<IWitConfig>(new WitConfig
{
	WitConfigDictionary = new Dictionary<CultureInfo, string>
	{
		{ new CultureInfo("fr"), ConfigurationManager.AppSettings[$"Wit.ApiKey_fr"] },
		{ new CultureInfo("en"), ConfigurationManager.AppSettings[$"Wit.ApiKey_en"] }
	}
});
```

* To define intent handlers for the actions defined in your Wit.ai application, decorate the handler methods like below:
```csharp
[WitIntent("Intent Name")]
```
where the intent name is how you defined it in your application.

## Use Case and Features
This is useful when you want to create a bot using wit.ai for language understanding and conversation flow. Microsoft Bot Framework is useful for making it easy to publish on several channels and having a good code structure. It's a great place to implement your action and update the wit context variables as well.

## Sample
### Weather App
The WitWeather sample uses [Wit.ai weather application](https://wit.ai/q3blend/weatherApp).

![alt tag](https://i.imgur.com/vtVQAYf.png)

We can see here that "getMyForecast" action needs to be executed. It is expected that location and forecast context variables will be added/updated in "getMyForecast" action like below:

```csharp
[Serializable]
[WitModel("Access Token")]
public class WeatherDialog : WitDialog
```

First, we added the Server Access Token. Now, we need to implement the "getMyForecast" action, which happens here:

```csharp
        [WitIntent("getMyForecast")]
        public async Task GetForecast(IDialogContext context, WitResult result)
        {
            //adding location to context
            Context["location"] =  result.Entities["location"][0].Value;

            //yahoo weather API
            var temp = await GetWeather(Context["location"]);

            //adding temp to context
            Context["forecast"] =  temp;
        }
```

## NuGet

https://www.nuget.org/packages/WoodenMoose.Bot.Builder.Witai/

## More Information
Read these resources for more information about the Microsoft Bot Framework, Bot Builder SDK and Wit.ai Services:

* [Microsoft Bot Framework Overview](https://docs.botframework.com/en-us/)
* [Microsoft Bot Framework Bot Builder SDK](https://github.com/Microsoft/BotBuilder)
* [Microsoft Bot Framework Samples](https://github.com/Microsoft/BotBuilder-Samples)
* [Wit.ai Converse REST Services Documentation](https://wit.ai/docs/http/20160526#post--converse-link)
