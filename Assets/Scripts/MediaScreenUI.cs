using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ARVI
{
    public class MediaScreenUI : MonoBehaviour
    {
        [Serializable]
        public class MediaItem
        {
            public string title;
            public Texture2D image;
            public VideoClip video;
        }

        [SerializeField] List<MediaItem> m_MediaItems = new List<MediaItem>();
        [SerializeField] Vector3 m_Position = new Vector3(0f, 1.6f, 3.2f);
        readonly List<Image> m_SelectionBackgrounds = new List<Image>();
        RawImage m_Screen;
        VideoPlayer m_VideoPlayer;
        RenderTexture m_VideoTexture;

        void Start()
        {
            BuildInterface();
            if (m_MediaItems.Count > 0)
                SelectMedia(0);
        }

        void BuildInterface()
        {
            var canvasObject = new GameObject("Media Screen UI");
            canvasObject.transform.SetPositionAndRotation(m_Position, Quaternion.identity);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            canvasObject.AddComponent<GraphicRaycaster>();
            var canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(800f, 450f);
            canvasRect.localScale = Vector3.one * 0.001f;

            var panel = CreatePanel(canvasObject.transform, "Panel", new Vector2(800f, 450f), Vector2.zero, new Color(0.035f, 0.05f, 0.09f, 0.96f));
            m_Screen = CreateRawImage(panel.transform, "Virtual Screen", new Vector2(624f, 351f), new Vector2(110f, 0f));
            var listPanel = CreatePanel(panel.transform, "Media List", new Vector2(210f, 390f), new Vector2(-285f, 0f), new Color(0.06f, 0.08f, 0.13f, 1f));
            CreateLabel(listPanel.transform, "Available media", new Vector2(180f, 36f), new Vector2(0f, 165f), 22);

            var list = listPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            list.padding = new RectOffset(16, 16, 58, 16);
            list.spacing = 10f;
            list.childControlHeight = true;
            list.childControlWidth = true;
            list.childForceExpandHeight = false;

            for (var i = 0; i < m_MediaItems.Count; i++)
            {
                var buttonObject = new GameObject(m_MediaItems[i].title);
                buttonObject.transform.SetParent(listPanel.transform, false);
                buttonObject.AddComponent<RectTransform>().sizeDelta = new Vector2(178f, 52f);
                var background = buttonObject.AddComponent<Image>();
                background.color = new Color(0.12f, 0.16f, 0.23f, 1f);
                var button = buttonObject.AddComponent<Button>();
                var index = i;
                button.onClick.AddListener(() => SelectMedia(index));
                CreateLabel(buttonObject.transform, m_MediaItems[i].title, new Vector2(160f, 42f), Vector2.zero, 16);
                m_SelectionBackgrounds.Add(background);
            }

            m_VideoPlayer = canvasObject.AddComponent<VideoPlayer>();
            m_VideoPlayer.playOnAwake = false;
            m_VideoPlayer.isLooping = true;
            m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        }

        void SelectMedia(int index)
        {
            if (index < 0 || index >= m_MediaItems.Count)
                return;

            for (var i = 0; i < m_SelectionBackgrounds.Count; i++)
                m_SelectionBackgrounds[i].color = i == index ? new Color(0.12f, 0.62f, 0.94f, 1f) : new Color(0.12f, 0.16f, 0.23f, 1f);

            var item = m_MediaItems[index];
            m_VideoPlayer.Stop();
            m_VideoPlayer.clip = item.video;
            if (m_VideoTexture != null)
            {
                m_VideoTexture.Release();
                Destroy(m_VideoTexture);
            }

            if (item.video != null)
            {
                m_VideoTexture = new RenderTexture(1280, 720, 0);
                m_VideoTexture.Create();
                m_VideoPlayer.targetTexture = m_VideoTexture;
                m_Screen.texture = m_VideoTexture;
                m_VideoPlayer.Play();
            }
            else
            {
                m_VideoPlayer.targetTexture = null;
                m_Screen.texture = item.image;
            }
        }

        static Image CreatePanel(Transform parent, string name, Vector2 size, Vector2 position, Color color)
        {
            var panelObject = new GameObject(name);
            panelObject.transform.SetParent(parent, false);
            var rect = panelObject.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            var image = panelObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        static RawImage CreateRawImage(Transform parent, string name, Vector2 size, Vector2 position)
        {
            var imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);
            var rect = imageObject.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            return imageObject.AddComponent<RawImage>();
        }

        static TextMeshProUGUI CreateLabel(Transform parent, string text, Vector2 size, Vector2 position, float fontSize)
        {
            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(parent, false);
            var rect = labelObject.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            var label = labelObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            return label;
        }

        void OnDestroy()
        {
            if (m_VideoTexture != null)
                m_VideoTexture.Release();
        }
    }
}
