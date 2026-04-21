using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;
public class dice_set
{
  public Texture2D[] dice_faces;
  public int[] dices;
  public burger[] dice_animations;
  public int dice_count;
  public dice_set(Texture2D dice_texture)
  {
    dice_faces = new Texture2D[6];
    dice_animations = new burger[4];
    dices = new int[4];
    dice_count = 0;
    for(int i = 0; i < 4; i++) dice_animations[i] = new burger(dice_texture);
    int faceHeight = dice_texture.height / 6;
    int faceWidth = dice_texture.width;

    for(int i = 0; i < 6; i++)
    {
      Color[] pixels = dice_texture.GetPixels(0, i * faceHeight, faceWidth, faceHeight);
      dice_faces[5 - i] = new Texture2D(faceWidth, faceHeight);
      dice_faces[5 - i].SetPixels(pixels);
      dice_faces[5 - i].Apply();
    }
  }
  public void genRandomDices()
  {
    dices[0] = Random.Range(1, 7);
    dice_animations[0].setOrientation(dices[0]);
    dices[1] = Random.Range(1, 7);
    dice_animations[1].setOrientation(dices[1]);
    dice_count = 2;
    if(dices[0] == dices[1])
    {
      dices[2] = dices[0];
      dice_animations[2].setOrientation(dices[2]);
      dices[3] = dices[0];
      dice_animations[3].setOrientation(dices[3]);
      dice_count = 4;
    }
  }
  public void removeDiceAt(int index)
  {
    int dice = dices[index];
    burger temp = dice_animations[index];
    for(int i = index + 1; i < dice_count; i++) {
      dice_animations[i - 1] = dice_animations[i];
      dices[i - 1] = dices[i];
    }
    dice_count--;
    dice_animations[dice_count] = temp;
    dices[dice_count] = dice;
  }
  public bool removeDice(int dice)
  {
    for(int i = 0; i < dice_count; i++)
    {
      if(dices[i] == dice)
      {
        removeDiceAt(i);
        return true;
      }
    }
    return false;
  }
}