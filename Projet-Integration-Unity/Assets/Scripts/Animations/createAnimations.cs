using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class createAnimations : MonoBehaviour
{
  public burger dice;
  static float angle = 0.1f;
  public Texture2D dice_texture;
  static Vector3 xAxis = new Vector3(1, 0, 0);
  static Vector3 yAxis = new Vector3(0, 1, 0);
  static Vector3 zAxis = new Vector3(0, 0, 1);
  CustomQuaternion downRotation = new CustomQuaternion(xAxis, angle);
  CustomQuaternion leftRotation = new CustomQuaternion(yAxis, angle);
  CustomQuaternion upRotation = new CustomQuaternion(xAxis, -angle);
  CustomQuaternion rightRotation = new CustomQuaternion(yAxis, -angle);
  List<CustomQuaternion> animation;

  void Start()
  {
    dice = new burger(dice_texture);
    //animation = new List<CustomQuaternion>();
    animation = dice.LoadAnimation("burgerAnimation.bin");
  }
  void Update()
  {
    /*if(Input.GetKeyDown(KeyCode.LeftArrow)) {
      dice.rotate(leftRotation);
      animation.Add(leftRotation);
    }
    if(Input.GetKeyDown(KeyCode.RightArrow)) {
      dice.rotate(rightRotation);
      animation.Add(rightRotation);
    }
    if(Input.GetKeyDown(KeyCode.UpArrow)) {
      dice.rotate(upRotation);
      animation.Add(upRotation);
    }
    if(Input.GetKeyDown(KeyCode.DownArrow)) {
      dice.rotate(downRotation);
      animation.Add(downRotation);
    }*/
    if (Input.GetKeyDown(KeyCode.Backspace) && animation.Count > 0)
    {
      animation.RemoveAt(animation.Count - 1);
      if(animation.Count > 0) dice.rotate(animation[animation.Count - 1]);
    }
    if(Input.GetKeyDown(KeyCode.Return) && !dice.is_rolling) {
      StartCoroutine(dice.playAnimation(animation));
    }
  }
  void OnGUI()
  {
    GUI.DrawTexture(new Rect(0, 0, 100, 100), dice.texture);
  }
  void OnApplicationQuit()
  {
    //SaveAnimation("rollAnimation.bin");
  }
  void SaveAnimation(string filename)
  {
      string path = Application.persistentDataPath + "/" + filename;
      using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(path, System.IO.FileMode.Create)))
      {
        writer.Write(animation.Count);
        foreach(CustomQuaternion q in animation)
        {
          writer.Write(q.x);
          writer.Write(q.y);
          writer.Write(q.z);
          writer.Write(q.w);
        }
      }
      Debug.Log("Animation saved to: " + path);
  }
}