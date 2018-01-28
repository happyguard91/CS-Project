using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour {

	public PIDController controller;
	public GameObject carObj;
	public ICar car;
	public Logger logger;
	public RoadBuilder roadBuilder;

    public Camera overheadCamera;

    public PathManager pathManager;

	public int numTrainingRuns = 1;
	int iRun = 0;
	int count = 0;
	bool houses_made = false;

	void Awake()
	{
		Debug.Log("Awake() in TrainingManger");
		car = carObj.GetComponent<ICar>();
	}

	// Use this for initialization
	void Start ()
	{
		Debug.Log("Start() in Training Manager");
		controller.endOfPathCB += new PIDController.OnEndOfPathCB(OnPathDone);

        RepositionOverheadCamera();
	}

	void SwapRoadToNewTextureVariation()
	{
		if(roadBuilder == null)
			return;

		roadBuilder.SetNewRoadVariation(iRun);
	}

	void StartNewRun()
	{
		count++;
		car.RestorePosRot();

		if (!houses_made)
		{
			houses_made = true;

			GameObject instance1 = Instantiate(Resources.Load("Baker_house", typeof(GameObject))) as GameObject;
			GameObject instance2 = Instantiate(Resources.Load("Baker_house", typeof(GameObject))) as GameObject;
			GameObject instance3 = Instantiate(Resources.Load("Baker_house", typeof(GameObject))) as GameObject;
			GameObject instance4 = Instantiate(Resources.Load("Baker_house", typeof(GameObject))) as GameObject;
			GameObject instance5 = Instantiate(Resources.Load("Baker_house", typeof(GameObject))) as GameObject;

			instance1.transform.position = new Vector3(58, 0, 147);
			instance1.transform.eulerAngles = new Vector3(-90, 0, 183);

			instance2.transform.position = new Vector3(33, 0, 209);
			instance2.transform.eulerAngles = new Vector3(-90, 0, 78);

			instance3.transform.position = new Vector3(58, 0, 243);
			instance3.transform.eulerAngles = new Vector3(-90, 0, -1);

			instance4.transform.position = new Vector3(33, 0, 310);
			instance4.transform.eulerAngles = new Vector3(-90, 0, 1);

			instance5.transform.position = new Vector3(58, 0, 343);
			instance5.transform.eulerAngles = new Vector3(-90, 0, -91);
		}

		if (count > 3)
		{
			count = 0;
			controller.pm.DestroyRoad();
			SwapRoadToNewTextureVariation();
			controller.pm.InitNewRoad();
		}
		controller.StartDriving();
        RepositionOverheadCamera();
	}

    public void RepositionOverheadCamera()
    {
        if(overheadCamera == null)
            return;

        Vector3 pathStart = pathManager.GetPathStart();
        Vector3 pathEnd = pathManager.GetPathEnd();
        Vector3 avg = (pathStart + pathEnd) / 2.0f;
        avg.y = overheadCamera.transform.position.y;
        overheadCamera.transform.position = avg;
    }


	void OnLastRunCompleted()
	{
		car.RequestFootBrake(1.0f);
		controller.StopDriving();
		logger.Shutdown();
	}

    public void OnMenuNextTrack()
    {
        iRun += 1;

		if(iRun >= numTrainingRuns)
            iRun = 0;

        StartNewRun();
        car.RequestFootBrake(1);
    }

    public void OnMenuRegenTrack()
    {
        StartNewRun();
        car.RequestFootBrake(1);
    }

	void OnPathDone()
	{
		iRun += 1;

		if(iRun >= numTrainingRuns)
		{
			OnLastRunCompleted();
		}
		else
		{
			StartNewRun();
		}
	}

	void Update()
	{
		//watch the car and if we fall off the road, reset things.
		if(car.GetTransform().position.y < -1.0f)
		{
			//OnPathDone();
		}

		if(logger.frameCounter + 1 % 1000 == 0)
		{
			//swap road texture left to right. or Y
			//roadBuilder.NegateYTiling();
		}
	}

}
