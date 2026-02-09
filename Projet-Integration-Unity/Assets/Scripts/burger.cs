using UnityEngine;
using System.Collections;
class CustomQuaternion {
    public float x, y, z, w;
    public CustomQuaternion(float X = 0, float Y = 0, float Z = 0, float W = 1) { 
        x = X; 
        y = Y;
        z = Z;
        w = W;
    }

    public CustomQuaternion(Vector3 axis, float angle) {
        float halfAngle = angle / 2;
        float s = Mathf.Sin(halfAngle);
        x = axis.x * s;
        y = axis.y * s;
        z = axis.z * s;
        w = Mathf.Cos(halfAngle);
    }

    public CustomQuaternion multiply(CustomQuaternion q) {
        return new CustomQuaternion(
            w * q.x + x * q.w + y * q.z - z * q.y,
            w * q.y - x * q.z + y * q.w + z * q.x,
            w * q.z + x * q.y - y * q.x + z * q.w,
            w * q.w - x * q.x - y * q.y - z * q.z);
    }

    public CustomQuaternion normalize() {
        float mag = 1 / Mathf.Sqrt(x * x + y * y + z * z + w * w);
        x *= mag;
        y *= mag;
        z *= mag;
        w *= mag;
        return this;
    }

    public Vector3 rotateVector(Vector3 v) {
        float newX = w * v.x + y * v.z - z * v.y;
        float newY = w * v.y - x * v.z + z * v.x;
        float newZ = w * v.z + x * v.y - y * v.x;
        float newW = -x * v.x - y * v.y - z * v.z;

        float vx = newW * -x + newX * w + newY * -z - newZ * -y;
        float vy = newW * -y - newX * -z + newY * w + newZ * -x;
        float vz = newW * -z + newX * -y - newY * -x + newZ * w;

        return new Vector3(vx, vy, vz);
    }

};

public class burger : MonoBehaviour
{
    static Color32 white = new Color32(255, 255, 255, 255);
    static Color32 black = new Color32(0, 0, 0, 255);
    static Color32 transparent = new Color32(0, 0, 0, 0);
    float faceWidth;
    float faceHeight;
    public float cubeWidth = 16;
    public int distanceFromCam = 1024;
    public float K1 = 889;
    public float deltaTime = 0.02f;
    const int steps = 12;
    float[] yBob = new float[steps];
    float[] xBob = new float[steps];
    float[] zBuffer;
    float zoff;
    static Vector3 xAxis = new Vector3(1, 0, 0);
    static Vector3 yAxis = new Vector3(0, 1, 0);
    static Vector3 zAxis = new Vector3(0, 0, 1);
    CustomQuaternion currentRotation = new CustomQuaternion(0, 0, 0, 1);
    CustomQuaternion downRotation = new CustomQuaternion(xAxis, -(Mathf.PI / 2) / steps);
    CustomQuaternion leftRotation = new CustomQuaternion(yAxis, (Mathf.PI / 2) / steps);
    CustomQuaternion upRotation = new CustomQuaternion(xAxis, (Mathf.PI / 2) / steps);
    CustomQuaternion rightRotation = new CustomQuaternion(yAxis, -(Mathf.PI / 2) / steps);
    private SpriteRenderer spriteRenderer;
    private Texture2D texture;


    private Sprite sprite;
    Color32[] face1, face2, face3, face4, face5, face6;

    Color32[] pixels;
    bool is_rolling;
    public void Start()
    {
        is_rolling = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        Color32[] faces = spriteRenderer.sprite.texture.GetPixels32();
        faceWidth = spriteRenderer.sprite.texture.width;
        faceHeight = spriteRenderer.sprite.texture.height / 6;

        int size = (int)(faceWidth * faceHeight);

        face1 = new Color32[size];
        face2 = new Color32[size];
        face3 = new Color32[size];
        face4 = new Color32[size];
        face5 = new Color32[size];
        face6 = new Color32[size];

        System.Array.Copy(faces, size * 0, face1, 0, size);
        System.Array.Copy(faces, size * 1, face2, 0, size);
        System.Array.Copy(faces, size * 2, face3, 0, size);
        System.Array.Copy(faces, size * 3, face4, 0, size);
        System.Array.Copy(faces, size * 4, face5, 0, size);
        System.Array.Copy(faces, size * 5, face6, 0, size);

        texture = new Texture2D(32, 32); // make dimensions adaptable
        texture.filterMode = FilterMode.Point;
        pixels = texture.GetPixels32();

        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), transform.position, spriteRenderer.sprite.pixelsPerUnit);
        spriteRenderer.sprite = sprite;
        
        zBuffer = new float[texture.width * texture.height];

        for (int i = 0; i < steps; i++) {
            float angle = Mathf.PI / 4 + (Mathf.PI * i) / (steps * 2);
            xBob[steps - i - 1] = (Mathf.Cos(angle) + Mathf.Cos(Mathf.PI / 4)) / Mathf.Sqrt(2); // pretty fucking dumb
            yBob[steps - i - 1] = (Mathf.Sin(angle) + Mathf.Sin(Mathf.PI / 4)) * (cubeWidth / Mathf.Sqrt(2));
        }
        
        for (int i = steps - 1; i > 0; i--)
        {
            xBob[i] -= xBob[i - 1];
        }
    }

    public void Update()
    {
        if(is_rolling) return;
        if(Input.GetKey ("up")) StartCoroutine(upRoll());
        else if(Input.GetKey ("left")) StartCoroutine(leftRoll());
        else if(Input.GetKey ("down")) StartCoroutine(downRoll());
        else if(Input.GetKey ("right")) StartCoroutine(rightRoll());
    }

    IEnumerator rightRoll()
    {
        is_rolling = true;

        for (int i = 0; i < steps; i++) {
            zoff = distanceFromCam - yBob[i];
            currentRotation = rightRotation.multiply(currentRotation);
            currentRotation.normalize();
            drawCube();
            transform.position += new Vector3(xBob[i], 0, 0);
            yield return new WaitForSeconds(deltaTime);
        }
        
        is_rolling = false;
    }
    IEnumerator leftRoll()
    {
        is_rolling = true;

        for (int i = 0; i < steps; i++) {
            zoff = distanceFromCam - yBob[i];
            currentRotation = leftRotation.multiply(currentRotation);
            currentRotation.normalize();
            drawCube();
            transform.position += new Vector3(-xBob[i], 0, 0);
            yield return new WaitForSeconds(deltaTime);
        }
        
        is_rolling = false;
    }
    IEnumerator upRoll()
    {
        is_rolling = true;

        for (int i = 0; i < steps; i++) {
            zoff = distanceFromCam - yBob[i];
            currentRotation = upRotation.multiply(currentRotation);
            currentRotation.normalize();
            drawCube();
            transform.position += new Vector3(0, xBob[i], 0);
            yield return new WaitForSeconds(deltaTime);
        }
        
        is_rolling = false;
    }
    IEnumerator downRoll()
    {
        is_rolling = true;

        for (int i = 0; i < steps; i++) {
            zoff = distanceFromCam - yBob[i];
            currentRotation = downRotation.multiply(currentRotation);
            currentRotation.normalize();
            drawCube();
            transform.position += new Vector3(0, -xBob[i], 0);
            yield return new WaitForSeconds(deltaTime);
        }
        
        is_rolling = false;
    }
    private float m00, m01, m02, m10, m11, m12, m20, m21, m22;
    void drawCube() {
        for(int i = 0; i < texture.height * texture.width; i++) {
            pixels[i] = transparent;
            zBuffer[i] = 0;
        }
        m00 = 1 - 2*(currentRotation.y*currentRotation.y + currentRotation.z*currentRotation.z);
        m01 = 2*(currentRotation.x*currentRotation.y - currentRotation.w*currentRotation.z);
        m02 = 2*(currentRotation.x*currentRotation.z + currentRotation.w*currentRotation.y);

        m10 = 2*(currentRotation.x*currentRotation.y + currentRotation.w*currentRotation.z);
        m11 = 1 - 2*(currentRotation.x*currentRotation.x + currentRotation.z*currentRotation.z);
        m12 = 2*(currentRotation.y*currentRotation.z - currentRotation.w*currentRotation.x);

        m20 = 2*(currentRotation.x*currentRotation.z - currentRotation.w*currentRotation.y);
        m21 = 2*(currentRotation.y*currentRotation.z + currentRotation.w*currentRotation.x);
        m22 = 1 - 2*(currentRotation.x*currentRotation.x + currentRotation.y*currentRotation.y);

        for (float cubeX = -cubeWidth / 2; cubeX < cubeWidth / 2; cubeX += 1) {
            for (float cubeY = -cubeWidth / 2; cubeY < cubeWidth / 2; cubeY += 1) {
                int index = (int)((cubeY + cubeWidth / 2) * (faceHeight / cubeWidth)) * (int)faceWidth + (int)((cubeX + cubeWidth / 2) * (faceWidth / cubeWidth));
                calculateForSurface(cubeX, cubeY, -cubeWidth / 2, face1[index]);
                calculateForSurface(cubeX, cubeY, cubeWidth / 2, face3[index]);
                calculateForSurface(cubeWidth / 2, cubeY, cubeX, face2[index]);
                calculateForSurface(-cubeWidth / 2, cubeY, cubeX, face5[index]);
                calculateForSurface(cubeY, cubeWidth / 2, cubeX, face4[index]);
                calculateForSurface(cubeY, -cubeWidth / 2, cubeX, face6[index]);
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
    }
    void calculateForSurface(float cubeX, float cubeY, float cubeZ, Color32 color) {
    
        float x = m00*cubeX + m01*cubeY + m02*cubeZ;
        float y = m10*cubeX + m11*cubeY + m12*cubeZ;
        float z = m20*cubeX + m21*cubeY + m22*cubeZ + zoff;

        float ooz = 1 / z;
        int xp = (int)(texture.width / 2 + K1 * ooz * x);
        int yp = (int)(texture.height / 2 + K1 * ooz * y);

        int idx = xp + yp * texture.width;
        if(0 <= xp && xp < texture.width && 0 <= yp && yp < texture.height){
            if (ooz > zBuffer[idx]) {
                zBuffer[idx] = ooz;
                pixels[idx] = color;
            }
        }
}
}