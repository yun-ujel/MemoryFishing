using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    [Space, SerializeField] private VideoPlayer player;

    [Header("Blackout")]
    [SerializeField] private RawImage screenBlackout;

    [Space, SerializeField, Range(0f, 2.0f)] private float blackoutTime;

    public event System.EventHandler<System.EventArgs> OnCutsceneFinishedEvent;

    private float counter = 0f;
    private bool cutsceneQueued;
    private bool cutscenePlaying;

    private float Alpha { set => screenBlackout.color = new Color(0, 0, 0, value); }

    public void StartCutscene()
    {
        cutsceneQueued = true;
        counter = 0f;
        player.loopPointReached += LoopPointReached;
    }

    private void LoopPointReached(VideoPlayer source)
    {
        source.Stop();
        cutsceneQueued = false;
        cutscenePlaying = false;
        counter = 0f;

        player.loopPointReached -= LoopPointReached;
        OnCutsceneFinishedEvent?.Invoke(this, new());
    }

    private void Update()
    {
        if (cutscenePlaying)
        {
            return;
        }

        if (!cutsceneQueued)
        {
            counter += Time.deltaTime;
            Alpha = Mathf.Lerp(1, 0, counter / blackoutTime);

            return;
        }

        counter += Time.deltaTime;
        Alpha = Mathf.Lerp(0, 1, counter / blackoutTime);

        if (counter > blackoutTime)
        {
            player.Play();
            cutscenePlaying = true;
        }
    }
}