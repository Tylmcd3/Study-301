using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecordingDebugController : MonoBehaviour
{

    //public Whiteboard wb;
    //static bool runOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        //if (!runOnce) StartCoroutine(nameof(DrawRoutine));
        //runOnce = true;
    }

    public KeyCode recordStartKey = KeyCode.F6;
    public KeyCode replayKey = KeyCode.F7;

	private void Update()
	{
        if (Input.GetKeyDown(recordStartKey))
		{
            RecordingMaster.SetRecordStartTime(Time.realtimeSinceStartup);
		}
		if (Input.GetKeyDown(replayKey))
		{
            RecordingMaster.CloseRecording();
            RecordingMaster.Replay();
        }
	}

	/*IEnumerator DrawRoutine()
	{
        yield return new WaitForSeconds(1.0f);
        Vector2 last = new Vector2(100, 100);
        Vector2 next = new Vector2(0, 0);
        Color[] c = Enumerable.Repeat(Color.red, 49).ToArray();
        for (int i = 0; i < 100; i++)
		{
            next = last + Time.deltaTime * Vector2.one * 500;
            wb.Draw((int)next.x, (int)next.y, 7, c, last);
            last = next;
            //Debug.Log(i);
            yield return new WaitForEndOfFrame();
        }

        //RecordingMaster.CloseRecording();
        //RecordingMaster.Replay();
	}*/
}
