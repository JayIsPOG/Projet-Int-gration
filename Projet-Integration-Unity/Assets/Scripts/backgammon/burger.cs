using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
public class burger
{
    static readonly Color32 transparent = new Color32(0, 0, 0, 0);
    static readonly float deltaTime = 0.02f;
    static readonly int steps = 40;
    static readonly float rotation = (2 * Mathf.PI) / steps;
    static readonly float cubeWidth = 36;
    static readonly int distanceFromCam = 1500;
    static readonly float zoom = 889;
    float faceWidth, faceHeight;
    float m00, m01, m02, m10, m11, m12, m20, m21, m22;
    float A, B, C;
    float[] zBuffer;
    public Texture2D texture;
    Color32[] face1, face2, face3, face4, face5, face6;
    Color32[] pixels;
    public bool is_rolling;
    public Texture2D dices;
    static float[][] face_orientations = new float[6][]{
        new float[3]{0, Mathf.PI/2, 0},
        new float[3]{0, 0, 3*Mathf.PI/2},
        new float[3]{0, 3*Mathf.PI/2, 0},
        new float[3]{0, Mathf.PI, 0},
        new float[3]{0, 0, Mathf.PI/2},
        new float[3]{0, 0, 0}
    };
    public burger(Texture2D dice_text)
    {
        A = 0;
        B = 0;
        C = 0;
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
        float[] angles = face_orientations[face - 1];
        A = angles[0];
        B = angles[1];
        C = angles[2];
    }
    public IEnumerator playAnimation() 
    {
        is_rolling = true;
        for(int i = 0; i < steps; i++) 
        {
            A += rotation;
            B += rotation;
            drawCube();
            yield return new WaitForSeconds(deltaTime);
        }
        is_rolling = false;
    }

    public void setRotation(float nA, float nB, float nC)
    {
        A = nA;
        B = nB;
        C = nC;
    }
    public void drawCube() {
        for(int i = 0; i < texture.height * texture.width; i++) 
        {
            pixels[i] = transparent;
            zBuffer[i] = 0;
        }

        float sinA = Mathf.Sin(A), sinB = Mathf.Sin(B), sinC = Mathf.Sin(C);
        float cosA = Mathf.Cos(A), cosB = Mathf.Cos(B), cosC = Mathf.Cos(C);

        m00 = sinA*sinB*cosC+cosA*sinC; m01 = cosA*cosC-sinA*sinB*sinC; m02 = -sinA*cosB;
        m10 = sinA*sinC-cosA*sinB*cosC; m11 = cosA*sinB*sinC+sinA*cosC; m12 = cosA*cosB;
        m20 = cosB*cosC;                m21 = -cosB*sinC;               m22 = sinB;

        for (float cubeX = -cubeWidth / 2; cubeX < cubeWidth / 2; cubeX += 1) 
        {
            for (float cubeY = -cubeWidth / 2; cubeY < cubeWidth / 2; cubeY += 1) 
            {
                int index = (int)((cubeY + cubeWidth / 2) * (faceHeight / cubeWidth)) * (int)faceWidth + (int)((cubeX + cubeWidth / 2) * (faceWidth / cubeWidth));// maybe just increment, idk
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
    void calculateForSurface(float cubeX, float cubeY, float cubeZ, Color32 color) 
    {
    
        float x = m00*cubeZ + m01*cubeX + m02*cubeY;
        float y = m10*cubeZ + m11*cubeX + m12*cubeY;
        float z_1 = 1 / (m20*cubeZ + m21*cubeX + m22*cubeY + distanceFromCam);

        int xp = (int)(texture.width / 2 + zoom * x * z_1);
        int yp = (int)(texture.height / 2 + zoom * y * z_1);

        int idx = xp + yp * texture.width;
        if(0 <= xp && xp < texture.width && 0 <= yp && yp < texture.height)
        {
            if (z_1 > zBuffer[idx]) 
            {
                zBuffer[idx] = z_1;
                pixels[idx] = color;
            }
        }
    }
}