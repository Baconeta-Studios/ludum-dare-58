using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Audio.FMODHandlers
{
    public class StartMenuAmbience : MonoBehaviour
    {
        private EventInstance bgMusic;

        void Start()
        {
            RuntimeManager.LoadBank("StartMenu", true);
            
            bgMusic = RuntimeManager.CreateInstance("event:/Ambience/NoirAmbience");
            bgMusic.start();
        }

        void OnDestroy()
        {
            bgMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            bgMusic.release();
            
            RuntimeManager.UnloadBank("StartMenu");
        }
    }
}
