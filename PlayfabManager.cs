using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class PlayfabManager
{
    private bool _running = true;
    public async void TryLogin(string username, string token)
    {
        PlayFabSettings.staticSettings.TitleId = "E9B77"; 

        var request = new LoginWithKongregateRequest { KongregateId = username, AuthTicket = token, CreateAccount = true, TitleId = "E9B77"  };
        var loginTask = await PlayFabClientAPI.LoginWithKongregateAsync(request);
        
         OnLoginComplete(loginTask);
    }

    private void OnLoginComplete(PlayFabResult<LoginResult> taskResult)
    {
        var apiError = taskResult.Error;

        if (apiError != null)
        {
            Console.ForegroundColor = ConsoleColor.Red; // Make the error more visible
            Console.WriteLine(PlayFabUtil.GenerateErrorReport(apiError));
            Console.ForegroundColor = ConsoleColor.Gray; // Reset to normal
        }
        _running = false; 
    }
    public async void Save(string data)
    {
        PlayFabResult<UpdateUserDataResult> result = await PlayFabClientAPI.UpdateUserDataAsync(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                    {"Save Data", data }
                }
        });
        if(result.Error != null)
        {
            Console.WriteLine(result.Error.GenerateErrorReport());
        }
        

    }
}
