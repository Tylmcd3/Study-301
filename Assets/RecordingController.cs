using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum recordingButton
{
    Start, 
    Stop,
    Replay,
}

public class RecordingController : MonoBehaviour
{
    public recordingButton buttonType;
    public GameObject Recorder;

    private RecordingMaster _recordingMaster;
    // Start is called before the first frame update
    void Start()
    {
        _recordingMaster = Recorder.GetComponent<RecordingMaster>();
        Material mat;
        mat = Instantiate(GetComponent<Renderer>().material);

        switch (buttonType)
        {
            case recordingButton.Start:
                Debug.Log(buttonType);
                mat.color = Color.green;
                break;
            case recordingButton.Replay:
                Debug.Log(buttonType);
                mat.color = Color.blue;
                break;
            case recordingButton.Stop:
                Debug.Log(buttonType);
                mat.color = Color.red;
                break;
            
        }

        GetComponent<Renderer>().material = mat;

    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (buttonType)
        {
            case recordingButton.Start:
                Debug.Log("Start");

                RecordingMaster.SetRecordStartTime(Time.realtimeSinceStartup);
                break;
            case recordingButton.Stop:
                Debug.Log("Stop");

                RecordingMaster.CloseRecording();
                break;
            case recordingButton.Replay:
                Debug.Log("Replay");

                RecordingMaster.Replay();
                break;
        }
    }
}
