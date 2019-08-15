using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class PlayfabManager
{
    private bool _running = true;
    private string loadString = "";
    public bool IsConnected = false;
    public async Task TryLogin(string userID, string token)
    {
        PlayFabSettings.staticSettings.TitleId = "E9B77"; 

        var request = new LoginWithKongregateRequest { KongregateId = userID, AuthTicket = token, CreateAccount = true, TitleId = "E9B77"  };
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
        else
        {
            IsConnected = true;
        }
        _running = false; 
    }
    public async void Save(string data)
    {
        if (IsConnected == false || data == null)
        {
            return;
        }
        string dataPart2 = "";
        if(data.Length > 4500)
        {
            dataPart2 = data.Substring(4500, data.Length - 4500);
            data = data.Substring(0, 4500);
        }
        PlayFabResult<UpdateUserDataResult> result = await PlayFabClientAPI.UpdateUserDataAsync(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                    {"Save Data", data },
                    {"Save Data2", dataPart2 }

                }
        });
        if(result.Error != null)
        {
            Console.WriteLine(result.Error.GenerateErrorReport());
        }
        

    }

    public async Task<string> LoadData()
    {
        loadString = "";
        PlayFabResult<GetUserDataResult> result = await PlayFabClientAPI.GetUserDataAsync(new GetUserDataRequest());
        loadString += result.Result.Data["Save Data"].Value;
        if(result.Result.Data.TryGetValue("Save Data2", out UserDataRecord extraData))
        {
            loadString += extraData.Value;
        }
        return loadString;
        
    }
}
