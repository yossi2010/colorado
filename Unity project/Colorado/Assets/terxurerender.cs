using UnityEngine;
using System.IO;

namespace UnityStandardAssets.Vehicles.Car
{
public class terxurerender : MonoBehaviour
{
    float picEvery = 0.1f, Timetotake = 0;
    // Use this for initialization
    int sqrx = 200, y = 66;
    public TextMesh _percentMesh;
    public TextMesh _nameMesh;
    public Camera VirtuCamera;
    public int resWidth = 200;
    public int resHeight = 66;
    private bool takeHiResShot = false;
	int im=0;
    void Start()
    {
        Timetotake = Time.time;
        VirtuCamera = GetComponent<Camera>();
        File.AppendAllText((Application.dataPath+"/../screenshots/Data.csv"), "This is a data file"+ System.Environment.NewLine);
    }



    public static string ScreenShotName(int width, int height,int imnum)
    {
        return string.Format("{0}/../screenshots/Data/Camera_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             imnum);
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }
public CarController car;
double ang=0;
    void LateUpdate()
    {
        takeHiResShot |= Input.GetKeyDown("k");
        if (takeHiResShot||Timetotake < Time.time)
        {
            ang=car.m_SteerAngle;
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            VirtuCamera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            VirtuCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            VirtuCamera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight,im);
            WriteData(filename,ang.ToString());
			im++;
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
			Timetotake = Time.time + picEvery;
        }
    }
    void WriteData(string f,string ang){
        File.AppendAllText((Application.dataPath+"/../screenshots/Data.csv"), string.Format("{0}, {1}",
                             f,
                             ang)+ System.Environment.NewLine);

    }
}
}

