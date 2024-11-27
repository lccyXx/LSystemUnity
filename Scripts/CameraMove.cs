using UnityEngine;
using UnityEngine.UI;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 300f;
    public float rotateSpeed = 200f;
    public float zoomSpeed = 200f;
    public float minZoom = 60f;
    public float maxZoom = 150f;

    private Camera cam;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private float defaultFieldOfView;

    //rotation
    public Slider rotationSlider;
    public Vector3 rotationCenter = Vector3.zero;
    private float lastSliderValue = 0f;

    
    void Start()
    {
        cam = GetComponent<Camera>();

        defaultPosition = new Vector3(-44f, 240f, -172f-300f);
        defaultRotation = Quaternion.Euler(0f, 0f, 0f);
        defaultFieldOfView = 60f;


        transform.position = defaultPosition;
        transform.rotation = defaultRotation;
        cam.fieldOfView = defaultFieldOfView;

        if (rotationSlider != null)
        {
            rotationSlider.value = 0;
            lastSliderValue = rotationSlider.value; // 初始化上一次的值
        }

    }

    void Update()
    {
        //MOUSE
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - zoom, minZoom, maxZoom);

        if (Input.GetMouseButton(2))
        {
            float moveX = -Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
            float moveY = -Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;
            Vector3 movement = new Vector3(moveX, moveY, 0);
            transform.Translate(movement, Space.World);
        }

        // WASD
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Input.GetKey(KeyCode.A)) moveHorizontal = -moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) moveHorizontal = moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) moveVertical = moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) moveVertical = -moveSpeed * Time.deltaTime;

        Vector3 keyboardMovement = new Vector3(moveHorizontal, 0, moveVertical);
        transform.Translate(keyboardMovement, Space.World);

        //slider rotATION
        if (rotationSlider != null)
        {
           float currentSliderValue = rotationSlider.value;
           if (currentSliderValue != lastSliderValue)
           {
                    float angle = (currentSliderValue - lastSliderValue) * rotateSpeed * Time.deltaTime;
                    transform.RotateAround(rotationCenter, Vector3.up, angle);
                    lastSliderValue = currentSliderValue; 
           }
        }
    }

    //reset button
    public void ResetCamera()
    {
        transform.position = defaultPosition;
        transform.rotation = defaultRotation;
        cam.fieldOfView = Mathf.Clamp(defaultFieldOfView, minZoom, maxZoom);
    }
}