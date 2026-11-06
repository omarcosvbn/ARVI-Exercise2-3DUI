using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace ARVI{
    public class MediaBrowser : MonoBehaviour{
        [Serializable]
        public class MediaItem{
            public string title;
            public Texture2D image;
            public VideoClip video;
        }

        [Header("Manual hierarchy references")]
        [SerializeField] Renderer screen;
        [SerializeField] VideoPlayer videoPlayer;

        [Header("Media list")]
        [SerializeField] List<MediaItem> media = new List<MediaItem>();

        void Awake(){
            if (videoPlayer != null){
                videoPlayer.playOnAwake = false;
                videoPlayer.source = VideoSource.VideoClip;
                videoPlayer.prepareCompleted += OnVideoPrepared;
            }

            Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include);
            for (int i = 0; i < transforms.Length; i++){
                if (transforms[i].name.StartsWith("Button") &&
                    transforms[i].GetComponent<MediaButtonVisuals>() == null){
                    transforms[i].gameObject.AddComponent<MediaButtonVisuals>();
                }
            }
        }

        void OnDestroy(){
            if (videoPlayer != null) videoPlayer.prepareCompleted -= OnVideoPrepared;
        }

        public void SelectMedia(int index){
            if (index < 0 || index >= media.Count){
                return;
            }

            var item = media[index];

            if (videoPlayer != null) videoPlayer.Stop();

            if (item.video != null){
                videoPlayer.clip = item.video;

                screen.material.mainTexture = videoPlayer.targetTexture;
                videoPlayer.Prepare();
                return;
            }

            screen.material.mainTexture = item.image;
        }

        void OnVideoPrepared(VideoPlayer player){
            player.Play();
        }
    }
}
