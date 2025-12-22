using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MESSAGE
{
    public enum ROLE
    {
        [EnumMember(Value = "system")]
        system,
        [EnumMember(Value = "user")]
        user,
        [EnumMember(Value = "assistant")]
        assistant
    }

    [JsonProperty("role"), JsonConverter(typeof(StringEnumConverter)), XmlAttribute("role")]
    public ROLE role { get; set; }
    [JsonProperty("content")]
    public string content = "";
}

public class CHOICES
{
    [JsonProperty("finish_reason")]
    public string finish_reason;
    [JsonProperty("index")]
    public int index;
    [JsonProperty("message")]
    public MESSAGE message;
}

public class USAGE
{
    [JsonProperty("completion_tokens")]
    public int completion_tokens;
    [JsonProperty("prompt_tokens")]
    public int prompt_tokens;
    [JsonProperty("total_tokens")]
    public int total_tokens;
}

public class CHAT_API
{
    [JsonProperty("model")]
    public string Model = "gpt-3.5-turbo-0301";
    //public string Model = "gpt-4";          //https://help.openai.com/en/articles/7102672-how-can-i-access-gpt-4
    [JsonProperty("messages")]
    public List<MESSAGE> messages = new List<MESSAGE>();
    //추가 옵션
    //[JsonProperty("temperature"), XmlAttribute("temperature")]
    //public int temperature = 1;
    //[JsonProperty("top_p"), XmlAttribute("top_p")]
    //public int top_p = 1;
    //[JsonProperty("n"), XmlAttribute("n")]
    //public int n = 1;
    //[JsonProperty("stream"), XmlAttribute("stream")]
    //public bool stream = false;
    //[JsonProperty("stop")]
    //public List<string> stop;
    [JsonProperty("max_tokens")]
    public int max_tokens = 500;
    //[JsonProperty("presence_penalty"), XmlAttribute("presence_penalty")]
    //public int presence_penalty = 0;
    //[JsonProperty("frequency_penalty"), XmlAttribute("frequency_penalty")]
    //public int frequency_penalty = 0;
    //[JsonProperty("logit_bias"), XmlAttribute("logit_bias")]
    //public int logit_bias = 1;
    //[JsonProperty("user"), XmlAttribute("user")]
    //public string user = "00";
    [JsonIgnore]
    public List<MESSAGE> ConceptSettingMessages { get; set; } = new List<MESSAGE>();
    public void Resetmessages()
    {
        messages.Clear();
        messages = new List<MESSAGE>(ConceptSettingMessages);
    }
}

public class CHAT_COMPLETE
{
    [JsonProperty("id")]
    public string id;
    [JsonProperty("choices")]
    public List<CHOICES> choices;
    [JsonProperty("created")]
    public int created;
    [JsonProperty("model")]
    public string model;
    [JsonProperty("system_fingerprint")]
    public string system_fingerprint;
    [JsonProperty("object")]
    public string type;
    [JsonProperty("usage")]
    public USAGE usage;
}

public class DefineMessageClass
{
    [JsonProperty("message")]
    public List<MESSAGE> DefineMessage = new List<MESSAGE>();

    public void RefineMessageData(ref List<MESSAGE> messages)
    {
        try
        {
            foreach(var data in DefineMessage)
            {
                messages.Add(new MESSAGE { role = data.role, content = data.content });
            }
        }
        catch (NullReferenceException e)
        {
            throw new Exception("Null Data" + e);
        }
    }

    public void DefineMessageData(List<MESSAGE> messages)
    {
        try
        {
            foreach (var data in messages)
            {
                DefineMessage.Add(new MESSAGE { role = data.role, content = data.content });
            }
        }
        catch (NullReferenceException e)
        {
            throw new Exception("Null Data" + e);
        }
    }
}

public class ChatGPT : MonoBehaviour
{
    readonly string url = "https://api.openai.com/v1/chat/completions";
    string key = string.Empty;//"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";

    string Location = "//define.json";
    StringBuilder SB = new StringBuilder();

    CHAT_API chat = new CHAT_API();
    DefineMessageClass defineChat = new DefineMessageClass();
    DefineMessageClass SaveChat = new DefineMessageClass();

    public TextMeshProUGUI TitleText;
    public TMP_InputField UserText;
    public TextMeshProUGUI ChatGPTText;

    public Button SendButton;

    public TMP_InputField input_Question;

    public Button SaveButton;

    readonly float PRD = 200;       // REQUESTS PER DAY : 일당 요청 수
    readonly float RPM = 3;         // REQUESTS PER MINUTE : 분당 요청 수

    bool isChat = true;
    bool IsChat
    {
        set
        {
            if (isChat == value) return;
            isChat = value;
            IsChatButton();
        }
        get
        {
            return isChat;
        }
    }
    public bool isLoading = false;

    private void Start()
    {
        key = Suncheon.GameManager.Instance.defaultData.openAIKey;
        //Location = Application.streamingAssetsPath + Location;
        Location = "Assets/Resources/" + Location;
        SendButton.onClick.AddListener(OnClickSendButton);
        input_Question.onEndEdit.AddListener(delegate { OnClickSendButton(); });

        if (SaveButton != null)
        {
            SaveButton.onClick.AddListener(() =>
            {
                SaveChat.DefineMessageData(chat.messages);
                JsonPaser.Save(Location, SaveChat);
            });
        }

        StartCoroutine(loadStreamingAsset(Location));
        //OnLoad();
    }

    void OnLoad()
    {
        defineChat = JsonPaser.Load<DefineMessageClass>(Location);
        defineChat.RefineMessageData(ref chat.messages);
    }

    void OnClickSendButton()
    {
        if(IsChat)
        {
            IsChat = false;
            AddMessage();
            SendChatGPT();
        }
    }
    
    void IsChatButton()
    {
        if(SendButton != null)
        SendButton.interactable = IsChat;
    }

    #region RPM타이머

    IEnumerator CheckTime()
    {
        float time_current = 0;
        float time_start = (float)System.DateTime.Now.TimeOfDay.TotalSeconds;
        while (true)
        {
            time_current = (float)System.DateTime.Now.TimeOfDay.TotalSeconds - time_start;
            if(time_current > 60)
            {
                IsChat = true;
                isLoading = false;
                ChatGPTText.text = "다시 한 번 질문해주세요!";
                break;
            }
            yield return null;
        }
        yield return null;
    }
    #endregion

    void AddMessage()
    {
        chat.messages.Add(new MESSAGE { role = MESSAGE.ROLE.user, content = UserText.text });
        TitleText.text = UserText.text;
        UserText.text = "";
    }

    void RemoveMessage(int number, int count)
    {
        chat.messages.RemoveRange(number, count);
    }

    void resetMessage()
    {
        chat.Resetmessages();
    }

    void SendChatGPT()
    {
        string json = JsonConvert.SerializeObject(chat);
        //Debug.Log(json);

        StartCoroutine(Post(json));
    }

    void SetChatGPTText(string json)
    {
        CHAT_COMPLETE GPTchat = new CHAT_COMPLETE();

        GPTchat = JsonConvert.DeserializeObject<CHAT_COMPLETE>(json);

        if(GPTchat.choices.LastOrDefault().finish_reason == "length")
        {
            ChatGPTText.text = "다시 한 번 질문해주세요!";
        }
        else if(GPTchat.choices.LastOrDefault().finish_reason == "stop")
        {
            StartCoroutine(TypeTextEffect(GPTchat.choices.LastOrDefault().message.content));
            chat.messages.Add(new MESSAGE { role = MESSAGE.ROLE.assistant, content = ChatGPTText.text });
        }
    }

    IEnumerator TypeTextEffect(string text)
    {
        ChatGPTText.text = string.Empty;

        SB.Length = 0;
        for(int i = 0; i < text.Length; i++)
        {
            SB.Append(text[i]);
            ChatGPTText.text = SB.ToString();

            //if(i % 8 == 0)
            //{
            //    SoundManager.instance.PlaySFX("Keyboard");
            //}
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator Post(string json)
    {
        string URL = url;
        StartCoroutine(LoadingGPT());

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(URL, string.Empty))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + key);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                //Debug.Log(www.error + " " + www.downloadHandler.text);
                yield return StartCoroutine(CheckTime());
            }
            else
            {
                isLoading = false;
                IsChat = true;
                SetChatGPTText(www.downloadHandler.text);
                //Debug.Log(www.downloadHandler.text);
            }
        }
    }

    IEnumerator LoadingGPT()
    {
        isLoading = true;

        char a = '.';
        int j = 1;

        while (isLoading)
        {
            ChatGPTText.text = "생각중";
            for (int i = 0; i < j; i++)
            {
                ChatGPTText.text += a;
            }

            if(j == 3)
            {
                j = 1;
            }
            else
            {
                j++;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator loadStreamingAsset(string fileName)
    {
        //string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        string filePath = fileName;

        string result;
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            WWW www = new WWW(filePath);
            yield return www;
            result = www.text;
            defineChat = JsonConvert.DeserializeObject<DefineMessageClass>(result);
            defineChat.RefineMessageData(ref chat.messages);
        }
        else
        {
            result = System.IO.File.ReadAllText(filePath);
            defineChat = JsonConvert.DeserializeObject<DefineMessageClass>(result);
            defineChat.RefineMessageData(ref chat.messages);
        }
    }
}


static public class JsonPaser
{
    static public T Load<T>(string FileLocation) where T : class
    {
        try
        {
            var jsonFile = File.ReadAllText(FileLocation);
            var data = JsonConvert.DeserializeObject<T>(jsonFile) as T;
            //var data = JsonUtility.FromJson<T>(jsonFile) as T;
            //Debug.Log("Load Success");
            return data;
        }
        catch(Exception e)
        {
            throw new JsonException("error Load" + e);
        }
    }

    static public void Save<T>(string FileLocation, T data) where T : class
    {
        try
        {
            //var content = JsonUtility.ToJson(data);
            var content = JsonConvert.SerializeObject(data, Formatting.Indented);
            //Debug.Log(content);
            File.WriteAllText(FileLocation, content);
        }
        catch(Exception e)
        {
            //Debug.LogError(e);
        }
    }
}
