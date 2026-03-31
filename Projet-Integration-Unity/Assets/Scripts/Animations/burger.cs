using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
public class CustomQuaternion {
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

public class burger
{
    static Color32 transparent = new Color32(0, 0, 0, 0);
    float faceWidth;
    float faceHeight;
    static readonly float deltaTime = 0.02f;
    static readonly float cubeWidth = 32;
    static readonly int distanceFromCam = 1500;
    static readonly float K1 = 889;
    float[] zBuffer;
    static CustomQuaternion[] face_orientations = new CustomQuaternion[6]{
        new CustomQuaternion(0.70710678118f, 0, 0, 0.70710678118f),
        new CustomQuaternion(0.70710678118f, 0, 0.70710678118f, 0),
        new CustomQuaternion(0.70710678118f, 0, 0, -0.70710678118f),
        new CustomQuaternion(0.70710678118f, 0.70710678118f, 0, 0),
        new CustomQuaternion(0.70710678118f, 0, -0.70710678118f, 0),
        new CustomQuaternion(0, 0, 0, 1)
    };
    public Texture2D texture;
    public CustomQuaternion currentRotation = new CustomQuaternion(0, 0, 0, 1);
    Color32[] face1, face2, face3, face4, face5, face6;
    Color32[] pixels;
    public bool is_rolling;
    public Texture2D dices;
    public burger(Texture2D dice_text)
    {
        is_rolling = false;
        dices = dice_text;
        faceWidth = dices.width;
        faceHeight = dices.height / 6;
        Color32[] faces = dices.GetPixels32();

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
        
        zBuffer = new float[texture.width * texture.height];
    }
    public void setOrientation(int face)
    {
        face--;
        currentRotation.x = face_orientations[face].x;
        currentRotation.y = face_orientations[face].y;
        currentRotation.z = face_orientations[face].z;
        currentRotation.w = face_orientations[face].w;
    }

    public void rotate(CustomQuaternion q)
    {
        currentRotation = q.multiply(currentRotation);
        currentRotation.normalize();
        drawCube();
    }
    public IEnumerator playAnimation(List<CustomQuaternion> frames){
        is_rolling = true;
        foreach(CustomQuaternion q in frames){
            rotate(q);
            yield return new WaitForSeconds(deltaTime);
        }
        is_rolling = false;
    }
    
    private float m00, m01, m02, m10, m11, m12, m20, m21, m22;
    public void drawCube() {
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
        float z = m20*cubeX + m21*cubeY + m22*cubeZ + distanceFromCam;

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
    public List<CustomQuaternion> LoadAnimation(string filename)
    {
        List<CustomQuaternion> animation = new List<CustomQuaternion>();
        string path = Application.persistentDataPath + "/" + filename;
        if (!System.IO.File.Exists(path)) return animation;
        using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(path, System.IO.FileMode.Open)))
        {
            int size = reader.ReadInt32();
            for(int i = 0; i < size; i++)
            {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            animation.Add(new CustomQuaternion(x, y, z, w));
            }
        }
        return animation;
    }
}