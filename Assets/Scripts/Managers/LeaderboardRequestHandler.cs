using System;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Utils;

namespace Managers
{
    public class LeaderboardRequestManager : EverlastingSingleton<LeaderboardRequestManager>
    {
        public bool allowSubmitScoreInEditor;
        public bool doYouReallyWantToSubmitInEditor;

        private const string RootUri = "http://5.78.45.57:3000";
        private const string GetScoresUriSingleplayer = RootUri + "/api/individual/singleplayer";
        private const string GetScoresUriMultiplayer = RootUri + "/api/individual/multiplayer";
        private const string SubmitScoreUriSingleplayer = RootUri + "/api/leaderboards/singleplayer?user={0}&score={1}";
        private const string SubmitScoreUriMultiplayer = RootUri + "/api/leaderboards/multiplayer?user={0}&score={1}";

        public ScoreCollection singleplayerScores;
        public ScoreCollection multiplayerScores;

        // The callback used to update text with leaderboard information that we have retrieved from the server.
        private delegate void Callback(ScoreCollection entryList, ScoreType scoreType);

        public static event Action<ScoreCollection> OnSingleplayerScoresUpdated;
        public static event Action<ScoreCollection> OnMultiplayerScoresUpdated;

        private static string GetEndpoint(bool submit, ScoreType scoreType)
        {
            return scoreType switch
            {
                ScoreType.Singleplayer => submit ? SubmitScoreUriSingleplayer : GetScoresUriSingleplayer,
                ScoreType.Multiplayer => submit ? SubmitScoreUriMultiplayer : GetScoresUriMultiplayer,
                _ => ""
            };
        }

        // Get a list of scores from the leaderboard server.
        private async Task<ScoreCollection> GetScores(ScoreType scoreType)
        {
            string endpoint = GetEndpoint(true, scoreType);
            using UnityWebRequest req = UnityWebRequest.Get(endpoint);
            var operation = req.SendWebRequest();

            // This is not a busy-loop, control is yielded back to the main-thread until the operation is complete.
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            switch (req.result)
            {
                case UnityWebRequest.Result.Success:
                    // Query succeeded. Convert from JSON string to objects, and then execute the callback.
                    string data = req.downloadHandler.text;
                    return JsonUtility.FromJson<ScoreCollection>("{\"highScores\": " + data + "}");
                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log("A connection error occurred during the leaderboard request.");
                    Debug.Log(req.responseCode);
                    Debug.Log(req.error);
                    throw new WebException("A protocol error occurred during the leaderboard request.");
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("A protocol error occurred during the leaderboard request.");
                    Debug.Log(req.responseCode);
                    Debug.Log(req.error);
                    throw new WebException("A protocol error occurred during the leaderboard request.");
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log("A data processing error occurred during the leaderboard request.");
                    throw new WebException("A data processing error occurred during the leaderboard request.");
                case UnityWebRequest.Result.InProgress:
                default:
                    Debug.Log("A error occurred during the leaderboard request: " + req.result + ".");
                    throw new WebException("A error occurred during the leaderboard request: " + req.result + ".");
            }
        }

        /**
         * Submit a user's score to the leaderboard server.
         * Starts a coroutine to make the request.
         */
        public async void SubmitScore(ScoreType scoreType, ScoreEntry scoreEntry)
        {
            try
            {
                if (!ShouldSubmitScore())
                {
                    return;
                }

                if (string.IsNullOrEmpty(scoreEntry.user))
                {
                    Debug.Log("Not submitting a score as there is no player name.");
                    return;
                }

                if (double.IsNaN(scoreEntry.score))
                {
                    Debug.Log("Not submitting the score as there is no score.");
                    return;
                }

                string endpoint = GetEndpoint(true, scoreType);
                string uri = string.Format(endpoint, scoreEntry.user, scoreEntry.score);

                using UnityWebRequest request = UnityWebRequest.PostWwwForm(uri, "");
                var operation = request.SendWebRequest();

                // This is not a busy-loop, control is yielded back to the main-thread until the operation is complete.
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                switch (request.result)
                {
                    case UnityWebRequest.Result.Success:
                        string data = request.downloadHandler.text;
                        // Query succeeded. Convert from JSON string to objects, and then execute the callback.
                        Debug.Log(data);
                        break;
                    case UnityWebRequest.Result.InProgress:
                        Debug.Log("Query is in progress.");
                        break;
                    case UnityWebRequest.Result.ConnectionError:
                        Debug.Log("A connection error occurred.");
                        Debug.Log(request.responseCode);
                        Debug.Log(request.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.Log("A protocol error occurred.");
                        Debug.Log(request.responseCode);
                        Debug.Log(request.error);
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.Log("A data processing error occurred.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /**
         * Helper function to disable score submission in the Unity Editor.
         * If both booleans are true, then score submission will be forced-on.
         */
        private bool ShouldSubmitScore()
        {
#if UNITY_EDITOR
            return allowSubmitScoreInEditor && doYouReallyWantToSubmitInEditor;
#else
            return true;
#endif
        }

        private void SaveData(ScoreCollection entryList, ScoreType scoreType)
        {
            if (scoreType == ScoreType.Singleplayer)
            {
                singleplayerScores = entryList;
                OnSingleplayerScoresUpdated?.Invoke(singleplayerScores);
            }
            else if (scoreType == ScoreType.Multiplayer)
            {
                multiplayerScores = entryList;
                OnMultiplayerScoresUpdated?.Invoke(multiplayerScores);
            }
        }
    }

    [Serializable]
    public enum ScoreType
    {
        Singleplayer,
        Multiplayer
    }

    // For JSON de/serialization.
    [Serializable]
    public struct ScoreCollection
    {
        public ScoreEntry[] scores;
    }

    // For JSON de/serialization.
    [Serializable]
    public struct ScoreEntry
    {
        public string user;
        public ulong score;
    }
}