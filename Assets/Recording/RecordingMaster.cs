using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class STUDYKeyframer
{
    Dictionary<float, object> frames;
    Action<object> playback;
    Type parameter;
    string owner;
    string method;

    

    public STUDYKeyframer(string o, string m, Action<object> p, Type param)
	{
        frames = new Dictionary<float, object>();
        playback = p;
        owner = o;
        method = m;
        parameter = param;

        RecordingMaster.RegisterKeyframer(this);
	}

    public void AddFrame(object data)
	{
        frames.Add(Time.realtimeSinceStartup - RecordingMaster.lastReboot, data);
	}

    public string Serialize()
	{
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<float, object> kvp in frames)
		{
            sb.Append($"{JsonUtility.ToJson(new KeyFrameMetadata { time = kvp.Key, owner = owner, method = method})}\n");
            sb.Append($"{JsonUtility.ToJson(kvp.Value)}\n");
		}
        return sb.ToString();
	}

    public void Perform(string instruction)
	{
        object param = JsonUtility.FromJson(instruction, parameter);
        playback(param);
	}

	internal bool IsKeyframer(string owner, string method)
	{
        return owner == this.owner && method == this.method;
	}

	internal void MergeFrames(SortedDictionary<KeyFrameMetadata, string> serialFrames)
	{
        foreach (KeyValuePair<float, object> kvp in frames)
        {
            serialFrames.Add(new KeyFrameMetadata { time = kvp.Key, owner = owner, method = method }, JsonUtility.ToJson(kvp.Value));
        }
    }
}

public struct KeyFrameMetadata: IComparable<KeyFrameMetadata>
{
    public float time;
    public string owner;
    public string method;

	public int CompareTo(KeyFrameMetadata obj)
	{
        return (time < obj.time) ? 0 : 1;
	}
}

public struct RecordingMetadata
{
    public float startTime;
}

public class RecordingMaster: MonoBehaviour
{
    static List<STUDYKeyframer> kfs = new List<STUDYKeyframer>();

    static string tmp_string = "";
    static float startTime = 0;
    public static float lastReboot;

    public static void RegisterKeyframer(STUDYKeyframer kf)
	{
        kfs.Add(kf);
	}

    public static void SetRecordStartTime(float t)
	{
        startTime = t - lastReboot;
        Debug.Log($"{t}, {startTime}");
	}        

    public static void CloseRecording()
	{
        StringBuilder sb = new StringBuilder();

        sb.Append(JsonUtility.ToJson(new RecordingMetadata
        {
            startTime = startTime,
        }) + '\n');

        SortedDictionary<KeyFrameMetadata, string> frames = new SortedDictionary<KeyFrameMetadata, string>();

        foreach (STUDYKeyframer kf in kfs)
		{
            kf.MergeFrames(frames);
        }

        foreach (KeyValuePair<KeyFrameMetadata, string> kvp in frames)
		{
            sb.Append($"{JsonUtility.ToJson(kvp.Key)}\n");
            sb.Append($"{kvp.Value}\n");
        }
        //Debug.Log(tmp_string);

        tmp_string = sb.ToString();
	}

    public static void Replay()
	{
        SceneManager.LoadScene(1);
        state = State.REPLAYING;
	}

    IEnumerator ReplayRecording()
    {
        yield return new WaitForEndOfFrame();
        string record = tmp_string;

        string timeString = record.Substring(0, record.IndexOf('\n'));
        record = record.Substring(record.IndexOf('\n') + 1);
        RecordingMetadata rmd = JsonUtility.FromJson<RecordingMetadata>(timeString);
        float time = rmd.startTime;

        bool deserializing = true;
        int caret = record.IndexOf('\n');
        SortedDictionary<KeyFrameMetadata, string> frames = new SortedDictionary<KeyFrameMetadata, string>();
        while (deserializing)
        {
            string meta = record.Substring(0, caret);
            record = record.Substring(caret + 1);

            caret = record.IndexOf('\n');
            string data = record.Substring(0, caret);
            record = record.Substring(caret + 1);

            KeyFrameMetadata kfmd = JsonUtility.FromJson<KeyFrameMetadata>(meta);
            frames.Add(kfmd, data);

            caret = record.IndexOf('\n');
            if (caret < 0) deserializing = false;
        }

        //Set up video capture
        var settings = new VideoEncoderSettings(width: 1920, height: 1080, framerate: 30, codec: VideoCodec.H264);
        settings.EncoderPreset = EncoderPreset.Fast;
        settings.CRF = 17;

        Texture2D writeTex = new Texture2D(1080, 720);
        Camera cam = GetComponentInChildren<Camera>();
        RenderTexture rt = cam.targetTexture;

        using (var file = MediaBuilder.CreateContainer(@"F:\example.mp4").WithVideo(settings).Create())
        {
            foreach (KeyValuePair<KeyFrameMetadata, string> kvp in frames)
            {
                while (kvp.Key.time > time)
                {
                    time += Time.deltaTime;
                    //Debug.Log($"anim time is {time}, but frame time is still {kvp.Key.time}");
                    //Capture Frame
                    cam.Render();
                    RenderTexture.active = rt;
                    writeTex.ReadPixels(new Rect(0, 0, 1080, 720), 0, 0);
                    
                    Span<byte> tex = writeTex.GetRawTextureData().AsSpan();

                    file.Video.AddFrame(new FFMediaToolkit.Graphics.ImageData(tex, FFMediaToolkit.Graphics.ImagePixelFormat.Rgba32, 1080, 720));

                    yield return new WaitForEndOfFrame();
                }
                FindKeyFramer(kvp.Key.owner, kvp.Key.method).Perform(kvp.Value);
            }
        }
        
    }

    public static STUDYKeyframer FindKeyFramer(string owner, string method)
	{
        foreach (STUDYKeyframer kf in kfs)
		{
            if (kf.IsKeyframer(owner, method)) return kf;
		}
        return null;
	}

    //Yeah, I'm using this hack singleton pattern again. Sorry.
    public static RecordingMaster singleton;
    enum State { RECORDING, REPLAYING}
    static State state = State.RECORDING;
	public void Start()
	{
        singleton = this;
        kfs.Clear();
        lastReboot = Time.realtimeSinceStartup;
        if (state == State.REPLAYING)
		{
            //TODO: Find out how to undo this
            FFmpegLoader.FFmpegPath = @".\ffmpeg\x86_64";
            Time.captureFramerate = 30;
            GetComponentInChildren<Camera>().targetTexture = new RenderTexture(1080, 720, 32);
            singleton.StartCoroutine(nameof(ReplayRecording));
        }
	}
}