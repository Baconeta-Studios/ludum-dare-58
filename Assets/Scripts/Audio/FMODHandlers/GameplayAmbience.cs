using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Audio.FMODHandlers
{
    public class GameplayAmbience : MonoBehaviour
    {
        private EventInstance bgMusic;

        void Start()
        {
            RuntimeManager.LoadBank("Gameplay", true);
            
            bgMusic = RuntimeManager.CreateInstance("event:/Ambience/SpookyAmbience");
            bgMusic.start();
        }

        void OnDestroy()
        {
            bgMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            bgMusic.release();
            
            RuntimeManager.UnloadBank("Gameplay");
        }
    }
}
