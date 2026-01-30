using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
class Quaternion {
    public float x, y, z, w;
    public Quaternion(float X = 0, float Y = 0, float Z = 0, float W = 1) { 
        x = X; 
        y = Y;
        z = Z;
        w = W;
    }

    public Quaternion(Vector3 axis, float angle) {
        float halfAngle = angle / 2;
        float s = Mathf.Sin(halfAngle);
        x = axis.x * s;
        y = axis.y * s;
        z = axis.z * s;
        w = Mathf.Cos(halfAngle);
    }

    public Quaternion multiply(Quaternion q) {
        return new Quaternion(
            w * q.x + x * q.w + y * q.z - z * q.y,
            w * q.y - x * q.z + y * q.w + z * q.x,
            w * q.z + x * q.y - y * q.x + z * q.w,
            w * q.w - x * q.x - y * q.y - z * q.z);
    }

    public Quaternion normalize() {
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
    static Color32 red = new Color32(255, 0, 0, 255);
    
    float face_dimension = 16;
    Color32[][][] faces = new Color32[][][]{
	new Color32[][]{
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,black,black,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,black,black,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black}
},
	new Color32[][]{
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black}

},
	new Color32[][]{
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,black,black,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,black,black,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black}
},
	new Color32[][]{
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black}
},
	new Color32[][]{
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,black,black,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,black,black,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black}
},
	new Color32[][]{
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,black,black,white,white,white,white,white,white,black,black,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,white,white,white,white,white,white,white,white,white,white,white,white,white,white,black},
		new Color32[]{black,black,black,black,black,black,black,black,black,black,black,black,black,black,black,black}
	}
};
    public float cubeWidth = 32;
    public int distanceFromCam = 200;
    public float K1 = 160;
    public float deltaTime = 0.5f;
    const int steps = 16;
    float[] yBob = new float[steps];
    float[] xBob = new float[steps];
    float[] zBuffer;
    float yoff;
    float xoff;
    float zoff;
    static Vector3 xAxis = new Vector3(1, 0, 0);
    static Vector3 yAxis = new Vector3(0, 1, 0);
    static Vector3 zAxis = new Vector3(0, 0, 1);
    Quaternion currentRotation = new Quaternion(0, 0, 0, 1);
    Quaternion downRotation = new Quaternion(xAxis, (Mathf.PI / 2) / steps);
    Quaternion leftRotation = new Quaternion(yAxis, (Mathf.PI / 2) / steps);
    Quaternion upRotation = new Quaternion(xAxis, -(Mathf.PI / 2) / steps);
    Quaternion rightRotation = new Quaternion(yAxis, -(Mathf.PI / 2) / steps);
    private SpriteRenderer spriteRenderer;
    private Texture2D texture;
    private Sprite sprite;

    Color32[] pixels;

    bool is_rolling;
    public void Start()
    {
        is_rolling = false;
        spriteRenderer = GetComponent<SpriteRenderer>();

        for (int i = 0; i < steps; i++) {
        float angle = Mathf.PI / 4 + (Mathf.PI * i) / (steps * 2);
            //xBob[steps - i - 1] = (Mathf.Cos(angle) + Mathf.Cos(Mathf.PI / 4)) * Mathf.Sqrt((cubeWidth / 2) * (cubeWidth / 2));
            //yBob[steps - i - 1] = (Mathf.Sin(angle) + Mathf.Sin(Mathf.PI / 4)) * Mathf.Sqrt((cubeWidth / 2) * (cubeWidth / 2));
            xBob[steps - i - 1] = 0;
            yBob[steps - i - 1] = 0;
        }

        // Create a new texture from the existing sprite
        Sprite originalSprite = spriteRenderer.sprite;
        texture = new Texture2D((int)originalSprite.rect.width, (int)originalSprite.rect.height);
        texture.filterMode = FilterMode.Point; // For Mathf.PIxel art
        
        // Copy Mathf.PIxels from original sprite
        pixels = originalSprite.texture.GetPixels32();
        texture.SetPixels32(pixels);
        texture.Apply();
        
        // Create new sprite from the texture
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
        
        zBuffer = new float[texture.width * texture.height];
    }

    public void Update()
    {
        if(is_rolling) return;
        StartCoroutine(roll());
    }

    IEnumerator roll()
    {
        is_rolling = true;

        yoff = 0;
        for (int i = 0; i < steps; i++) {
            zoff = distanceFromCam - yBob[i];
            xoff = xBob[i];
            currentRotation = rightRotation.multiply(currentRotation);
            //currentRotation.normalize();
            drawCube1();
            yield return new WaitForSeconds(deltaTime);
        }
        
        is_rolling = false;
    }

    void draw_line(int x0, int y0, int x1, int y1, Color32 color) {
    int dx = Mathf.Abs(x1 - x0);
    int sx = x0 < x1 ? 1 : -1;
    int dy = -Mathf.Abs(y1 - y0);
    int sy = y0 < y1 ? 1 : -1;
    int err = dx + dy;
    int e2;

    while (x0 != x1 || y0 != y1) {
        pixels[x0 + y0 * texture.width] = color;

        e2 = err << 1;

        if (e2 >= dy) { 
            err += dy; 
            x0 += sx; 
        }
        if (e2 <= dx) { 
            err += dx; 
            y0 += sy; 
        }
    }
}
    void drawCube1() {
        for(int i = 0; i < texture.height * texture.width; i++) {
            pixels[i] = white;
            zBuffer[i] = 0;
        }

        for (float cubeX = -cubeWidth / 2; cubeX < cubeWidth / 2; cubeX += 1) {
            for (float cubeY = -cubeWidth / 2; cubeY < cubeWidth / 2; cubeY += 1) {
                int i = (int)((cubeY + cubeWidth / 2) * (face_dimension / cubeWidth));
                int j = (int)((cubeX + cubeWidth / 2) * (face_dimension / cubeWidth));
                calculateForSurface(cubeX, cubeY, -cubeWidth / 2, faces[0][i][j]);
                calculateForSurface(cubeX, cubeY, cubeWidth / 2, faces[2][i][j]);
                calculateForSurface(cubeWidth / 2, cubeY, cubeX, faces[1][i][j]);
                calculateForSurface(-cubeWidth / 2, cubeY, cubeX, faces[4][i][j]);
                calculateForSurface(cubeY, cubeWidth / 2, cubeX, faces[3][i][j]);
                calculateForSurface(cubeY, -cubeWidth / 2, cubeX, faces[5][i][j]);
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
    }
    void calculateForSurface(float cubeX, float cubeY, float cubeZ, Color32 color) {
    
        float newX = currentRotation.w * cubeX + currentRotation.y * cubeZ - currentRotation.z * cubeY;
        float newY = currentRotation.w * cubeY - currentRotation.x * cubeZ + currentRotation.z * cubeX;
        float newZ = currentRotation.w * cubeZ + currentRotation.x * cubeY - currentRotation.y * cubeX;
        float newW = -currentRotation.x * cubeX - currentRotation.y * cubeY - currentRotation.z * cubeZ;

        float x = newW * -currentRotation.x + newX * currentRotation.w + newY * -currentRotation.z - newZ * -currentRotation.y + xoff;
        float y = newW * -currentRotation.y - newX * -currentRotation.z + newY * currentRotation.w + newZ * -currentRotation.x + yoff;
        float z = newW * -currentRotation.z + newX * -currentRotation.y - newY * -currentRotation.x + newZ * currentRotation.w + zoff;

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