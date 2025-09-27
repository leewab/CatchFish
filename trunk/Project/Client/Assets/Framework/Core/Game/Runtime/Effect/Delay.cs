using UnityEngine;

public class Delay : MonoBehaviour 
{
	public float delayTime = 1.0f;

    private bool mIsFirst = true;
	void Start () {		
		gameObject.SetActiveRecursively(false);
		Invoke("DelayFunc", delayTime);
	}
	
	void DelayFunc()
	{
		if (gameObject == null) return;
		gameObject.SetActiveRecursively(true);
        mIsFirst = false;
    }

    public void Restart()
    {
        if(!mIsFirst)
        {
            Start();
        }
    }
}
