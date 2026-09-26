using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using EasyBuildSystem.Features.Runtime.Buildings.Part;
using EasyBuildSystem.Features.Runtime.Buildings.Manager;

public class PlayfabManager : MonoBehaviour
{
    private void Awake()
    {
        
    }

    private void Start()
    {
        Login();
    }

    private void HookInEvents()
    {
        GameManager gm = FindObjectOfType<GameManager>();

        if(gm != null)
        {
            gm.OnStartRound += () => EventRoundStarted(gm.Round());
            gm.OnEndRound += () => EventRoundOver(gm.Round(), gm.Enemies(), gm.Cash());
        }
        
        if(BuildingManager.Instance != null)
        {
            BuildingManager.Instance.OnPlacingBuildingPartEvent.AddListener((BuildingPart part) => EventTrapPlaced(part.name, gm.TimeElapsed(), gm.Cash(), part.transform.position));
            BuildingManager.Instance.OnDestroyingBuildingPartEvent.AddListener((BuildingPart part) => { 
                if(part.State == BuildingPart.StateType.DESTROY)
                    EventObjectDeleted(part.name, part.transform.position);
            });
        }
    }

    private void Login()
    {
        string customId = SystemInfo.deviceUniqueIdentifier;
#if UNITY_WEBGL
            Debug.Log("Thank you for using WebGL!");
            customId = "webgl_" + System.Guid.NewGuid().ToString();
#endif

        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        //Debug.Log("Successful login/account create!");
        HookInEvents();
    }

    void OnError(PlayFabError error)
    {
        //Debug.Log("Error while logging in/creating account!");
        Debug.Log(error.GenerateErrorReport());
    }

    void OnWritePlayerEvent(WriteEventResponse response)
    {
        //Debug.Log("Successfully sent request.");
    }

    public void EventPlayMainMenuPressed()
    {
        var request = new WriteClientPlayerEventRequest
        {
            EventName = "play_main_menu_pressed",
        };
        PlayFabClientAPI.WritePlayerEvent(request, OnWritePlayerEvent, OnError);
    }

    public void EventHowToPlayPressed()
    {
        var request = new WriteClientPlayerEventRequest
        {
            EventName = "how_to_play_pressed",
        };
        PlayFabClientAPI.WritePlayerEvent(request, OnWritePlayerEvent, OnError);
    }

    public void EventSettingsPressed()
    {
        var request = new WriteClientPlayerEventRequest
        {
            EventName = "settings_pressed",
        };
        PlayFabClientAPI.WritePlayerEvent(request, OnWritePlayerEvent, OnError);
    }

    public void EventRoundStarted(int roundNumber)
    {
        // Call when player hits round started.
        var request = new WriteClientPlayerEventRequest
        {
            EventName = "round_started",
            Body = new Dictionary<string, object>
            {
                { "round_number", roundNumber }
            }
        };
        PlayFabClientAPI.WritePlayerEvent(request, OnWritePlayerEvent, OnError);
    }

    public void EventTrapPlaced(string trapName, float timeRemaining, float currency_remaining, Vector3 position)
    {
        // Call when player places a trap and what time they placed it.
        // If before round started, just send the what the current round time is going to be.

        var request = new WriteClientPlayerEventRequest
        {
            EventName = "trap_placed",
            Body = new Dictionary<string, object>
            {
                { "trap_name", trapName }, {"time_remaining", timeRemaining }, {"currency_remaining", currency_remaining }, {"pos_x", position.x}, { "pos_y", position.y }, {"pos_z", position.z}
            }
        };
        PlayFabClientAPI.WritePlayerEvent(request, OnWritePlayerEvent, OnError);
    }

    public void EventObjectDeleted(string name, Vector3 position)
    {
        // Call when player places a trap and what time they placed it.
        // If before round started, just send the what the current round time is going to be.

        var request = new WriteClientPlayerEventRequest
        {
            EventName = "object_deleted",
            Body = new Dictionary<string, object>
            {
                { "name", name }, {"pos_x", position.x}, { "pos_y", position.y }, {"pos_z", position.z}
            }
        };
        PlayFabClientAPI.WritePlayerEvent(request, OnWritePlayerEvent, OnError);
    }

    public void EventRoundOver(int roundNumber, int animalsRemaining, int currency_remaining)
    {
        var request = new WriteClientPlayerEventRequest
        {
            EventName = "round_ended",
            Body = new Dictionary<string, object>
            {
                {"round_number", roundNumber }, {"animals_remaining", animalsRemaining}, {"currency_remaining", currency_remaining }
            }
        };
        PlayFabClientAPI.WritePlayerEvent(request, OnWritePlayerEvent, OnError);
    }

    //public void EventRagdoll(string trapName, string animalName, Vector3 position)
    //{
    //    var request = new WriteClientPlayerEventRequest
    //    {
    //        EventName = "ragdoll",
    //        Body = new Dictionary<string, object>
    //        {
    //            {"trap_name", trapName }, {"animal_name", animalName }, {"pos_x", position.x}, { "pos_y", position.y }, {"pos_z", position.z}
    //        }
    //    };
    //    PlayFabClientAPI.WritePlayerEvent(request, OnWritePlayerEvent, OnError);
    //}

    //public void EventAnimalFell(string animalName, int animalsRemaining, Vector3 position)
    //{
    //    var request = new WriteClientPlayerEventRequest
    //    {
    //        EventName = "animal_fell",
    //        Body = new Dictionary<string, object>
    //        {
    //            {"animal_name", animalName }, {"animals_remaining", animalsRemaining }, {"pos_x", position.x}, { "pos_y", position.y }, {"pos_z", position.z}
    //        }
    //    };
    //    PlayFabClientAPI.WritePlayerEvent(request, OnWritePlayerEvent, OnError);
    //}

}