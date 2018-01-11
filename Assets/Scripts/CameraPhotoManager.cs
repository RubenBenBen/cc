using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class CameraPhotoManager : MonoBehaviour {

    private Color skinColor;

    // For saving to the _savepath
    private string _SavePath = "D:/Ruben/Unity/CameraPlatformer/MasterPieces/"; //Change the path here!
    int _CaptureCounter;

    int pixelIndex;
    private float[] distanceArray;

    public RawImage cameraImage;
    //public RectTransform imageParent;
    //public AspectRatioFitter imageFitter;

    // Device cameras
    WebCamDevice frontCameraDevice;
    WebCamDevice backCameraDevice;
    WebCamDevice activeCameraDevice;

    WebCamTexture frontCameraTexture;
    WebCamTexture backCameraTexture;
    WebCamTexture activeCameraTexture;

    // Image rotation
   // Vector3 rotationVector = new Vector3(0f, 0f, 0f);

    // Image uvRect
   // Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
   // Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

    // Image Parent's scale
  //  Vector3 defaultScale = new Vector3(1f, 1f, 1f);
  //  Vector3 fixedScale = new Vector3(-1f, 1f, 1f);

    // Use this for initialization
    void Start () {

        skinColor = hexToColor("D3B8B8");

        //Debug.Log(ColorDistance(Color.white, Color.black));
        //Debug.Log(ColorDistance(new Color32(0, 0, 0, 255), new Color32(255, 255, 255, 255)));


        if (PlayerPrefs.HasKey("imageCount")) {
            _CaptureCounter = PlayerPrefs.GetInt("imageCount");
        } else {
            _CaptureCounter = 0;
        }


        // Check for device cameras
        if (WebCamTexture.devices.Length == 0) {
            Debug.Log("No devices cameras found");
            return;
        }

        // Get the device's cameras and create WebCamTextures with them
        frontCameraDevice = WebCamTexture.devices.Last();
        backCameraDevice = WebCamTexture.devices.First();

        frontCameraTexture = new WebCamTexture(frontCameraDevice.name);
        backCameraTexture = new WebCamTexture(backCameraDevice.name);

        // Set camera filter modes for a smoother looking image
        frontCameraTexture.filterMode = FilterMode.Trilinear;
        backCameraTexture.filterMode = FilterMode.Trilinear;

        // Set the camera to use by default
        SetActiveCamera(backCameraTexture);
    }

   /* void Update () {
        // Skip making adjustment for incorrect camera data
        if (activeCameraTexture.width < 100) {
            Debug.Log("Still waiting another frame for correct info...");
            return;
        }

        // Rotate image to show correct orientation 
        rotationVector.z = -activeCameraTexture.videoRotationAngle;
        //image.rectTransform.localEulerAngles = rotationVector;

        // Set AspectRatioFitter's ratio
        float videoRatio =
            (float) activeCameraTexture.width / (float) activeCameraTexture.height;
        //imageFitter.aspectRatio = videoRatio;

        // Unflip if vertically flipped
        //image.uvRect = activeCameraTexture.videoVerticallyMirrored ? fixedRect : defaultRect;

        // Mirror front-facing camera's image horizontally to look more natural
        //imageParent.localScale = activeCameraDevice.isFrontFacing ? fixedScale : defaultScale;
    }*/

    // Set the device camera to use and start it
    public void SetActiveCamera (WebCamTexture cameraToUse) {
        if (activeCameraTexture != null) {
            activeCameraTexture.Stop();
        }

        activeCameraTexture = cameraToUse;
        activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device =>
            device.name == cameraToUse.deviceName);

        cameraImage.texture = activeCameraTexture;
        //cameraImage.material.mainTexture = activeCameraTexture;
        Vector3 scale = cameraImage.transform.localScale;
        scale.x *= cameraToUse == frontCameraTexture ? 1 : -1;
        cameraImage.transform.localScale = scale;
        Debug.Log(scale);
        activeCameraTexture.Play();
    }

    // Switch between the device's front and back camera
    public void SwitchCamera () {
        SetActiveCamera(activeCameraTexture.Equals(frontCameraTexture) ?
            backCameraTexture : frontCameraTexture);
    }

    public void TakeSnapshot () {
        Color clr = transform.Find("CameraImage").GetComponent<RawImage>().color;
        transform.Find("CameraImage").GetComponent<RawImage>().color = new Color(clr.r, clr.g, clr.b, 1);

        Color[] colorPixels = activeCameraTexture.GetPixels();
        Color[] resultPixels = new Color[colorPixels.Length];
        Color[] showPixels = new Color[colorPixels.Length];

        /*for (int i = 0 ; i < 5 ; i++) {
            Debug.Log(ColorDistance(Color.cyan, colorPixels[i]));
        }*/

        distanceArray = new float[colorPixels.Length];
        float max = 0;
        float min = 10000;
        for (int i = 0 ; i < colorPixels.Length ; i++) {
            distanceArray[i] = ColorDistance(colorPixels[i], Color.white);
            min = Mathf.Min(min, distanceArray[i]);
            max = Mathf.Max(max, distanceArray[i]);
        }
        //Debug.Log(min);
        //Debug.Log(max);
        for (int i = 0 ; i < colorPixels.Length ; i++) {
            if (distanceArray[i] < 300) {
                resultPixels[i] = Color.clear;
                showPixels[i] = Color.white;
            } else if (distanceArray[i] < 400) {
                resultPixels[i] = Color.clear;
                showPixels[i] = Color.white;
            } else if (distanceArray[i] < 500) {
                resultPixels[i] = Color.cyan;
                showPixels[i] = Color.cyan;
            } else if (distanceArray[i] < 600) {
                resultPixels[i] = Color.blue;
                showPixels[i] = Color.blue;
            } else if (distanceArray[i] < 700) {
                resultPixels[i] = Color.magenta;
                showPixels[i] = Color.magenta;
            } else {
                resultPixels[i] = Color.black;
                showPixels[i] = Color.black;
            }
        }


        Texture2D snap = new Texture2D(activeCameraTexture.width, activeCameraTexture.height);
        snap.SetPixels(colorPixels);
        snap.Apply();

        Texture2D snap2 = new Texture2D(activeCameraTexture.width, activeCameraTexture.height);
        snap2.SetPixels(resultPixels);
        snap2.Apply();

        Texture2D snap3 = new Texture2D(activeCameraTexture.width, activeCameraTexture.height);
        snap3.SetPixels(showPixels);
        snap3.Apply();

        /* System.IO.File.WriteAllBytes(_SavePath + _CaptureCounter + ".png", snap.EncodeToPNG());
         ++_CaptureCounter;
         PlayerPrefs.SetInt("imageCount", _CaptureCounter);
         System.IO.File.WriteAllBytes(_SavePath + _CaptureCounter + ".png", snap2.EncodeToPNG());
         ++_CaptureCounter;
         PlayerPrefs.SetInt("imageCount", _CaptureCounter);*/

        transform.Find("ResultImage").GetComponent<RawImage>().texture = snap3;
        transform.Find("ResultImage").GetComponent<RawImage>().color = Color.white;
        transform.Find("ResultImage").localScale = cameraImage.transform.localScale;
        transform.Find("ResultImage").gameObject.SetActive(true);

        InitPrototype(Sprite.Create(snap2, new Rect(0, 0, snap2.width, snap2.height), new Vector2(0.5f, 0.5f)));

        activeCameraTexture.Stop();
    }

    private void InitPrototype (Sprite initSprite) {
        GetComponent<PrototypeManager>().InitPrototype(initSprite);
        transform.Find("PrototypeTransform").localScale = cameraImage.transform.localScale;
        transform.Find("Player").gameObject.SetActive(true);
    }

    public void DebugPixel () {
        for (int i = 0 ; i < 640 ; i++) {
            Debug.Log(distanceArray[i + pixelIndex * 640]);
        }
        pixelIndex++;
    }

    private float ColorToFloat (Color color) {
        return color.r + color.g + color.b;
    }

    private Color[] GetResultPixels (Color[] photoPixels) {
        return null;
    }

    public void RestartScene () {
        SceneManager.LoadScene(0);
    }

    private Color hexToColor (string hex) {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8) {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

    private float ColorDistance (Color32 e1, Color32 e2) {
        long rmean = ( (long) e1.r + (long) e2.r ) / 2;
        long r = ( (long) e1.r - (long) e2.r );
        long g = ( (long) e1.g - (long) e2.g );
        long b = ( (long) e1.b - (long) e2.b );
        return Mathf.Sqrt(( ( ( 512 + rmean ) * r * r ) >> 8 ) + 4 * g * g + ( ( ( 767 - rmean ) * b * b ) >> 8 ));
    }

    private float ColorDistance (Color e1, Color e2) {
        long rmean = ( Convert.ToInt64(e1.r * 255) + Convert.ToInt64(e2.r * 255) ) / 2;
        long r = ( Convert.ToInt64(e1.r * 255) - Convert.ToInt64(e2.r * 255) );
        long g = ( Convert.ToInt64(e1.g * 255) - Convert.ToInt64(e2.g * 255) );
        long b = ( Convert.ToInt64(e1.b * 255) - Convert.ToInt64(e2.b * 255) );
        return Mathf.Sqrt(( ( ( 512 + rmean ) * r * r ) >> 8 ) + 4 * g * g + ( ( ( 767 - rmean ) * b * b ) >> 8 ));
    }
}